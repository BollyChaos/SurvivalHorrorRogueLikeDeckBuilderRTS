Shader "UI/ShinyURP"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        // Brillo
        _ShineColor ("Shine Color", Color) = (1,1,1,1)
        _ShineWidth ("Shine Width", Range(0.01, 1)) = 0.2
        _ShineSpeed ("Shine Speed", Range(0.0, 5.0)) = 1.0
        _Intensity ("Intensity", Range(0,3)) = 1

        // Dirección del brillo (normalizado)
        _Direction ("Shine Direction (XY)", Vector) = (1, 1, 0, 0)
    }

    SubShader
    {
        Tags {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "ForceNoShadowCasting"="True"
        }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _MainTex_ST;
            float4 _Color;

            float4 _ShineColor;
            float _ShineWidth;
            float _ShineSpeed;
            float _Intensity;

            float4 _Direction; // solo usamos xy

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            // MÁSCARA DE BRILLO BASADA EN UN VECTOR
            float shinyMask(float2 uv)
            {
                // Normalizamos el vector para evitar problemas
                float2 dir = normalize(_Direction.xy);

                // Movimiento del brillo
                float t = frac(_Time.y * _ShineSpeed);

                // Proyección del UV sobre el vector
                float shinePos = dot(uv, dir);

                float dist = abs(shinePos - t);
                
                // Mask suave
                return saturate(1.0 - smoothstep(_ShineWidth * 0.5, _ShineWidth, dist));
            }

           half4 frag (Varyings i) : SV_Target
{
    float4 texCol = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

    // Respetamos el color del Image
    float4 baseCol = texCol * _Color;

    float mask = shinyMask(i.uv);

    // Brillo multiplicado por el alpha de la textura principal (para que solo aparezca sobre el sprite)
    float4 shine = _ShineColor * mask * _Intensity * baseCol.a;

    // Combinamos respetando alpha del sprite
    float3 finalRGB = baseCol.rgb + shine.rgb;
    float finalAlpha = baseCol.a;

    return float4(finalRGB, finalAlpha);
}


            ENDHLSL
        }
    }
}
