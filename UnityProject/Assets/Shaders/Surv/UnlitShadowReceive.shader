// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Velcro/UnlitShadowReceive" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		[NoScaleOffset]_MainTex("Base (RGB)", 2D) = "white" {}
		_CutOff("Cutoff", Range(0,1)) = 0.5
		_MipScale("Mip Level Alpha Scale", Range(0,1)) = 0.25

	}

		SubShader{
			Pass {
				Tags { "LightMode" = "ForwardBase" "RenderQueue" = "AlphaTest" "RenderType" = "TransparentCutout"  }
				Cull Off
				AlphaToMask On
				//ZWrite Off
				//Blend SrcAlpha OneMinusSrcAlpha
				
				CGPROGRAM
				#pragma vertex vert 
				#pragma fragment frag 
				#include "UnityCG.cginc" 
				#pragma multi_compile_fwdbase 
				#include "AutoLight.cginc" 
				sampler2D _MainTex;
				//float4 _MainTex_ST;
				fixed4 _Color;
				half _CutOff;
				float4 _MainTex_TexelSize;
				half _MipScale;


				float CalcMipLevel(float2 texture_coord)
				{
					float2 dx = ddx(texture_coord);
					float2 dy = ddy(texture_coord);
					float delta_max_sqr = max(dot(dx, dx), dot(dy, dy));

					return max(0.0, 0.5 * log2(delta_max_sqr));
				}

				struct v2f {
					float4 pos : SV_POSITION;
					LIGHTING_COORDS(0,1)
					float2 uv : TEXCOORD2;
				};

				v2f vert(appdata_base v) {
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					//o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.uv = (v.texcoord);
					TRANSFER_VERTEX_TO_FRAGMENT(o);
					return o;
				}

				fixed4 frag(v2f i) : COLOR
				{
					float attenuation = LIGHT_ATTENUATION(i);
					fixed4 col = tex2D(_MainTex, i.uv) * _Color;
					// rescale alpha by mip level (if not using preserved coverage mip maps)
					col.a *= 1 + max(0, CalcMipLevel(i.uv * _MainTex_TexelSize.zw)) * _MipScale;
					// rescale alpha by partial derivative
					col.a = (col.a - _CutOff) / max(fwidth(col.a), 0.0001) + 0.5;
					//clip(col.a - _CutOff);
					//col *= attenuation;
					return (col);
				}
				ENDCG
			}
	}
		Fallback "VertexLit"
}