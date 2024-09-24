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

	SubShader {
		Tags {"RenderType" = "Transparent" "Queue" = "Transparent" "IgnoreProjector" = "True"}
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert alpha

		sampler2D _Texture;
		fixed4 _Color;

		struct Input {
			float2 uv_Texture;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D(_Texture, IN.uv_Texture) + _Color;
			o.Albedo = c.rgb;
			o.Alpha = 0.3;
		}
		ENDCG
	}

	Fallback "Transparent/VertexLit"
}
