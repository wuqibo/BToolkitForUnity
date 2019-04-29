Shader "BToolkit/InLight"
{
    Properties{
        _MainTex("Main Tex",2D)="white"{}
        _RimColor("Rim Color",Color)=(0.26,0.19,0.16,0.0)
        _RimPower("Rim Power",Range(0,10))=3.0
    }

    SubShader{

        Tags{"RenderType"="Transparent"}

        CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _MainTex;
        float4 _RimColor;
        float _RimPower;

        struct Input{
            float2 uv_MainTex;
            float3 viewDir;
        };

        void surf(Input i,inout SurfaceOutput o){
            float4 c = tex2D(_MainTex,i.uv_MainTex);
            o.Albedo = c.rgb;

            half rim = 1.0-saturate(dot(normalize(i.viewDir),o.Normal));
            o.Emission = _RimColor.rgb * pow(rim, _RimPower);
        }

        ENDCG
    }

    Fallback "Diffuse"
}
