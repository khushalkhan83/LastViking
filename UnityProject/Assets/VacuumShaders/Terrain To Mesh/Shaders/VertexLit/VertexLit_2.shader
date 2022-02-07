Shader "VacuumShaders/Terrain To Mesh/Vertex Lit/2 Textures" 
{
	Properties 
	{
		_Color("Color", color) = (1, 1, 1, 1)
		[NoScaleOffset] _V_T2M_Control ("Control Map (RGBA)", 2D) = "black" {}

		//TTM				
		[V_T2M_Layer] _V_T2M_Splat1 ("Layer 1 (R)", 2D) = "white" {}
		[HideInInspector] _V_T2M_Splat1_uvScale("", float) = 1	

		[V_T2M_Layer] _V_T2M_Splat2 ("Layer 2 (G)", 2D) = "white" {}
		[HideInInspector] _V_T2M_Splat2_uvScale("", float) = 1	

		_V_T2M_Lightmap_Multiplier("Lightmap Multiplier", float) = 1


		//Fallback use only
		[NoScaleOffset]_MainTex ("BaseMap (Fallback use only!)", 2D) = "white" {}
	}

 
	SubShader 
	{ 
		LOD 80
		Tags { "RenderType"="Opaque" }
	 
	Pass 
	{
		Tags { "LIGHTMODE"="Vertex" "RenderType"="Opaque" }
		
		CGPROGRAM		
		#pragma vertex vert
		#pragma fragment frag
		#pragma target 2.0
		#include "UnityCG.cginc"
		#pragma multi_compile_fog
		#define USING_FOG (defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2))

		// ES2.0/WebGL/3DS can not do loops with non-constant-expression iteration counts :(
		#if defined(SHADER_API_GLES)
		  #define LIGHT_LOOP_LIMIT 8
		#elif defined(SHADER_API_N3DS)
		  #define LIGHT_LOOP_LIMIT 4
		#else
		  #define LIGHT_LOOP_LIMIT unity_VertexLightParams.x
		#endif
		#define ENABLE_SPECULAR (!defined(SHADER_API_N3DS))

		// Compile specialized variants for when positional (point/spot) and spot lights are present
		#pragma multi_compile __ POINT SPOT

		#define PASS_VERTEXLIT  


		#include "../cginc/T2M_VertexLit.cginc" 		 

		
		ENDCG 
	}
 

	Pass 
	{
		Tags { "LIGHTMODE"="VertexLM" "RenderType"="Opaque" }
	  
		CGPROGRAM
		#include "HLSLSupport.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityShaderUtilities.cginc"
	


		#pragma vertex vert
		#pragma fragment frag
		#pragma target 2.0
		#include "UnityCG.cginc"
		#pragma multi_compile_fog
		#define USING_FOG (defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2))


		#define PASS_VERTEXLIT_LM  
		

		#include "../cginc/T2M_VertexLit.cginc" 	


		ENDCG
	 }

	} //SubShader

	FallBack "Hidden/VacuumShaders/Fallback/VertexLit"

} //Shader