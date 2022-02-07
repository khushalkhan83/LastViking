// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Surv/Ocean"
{
	Properties 
	{
		_Shininess ("Sun Shininess", Range (2.0, 500.0)) = 200.0
		_GerstnerIntensity("Per vertex displacement", Float) = 1.0
		_GAmplitude ("Wave Amplitude", Vector) = (0.3 ,0.35, 0.25, 0.25)
		_GFrequency ("Wave Frequency", Vector) = (1.3, 1.35, 1.25, 1.25)
		_GSteepness ("Wave Steepness", Vector) = (1.0, 1.0, 1.0, 1.0)
		_GSpeed ("Wave Speed", Vector) = (1.2, 1.375, 1.1, 1.5)
		_GDirectionAB ("Wave Direction", Vector) = (0.3 ,0.85, 0.85, 0.25)
		_GDirectionCD ("Wave Direction", Vector) = (0.1 ,0.9, 0.5, 0.5)
	}

CGINCLUDE

	#include "UnityCG.cginc"
	#include "WaterInclude.cginc"

	struct appdata
	{
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};

	// edge & shore fading
	uniform float4 _InvFadeParemeter;

	// specularity
	uniform float _Shininess;

	uniform float4 _GAmplitude;
	uniform float4 _GFrequency;
	uniform float4 _GSteepness;
	uniform float4 _GSpeed;
	uniform float4 _GDirectionAB;
	uniform float4 _GDirectionCD;
	
	float4 Ocean_ColorBase, Ocean_ColorReflection;

	struct v2f
	{
		float4 pos : SV_POSITION;
		float4 color : COLOR;

		UNITY_FOG_COORDS(1)
	};

	void GerstnerMod 
	(	out half3 offs, 
		out half3 nrml,
		half3 vtx,
		half3 tileableVtx, 
		half4 amplitude,
		half4 frequency,
		half4 steepness, 
		half4 speed,
		half4 directionAB,
		half4 directionCD 
	) 
	{
			offs = GerstnerOffset4(tileableVtx.xz, steepness, amplitude, frequency, speed, directionAB, directionCD);
			nrml = GerstnerNormal4(tileableVtx.xz + offs.xz, amplitude, frequency, speed, directionAB, directionCD);		
	}
	
	v2f vert(appdata_full v)
	{
		v2f o;
		
		UNITY_SETUP_INSTANCE_ID(v);

		half3 worldSpaceVertex = mul(unity_ObjectToWorld, (v.vertex)).xyz;
		half3 vtxForAni = (worldSpaceVertex).xzz;

		half amplitudeMask = v.color.r;
		half4 amplitudeMod = _GAmplitude * amplitudeMask;

		half3 nrml;
		half3 offsets;
		GerstnerMod(offsets, nrml, v.vertex.xyz, vtxForAni, amplitudeMod, _GFrequency, _GSteepness, _GSpeed, _GDirectionAB, _GDirectionCD);

		v.vertex.xyz += offsets;
	
		o.pos = UnityObjectToClipPos(v.vertex);

		// Specular.
		float4x4 modelMatrix = unity_ObjectToWorld;
        float3x3 modelMatrixInverse = unity_WorldToObject;

        float3 viewDirection = normalize(_WorldSpaceCameraPos - mul(modelMatrix, v.vertex).xyz);
        float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);

		float3 specularReflection = unity_AmbientEquator * pow(max(0.0, dot(
                  reflect(-lightDirection, nrml),
                  viewDirection)), _Shininess);

		// Fresnel.
		float viewDot = abs(dot(viewDirection, nrml));

		float edge = clamp(min(1.0, 0.05f / viewDot), 0, 1);
		float edge2 = 1 - clamp(min(1.0, 0.4f / viewDot), 0, 1);

		specularReflection -= edge2 * 0.3f;
		specularReflection += edge * 0.23f;

		// Transparency.
		float cameraDistasne = distance(worldSpaceVertex, _WorldSpaceCameraPos.xyz);
		half distanceMask = cameraDistasne * 0.03f;

		half viewTransparency = 1. - 5 / max(6, cameraDistasne + 4);
		half transparency = min(viewTransparency, v.color.b);

		// Final.
		float3 finalColor = lerp(Ocean_ColorBase, Ocean_ColorReflection, offsets.y * 0.3f + 0.8f);
		finalColor += specularReflection;

		o.color = float4(finalColor, transparency);

		UNITY_TRANSFER_FOG(o, o.pos);
		UNITY_APPLY_FOG_COLOR(o.fogCoord, o.color, unity_FogColor + 0.05f);

		return o;
	}

	half4 frag( v2f i ) : SV_Target
	{
		return i.color;
	}
	
ENDCG

Subshader
{
	Tags 
	{
		"RenderType"="Transparent"
		"DisableBatching"= "True"
	}
	
	ColorMask RGB
	
	Pass 
	{
		Blend SrcAlpha OneMinusSrcAlpha
		ZTest Less
		ZWrite On
		Cull Back
			
		CGPROGRAM
		
			#pragma target 3.0
		
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
	
		ENDCG
	}
}


Fallback "Transparent/Diffuse"
}
