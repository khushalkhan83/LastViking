
	// Note: Sometimes Unity require restart to change assigned array size.
	#define LIGHTS_COUNT 10
	#define LIGHTS_SPREAD 0.7f
	#define LIGHTS_CELING 0.5f
	
	float4 Micro_Lights[LIGHTS_COUNT];
	float3 Micro_LightColor;

	float sqrMagnitude(half3 Vector)
	{
		return Vector.x * Vector.x + Vector.y * Vector.y +  Vector.z * Vector.z;
	}

	float3 calculateMicroLight(half3 position)
	{
		#if defined(MICRO_LIGHT_ON)
//			float _lightStrength = 0;
//	
//			for (int i = 0; i < LIGHTS_COUNT; i++)
//			{
//				half3 _difference = Micro_Lights[i] - position;
//				half _intensity = Micro_Lights[i].a;
//
//				_lightStrength += clamp(1- sqrMagnitude(_difference) * LIGHTS_SPREAD, 0, 1) * _intensity;
//			}
//	
//			return clamp(_lightStrength, 0, LIGHTS_CELING) * Micro_LightColor;
			return 0;
		#else
			return 0;
		#endif
	}

	float3 blednInMicroLight(float3 color, float3 microLightColor)
	{
//		#if defined(MICRO_LIGHT_ON)
//			return 2 * color * microLightColor + microLightColor * 0.2f; 
//		#endif

		return 0;
	}
