#ifndef VACUUM_SHADERS_T2M_VERTEXLIT_CGINC
#define VACUUM_SHADERS_T2M_VERTEXLIT_CGINC

#include "../cginc/T2M_Variables.cginc"
#include "../cginc/CurvedWorld.cginc"


float4 _V_T2M_Control_ST;
#ifdef V_T2M_2_CONTROL_MAPS
	float4 _V_T2M_Control2_ST; 
#endif

float _V_T2M_Lightmap_Multiplier;



		int4 unity_VertexLightParams; // x: light count, y: zero, z: one (y/z needed by d3d9 vs loop instruction)


		// Compute illumination from one light, given attenuation
		half3 computeLighting (int idx, half3 dirToLight, half3 eyeNormal, half3 viewDir, half4 diffuseColor, half shininess, half atten, inout half3 specColor) 
		{
		half NdotL = max(dot(eyeNormal, dirToLight), 0.0);
		// diffuse
		half3 color = NdotL * diffuseColor.rgb * unity_LightColor[idx].rgb;
		return color * atten;
		}

		// Compute attenuation & illumination from one light
		half3 computeOneLight(int idx, float3 eyePosition, half3 eyeNormal, half3 viewDir, half4 diffuseColor, half shininess, inout half3 specColor) 
		{
		  float3 dirToLight = unity_LightPosition[idx].xyz;
		  half att = 1.0;
		  #if defined(POINT) || defined(SPOT)
			dirToLight -= eyePosition * unity_LightPosition[idx].w;
			// distance attenuation
			float distSqr = dot(dirToLight, dirToLight);
			att /= (1.0 + unity_LightAtten[idx].z * distSqr);
			if (unity_LightPosition[idx].w != 0 && distSqr > unity_LightAtten[idx].w) att = 0.0; // set to 0 if outside of range
			distSqr = max(distSqr, 0.000001); // don't produce NaNs if some vertex position overlaps with the light
			dirToLight *= rsqrt(distSqr);
			#if defined(SPOT)
			  // spot angle attenuation
			  half rho = max(dot(dirToLight, unity_SpotDirection[idx].xyz), 0.0);
			  half spotAtt = (rho - unity_LightAtten[idx].x) * unity_LightAtten[idx].y;
			  att *= saturate(spotAtt);
			#endif
		  #endif
		  att *= 0.5; // passed in light colors are 2x brighter than what used to be in FFP
		  return min (computeLighting (idx, dirToLight, eyeNormal, viewDir, diffuseColor, shininess, att, specColor), 1.0);
		}


		// vertex shader input data
		struct appdata 
		{
		  float4 vertex : POSITION;
		  float3 normal : NORMAL;
		  float3 uv0 : TEXCOORD0;
		  float3 uv1 : TEXCOORD1;
		  UNITY_VERTEX_INPUT_INSTANCE_ID
		};

		// vertex-to-fragment interpolators
		struct v2f  
		{
		  float4 pos : SV_POSITION;

		  fixed4 color : COLOR0;
		  float2 uvLM : TEXCOORD0;
		  #if USING_FOG
			fixed fog : TEXCOORD1;
		  #endif
		  

		  float2 uv_V_T2M_Control : TEXCOORD2;
		  #ifdef V_T2M_2_CONTROL_MAPS
		    float2 uv_V_T2M_Control2 : TEXCOORD3;
		  #endif

		  UNITY_VERTEX_OUTPUT_STEREO
		};



		// vertex shader
		v2f vert (appdata v) 
		{
			  v2f o = (v2f)0;
			  UNITY_SETUP_INSTANCE_ID(v);
			  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);


			  //CurvedWorld vertex transform
			  CURVED_WORLD_TRANSFORM_POINT(v.vertex);

			  // transform position
			  o.pos = UnityObjectToClipPos(v.vertex);

			  o.uv_V_T2M_Control = v.uv0.xy * _V_T2M_Control_ST.xy + _V_T2M_Control_ST.zw;
			  #ifdef V_T2M_2_CONTROL_MAPS
				o.uv_V_T2M_Control2 = v.uv0.xy * _V_T2M_Control2_ST.xy + _V_T2M_Control2_ST.zw;
			  #endif


			  #if defined(PASS_VERTEXLIT)

				half4 color = half4(0,0,0,1.1);
				float3 eyePos = UnityObjectToViewPos(v.vertex.xyz); //mul (UNITY_MATRIX_MV, float4(v.vertex.xyz,1)).xyz;
				half3 eyeNormal = normalize (mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal).xyz);
				half3 viewDir = 0.0;
				// lighting
				half3 lcolor = half4(0,0,0,1).rgb + half4(1,1,1,1).rgb * glstate_lightmodel_ambient.rgb;
				half3 specColor = 0.0;
				half shininess = 0 * 128.0;
				for (int il = 0; il < LIGHT_LOOP_LIMIT; ++il) {
					lcolor += computeOneLight(il, eyePos, eyeNormal, viewDir, half4(1,1,1,1), shininess, specColor);
				}
				color.rgb = lcolor.rgb;
				color.a = half4(1,1,1,1).a;
				o.color = saturate(color);


				// fog
			    #if USING_FOG
					float fogCoord = length(eyePos.xyz); // radial fog distance
					UNITY_CALC_FOG_FACTOR_RAW(fogCoord);
					o.fog = saturate(unityFogFactor);
			  	#endif

			  #elif defined(PASS_VERTEXLIT_LM)

				o.uvLM = v.uv1.xy * unity_LightmapST.xy + unity_LightmapST.zw;

				#if USING_FOG
					float3 eyePos = UnityObjectToViewPos(v.vertex);
					float fogCoord = length(eyePos.xyz);
					UNITY_CALC_FOG_FACTOR_RAW(fogCoord);
					o.fog = saturate(unityFogFactor);
				#endif

			  #endif			  
			  
			  return o;
		}



		// fragment shader
		fixed4 frag (v2f IN) : SV_Target 
		{
			half4 splat_control = tex2D (_V_T2M_Control, IN.uv_V_T2M_Control);		

			fixed4 mainTex  = splat_control.r * tex2D (_V_T2M_Splat1, IN.uv_V_T2M_Control * _V_T2M_Splat1_uvScale);
				mainTex += splat_control.g * tex2D (_V_T2M_Splat2, IN.uv_V_T2M_Control * _V_T2M_Splat2_uvScale);
			
			#ifdef V_T2M_3_TEX
				mainTex += splat_control.b * tex2D (_V_T2M_Splat3, IN.uv_V_T2M_Control * _V_T2M_Splat3_uvScale);
			#endif
			#ifdef V_T2M_4_TEX
				mainTex += splat_control.a * tex2D (_V_T2M_Splat4, IN.uv_V_T2M_Control * _V_T2M_Splat4_uvScale);
			#endif


			#ifdef V_T2M_2_CONTROL_MAPS
				half4 splat_control2 = tex2D (_V_T2M_Control2, IN.uv_V_T2M_Control2);

				mainTex.rgb += tex2D (_V_T2M_Splat5, IN.uv_V_T2M_Control2 * _V_T2M_Splat5_uvScale) * splat_control2.r;

				#ifdef V_T2M_6_TEX
					mainTex.rgb += tex2D (_V_T2M_Splat6, IN.uv_V_T2M_Control2 * _V_T2M_Splat6_uvScale) * splat_control2.g;
				#endif

				#ifdef V_T2M_7_TEX
					mainTex.rgb += tex2D (_V_T2M_Splat7, IN.uv_V_T2M_Control2 * _V_T2M_Splat7_uvScale) * splat_control2.b;
				#endif

				#ifdef V_T2M_8_TEX
					mainTex.rgb += tex2D (_V_T2M_Splat8, IN.uv_V_T2M_Control2 * _V_T2M_Splat8_uvScale) * splat_control2.a;
				#endif
			#endif

			mainTex.rgb *= _Color.rgb;





			fixed4 c = 0;
				  
			
			#if defined(PASS_VERTEXLIT)

				c.rgb = mainTex.rgb * IN.color.rgb;
				c.rgb *= 2;

			#elif defined(PASS_VERTEXLIT_LM)

				half3 lm = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, IN.uvLM.xy));
				c.rgb = mainTex * lm * _V_T2M_Lightmap_Multiplier;

			#endif


			c.a = 1;

			// fog
			#if USING_FOG
				c.rgb = lerp (unity_FogColor.rgb, c.rgb, IN.fog);
			#endif

			return c;
		}



#endif