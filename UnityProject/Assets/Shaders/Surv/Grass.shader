// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Surv/Grass"
{
	Properties
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Wind("Wind", Vector) = (0,0,0,0)
	}

	SubShader 
	{
		Tags 
		{
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
		}
		
		LOD 1

		Pass 
		{
			
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

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
					float4 normal : NORMAL;
					float4 color : COLOR;
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};
				
				struct v2f
				{
					float4 vertex : SV_POSITION;
					float2 texcoord : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					UNITY_VERTEX_OUTPUT_STEREO
				};

				uniform sampler2D _MainTex;
				uniform float4 _MainTex_ST;
				uniform float4 Terrain_ColorMultiplier, Terrain_ColorAddition;
				uniform float4 _Wind;

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
					
					o.vertex = UnityObjectToClipPos(v.vertex + mul(unity_WorldToObject, float4(PlantAnimation(mul(unity_ObjectToWorld, v.vertex), v.color.r), 0)));
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

					UNITY_TRANSFER_FOG(o, o.vertex);
					return o;
				}

				fixed4 frag (v2f i) : SV_Target
				{
					fixed4 col = tex2D(_MainTex, i.texcoord);
					float4 _ambientColor = col * Terrain_ColorMultiplier + Terrain_ColorAddition;
					float4 finalColor = _ambientColor;
					
					finalColor.a = col.a;

					UNITY_APPLY_FOG(i.fogCoord, finalColor);

					return finalColor;
				}
			ENDCG
			Cull Off
		}
	}
		
	FallBack "VertexLit"
}