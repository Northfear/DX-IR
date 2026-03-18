Shader "Texture + Texture " {
Properties {
	_MainTex ("Texture 1 (RGB)", 2D) = "" {}
	_Texture2 ("Texture 2  (RGB)", 2D) = "" {}
}

SubShader {
	Pass {
		SetTexture [_MainTex] { combine texture, texture alpha }
		SetTexture [_Texture2] { combine previous + texture double }
	}
}
}
