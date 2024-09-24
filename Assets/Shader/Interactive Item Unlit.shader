Shader "Interactive Item Unlit Shader" {
Properties {
	_ObjectCenter ("Center of the Mesh", Vector) = (0,0,0,1)
	_Overlay ("Overlay Grid Texture", 2D) = "white" {}
	_MaxDistance ("Max Distance For Glow", Float) = 1
	_BlendFactor ("Percent Overlay Blend", Range(0,0.5)) = 0.2
	_OutlineColor ("Outline Color", Color) = (0.9922,0.7647,0,255)
	_Outline ("Outline width", Range(0.002,0.03)) = 0.01
	_DiffuseMap ("Base Texture", 2D) = "white" {}
}
	//DummyShaderTextExporter
	
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Lambert fullforwardshadows
#pragma target 3.0
		sampler2D _DiffuseMap;
		struct Input
		{
			float2 uv_DiffuseMap;
		};
		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 c = tex2D(_DiffuseMap, IN.uv_DiffuseMap);
			o.Albedo = c.rgb;
		}
		ENDCG
	}
}
