// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Outline"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
		_Size("Size", Range(0., 1.)) = 1
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Overlay"
			"RenderType" = "Opaque"
			"IgnoreProjector" = "True"
		}

		LOD 100

		CGINCLUDE
			
			#include "UnityCG.cginc"
			
			uniform float _Size;
			uniform float4 _Color;
			
			void vert(inout half4 pos : POSITION, in half3 normal : NORMAL)
			{
				pos.xyz += normal * _Size;
				pos = UnityObjectToClipPos(pos);
			}
			
			fixed4 frag() : SV_TARGET
			{
				return _Color;
			}
			
		ENDCG

		Pass
		{
			ZWrite Off
			Cull Front
			ZTest Less
			Lighting Off
			Blend One Zero

			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
			ENDCG
		}

		Pass
		{
			ZWrite Off
			Cull Back
			ZTest Greater
			Lighting Off
			Blend One Zero

			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
			ENDCG

		}
	}
}
