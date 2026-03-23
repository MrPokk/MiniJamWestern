Shader "Custom/ShimmeringPatternURP"
{
    Properties
    {
        [MainTexture] _MainTex ("Pattern Texture (Sprite)", 2D) = "white" {}
        _Color1 ("Shimmer Color A", Color) = (1, 0.5, 0, 1)
        _Color2 ("Shimmer Color B", Color) = (0, 0.5, 1, 1)
        
        _NoiseScale ("Noise Scale", Float) = 4.0
        _DistortionStrength ("Distortion Strength", Range(0, 0.2)) = 0.05
        _Speed ("Speed", Float) = 1.0
        _PatternScroll ("Pattern Scroll Speed", Vector) = (0, 0, 0, 0)
    }

    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent" 
            "RenderPipeline" = "UniversalPipeline" 
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

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
                float4 color        : COLOR; // Цвет из SpriteRenderer
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

            // Вспомогательные функции шума
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

            // Fractal Brownian Motion (Слои шума)
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
                
                // 1. Генерируем два слоя шума для искажения UV
                float noiseX = fbm(IN.uv * _NoiseScale + time);
                float noiseY = fbm(IN.uv * _NoiseScale - time * 0.5);
                float2 distortion = float2(noiseX, noiseY) * _DistortionStrength;

                // 2. Рассчитываем финальные UV для паттерна
                // Смещаем их по времени (скроллинг) + добавляем искажение (шум)
                float2 patternUV = IN.uv + distortion + (_PatternScroll * _Time.y);

                // 3. Берем текстуру (паттерн)
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_mainTex, patternUV);

                // 4. Создаем переливающийся цвет на основе шума
                float colorMask = fbm(IN.uv * (_NoiseScale * 0.5) + time * 0.3);
                half4 shimmerColor = lerp(_Color1, _Color2, colorMask);

                // 5. Комбинируем всё
                // Умножаем текстуру на переливающийся цвет и на цвет из SpriteRenderer
                half4 finalColor = texColor * shimmerColor * IN.color;

                // Если в текстуре есть альфа-канал, используем его
                return finalColor;
            }
            ENDHLSL
        }
    }
}