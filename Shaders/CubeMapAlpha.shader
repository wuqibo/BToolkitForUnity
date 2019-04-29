Shader "BToolkit/CubeMapAlpha" {
	Properties{
		_Color("Color", Color) = (0,0,0,0.5)
		_MainTex("Base (RGB)", 2D) = "white" {}
	    _Cube("CubeMap",CUBE) = ""{}
		_ReflAmount("Reflection Amount", Range(0.01, 1)) = 1
	}
	SubShader{
		Tags{ "RenderType" = "Opaque" "Queue" = "Transparent" }
		LOD 200

		CGPROGRAM
        #pragma surface surf Lambert alpha  

		float4 _Color;
		sampler2D _MainTex;
		samplerCUBE _Cube;
		float _ReflAmount;

		struct Input {
			float2 uv_MainTex;
			float3 worldRefl;
		};
	    
	    void surf(Input IN, inout SurfaceOutput o) {
	    	half4 c = tex2D(_MainTex, IN.uv_MainTex)*_Color;
	    	o.Albedo = c.rgb;
			o.Emission = texCUBE(_Cube, IN.worldRefl).rgb*_ReflAmount;
	    	o.Alpha = c.a;
	    }
	    ENDCG
	}
	FallBack "Diffuse"
}
