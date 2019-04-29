Shader "BToolkit/Snow"
{
    Properties {
       _MainTex ("Texture", 2D) = "white" {}
       _BumpMap ("Normal", 2D) = "bump" {}
       _Snow ("Snow Level", Range(0,1) ) = 0
       _SnowColor ("Snow Color", Color) = (1,1,1)
       _SnowDirection ("Snow Direction", Vector) = (0,1,0)
    }
    SubShader {
      Tags { "RenderType" = "Opaque" }
      CGPROGRAM
      #pragma surface surf Standard fullforwardshadows
      struct Input {
          float2 uv_MainTex;
          float2 uv_BumpMap;
          float3 worldNormal; INTERNAL_DATA
      };
      float4 _Color;
      sampler2D _MainTex;
      sampler2D _BumpMap;
      float _Snow;
      float4 _SnowColor;
      float4 _SnowDirection;
      void surf (Input IN, inout SurfaceOutputStandard o) {
          if (dot(WorldNormalVector(IN, o.Normal), _SnowDirection.xyz) > lerp(1,-1,_Snow)) {
              o.Albedo = _SnowColor.rgb;
          } else {
              o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
          }
          o.Normal = UnpackNormal(tex2D (_BumpMap, IN.uv_BumpMap));
      }
      ENDCG
    } 

    Fallback "Diffuse"
}
