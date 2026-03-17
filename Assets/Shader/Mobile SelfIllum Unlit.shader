Shader "Self-Illumin/Unlit" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _Illum ("Illumin (A)", 2D) = "white" {}
 _EmissionLM ("Emission (Lightmapper)", Float) = 0
}
SubShader { 
 LOD 100
 Tags { "RenderType"="Opaque" }
 Pass {
  Tags { "RenderType"="Opaque" }
  SetTexture [_MainTex] { combine texture }
 }
}
Fallback "VertexLit"
}