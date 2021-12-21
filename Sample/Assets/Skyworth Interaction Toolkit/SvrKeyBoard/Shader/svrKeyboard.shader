Shader "Svr/Keyboard"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
	}
	SubShader{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}
		LOD 100
		ZWrite Off
		Cull Off
		Lighting Off
		ZTest [unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha

		Pass {
			GLSLPROGRAM
			//#version 310 es
			#include "UnityCG.glslinc"
			#define SHADERLAB_GLSL
			#pragma only_renderers gles gles3
			#extension GL_OES_EGL_image_external : enable
			#extension GL_OES_EGL_image_external_essl3 : enable
			#extension GL_OVR_multiview : enable
			#extension GL_OVR_multiview2 : enable
			// Force high precision on our shader to prevent rounding errors from creeping up
			precision highp float;
			#ifdef VERTEX
				#if __VERSION__ >= 300
					/// Unity Stereo uniforms
					layout(std140) uniform UnityStereoGlobals {
						mat4 unity_StereoMatrixP[2];
						mat4 unity_StereoMatrixV[2];
						mat4 unity_StereoMatrixInvV[2];
						mat4 unity_StereoMatrixVP[2];
						mat4 unity_StereoCameraProjection[2];
						mat4 unity_StereoCameraInvProjection[2];
						mat4 unity_StereoWorldToCamera[2];
						mat4 unity_StereoCameraToWorld[2];
						vec3 unity_StereoWorldSpaceCameraPos[2];
						vec4 unity_StereoScaleOffset[2];
					};
					layout(std140) uniform UnityStereoEyeIndices {
						vec4 unity_StereoEyeIndices[2];
					};
					#if defined(STEREO_MULTIVIEW_ON)
						// For GL_OVR_multiview we use gl_ViewID_OVR to get the current view index
						layout(num_views = 2) in;
					#endif
				#endif

				varying vec2 uvs;
				uniform vec4 _MainTex_ST;
				uniform mat4 video_matrix;

				vec2 transformTex(vec2 texCoord, vec4 texST) {
					return (texCoord.xy * texST.xy + texST.zw);
				}

				void main() {
					#if defined (STEREO_MULTIVIEW_ON)
						gl_Position = unity_StereoMatrixVP[gl_ViewID_OVR] * unity_ObjectToWorld * gl_Vertex;
					#else
						gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
					#endif
					vec4 untransformedUV = gl_MultiTexCoord0;

					uvs = transformTex((video_matrix * untransformedUV).xy, _MainTex_ST);
				}
			#endif

			#ifdef FRAGMENT
				varying vec2 uvs;
				uniform samplerExternalOES _MainTex;
				uniform vec4 _Color;
				void main() {
					#if __VERSION__ >= 300
						gl_FragColor = texture(_MainTex, uvs.xy) * _Color;
					#else
						gl_FragColor = texture2D(_MainTex, uvs.xy) * _Color;
					#endif
				}
			#endif
			ENDGLSL
		}
	}
	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}
		LOD 100
		ZWrite Off
		Cull Off
		Lighting Off
		ZTest [unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;
			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv) * _Color;
				return col;
			}
			ENDCG
		}
	}
}
