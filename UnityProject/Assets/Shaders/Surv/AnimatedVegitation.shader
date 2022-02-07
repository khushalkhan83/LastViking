// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Surv/AnimatedVegitation" 
{
	Properties 
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
		_Wind("Wind", Vector) = (0,0,0,0)
	}

	SubShader 
	{
		Tags
		{
			"Queue"="AlphaTest"
			"IgnoreProjector"="True"
			"RenderType"="TransparentCutout"
		}

		LOD 2
		
		Pass 
		{
			CGPROGRAM
				
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0
				#pragma multi_compile_fog
				#pragma multi_compile_instancing

				#include "UnityCG.cginc"

				struct appdata_t 
				{
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
					float3 normal : NORMAL;
					float4 color : COLOR;
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};

				struct v2f 
				{
					float4 vertex : SV_POSITION;
					float2 texcoord : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					UNITY_VERTEX_OUTPUT_STEREO
					float4 color : COLOR;
				};

				uniform sampler2D _MainTex;
				uniform float4 _MainTex_ST;
				uniform	float4 _Wind;
				uniform float4 Terrain_ColorMultiplier, Terrain_ColorAddition, Environment_Color;
				uniform float3 Light_Direction, Light_Color;
				uniform fixed _Cutoff;

				inline half4 SmoothCurve(half4 x)
				{   
					return x * x * (3.0 - 2.0 * x);   
				}

				inline half4 TriangleWave(half4 x)
				{   
					return abs(frac(x + 0.5) * 2.0 - 1.0);   
				}

				inline half4 SmoothTriangleWave(half4 x)
				{   
					return SmoothCurve(TriangleWave(x));   
				}

				inline half3 PlantAnimation(half3 position, half animation)
				{	
					half4 vWaves = SmoothTriangleWave(frac((_Time.yy * 2.9f + (position.x * 0.2 * animation + position.z * 0.2 * animation)).xxyy * half4(1.975, 0.793, 0.375, 0.285)));
					
					return _Wind.xyz * (vWaves.z + vWaves.w) * animation * _Wind.w;
				}

				v2f vert (appdata_t v)
				{
					v2f o;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_OUTPUT(v2f, o);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

					half3 animatedVertex = PlantAnimation(mul(unity_ObjectToWorld, v.vertex), 1 - v.color.r);

					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					
					o.vertex = UnityObjectToClipPos(v.vertex + mul(unity_WorldToObject, float4(animatedVertex.xyz, 0)));
					UNITY_TRANSFER_FOG(o, o.vertex);

					fixed3 normalDirection = normalize(mul(half4(v.normal, 0.0), unity_WorldToObject).xyz);
					fixed3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);

					o.color.rgb =  Terrain_ColorMultiplier * 0.5f + clamp(normalDirection.y, 0.15f, 1.1f) * unity_AmbientSky;
					o.color.a = Light_Color * max(0.0, dot(normalDirection, Light_Direction)) * 2;

					return o;
				}

				fixed4 frag (v2f i) : SV_Target
				{
					fixed4 col = tex2D(_MainTex, i.texcoord);

					col.rgb = col.rgb * i.color.rgb + Terrain_ColorAddition;
			
					clip(col.a - _Cutoff);
					UNITY_APPLY_FOG(i.fogCoord, col.rgb);

					return col;
				}

			ENDCG
			Cull Off
		}

		Pass //shadow caster
		{
			Name "ShadowCaster"
			Tags
			{
				"LightMode" = "ShadowCaster"
			}

			ZWrite On ZTest LEqual

			CGPROGRAM
				#pragma vertex vert_surf
				#pragma fragment frag_surf
				#pragma multi_compile_shadowcaster
				
				#include "UnityCG.cginc"

				fixed _Cutoff;
				float4 _MainTex_ST;
				sampler2D _MainTex;

				struct v2f_surf
				{
					V2F_SHADOW_CASTER;
					float2 hip_pack0 : TEXCOORD1;
				};

				v2f_surf vert_surf (appdata_full v) 
				{
					v2f_surf o;

					o.hip_pack0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
					TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
					return o;
				}

				half4 frag_surf (v2f_surf IN) : SV_Target 
				{
					fixed alpha = tex2D(_MainTex, IN.hip_pack0.xy).a;
					clip (alpha - _Cutoff);
					SHADOW_CASTER_FRAGMENT(IN)
				}

			ENDCG
		}
	}

	SubShader
	{
		Tags
		{
			"Queue" = "AlphaTest"
			"IgnoreProjector" = "True"
			"RenderType" = "TransparentCutout"
		}

		LOD 1

		Pass
		{
			CGPROGRAM
				
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0
				#pragma multi_compile_fog
				
				#include "UnityCG.cginc"
				
				struct appdata_t
				{
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
					float3 normal : NORMAL;
					float4 color : COLOR;
				};

				struct v2f
				{
					float4 vertex : SV_POSITION;
					float2 texcoord : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					float4 color : COLOR;
				};

				uniform sampler2D _MainTex;
				uniform float4 _MainTex_ST;
				uniform float4 Terrain_ColorMultiplier;
				uniform float4 Terrain_ColorAddition;
				uniform float4 Environment_Color;
				uniform float3 Light_Direction, Light_Color;
				uniform fixed _Cutoff;

				v2f vert(appdata_t v)
				{
					v2f o;

					o.vertex = UnityObjectToClipPos(v.vertex);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					UNITY_TRANSFER_FOG(o,o.vertex);

					fixed3 normalDirection = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);
					fixed3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);

					o.color.rgb = Terrain_ColorMultiplier * 0.5f + clamp(normalDirection.y, 0.15f, 1.1f) * unity_AmbientSky;
					o.color.a = Light_Color * max(0.0, dot(normalDirection, Light_Direction)) * 2;

					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 col = tex2D(_MainTex, i.texcoord);

					col.rgb = col.rgb * i.color.rgb + Terrain_ColorAddition;

					clip(col.a - _Cutoff);
					UNITY_APPLY_FOG(i.fogCoord, col.rgb);

					return col;
				}

			ENDCG
			Cull Off
		}
	}

	FallBack "VertexLit"
}
