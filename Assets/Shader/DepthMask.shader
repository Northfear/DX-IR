Shader "Masked/Mask" {
SubShader { 
 Tags { "QUEUE"="Geometry+10" }
 Pass {
  Tags { "QUEUE"="Geometry+10" }
  ColorMask 0
 }
}
}