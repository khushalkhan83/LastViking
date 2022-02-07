Shader "Unlit/WaterLow"
{
    Properties
    {
		_Color("Color", Color) = (0, 0, 0, 1)
		_ViewDistance("ViewDistance", float) = 1
		_LightBlend("LightBlend", range(0, 1)) = 0.5
    }

	CGINCLUDE
		
        #include "UnityCG.cginc"
		
        struct appdata
        {
            float4 vertex : POSITION;
        };
		
        struct v2f
        {
			float4 vertex : SV_POSITION;
			fixed4 color : COLOR;
			float4 worldPosition : COLOR1;
		};
		
		uniform sampler2D _CameraDepthTexture;
		
		uniform fixed4 Ocean_ColorBase;
		uniform fixed4 _Color;
		uniform half _ViewDistance;
		uniform fixed _LightBlend;
		
        v2f vert (appdata v)
        {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
			o.color = fixed4(lerp(_Color.rgb, Ocean_ColorBase.rgb, _LightBlend), _Color.a);
			o.worldPosition = mul(unity_ObjectToWorld, v.vertex);
            return o;
        }
		
        fixed4 frag (v2f i) : COLOR
        {
			half distance = length(_WorldSpaceCameraPos.xyz - i.worldPosition.xyz);
			fixed dy = clamp(0, 2, _WorldSpaceCameraPos.y - i.worldPosition.y) / 2;

			i.color.a = lerp(i.color.a, 1, (distance / dy) / _ViewDistance);
			return i.color;
        }
		
	ENDCG
	
    SubShader
    {
        Tags 
		{
			"RenderType" = "Transparent"
			"Queue" = "Transparent+1"
		}

		Blend SrcAlpha OneMinusSrcAlpha
		Cull Back

        Pass
        {
            CGPROGRAM

				#pragma vertex vert
				#pragma fragment frag

            ENDCG
        }
    }
}
