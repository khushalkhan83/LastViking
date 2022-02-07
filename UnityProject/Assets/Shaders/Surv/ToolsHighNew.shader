Shader "Surv/ToolsHighNew"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BumpMap("Normalmap", 2D) = "bump" {}
		_MetallicGlossMap("Metal (R) Roughness (A)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0, 1)) = 0.5
	    _Metallic("Metallic", Range(0, 1)) = 0.5
		_Color("Main Color", Color) = (1,1,1,1)

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "ForceNoShadowCasting" = "True" }
        LOD 3

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard nodirlightmap nolightmap nofog fullforwardshadows 

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _MetallicGlossMap;
		float _Glossiness;
		float _Metallic;
		fixed4 _Color;

        struct Input
        {
            float2 uv_MainTex;
			float2 uv_BumpMap;
			float2 uv_MetallicGlossMap;
			float4 color : COLOR;
        };


        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			fixed4 m = tex2D (_MetallicGlossMap, IN.uv_MetallicGlossMap);
			o.Albedo = c.rgb;

            // Metallic and smoothness come from slider variables
            o.Metallic = m.r * _Metallic;
            o.Smoothness = (m.a * _Glossiness);
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
          //  o.Alpha = c.a;
			//o.Occlusion = m.b;
        }
        ENDCG
    }
    FallBack "Surv/ToolsMed"
}
