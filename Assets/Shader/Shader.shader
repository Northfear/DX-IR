Shader "BumpSpecProbed" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _SpecularColor ("Specular Color", Color) = (0.455224,0.455224,0.455224,1)
 _Shininess ("Shininess", Range(0.03,1)) = 0.1
 _ProbeShininess ("Probe Spec Pow", Range(1,20)) = 2
 _ProbeSpecStr ("Probe Spec Multiply", Range(0,2)) = 0.5
 _ProbeSpecSub ("Probe Spec Subtrator", Range(0,4)) = 2
 _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
 _BumpMap ("Normalmap", 2D) = "bump" {}
}
	//DummyShaderTextExporter
	
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Lambert fullforwardshadows
#pragma target 3.0
		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};
		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
		}
		ENDCG
	}
}