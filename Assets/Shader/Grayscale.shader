Shader "Hidden/GrayScale"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Weight("Weight", Range(0, 1)) = 1     // 0→원본, 1→완전 흑백
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
float4 _MainTex_ST;
float _Weight;

float4 frag(v2f_img i) : SV_Target
            {
                float4 col  = tex2D(_MainTex, i.uv);
float gray = dot(col.rgb, float3(0.299, 0.587, 0.114));
float3 gCol = float3(gray, gray, gray);
col.rgb = lerp(col.rgb, gCol, _Weight);   // 블렌드
return col;
            }
            ENDCG
        }
    }
}