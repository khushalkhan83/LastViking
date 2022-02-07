Shader "Grayscale"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Blend("Blend", float) = 0
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _MainTex;
			fixed _Blend;

			fixed3 lerp(fixed3 a, fixed3 b, fixed w)
			{
				return a + w * (b - a);
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed gray = col.r * 0.11 + col.g * 0.66 + col.b * 0.23;

				col.rgb = lerp(col.rgb, fixed3(gray, gray, gray), _Blend);

				return col;
			}
			ENDCG
		}
	}
}
