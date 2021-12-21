Shader "Svr/SvrNoloController"
{
	Properties
	{
		_Color("Color", COLOR) = (1, 1, 1, 1)
		_MainTex("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True"  }
		LOD 100
		ZWrite On
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			/// The size of the touch display.  A value of 1 sets the radius to equal the touchpad radius
			#define _GVR_DISPLAY_RADIUS .25

			// How opaque is the battery indicator when illuminated
			#define _GVR_BATTERY_ACTIVE_ALPHA 0.9

			//How opaque is the battery indicator when not illuminated
			#define _GVR_BATTERY_OFF_ALPHA 0.25

			// How much do the app and system buttons depress when pushed
			#define _BUTTON_PRESS_DEPTH 0.101

			// Larger values tighten the feather
			#define _TOUCH_FEATHER 8

			/// The center of the touchpad in UV space
			/// Only change this value if you also change the UV layout of the mesh
			#define _GVR_TOUCHPAD_CENTER half2(0.301, 0.1674)

			/// The radius of the touchpad in UV space, based on the geometry
			/// Only change this value if you also change the UV layout of the mesh
			#define _GVR_TOUCHPAD_RADIUS .110


			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
			};

			struct v2f {
				half2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				half4 color : TEXCOORD1;
				half2 touchVector : TEXCOORD2;
				half alpha : TEXCOORD3;
			};

			sampler2D _MainTex;
			half4 _GvrControllerAlpha;
			float4 _MainTex_ST;
			uniform float _TriggerAlpha;

			half4 _Color;
			half4 _GvrTouchPadColor;
			half4 _GvrAppButtonColor;
			half4 _GvrSystemButtonColor;
			half4 _GvrVolumeUpButtonColor;
			half4 _GvrVolumeDownButtonColor;
			half4 _GvrBatteryColor;
			half4 _GvrTriggerColor;
			half4 _GvrTouchInfo;//xy position, z touch duration, w battery info

			v2f vert(appdata v) {
				v2f o;
				float4 vertex4;
				vertex4.xyz = v.vertex;
				vertex4.w = 1.0;

				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.color = half4(0,0,0,0);
				o.touchVector = half2(0,0);


				half batteryOrController = saturate(10.0 * (v.color.a - 0.8));
				half batteryMask = saturate(10.0 * (1 - v.color.a));
				half batteryLevelMask = saturate(20.0 * (v.color.a - _GvrTouchInfo.w));
				o.alpha = batteryOrController;
				o.color.a = _GvrBatteryColor.a * batteryMask * (batteryLevelMask * _GVR_BATTERY_ACTIVE_ALPHA + (1 - batteryLevelMask)*_GVR_BATTERY_OFF_ALPHA);
				o.color.rgb = batteryMask * (batteryLevelMask * _GvrBatteryColor.rgb);

				// v.color.r = Touchpad, v.color.g = AppButton, v.color.b = SystemButton, v.color.a = BatteryIndicator
				// Update touch vector info, but only if in the touchpad region.
				_GvrTouchInfo.xy *= -1;
				//This is the distance between the scaled center of the touchpad in UV space, and the input coords
				half2 touchPosition = ((v.uv - _GVR_TOUCHPAD_CENTER) / _GVR_TOUCHPAD_RADIUS - _GvrTouchInfo.xy);

				// the duration of a press + minimum radius
				half scaledInput = _GvrTouchInfo.z + _GVR_DISPLAY_RADIUS;

				// Apply a cubic function, but make sure when press duration =1 , we cancel out the min radius
				half bounced = 2 * (2 * scaledInput - scaledInput * scaledInput) -
					(1 - 2.0*_GVR_DISPLAY_RADIUS*_GVR_DISPLAY_RADIUS);

				if (v.color.r == 1 && v.color.g == 0 && v.color.b == 0)
				{
					//touchpad
					o.color.rgb += v.color.r * _GvrTouchInfo.z * _GvrTouchPadColor.rgb;
					o.color.a += v.color.r * _GvrTouchInfo.z;
					o.touchVector = ((2 - bounced)*((1 - _GvrControllerAlpha.y) / _GVR_DISPLAY_RADIUS) *touchPosition);
				}
				if (v.color.r == 0 && v.color.g == 1 && v.color.b == 0)
				{
					//appbutton
					o.color.rgb += v.color.g * _GvrControllerAlpha.z * _GvrAppButtonColor.rgb;
					o.color.a += v.color.g * _GvrControllerAlpha.z;
					vertex4.y -= v.color.g * _BUTTON_PRESS_DEPTH*_GvrControllerAlpha.z;
				}
				if (v.color.r == 0 && v.color.g == 1 && v.color.b == 1)
				{
					//triggerbutton
					o.color.rgb += _TriggerAlpha * _GvrTriggerColor;
					o.color.a += _TriggerAlpha;
					vertex4.y -= _TriggerAlpha * _BUTTON_PRESS_DEPTH;
				}
				if (v.color.r == 0 && v.color.g == 0 && v.color.b == 1)
				{
					//systembutton
					o.color.rgb += v.color.b * _GvrControllerAlpha.w * _GvrSystemButtonColor.rgb;
					o.color.a += v.color.b * _GvrControllerAlpha.w;
					vertex4.y -= v.color.b *  _BUTTON_PRESS_DEPTH*_GvrControllerAlpha.w;
				}
				//if (v.color.r == 1 && v.color.g == 1 && v.color.b == 0)
				//{
				//	//volum+
				//	o.color.rgb += v.color.g * _GvrControllerAlpha.w * _GvrVolumeUpButtonColor.rgb;
				//	o.color.a += v.color.g * _GvrControllerAlpha.w;;
				//	vertex4.y -= v.color.g *  _BUTTON_PRESS_DEPTH*_GvrControllerAlpha.w;
				//}
				//if (v.color.r == 1 && v.color.g == 0 && v.color.b == 1)
				//{
				//	//volum-
				//	o.color.rgb += v.color.b * _GvrControllerAlpha.z * _GvrVolumeDownButtonColor.rgb;
				//	o.color.a += v.color.b * _GvrControllerAlpha.z;;
				//	vertex4.y -= v.color.b *  _BUTTON_PRESS_DEPTH*_GvrControllerAlpha.z;
				//}

				o.vertex = UnityObjectToClipPos(vertex4);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target{

				// Compute the length from a touchpoint, scale the value to control the edge sharpness.
				half len = saturate(_TOUCH_FEATHER*(1 - length(i.touchVector)));
				i.color = i.color *len;

				half4 texcol = tex2D(_MainTex, i.uv);
				half3 tintColor = (i.color.rgb + (1 - i.color.a) * _Color.rgb);

				// Tint the controller based on luminance
				half luma = Luminance(tintColor);
				tintColor = texcol.rgb *(tintColor + .25*(1 - luma));

				/// Battery indicator.
				texcol.a = i.alpha * texcol.a + (1 - i.alpha)*(texcol.r)* i.color.a;
				texcol.rgb = i.alpha * tintColor + (1 - i.alpha)*i.color.rgb;
				texcol.a *= _GvrControllerAlpha.x;


				return texcol;
			}
			ENDCG
		}
	}
}
