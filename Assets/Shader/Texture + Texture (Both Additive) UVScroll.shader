Shader "Mobile/Texture + Texture (Both Additive) with UV Scrolling" {
Properties {
 _MainTex ("Base Additive (RGB)", 2D) = "white" {}
 _AdditiveTex ("Additive (RGB)", 2D) = "black" {}
 _ScrollingSpeedBase ("Scrolling speed Base", Vector) = (0,0,0,0)
 _ScrollingSpeedAdditive ("Scrolling speed Additive", Vector) = (0,0,0,0)
}
	//DummyShaderTextExporter
	
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard fullforwardshadows
#pragma target 3.0
		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
		}
		ENDCG
	}
}