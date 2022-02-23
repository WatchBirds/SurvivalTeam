Shader "Custom/MySh"
{
    Properties
	{_Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
		_Diffuse("Diffuse", 2D) = "white" {}
		_Emission("Emission", 2D) = "white" {}
		_UVScrollSpeed("UVScroll Speed", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
		
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Diffuse;
		uniform float4 _Diffuse_ST;
		uniform sampler2D _Emission;
		uniform float _UVScrollSpeed;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Diffuse = i.uv_texcoord * _Diffuse_ST.xy + _Diffuse_ST.zw;
			o.Albedo = tex2D( _Diffuse, uv_Diffuse ).rgb;
			float mulTime12 = _Time.y * _UVScrollSpeed;
			float2 panner1 = ( mulTime12 * float2( 1,0 ) + i.uv_texcoord);
			o.Emission = tex2D( _Emission, panner1 ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
    FallBack "Diffuse"
}
