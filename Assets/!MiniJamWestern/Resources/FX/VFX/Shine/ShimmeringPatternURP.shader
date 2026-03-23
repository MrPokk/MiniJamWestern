Shader "Custom/ShimmeringPatternURP"
{
    Properties
    {
        [MainTexture] _MainTex ("Pattern Texture (Grayscale)", 2D) = "white" {}
        _Color1 ("Color for White Pixels", Color) = (1, 1, 1, 1)
        _Color2 ("Color for Black Pixels", Color) = (0, 0, 0, 1)
        
        _NoiseScale ("Noise Scale", Float) = 4.0
        _DistortionStrength ("Distortion Strength", Range(0, 0.2)) = 0.05
        _Speed ("Speed", Float) = 1.0
        _PatternScroll ("Pattern Scroll Speed", Vector) = (0, 0, 0, 0)

        [Header(Stencil)]
        _Stencil ("Stencil ID", Float) = 0
        _StencilComp ("Stencil Comparison", Float) = 8
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent" 
            "RenderPipeline" = "UniversalPipeline" 
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        ColorMask [_ColorMask]

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
                float4 color        : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 uv           : TEXCOORD0;
                float4 color        : COLOR;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_mainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _Color1;
                float4 _Color2;
                float _NoiseScale;
                float _DistortionStrength;
                float _Speed;
                float2 _PatternScroll;
            CBUFFER_END

            float hash(float2 p) {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453123);
            }

            float noise(float2 p) {
                float2 i = floor(p);
                float2 f = frac(p);
                f = f * f * (3.0 - 2.0 * f);
                float a = hash(i);
                float b = hash(i + float2(1, 0));
                float c = hash(i + float2(0, 1));
                float d = hash(i + float2(1, 1));
                return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
            }

            float fbm(float2 p) {
                float v = 0.0;
                float a = 0.5;
                for (int i = 0; i < 3; i++) {
                    v += a * noise(p);
                    p *= 2.0;
                    a *= 0.5;
                }
                return v;
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                OUT.color = IN.color;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float time = _Time.y * _Speed;
                
                // 1. Искажение UV шумом
                float nX = fbm(IN.uv * _NoiseScale + time);
                float nY = fbm(IN.uv * _NoiseScale - time * 0.5);
                float2 distortion = float2(nX, nY) * _DistortionStrength;

                // 2. Скроллинг и выборка паттерна
                float2 patternUV = IN.uv + distortion + (_PatternScroll * _Time.y);
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_mainTex, patternUV);

                // 3. Получаем яркость паттерна (насколько пиксель "белый")
                // Можно использовать r-канал, если текстура черно-белая
                float patternValue = dot(texColor.rgb, float3(0.333, 0.333, 0.333));

                // 4. Генерируем маску мерцания (shimmer)
                float shimmer = fbm(IN.uv * (_NoiseScale * 0.5) + time * 0.3);
                
                // 5. Логика смешивания:
                // Мы берем яркость паттерна и модулируем её шумом.
                // Это заставляет "белые" части текстуры переливаться.
                float finalMask = saturate(patternValue * (0.4 + shimmer * 0.6));

                // 6. Заменяем цвета: 0 (черный) -> _Color2, 1 (белый) -> _Color1
                half4 finalColor = lerp(_Color2, _Color1, finalMask);

                // Умножаем на прозрачность текстуры и цвет из Image/SpriteRenderer
                finalColor.a *= texColor.a * IN.color.a;
                finalColor.rgb *= IN.color.rgb;

                return finalColor;
            }
            ENDHLSL
        }
    }
}