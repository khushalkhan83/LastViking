// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Surv/EnvironmentSimple" 
{

	Properties 
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_ShadowCull ("Shadow Culling", Float) = 0
	}

	SubShader 
	{
		Tags 
		{
			"Queue"="Geometry"
			"IgnoreProjector"="True"
			"RenderType"="Geometry"
			"LightMode" = "ForwardBase"
		}
		
		LOD 1

		Lighting On
		ZWrite On

		Pass
		{
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0
				#pragma multi_compile_fog
				#pragma multi_compile_fwdbase
				#pragma multi_compile_instancing
				#pragma shader_feature _NORMALMAP

				#include "UnityCG.cginc"
				#include "AutoLight.cginc"
				#include "Lighting.cginc"
				
				struct appdata_t
				{
					float4 vertex : POSITION;
					fixed2 texcoord : TEXCOORD0;
					fixed3 normal : NORMAL;
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};

				struct v2f 
				{
					float4 vertex : SV_POSITION;
					fixed2 texcoord : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					SHADOW_COORDS(2)
					UNITY_VERTEX_INPUT_INSTANCE_ID
					fixed4 color : COLOR;
				};

				sampler2D _MainTex;
				fixed4 _MainTex_ST;
				fixed4 Terrain_ColorMultiplier, Terrain_ColorAddition, Environment_Color, Terrain_ColorShadow;
				fixed3 Light_Direction, Light_Color;
				half Terrain_ShadowDistance;
				half _ShadowCull;

				v2f vert (appdata_t v)
				{
					v2f o;
					UNITY_SETUP_INSTANCE_ID(v);

					o.vertex = UnityObjectToClipPos(v.vertex);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					UNITY_TRANSFER_FOG(o,o.vertex);
					TRANSFER_SHADOW(o);

					fixed3 normalDirection = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);
					fixed3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
					half3 vertexInWorldSpace = mul(unity_ObjectToWorld, v.vertex);

					fixed diffuseReflection = Light_Color * max(0.0, dot(normalDirection, Light_Direction));

					fixed3 upNormal = dot(normalDirection, float3(0, 1, 0));
					fixed3 downNormal = dot(normalDirection, float3(0,-1,0));

					fixed3 skyMask = clamp(upNormal + 0.25f, 0.15f, 1.1f) - clamp((downNormal - 0.44f) * 4, 0, 0.1f);
					fixed3 groundMask = clamp(downNormal + 0.3f, 0.1f, 0.8f) - clamp((upNormal - 0.44f) * 4, 0, 0.1f);

					o.color.rgb = (skyMask * Environment_Color * 0.95f) + (groundMask * Terrain_ColorMultiplier * 0.6f) + (diffuseReflection * 1.1f);

				#if defined(SHADOWS_SCREEN)
					// Shadow culling.
					float cameraDistasne = distance(vertexInWorldSpace, _WorldSpaceCameraPos.xyz);
					o.color.a = clamp((cameraDistasne - Terrain_ShadowDistance + 2), _ShadowCull, 1);
				#endif

					return o;
				}

				fixed4 frag (v2f i) : SV_Target
				{
					fixed4 composed = tex2D(_MainTex, i.texcoord);
					composed.rgb = composed.rgb * i.color.rgb + Terrain_ColorAddition;

					UNITY_APPLY_FOG(i.fogCoord, composed);

				// Shadow variant.
				#if defined(SHADOWS_SCREEN)
					composed *= lerp(1, 0.3, clamp((1 - SHADOW_ATTENUATION(i)) - i.color.a, 0, 1));
				#endif

					return composed;
				}
				
			ENDCG
		}
	}

	FallBack "VertexLit"
}
