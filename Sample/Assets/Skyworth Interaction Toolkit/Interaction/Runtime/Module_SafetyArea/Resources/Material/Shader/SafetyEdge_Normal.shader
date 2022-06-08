Shader "Unlit/SafetyEdge_Normal"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        //_NearTex ("NearTex", 2D) = "white" {}
        _MainTex2("Texture2", 2D) = "white" {}
        _MainTex3("Texture3", 2D) = "white" {}
    }
    SubShader
    {
        Tags { //"RenderType" = "Opaque"
               //"LightMode" = "ShadowCaster"}
            }
        LOD 100

        cull off
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            // Upgrade NOTE: excluded shader from DX11, OpenGL ES 2.0 because it uses unsized arrays
            //#pragma exclude_renderers d3d11 gles
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            //#pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                //float2 nearuv : TEXCOORD3;
                float2 uv2 : TEXCOORD4;
                float2 uv3 : TEXCOORD5;
                float viewPosZ : TEXCOORD2;
                float4 screenPos : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            //sampler2D _NearTex;
            //float4 _NearTex_ST;
            sampler2D _MainTex2;
            float4 _MainTex2_ST;
            sampler2D _MainTex3;
            float4 _MainTex3_ST;

            //sampler2D _CameraDepthTexture;
            fixed3 interactionPosition[3];
            int positionCount;
            fixed alpha;
            int bowHead;
            fixed4 _Color1;
            fixed4 _Color2;
            fixed4 settingsColor;

            v2f vert (appdata v)
            {
                v2f o;

                _MainTex2_ST.w = -_Time.y / 2;
                _MainTex3_ST.z = -1 + (_Time.y / 2) % 2;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                //o.nearuv = TRANSFORM_TEX(v.uv, _NearTex);
                o.uv2 = TRANSFORM_TEX(v.uv, _MainTex2);
                o.uv3 = TRANSFORM_TEX(v.uv, _MainTex3);
                o.screenPos = ComputeScreenPos(o.vertex);
                o.viewPosZ = UnityObjectToViewPos(v.vertex).z;
                return o;
            }


            fixed getDistance(fixed3 position[3], fixed3 screenPosition)
            {
                float cdist = 99999.0;

                for (uint i = 0; i < positionCount; i++) {
                    cdist = min(cdist, distance(position[i], screenPosition));
                }
                return cdist;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                //fixed4 nearCol = tex2D(_NearTex, i.nearuv);
                fixed4 col2 = tex2D(_MainTex2, i.uv2);
                fixed4 col3 = tex2D(_MainTex3, i.uv3);

                fixed2 screenPos = fixed2(i.screenPos.xy / i.screenPos.w);               
                fixed dist = getDistance(interactionPosition, fixed3(screenPos.x, screenPos.y, -i.viewPosZ));

                //col = fixed4(i.viewPosZ, i.viewPosZ, i.viewPosZ, 1.0);
                fixed4 normalCol = _Color1 * col + col2 * _Color2;
                col = fixed4(normalCol.r, normalCol.g, normalCol.b, alpha * col.a);

                if (dist < 1.0)
                {
                    //col = fixed4(i.viewPosZ, i.viewPosZ, i.viewPosZ, 1.0);
                    col = fixed4(settingsColor.r, settingsColor.g, settingsColor.b, alpha * col.a);//fixed4(0.9579, 0.0275, 0.1569, nearCol.a * alpha);
                }

                if (dist < 0.2)
                {
                    col = fixed4(0, 0, 0, 0);
                }

                if (dist >= 0.2 && dist < 0.22)
                {
                    col = fixed4(0.9569, 0.0275, 0.1569, 1);
                }

                if (bowHead == 1 && i.uv.y < 0.05)
                {
                    //col = settingsColor;
                    col = _Color1 + col3 * _Color2;
                }
                return col;
            }
            ENDCG
        }
    }
}
