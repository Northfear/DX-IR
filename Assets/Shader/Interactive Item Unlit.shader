Shader "Interactive Item Unlit Shader" {
Properties {
	_ObjectCenter ("Center of the Mesh", Vector) = (0, 0, 0, 1)
	_Overlay ("Overlay Grid Texture", 2D) = "white" {}
	_MaxDistance ("Max Distance For Glow", Float) = 1
	_BlendFactor ("Percent Overlay Blend", Range(0, 0.5)) = 0.2
	_OutlineColor ("Outline Color", Color) = (0.9922, 0.7647, 0, 255)
	_Outline ("Outline width", Range(0.002, 0.03)) = 0.01
	_DiffuseMap ("Base Texture", 2D) = "white" {}
}

	SubShader
	{
		Tags{ "RenderType" = "Opaque" }

		Pass
		{
			Name "Outline"
			Cull Front
			ZWrite On
			ColorMask RGB

			CGPROGRAM
				#include "UnityCG.cginc"
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0

				//static const float MIN_FADEOFF_TRANSITION_DISTANCE = 1.f;
				//static const float MIN_FADEOFF_DISTANCE = 1.5f;
				static const float MAX_FADEOFF_TRANSITION_DISTANCE = 4.f;
				static const float MAX_FADEOFF_MOD = 4.f;

				uniform float _Outline;
				uniform float4 _OutlineColor;
				uniform float _MaxDistance;
				float3 _ObjectCenter;

				struct appdata {
					float4 vertex : POSITION;
					float3 normal : NORMAL;
				};

				float4 vert(appdata v) : SV_POSITION
				{
					//Camera position in world
					float3 targetPos = _WorldSpaceCameraPos;

					//Object position in world
					float3 objectPos = mul(_Object2World, v.vertex).xyz;

					//Distance from the object to the camera
					float dist = distance(targetPos, objectPos + _ObjectCenter);

					//float alphaMinDistance = smoothstep(MIN_FADEOFF_DISTANCE, MIN_FADEOFF_DISTANCE + MIN_FADEOFF_TRANSITION_DISTANCE, dist);
					float maxFadeoffDistance = MAX_FADEOFF_MOD * _MaxDistance;
					float alphaMaxDistance = 1 - smoothstep(maxFadeoffDistance, maxFadeoffDistance + MAX_FADEOFF_TRANSITION_DISTANCE, dist);
					//float alphaDistanceMod = min(alphaMinDistance, alphaMaxDistance);

					float4 pos = mul(UNITY_MATRIX_MVP, v.vertex);
					float3 norm = mul((float3x3)UNITY_MATRIX_MV, v.normal);
					norm.x *= UNITY_MATRIX_P[0][0];
					norm.y *= UNITY_MATRIX_P[1][1];
					pos.xy += norm.xy * pos.z * _Outline * alphaMaxDistance;

					return pos;
				}

				float4 frag(void) : COLOR 
				{
					return _OutlineColor;
				}
			ENDCG
		}

		CGPROGRAM
			#pragma surface surf Lambert vertex:vert

			//static const float MIN_FADEOFF_TRANSITION_DISTANCE = 1.f;
			//static const float MIN_FADEOFF_DISTANCE = 1.5f;
			static const float MAX_FADEOFF_TRANSITION_DISTANCE = 4.f;
			static const float MAX_FADEOFF_MOD = 4.f;

			sampler2D _DiffuseMap;
			sampler2D _Overlay;
			float _BlendFactor;
			float4 _Overlay_ST;
			float _MaxDistance;
			float3 _ObjectCenter;

			struct Input {
				float2 uv_DiffuseMap;
				float4 screenPos;
				float3 objPos;
			};

			void vert (inout appdata_full v, out Input o) {
				UNITY_INITIALIZE_OUTPUT(Input, o);
				o.objPos = mul(_Object2World, v.vertex).xyz;
			}

			void surf (Input IN, inout SurfaceOutput o) {
				half4 mainTex = tex2D(_DiffuseMap, IN.uv_DiffuseMap);
				float2 overlayTexCoordinate = IN.screenPos.xy / IN.screenPos.w;
				overlayTexCoordinate = TRANSFORM_TEX(overlayTexCoordinate, _Overlay);
				fixed4 overlayTex = tex2D(_Overlay, overlayTexCoordinate);

				//Camera position in world
				float3 targetPos = _WorldSpaceCameraPos;

				//Object position in world
				float3 objectPos = IN.objPos;

				//Distance from the object to the camera
				float dist = distance(targetPos, objectPos + _ObjectCenter);

				//float alphaMinDistance = smoothstep(MIN_FADEOFF_DISTANCE, MIN_FADEOFF_DISTANCE + MIN_FADEOFF_TRANSITION_DISTANCE, dist);
				float maxFadeoffDistance = MAX_FADEOFF_MOD * _MaxDistance;
				float alphaMaxDistance = 1 - smoothstep(maxFadeoffDistance, maxFadeoffDistance + MAX_FADEOFF_TRANSITION_DISTANCE, dist);
				//float alphaDistanceMod = min(alphaMinDistance, alphaMaxDistance);

				mainTex = lerp(mainTex, overlayTex, alphaMaxDistance * _BlendFactor);

				o.Albedo = mainTex.rgb;
				o.Alpha = mainTex.a;
			}
		ENDCG
	}

	Fallback "Diffuse"
}
