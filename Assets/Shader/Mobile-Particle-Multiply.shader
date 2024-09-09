Shader "Mobile/Particles/Multiply" {
Properties {
 _MainTex ("Particle Texture", 2D) = "white" {}
}
SubShader { 
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
  BindChannels {
   Bind "vertex", Vertex
   Bind "color", Color
   Bind "texcoord", TexCoord
  }
  ZWrite Off
  Cull Off
  Fog {
   Color (1,1,1,1)
  }
  Blend Zero SrcColor
  SetTexture [_MainTex] { combine texture * primary }
  SetTexture [_MainTex] { ConstantColor (1,1,1,1) combine previous lerp(previous) constant }
 }
}
SubShader { 
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
  BindChannels {
   Bind "vertex", Vertex
   Bind "color", Color
   Bind "texcoord", TexCoord
  }
  ZWrite Off
  Cull Off
  Fog {
   Color (1,1,1,1)
  }
  Blend Zero SrcColor
  SetTexture [_MainTex] { ConstantColor (1,1,1,1) combine texture lerp(texture) constant }
 }
}
}