Shader "Custom/ShellShader" {
    SubShader {
        Tags { 
            "RenderType"="Opaque"
            "Queue"="Geometry"
        }
        Cull Off
        
        Pass {
            Tags {
                "Lighting"="ForwardBase"
            } 

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "ShellShaderLighting.cginc"
            ENDCG
        }
        Pass {
            Tags {
                "Lighting"="ForwardAdd"
            } 
            Blend One One
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd
            #include "ShellShaderLighting.cginc"
            ENDCG
        }
    }
}
