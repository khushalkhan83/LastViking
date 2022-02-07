Shader "Surv/ToolsMed"
{
    Properties
    {
        //_Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BumpMap("Normalmap", 2D) = "bump" {}
		[Toggle] _ADDLIGHT("Add White ", Float) = 1

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "ForceNoShadowCasting" = "True" }
        LOD 2

        CGPROGRAM
		#pragma surface surf Lambert nodirlightmap nolightmap nofog noforwardadd //fullforwardshadows
		#pragma shader_feature _ADDLIGHT_ON
        #pragma target 3.0

        sampler2D _MainTex;
		sampler2D _BumpMap;
		//fixed4 _Color;

        struct Input
        {
            float2 uv_MainTex;
			float2 uv_BumpMap;
        };
			   
		void surf(Input IN, inout SurfaceOutput o) 
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);			
			o.Albedo = c.rgb * 0.85;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
        }
        ENDCG
    }
    FallBack "Surv/EnvironmentSimple"
}
