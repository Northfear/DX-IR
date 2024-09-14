Shader "BumpSpecProbed"
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_SpecularColor("Specular Color", Color) = (0.4552239,0.4552239,0.4552239,1)
		_Shininess("Shininess", Range(0.03,1) ) = 0.1
		_ProbeShininess("Probe Spec Pow", Range(1,20) ) = 2
		_ProbeSpecStr("Probe Spec Multiply", Range(0,2) ) = 0.5
		_ProbeSpecSub("Probe Spec Subtrator", Range(0,4) ) = 2
		_MainTex("Base (RGB) Gloss (A)", 2D) = "white" {}
		_BumpMap("Normalmap", 2D) = "bump" {}
	}

	SubShader
	{
		Tags
		{
			"Queue"="Geometry"
			"IgnoreProjector"="False"
			"RenderType"="Opaque"
		}

		Cull Back
		ZWrite On
		ZTest LEqual
		ColorMask RGBA
		Fog{
	}

		CGPROGRAM
		#pragma surface surf BlinnPhongEditor  vertex:vert novertexlights
		#pragma target 3.0


			float4 _Color;
			float4 _SpecularColor;
			float _Shininess;
			float _ProbeShininess;
			float _ProbeSpecStr;
			float _ProbeSpecSub;
			sampler2D _MainTex;
			sampler2D _BumpMap;

			struct EditorSurfaceOutput {
				half3 Albedo;
				half3 Normal;
				half3 Emission;
				half3 Gloss;
				half Specular;
				half Alpha;
				half4 Custom;
			};

			inline half4 LightingBlinnPhongEditor_PrePass (EditorSurfaceOutput s, half4 light)
			{
				half3 spec = light.a * s.Gloss;
				half4 c;
				c.rgb = (s.Albedo * light.rgb + light.rgb * spec);
				c.a = s.Alpha;
				return c;
			}

			inline half4 LightingBlinnPhongEditor (EditorSurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
			{
				half3 h = normalize (lightDir + viewDir);

				half diff = max (0, dot ( lightDir, s.Normal ));

				float nh = max (0, dot (s.Normal, h));
				float spec = pow (nh, s.Specular*128.0);

				half4 res;
				res.rgb = _LightColor0.rgb * diff;
				res.w = spec * Luminance (_LightColor0.rgb);
				res *= atten * 2.0;

				return LightingBlinnPhongEditor_PrePass( s, res );
			}

			struct Input {
				float3 viewDir;
				float2 uv_MainTex;
				float2 uv_BumpMap;
				float3 worldNormal;
				float3 worldRefl;
				INTERNAL_DATA
			};

			void vert (inout appdata_full v, out Input o) {
				UNITY_INITIALIZE_OUTPUT(Input,o);
				float4 VertexOutputMaster0_0_NoInput = float4(0,0,0,0);
				float4 VertexOutputMaster0_1_NoInput = float4(0,0,0,0);
				float4 VertexOutputMaster0_2_NoInput = float4(0,0,0,0);
				float4 VertexOutputMaster0_3_NoInput = float4(0,0,0,0);
			}


			void surf (Input IN, inout EditorSurfaceOutput o) {
				o.Normal = float3(0.0,0.0,1.0);
				o.Alpha = 1.0;
				o.Albedo = 0.0;
				o.Emission = 0.0;
				o.Gloss = 0.0;
				o.Specular = 0.0;
				o.Custom = 0.0;

				float4 Tex2D0=tex2D(_MainTex,(IN.uv_MainTex.xyxy).xy);
				float4 Multiply0=Tex2D0 * _Color;
				float4 Tex2DNormal0=float4(UnpackNormal( tex2D(_BumpMap,(IN.uv_BumpMap.xyxy).xy)).xyz, 1.0 );
				float4 Multiply1=Tex2D0.aaaa * _SpecularColor;
				float4 Master0_2_NoInput = float4(0,0,0,0);
				float4 Master0_7_NoInput = float4(0,0,0,0);
				float4 Master0_6_NoInput = float4(1,1,1,1);
				o.Albedo = Multiply0;
				o.Normal = Tex2DNormal0;
				o.Specular = _Shininess.xxxx;
				o.Gloss = Multiply1;
				o.Alpha = Tex2D0.aaaa;
				float4 Fresnel0=(1.0 - dot( normalize( float4( IN.viewDir.x, IN.viewDir.y,IN.viewDir.z,1.0 ).xyz), normalize( Tex2DNormal0.xyz ) )).xxxx;
				o.Normal = normalize(o.Normal);
				fixed3 worldN = WorldNormalVector(IN, o.Normal);
				fixed3 worldR = normalize(WorldReflectionVector(IN, o.Normal));
				float3 fProbeSpecCol = ShadeSH9(float4(worldR,1.0));
				float fProbeSpecMask = pow(max(_ProbeSpecStr*(length(fProbeSpecCol)-_ProbeSpecSub-Fresnel0*2),0),_ProbeShininess+Fresnel0);
				o.Emission = o.Albedo*ShadeSH9(float4(worldN,1.0)) + o.Gloss*fProbeSpecCol*fProbeSpecMask;
			}
		ENDCG
	}
	Fallback "Diffuse"
}
