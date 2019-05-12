Shader "BToolkit/GrayToAlpha"
{
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("MainTex", 2D) = "white" {}
		_AlphaTex ("AlphaTex", 2D) = "white" {}
	}
	SubShader {
	    Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert alpha
        
        fixed4 _Color;
        sampler2D _MainTex;
        sampler2D _AlphaTex;
        
        struct Input {
            float2 uv_MainTex;
        };
        
        void surf (Input IN, inout SurfaceOutput o) {
            fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 alphaTex = tex2D(_AlphaTex, IN.uv_MainTex);
            o.Albedo = mainTex.rgb * _Color.rgb;
            o.Alpha = alphaTex.r;
        }
        ENDCG
	}
	FallBack "Diffuse"
}
