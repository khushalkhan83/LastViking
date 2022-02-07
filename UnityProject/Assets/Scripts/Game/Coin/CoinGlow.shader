Shader "Surv/CoinGlow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (0,0,0,0)
		_SpotColor ("Spot Color", Color) = (0,0,0,0)
		_Noise ("Noise", 2D) = "black" {}
		_NoiseScale ("Noise Scale", Vector) = (0,0,0,0)
		_NoiseMaskMorph ("Noise Mask Morph", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
		    ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
				float4 noiseUV : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
			sampler2D _Noise;
            float4 _MainTex_ST;
			float4 _Color;
			float4 _SpotColor;
			float4 _NoiseScale;
			float4 _NoiseMaskMorph;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.noiseUV.xy = v.uv * _NoiseScale.xy + _NoiseScale.zw + float2(_Time.x * 2, -_Time.x * 2);
				o.noiseUV.zw = v.uv * _NoiseScale.xy * 0.5f + _NoiseScale.zw + float2(_Time.x * -2, -_Time.x * 2);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				//fixed noise1 = tex2D(_Noise, i.noiseUV.xy).x;
				//fixed noise2 = tex2D(_Noise, i.noiseUV.zw).x;

				//float2 _maskSampleUV = i.uv + float2((noise1 * noise2 * _NoiseMaskMorph.x * 4) - _NoiseMaskMorph.x, (noise1 * noise2 * _NoiseMaskMorph.y * 4) - _NoiseMaskMorph.y);
				fixed4 col = tex2D(_MainTex, i.uv);

				

				//float noiseMix = 1 - clamp(noise1 * noise2, 0, 0.3f) * 3.33f;
				//
				//float4 final = clamp(col - 0.1f - noiseMix * 0.2f, 0, 1.0f) * 1.5f * _Color;

				//final.rgb += _SpotColor * (1 - clamp(noise1, 0, 0.1f) * 10);

                return col * _Color;
            }
            ENDCG
        }
    }
}
