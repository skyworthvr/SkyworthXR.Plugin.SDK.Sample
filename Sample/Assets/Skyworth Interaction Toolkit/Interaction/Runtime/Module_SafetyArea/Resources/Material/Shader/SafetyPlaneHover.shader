Shader "Unlit/SafetyPlaneHover"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			// make fog work
			//#pragma multi_compile_fog

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				//fixed4 color : COLOR;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				//fixed4 color : COLOR;
				float2 uv : TEXCOORD0;
				float4 vertexLocalPosition : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 pointerLocalPosition;
			float brushSize;
			//fixed4 _Color;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				//o.color = v.color;
				o.vertexLocalPosition = v.vertex;
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				//UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed dist = distance(i.vertexLocalPosition, pointerLocalPosition);
				
				clip(brushSize - dist);
				
				fixed alpha = min((brushSize - dist) / (brushSize * 0.3), 1.0);

				// apply fog
				//UNITY_APPLY_FOG(i.fogCoord, col);
				return  col * alpha;
			}
			ENDCG
		}
	}
}
