Shader "Surv/Ground"
{
	Properties
	{
		_MainTex("BaseMap (RGB)", 2D) = "white" {}
		_WaterLevel("WaterLevel", float) = 0.75
		_WaterLevelModifier("Water Level Modifier", Color) = (1,1,1,1)
		_WaterLevelAnimation("WaterLevelAnimation", float) = 0.75
	}

		SubShader
		{
			LOD 1

			Pass
			{
				Tags
				{
					"RenderType" = "Opaque"
					"Queue" = "Geometry-100"
					"LightMode" = "ForwardBase"
				}

				Lighting On
				ZWrite On

				CGPROGRAM

					#include "UnityCG.cginc"
					#include "AutoLight.cginc"
					#include "Lighting.cginc"

					#pragma vertex vert
					#pragma fragment frag
					#pragma multi_compile_fwdbase

					struct Input
					{
						float2 uv : TEXCOORD0;

						float4 waterMulDistance : TEXCOORD2;
						SHADOW_COORDS(3)

						float4 vertex : SV_POSITION;
						float2 color : COLOR;
					};

					float4 _MainTex_ST;
					sampler2D _MainTex;
					float _WaterLevel;
					float _WaterLevelAnimation;
					float4 _WaterLevelModifier;

					float4 Terrain_AmbientSky;
					float4 Terrain_ColorMultiplier;
					float4 Terrain_ColorAddition;
					float4 Terrain_ColorEdge;
					float4 Terrain_ColorShadow;
					float Terrain_ShadowDistance;

					Input vert(appdata_full v)
					{
						Input o;

						float4x4 modelMatrix = unity_ObjectToWorld;
						float4x4 modelMatrixInverse = unity_WorldToObject;

						float3 normal = normalize(mul(float4(v.normal, 0.0), modelMatrixInverse).xyz);
						float3 viewDir = normalize(_WorldSpaceCameraPos - mul(modelMatrix, v.vertex).xyz);
						float viewN = abs(dot(viewDir, normal));
						float edge = clamp(0.1f / viewN, 0, 1);
						float edge2 = 1 - clamp(0.5f / viewN, 0, 1);

						o.color.rg = float2(edge, edge2);
						o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);  // Need to manually transform uv here, as we choose not to use 'uv' prefix for this texcoord.

						float3 vertexInWorldSpace = mul(unity_ObjectToWorld, v.vertex);
						// Shadow culling.
						float cameraDistasne = distance(vertexInWorldSpace, _WorldSpaceCameraPos.xyz);

						// Coin Luminance removes shadow.
						o.waterMulDistance.w = clamp((cameraDistasne - Terrain_ShadowDistance + 2), 0, 1); // * Fade

						// Water Level Multiplier.
						float _waterLevelAnimation = _SinTime.w * _WaterLevelAnimation;
						float _waterLevelMask = (1 - clamp(((vertexInWorldSpace.y - _WaterLevel) * 0.8f + _waterLevelAnimation), 0, 1)) * 0.7f;

						o.waterMulDistance.rgb = lerp(float3(1,1,1), _WaterLevelModifier, _waterLevelMask);

						float4 pos = UnityObjectToClipPos(v.vertex);

						TRANSFER_SHADOW(o);

						o.vertex = pos;

						return o;
					}

					fixed4 frag(Input i) : SV_Target
					{
						// Sample Terrain.
						fixed4 _mixedDiffuse = tex2D(_MainTex, i.uv);

						// Shade ambient.
						float4 _ambientDiffuse = i.waterMulDistance.rgbb * _mixedDiffuse * Terrain_ColorMultiplier + Terrain_ColorAddition + (i.color.r * Terrain_ColorEdge) - (i.color.g * Terrain_ColorEdge);

							// Sample Shadow and cull by distance.
							fixed  _attenShadow = SHADOW_ATTENUATION(i);
							fixed _shadow = clamp((1 - _attenShadow) - i.waterMulDistance.w,0,1);

							half3 _shadowEffect = lerp(1, Terrain_ColorShadow, _shadow);

							float3 _color = _ambientDiffuse.rgb * _shadowEffect;

						return float4(_color, 1);
					}
				ENDCG
			}
		}

			FallBack "VertexLit"
}
