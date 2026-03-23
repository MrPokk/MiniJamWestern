Shader "Custom/ComplexNoiseEnergy"
{
    Properties
    {
        _Color1 ("Color Primary", Color) = (0.2, 0.5, 1.0, 1.0)
        _Color2 ("Color Secondary", Color) = (0.5, 0.0, 1.0, 1.0)
        _Speed ("Movement Speed", Vector) = (0.2, 0.2, 0, 0)
        _Scale ("Noise Scale", Float) = 3.0
        _Distortion ("Distortion Strength", Range(0, 2)) = 0.5
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
                float4 color        : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 uv           : TEXCOORD0;
                float4 color        : COLOR;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _Color1;
                float4 _Color2;
                float2 _Speed;
                float _Scale;
                float _Distortion;
            CBUFFER_END

            // Функция псевдорандома
            float hash(float2 p) {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            // Плавный шум (Value Noise)
            float noise(float2 p) {
                float2 i = floor(p);
                float2 f = frac(p);
                f = f * f * (3.0 - 2.0 * f);
                float a = hash(i);
                float b = hash(i + float2(1.0, 0.0));
                float c = hash(i + float2(0.0, 1.0));
                float d = hash(i + float2(1.0, 1.0));
                return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
            }

            // Наслоение шума (FBM - Fractal Brownian Motion)
            float fbm(float2 p) {
                float v = 0.0;
                float a = 0.5;
                float2 shift = float2(100.0, 100.0);
                for (int i = 0; i < 5; ++i) {
                    v += a * noise(p);
                    p = p * 2.0 + shift;
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
                float2 uv = IN.uv * _Scale;
                float time = _Time.y;

                // 1. Создаем искажение UV (Domain Warping)
                // Это заставляет шум "течь" как жидкость
                float2 q = float2(
                    fbm(uv + _Speed * time),
                    fbm(uv + float2(1.0, 1.0))
                );

                float2 r = float2(
                    fbm(uv + 4.0 * q + float2(1.7, 9.2) + 0.15 * time),
                    fbm(uv + 4.0 * q + float2(8.3, 2.8) + 0.126 * time)
                );

                // Итоговое значение шума
                float f = fbm(uv + r * _Distortion);

                // 2. Рассчитываем цвета
                // Смешиваем два основных цвета на основе шума
                half4 color = lerp(_Color1, _Color2, f);

                // Добавляем немного яркости (glow) в зависимости от "плотности" шума
                color += lerp(half4(0,0,0,0), half4(1,1,1,1), pow(f, 3.0)) * 0.5;

                // Учитываем прозрачность и цвет из SpriteRenderer (IN.color)
                color *= IN.color;
                
                // Делаем эффект мягче по краям (опционально)
                float alpha = f * IN.color.a;
                
                return half4(color.rgb, alpha);
            }
            ENDHLSL
        }
    }
}