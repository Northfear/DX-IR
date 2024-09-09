Shader "Mobile/ShieldFX" {
Properties {
 _Color ("_Color", Color) = (0,1,0,1)
 _Inside ("_Inside", Range(0,0.2)) = 0
 _Rim ("_Rim", Range(1,2)) = 1.2
 _Texture ("_Texture", 2D) = "white" {}
 _Speed ("_Speed", Range(5,15)) = 5
 _Tile ("_Tile", Range(1,10)) = 5
 _Strength ("_Strength", Range(0,5)) = 1.5
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