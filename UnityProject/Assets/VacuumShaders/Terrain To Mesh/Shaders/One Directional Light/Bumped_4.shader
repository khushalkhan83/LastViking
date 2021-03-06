// VacuumShaders 2015
// https://www.facebook.com/VacuumShaders

Shader "VacuumShaders/Terrain To Mesh/One Directional Light/Bumped/4 Textures" 
{ 
	Properties     
	{
		_Color("Color", color) = (1, 1, 1, 1)
<<<<<<< Updated upstream
		[NoScaleOffset]_V_T2M_Control ("Control Map (RGBA)", 2D) = "black" {}
=======
		[NoScaleOffset] _V_T2M_Control ("Control Map (RGBA)", 2D) = "black" {}
>>>>>>> Stashed changes
		 
		//TTM				
		[V_T2M_Layer] _V_T2M_Splat1 ("Layer 1 (R)", 2D) = "white" {}
		[HideInInspector] _V_T2M_Splat1_uvScale("", float) = 1	
		[HideInInspector] _V_T2M_Splat1_bumpMap("", 2D) = ""{}	

		[V_T2M_Layer] _V_T2M_Splat2 ("Layer 2 (G)", 2D) = "white" {}
		[HideInInspector] _V_T2M_Splat2_uvScale("", float) = 1	
		[HideInInspector] _V_T2M_Splat2_bumpMap("", 2D) = ""{}	
		
		[V_T2M_Layer] _V_T2M_Splat3 ("Layer 3 (B)", 2D) = "white" {}
		[HideInInspector] _V_T2M_Splat3_uvScale("", float) = 1	
		[HideInInspector] _V_T2M_Splat3_bumpMap("", 2D) = ""{}	
		
		[V_T2M_Layer] _V_T2M_Splat4 ("Layer 4 (A)", 2D) = "white" {}
		[HideInInspector] _V_T2M_Splat4_uvScale("", float) = 1	
		[HideInInspector] _V_T2M_Splat4_bumpMap("", 2D) = ""{}		

		_V_T2M_Lightmap_Multiplier("Lightmap Multiplier", float) = 1


		//Fallback use only
		[NoScaleOffset]_MainTex ("BaseMap (Fallback use only!)", 2D) = "white" {}
	}
	 
	
	SubShader   
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		     
		Pass 
	    {   
			Name "FORWARD"
			Tags { "LightMode" = "ForwardBase" } 

			CGPROGRAM
			#pragma vertex vert 
	    	#pragma fragment frag		  
			#pragma multi_compile_fwdbase nodynlightmap nodirlightmap
			#pragma multi_compile_fog
			#pragma target 2.0
			#pragma exclude_renderers gles 
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"


			#define V_T2M_3_TEX
			#define V_T2M_4_TEX
			#define V_T2M_BUMP

			#include "../cginc/T2M_ODL.cginc" 

			ENDCG

		} //Pass

	} //SubShader
	 
<<<<<<< Updated upstream
	FallBack "Hidden/VacuumShaders/Fallback/VertexLit"
=======
	FallBack "VacuumShaders/Terrain To Mesh/Vertex Lit/4 Textures"
>>>>>>> Stashed changes

} //Shader
