Shader "Surv/Tools"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0.0, 1.0)) = 0
		[Normal] _BumpMap("Normal Map", 2D) = "bump" {}
		[HideInInspector] _BumpScale("Scale", Float) = 1.0// Blending state

		[HideInInspector] _EmissionColor("Color", Color) = (0,0,0)
		[HideInInspector] _Mode("__mode", Float) = 0.0
		[HideInInspector] _SrcBlend("__src", Float) = 1.0
		[HideInInspector] _DstBlend("__dst", Float) = 0.0
		[HideInInspector] _ZWrite("__zw", Float) = 1.0
	}

		CGINCLUDE
    #define UNITY_SETUP_BRDF_INPUT MetallicSetup
			ENDCG
			
	SubShader
	{
		Tags
		{
			"RenderType" = "Opaque"
			"PerformanceChecks" = "False"
		}

		LOD 2

		// ------------------------------------------------------------------
		//  Base forward pass (directional light, emission, lightmaps, ...)
		Pass
		{
			Name "FORWARD"
			Tags
			{
				"LightMode" = "ForwardBase"
			}

			//Blend[_SrcBlend][_DstBlend]
			ZWrite[_ZWrite]

			CGPROGRAM
			#pragma target 2.0

		// -------------------------------------


		#pragma shader_feature _NORMALMAP
		//#pragma shader_feature _METALLICGLOSSMAP

		#pragma multi_compile_fwdbase
		//#pragma multi_compile_fog
		// Uncomment the following line to enable dithering LOD crossfade. Note: there are more in the file to uncomment for other passes.
		//#pragma multi_compile _ LOD_FADE_CROSSFADE

		#pragma vertex vertBase
		#pragma fragment fragBase
		#include "UnityStandardCoreForward.cginc"
	ENDCG
}
/*
		// ------------------------------------------------------------------
		//  Additive forward pass (one light per pass)
		Pass
		{
			Name "FORWARD_DELTA"
			Tags
			{
				"LightMode" = "ForwardAdd"
			}

			//Blend[_SrcBlend] One
			//Fog { Color(0,0,0,0) } // in additive pass fog should be black
			ZWrite Off
			ZTest LEqual

			CGPROGRAM
				#pragma target 3.0

		// -------------------------------------

		#pragma shader_feature _NORMALMAP

		#pragma multi_compile_fwdadd_fullshadows
		#pragma multi_compile_fog

		// Uncomment the following line to enable dithering LOD crossfade. Note: there are more in the file to uncomment for other passes.
		//#pragma multi_compile _ LOD_FADE_CROSSFADE

		#pragma vertex vertAdd
		#pragma fragment fragAdd
		#include "UnityStandardCoreForward.cginc"

	ENDCG

}

		// ------------------------------------------------------------------
		//  Shadow rendering pass
		Pass
		{
			Name "ShadowCaster"
			Tags
			{
				"LightMode" = "ShadowCaster"
			}
			ZWrite On
			ZTest LEqual

			CGPROGRAM
				#pragma target 3.0

			//	#pragma multi_compile_shadowcaster
		// Uncomment the following line to enable dithering LOD crossfade. Note: there are more in the file to uncomment for other passes.
		//#pragma multi_compile _ LOD_FADE_CROSSFADE

		#pragma vertex vertShadowCaster
		#pragma fragment fragShadowCaster
		#include "UnityStandardShadow.cginc"
	ENDCG
}

		// ------------------------------------------------------------------
		//  Deferred pass
		Pass
		{
			Name "DEFERRED"
			Tags
			{
				"LightMode" = "Deferred"
			}

			CGPROGRAM

				#pragma target 3.0
				#pragma exclude_renderers nomrt

		// -------------------------------------

		#pragma shader_feature _NORMALMAP

		#pragma multi_compile_prepassfinal
		// Uncomment the following line to enable dithering LOD crossfade. Note: there are more in the file to uncomment for other passes.
		//#pragma multi_compile _ LOD_FADE_CROSSFADE

		#pragma vertex vertDeferred
		#pragma fragment fragDeferred

		#include "UnityStandardCore.cginc"

	ENDCG
}
*/
		// ------------------------------------------------------------------
		// Extracts information for lightmapping, GI (emission, albedo, ...)
		// This pass it not used during regular rendering.
/*		Pass
		{
			Name "META"
			Tags
			{
				"LightMode" = "Meta"
			}

			Cull Off

			CGPROGRAM

				#pragma vertex vert_meta
				#pragma fragment frag_meta

				#pragma shader_feature EDITOR_VISUALIZATION

				#include "UnityStandardMeta.cginc"

			ENDCG
		}*/
	}

		FallBack "Surv/EnvironmentSimple"
}
