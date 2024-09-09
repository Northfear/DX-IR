Shader "Mobile/Dissolve/DissolveDiffuse" {
Properties {
 _DissolvePower ("Dissolve Power", Range(1,0)) = 0.2
 _DissolveEmissionColor ("Dissolve Emission Color", Color) = (1,1,1,1)
 _MainTex ("Main Texture", 2D) = "white" {}
 _DissolveTex ("Dissolve Texture", 2D) = "white" {}
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