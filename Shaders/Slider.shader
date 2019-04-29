// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "BToolkit/Slider" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB)", 2D) = "white" {}
	    _Cutoff("Cut Off Value", float) = 0.15
		_Value("Value", Range(0,1)) = 1
	}
	SubShader{
		Tags{ "RenderType" = "Opaque" "Queue" = "Geometry+1001" }
		ZWrite Off

		Pass{
		    AlphaTest Greater[_Cutoff]
		    CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
		    
			uniform float4 _Color;
		    uniform sampler2D _MainTex;
	        uniform float _Cutoff;
			uniform float _Value;
		    
	        struct v2f{
	        	float2 uv : TEXCOORD;
	        	float4 pos : SV_POSITION;
	        };
	        
	        v2f vert(appdata_base IN){
	        	v2f o;
	        	o.uv = IN.texcoord;
	        	o.pos = UnityObjectToClipPos(IN.vertex);
	        	return o;
	        }
	        
	        float4 frag(v2f v) : COLOR{
	        	float4 c = tex2D(_MainTex, v.uv)*_Color;
				if (c.a < _Cutoff) {
					discard;
				}
				if (v.uv.x < _Value) {
					c.a = 1;
				} else {
					discard;
				}
	        	return c;
	        }
		    ENDCG
	    }
	}
	FallBack "Diffuse"
}