Shader "UI/HolofoilURP_AlphaTinted"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _HoloTex("Holo Texture", 2D) = "white" {}
        _Color("Color Tint", Color) = (1,1,1,1)
        _Tiling("Holo Tiling", Float) = 3.0
        _Speed("Holo Speed", Float) = 0.5
        _BumpStrength("Bump Strength", Float) = 0.05
        _ShineWidth("Shine Width", Float) = 0.2
        _ShineSpeed("Shine Speed", Float) = 0.8
        _ShineDir("Shine Direction", Vector) = (1,0,0,0)
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True" "RenderPipeline"="UniversalPipeline" "CanUseSpriteAtlas"="True" }
        LOD 100

        Pass
        {
             Blend SrcAlpha OneMinusSrcAlpha
    Cull Off
    ZWrite Off

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

            TEXTURE2D(_HoloTex);
            SAMPLER(sampler_HoloTex);

            float4 _Color;
            float _Tiling;
            float _Speed;
            float _BumpStrength;
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
    // Textura base
    float4 baseCol = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
    baseCol.rgb *= IN.color.rgb;
    float alphaMain = baseCol.a;

    // Textura holo
    float2 holoUV = IN.uv * _Tiling + float2(_Time.y * _Speed, _Time.y * _Speed * 0.5);
    float4 holoCol = SAMPLE_TEXTURE2D(_HoloTex, sampler_HoloTex, frac(holoUV));
    float alphaHolo = holoCol.a;

    // Brillo arcoíris
    float shine = ShinyMask(IN.uv);
    shine *= alphaHolo; // solo sobre holo

    // Rainbow tint del holo
    float3 hue = 0.5 + 0.5 * sin(float3(0,2,4) + _Time.y * 2.0);
    float3 holoRainbow = holoCol.rgb * hue;

    // Mezclar holo + brillo
    float3 holoMix = holoRainbow + shine;

    // Aplicar máscara de alpha principal
    holoMix *= alphaMain;

    // Mezclar base + holo respetando alpha de la holo texture
    float3 finalRGB = lerp(baseCol.rgb, holoMix, alphaHolo);

    // Alpha final = alpha de la textura principal
    float finalAlpha = alphaMain;

    return float4(finalRGB, finalAlpha);
}




            ENDHLSL
        }
    }

    FallBack "Unlit/Transparent"
}
