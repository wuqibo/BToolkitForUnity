Shader "BToolkit/CubeMap" {  
    Properties {  
	    _Color ("Color", Color) = (0,0,0,1)
        _MainTex("Base (RGB)",2D) = "white" {}
        _Cube("CubeMap",CUBE) = ""{}
        _ReflAmount("Reflection Amount", Range(0.01, 1)) = 0.5
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.5
    }  
    SubShader {  
        Tags { "RenderType"="Opaque" }
		LOD 200
		Cull Off
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows 

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0
          
        sampler2D _MainTex;
        samplerCUBE _Cube;
        float _ReflAmount;
        half _Glossiness;
        half _Metallic;
		float4 _Color;
         
        struct Input {
            float2 uv_MainTex;
            float3 worldRefl;
        };
        void surf (Input IN, inout SurfaceOutputStandard o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex)*_Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
            o.Emission=texCUBE(_Cube, IN.worldRefl).rgb*_ReflAmount;
        }
        ENDCG  
    }  
    FallBack "Diffuse"  
}