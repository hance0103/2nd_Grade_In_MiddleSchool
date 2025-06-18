Shader "Custom/StencilWriter"
{
    SubShader
    {
        Tags { "Queue"="Geometry+1" } // 먼저 렌더
        ColorMask 0 // 색은 안 그리고
        ZWrite Off

        Stencil
        {
            Ref 1
            Comp Always
            Pass Replace
        }

        Pass {}
    }
}
