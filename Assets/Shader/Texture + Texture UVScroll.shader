Shader "Mobile/Texture + Texture with UV Scrolling" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _AdditiveTex ("Additive (RGB)", 2D) = "black" {}
 _ScrollingSpeedBase ("Scrolling speed Base", Vector) = (0,0,0,0)
 _ScrollingSpeedAdditive ("Scrolling speed Additive", Vector) = (0,0,0,0)
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