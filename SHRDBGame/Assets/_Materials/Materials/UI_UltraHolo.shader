Shader "UI/UltraHolo"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _NormalTex("Normal Map", 2D) = "bump" {}
        _TileTex("Mosaic Texture", 2D) = "white" {}
        _Color("Tint Color", Color) = (1,1,1,1)
        _TileSpeed("Tile Speed", Float) = 0.5
        _TileScale("Tile Scale", Float) = 3.0
        _BumpStrength("Bump Strength", Float) = 0.05
        _ShineIntensity("Shine Intensity", Float) = 1.0
        _ShineWidth("Shine Width", Float) = 0.2
        _ShineSpeed("Shine Speed", Float) = 1.0
        _ShineDir("Shine Direction", Vector) = (1,0,0,0)
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True" "RenderPipeline"="UniversalPipeline" }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_NormalTex);
            SAMPLER(sampler_NormalTex);
            TEXTURE2D(_TileTex);
            SAMPLER(sampler_TileTex);

            float4 _Color;
            float _TileSpeed;
            float _TileScale;
            float _BumpStrength;
            float _ShineIntensity;
            float _ShineWidth;
            float _ShineSpeed;
            float4 _ShineDir;

            Varyings Vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                OUT.color = IN.color * _Color;
                return OUT;
            }

            float ShinyMask(float2 uv)
            {
                float2 dir = normalize(_ShineDir.xy);
                float t = frac(_Time.y * _ShineSpeed);
                float shinePos = dot(uv, dir);
                float dist = abs(shinePos - t);
                return saturate(1.0 - smoothstep(_ShineWidth * 0.5, _ShineWidth, dist));
            }

            half4 Frag(Varyings IN) : SV_Target
            {
                // Textura principal y alpha
                float4 baseCol = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                baseCol.rgb *= IN.color.rgb;
                float alphaMask = baseCol.a;

                // Textura mosaico
                float2 tileUV = IN.uv * _TileScale + float2(_Time.y * _TileSpeed, _Time.y * _TileSpeed * 0.5);
                float4 tileCol = SAMPLE_TEXTURE2D(_TileTex, sampler_TileTex, frac(tileUV));

                // Normal map para bump
                float3 normalSample = SAMPLE_TEXTURE2D(_NormalTex, sampler_NormalTex, IN.uv).rgb * 2.0 - 1.0;
                // Simulación de relieve: multiplicamos normal.z para dar brillo sutil
                float bumpEffect = normalSample.z * _BumpStrength;

                // Brillo animado
                float shine = ShinyMask(IN.uv) * _ShineIntensity;

                // Combinar: mosaico + bump + brillo, todo en alpha de base
                float3 finalRGB = baseCol.rgb + (tileCol.rgb * 0.2 + bumpEffect + shine) * alphaMask;
                finalRGB = saturate(finalRGB);

                return float4(finalRGB, alphaMask);
            }

            ENDHLSL
        }
    }

    FallBack "Unlit/Transparent"
}
