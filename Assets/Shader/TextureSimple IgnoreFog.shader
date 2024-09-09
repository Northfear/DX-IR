Shader "Mobile/Unlit/Simple No Fog" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
}
SubShader { 
 Pass {
  Fog { Mode Off }
  SetTexture [_MainTex] { combine texture, texture alpha }
 }
}
}