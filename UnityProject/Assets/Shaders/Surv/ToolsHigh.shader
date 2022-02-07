Shader "Surv/ToolsHigh"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
		_GlossMapScale("Smoothness Scale", Range(0.0, 1.0)) = 1.0
		[Enum(Metallic Alpha,0,Albedo Alpha,1)] _SmoothnessTextureChannel("Smoothness texture channel", Float) = 0
		[Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
		_MetallicGlossMap("Metallic", 2D) = "white" {}
		[ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
		[ToggleOff] _GlossyReflections("Glossy Reflections", Float) = 1.0
		[Normal] _BumpMap("Normal Map", 2D) = "bump" {}
		_BumpScale("Scale", Float) = 1.0// Blending state
		[HideInInspector] _EmissionColor("Color", Color) = (0,0,0)
		// Blending state

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
		Tags { "RenderType" = "Opaque" "PerformanceChecks" = "False"  "ForceNoShadowCasting" = "True" }

		LOD 3
		// ------------------------------------------------------------------
		//  Base forward pass (directional light, emission, lightmaps, ...)
		Pass
		{
			Name "FORWARD"
			Tags { "LightMode" = "ForwardBase"  "RenderType" = "Opaque" "PerformanceChecks" = "False"  }

			//Blend[_SrcBlend][_DstBlend]
			ZWrite[_ZWrite]

			CGPROGRAM
				#pragma target 3.0
				#pragma include_renderers d3d11_9x glcore gles gles3 metal 

				// -------------------------------------

				#pragma shader_feature _NORMALMAP
				//#pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
				#pragma shader_feature_local _METALLICGLOSSMAP
				#pragma shader_feature_local _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
				#pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF
				#pragma shader_feature_local _GLOSSYREFLECTIONS_OFF
				#pragma multi_compile_fwdbase
				//#pragma multi_compile_fog
				//#pragma multi_compile_instancing
				// Uncomment the following line to enable dithering LOD crossfade. Note: there are more in the file to uncomment for other passes.
				//#pragma multi_compile _ LOD_FADE_CROSSFADE

				#pragma vertex vertBase
				#pragma fragment fragBase
				#include "UnityStandardCoreForward.cginc"
			ENDCG
		}

		// ------------------------------------------------------------------
		//  Additive forward pass (one light per pass)
/*
		Pass
		{
			Name "FORWARD_DELTA"
			Tags { "LightMode" = "ForwardAdd" }
			Blend[_SrcBlend] One
			Fog { Color(0,0,0,0) } // in additive pass fog should be black
			ZWrite Off
			ZTest LEqual

			CGPROGRAM
				#pragma target 3.0

				// -------------------------------------

				#pragma shader_feature _NORMALMAP
				#pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
				#pragma shader_feature_local _METALLICGLOSSMAP
				#pragma shader_feature_local _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
				#pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF

				#pragma multi_compile_fwdadd_fullshadows
				#pragma multi_compile_fog
				// Uncomment the following line to enable dithering LOD crossfade. Note: there are more in the file to uncomment for other passes.
				//#pragma multi_compile _ LOD_FADE_CROSSFADE

				#pragma vertex vertAdd
				#pragma fragment fragAdd
				#include "UnityStandardCoreForward.cginc"

			ENDCG
		}*/
		// ------------------------------------------------------------------
		//  Shadow rendering pass
/*		Pass
		{
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }

			ZWrite On ZTest LEqual

			CGPROGRAM
				#pragma target 3.0

				// -------------------------------------

				#pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
				#pragma shader_feature_local _METALLICGLOSSMAP
				#pragma shader_feature_local _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
				#pragma multi_compile_shadowcaster
				#pragma multi_compile_instancing
				// Uncomment the following line to enable dithering LOD crossfade. Note: there are more in the file to uncomment for other passes.
				//#pragma multi_compile _ LOD_FADE_CROSSFADE

				#pragma vertex vertShadowCaster
				#pragma fragment fragShadowCaster

				#include "UnityStandardShadow.cginc"
			ENDCG
		}
*/
		// ------------------------------------------------------------------
		//  Deferred pass
/*
		Pass
		{
			Name "DEFERRED"
			Tags { "LightMode" = "Deferred" }

			CGPROGRAM
				#pragma target 3.0
				#pragma exclude_renderers nomrt

				// -------------------------------------

				#pragma shader_feature _NORMALMAP
				#pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
				#pragma shader_feature_local _METALLICGLOSSMAP
				#pragma shader_feature_local _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
				#pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF

				#pragma multi_compile_prepassfinal
				#pragma multi_compile_instancing
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
/*
		Pass
		{
			Name "META"
			Tags { "LightMode" = "Meta" }

			Cull Off

			CGPROGRAM
				#pragma vertex vert_meta
				#pragma fragment frag_meta

				#pragma shader_feature_local _METALLICGLOSSMAP
				#pragma shader_feature_local _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
				#pragma shader_feature EDITOR_VISUALIZATION

				#include "UnityStandardMeta.cginc"
			ENDCG
		}
		*/
	}

	FallBack "Surv/Tools"
	CustomEditor "ToolHighShaderEditor"
}
