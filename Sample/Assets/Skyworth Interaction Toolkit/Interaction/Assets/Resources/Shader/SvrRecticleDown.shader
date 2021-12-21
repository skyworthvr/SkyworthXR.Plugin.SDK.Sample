Shader "Svr/SvrRecticleDown"
{
	Properties
	{
		
		_Color ("Color", Color) = (0.5,0.5,0.5,0.5)
		_FillAmount("360 degree fill", Range(0, 360)) = 0
		_InnerDiameter("InnerDiameter", Range(0, 10.0)) = 1.5
		_OuterDiameter("OuterDiameter", Range(0.00872665, 10.0)) = 2.0
		_DistanceInMeters("DistanceInMeters", Range(0.0, 100.0)) = 2.0
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent+10" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha
		AlphaTest Off
		Cull Back
		Lighting Off
		ZWrite Off
		ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
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

			uniform fixed4 _Color;
			uniform float _FillAmount;
			uniform float _InnerDiameter;
			uniform float _OuterDiameter;
			uniform float _DistanceInMeters;

			v2f vert (appdata v)
			{
				float scale = lerp(_OuterDiameter, _InnerDiameter, v.vertex.z);

				float3 vert_out = float3(v.vertex.x * scale * 1.5f, v.vertex.y * scale*1.5f, _DistanceInMeters);
				v2f o;
				o.vertex = UnityObjectToClipPos(vert_out);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture

				float2 pos = normalize(float2(i.uv.x - 0.5, i.uv.y - 0.5));
				float2 axis_x = float2(1, 0);
				float2 axis_y = float2(0, 1);
				float angle_x = dot(axis_x, pos);
				float angle_y = dot(axis_y, pos);
				float fill = (360 - _FillAmount) / 180;
				if (fill < 1)
				{
					if (acos(angle_x) < fill * 3.1415926 && angle_y > 0)
					{
						fixed4 col = fixed4(0, 0, 0, 0);
						return col;
					}
				}
				else
				{
					if (angle_y > 0 || acos(angle_x) > (2 - fill) * 3.1415926)
					{
						fixed4 col = fixed4(0, 0, 0, 0);
						return col;
					}
				}

				fixed4 col = _Color;

				return col;
			}
			ENDCG
		}
	}
}
