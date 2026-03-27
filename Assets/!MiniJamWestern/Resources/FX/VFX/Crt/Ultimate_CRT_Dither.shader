Shader "Custom_VFX/Pixel/PSX_Retro_Composite"
{
    Properties
    {
        [Header(PSX Resolution)]
        _PixelHeight ("Internal Resolution Y", Float) = 240.0 // Стандарт PSX (240p)
        
        [Header(PSX Color Depth)]
        _ColorLevels ("Color Precision", Range(4, 256)) = 32.0 // 32 = 5 бит (стандарт PS1)
        
        [Header(Dithering)]
        _DitherStrength ("Dither Strength", Range(0, 1)) = 0.1 // Сила смешивания
        _DarkDither ("Dark Noise Suppression", Range(0, 1)) = 0.0 // Убирать шум в черном цвете?
        
        [Header(CRT Simulation)]
        _Distortion ("Screen Curvature", Range(-0.2, 0.2)) = 0.02
        _Zoom ("Zoom (Overscan)", Range(0.8, 1.2)) = 1.0
        
        [Header(Composite Signal Artifacts)]
        _Bleeding ("Color Bleeding", Range(0, 2)) = 0.5 // Размытие цветов (NTSC эффект)
        _ScanlineIntensity ("Scanline Intensity", Range(0, 1)) = 0.25
        _VignetteStrength ("Vignette Strength", Range(0, 5)) = 0.8
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off
        
        Pass
        {
            Name "PSX_PostProcess"

            HLSLPROGRAM
            
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float _PixelHeight;
                float _ColorLevels;
                float _DitherStrength;
                float _DarkDither;
                float _Distortion;
                float _Zoom;
                float _Bleeding;
                float _ScanlineIntensity;
                float _VignetteStrength;
            CBUFFER_END

            // Классическая матрица Байера 4x4 (стандарт для PS1 игр)
            static const float4x4 bayerDither = float4x4(
                 0.0,  8.0,  2.0, 10.0,
                12.0,  4.0, 14.0,  6.0,
                 3.0, 11.0,  1.0,  9.0,
                15.0,  7.0, 13.0,  5.0
            ) / 16.0;

            // Искривление экрана (ЭЛТ)
            float2 CurveUV(float2 uv)
            {
                float2 centered = uv - 0.5;
                float r2 = dot(centered, centered);
                float2 distorted = centered * (1.0 + _Distortion * r2) * _Zoom;
                return distorted + 0.5;
            }

            float4 Frag (Varyings input) : SV_Target
            {
                // 1. CRT Геометрия
                float2 uv = CurveUV(input.texcoord);
                
                // Черные полосы за пределами экрана
                if (uv.x < 0.0 || uv.x > 1.0 || uv.y < 0.0 || uv.y > 1.0) 
                    return float4(0,0,0,1);

                // 2. Расчет пиксельной сетки (Downsampling)
                float aspect = _ScreenParams.x / _ScreenParams.y;
                float2 res = float2(_PixelHeight * aspect, _PixelHeight);
                float2 pixelUV = floor(uv * res) / res;

                // 3. Эмуляция композитного сигнала (Color Bleeding)
                // Слегка смещаем UV для разных каналов, чтобы имитировать дешевый кабель
                float bleedX = _Bleeding / res.x;
                
                float4 color;
                color.r = SAMPLE_TEXTURE2D(_BlitTexture, sampler_PointClamp, pixelUV + float2(bleedX, 0)).r;
                color.g = SAMPLE_TEXTURE2D(_BlitTexture, sampler_PointClamp, pixelUV).g;
                color.b = SAMPLE_TEXTURE2D(_BlitTexture, sampler_PointClamp, pixelUV - float2(bleedX, 0)).b;
                color.a = 1.0;

                // 4. Дизеринг (Dithering)
                // PSX применяет дизеринг ПЕРЕД уменьшением глубины цвета, чтобы "смешать" оттенки
                
                // Находим позицию пикселя на экране для паттерна
                uint2 ditherPos = uint2(uv * res);
                float ditherVal = bayerDither[ditherPos.x % 4][ditherPos.y % 4];
                
                // Центрируем значение (-0.5 ... 0.5)
                float ditherOffset = (ditherVal - 0.5);
                
                // Уменьшаем дизеринг в тенях, если нужно (чтобы черный оставался черным)
                float lum = dot(color.rgb, float3(0.299, 0.587, 0.114));
                float ditherMask = lerp(1.0, smoothstep(0.0, 0.1, lum), _DarkDither);
                
                // Применяем дизеринг
                color.rgb += ditherOffset * _DitherStrength * ditherMask;

                // 5. Квантование цвета (Color Banding)
                // Округляем цвета до 32 уровней (или сколько выставлено)
                color.rgb = floor(color.rgb * _ColorLevels) / _ColorLevels;

                // 6. Сканлайны (Scanlines)
                // Простые синусоидальные линии
                float scanline = sin(uv.y * res.y * PI * 2.0); // Частота совпадает с пикселями
                float scanIntensity = ((scanline + 1.0) * 0.5);
                // Делаем линии мягче
                color.rgb *= lerp(1.0, 0.7 + 0.3 * scanIntensity, _ScanlineIntensity);

                // 7. Виньетка
                float2 d = uv - 0.5;
                float dist = dot(d, d);
                float vignette = smoothstep(0.8, 0.8 - _VignetteStrength * 0.5, dist * 1.5);
                color.rgb *= vignette;

                return color;
            }
            
            ENDHLSL
        }
    }
}