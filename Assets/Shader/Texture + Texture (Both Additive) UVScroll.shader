Shader "Mobile/Texture + Texture (Both Additive) with UV Scrolling" {
Properties {
	_MainTex ("Base Additive (RGB)", 2D) = "white" {}
	_AdditiveTex ("Additive (RGB)", 2D) = "black" {}
	_ScrollingSpeedBase ("Scrolling speed Base", Vector) = (0,0,0,0)
	_ScrollingSpeedAdditive ("Scrolling speed Additive", Vector) = (0,0,0,0)
}

	SubShader
	{
		Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "True" "RenderType" = "Transparent" }

		Blend One One
		ColorMask RGB

		Pass
		{

			CGPROGRAM
			#pragma vertex v
			#pragma fragment p

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _AdditiveTex;
			float4 _AdditiveTex_ST;

			float4 _ScrollingSpeedBase;
			float4 _ScrollingSpeedAdditive;

			struct VertOut
			{
				float4 position : POSITION;
				float2 uv : TEXCOORD0;
			};

			VertOut v( float4 position : POSITION, float3 norm : NORMAL, float2 uv : TEXCOORD0 )
			{
				VertOut OUT;

				OUT.position = mul( UNITY_MATRIX_MVP, position );
				OUT.uv = uv;

				return OUT;
			}

			struct PixelOut
			{
				float4 color : COLOR;
			};

			PixelOut p ( VertOut input )
			{
				PixelOut OUT;
			
				float2 flowUV = input.uv * _MainTex_ST.xy + _MainTex_ST.zw + float2( _ScrollingSpeedBase.x * _Time.y, _ScrollingSpeedBase.y * _Time.y );
				float2 maskUV = input.uv * _AdditiveTex_ST.xy + _AdditiveTex_ST.zw + float2( _ScrollingSpeedAdditive.x * _Time.y, _ScrollingSpeedAdditive.y * _Time.y );
				float4 mainColor = tex2D( _MainTex, flowUV );
				float4 additiveColor = tex2D( _AdditiveTex, maskUV );
				float4 finalColor = mainColor + additiveColor;

				OUT.color = finalColor;

				return OUT;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
