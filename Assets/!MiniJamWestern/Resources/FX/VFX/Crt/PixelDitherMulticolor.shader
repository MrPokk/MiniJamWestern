Shader "Custom_VFX/Pixel/PixelDither_Multicolor"
{   
    Properties
    {
        [Header(Pixelation)]
        _PixelDensity ("Pixel Density", Float) = 256.0
        
        [Header(Color Reduction)]
        _ColorDepth ("Color Colors (Levels)", Range(2, 256)) = 16.0
        _DitherSpread ("Dither Spread", Range(0, 1)) = 0.1
    }

    SubShader
    {
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

        CBUFFER_START(UnityPerMaterial)
            float _PixelDensity;
            float _ColorDepth;
            float _DitherSpread;
        CBUFFER_END

        static const float4x4 bayerDither = float4x4(
             0.0,  8.0,  2.0, 10.0,
            12.0,  4.0, 14.0,  6.0,
             3.0, 11.0,  1.0,  9.0,
            15.0,  7.0, 13.0,  5.0
        ) / 16.0;

        ENDHLSL

        Tags { "RenderType"="Opaque" }
        LOD 100
        ZWrite Off Cull Off
        
        Pass
        {
            Name "DitherFullColorPass"

            HLSLPROGRAM
            
            #pragma vertex Vert
            #pragma fragment Frag

            float4 Frag (Varyings input) : SV_Target
            {
                // 1. Настройка пикселизации
                float aspect = _ScreenParams.x / _ScreenParams.y;
                float2 pixelCount = float2(_PixelDensity * aspect, _PixelDensity);
                
                // Округляем UV координаты для эффекта "пикселей"
                float2 pixelUV = floor(input.texcoord * pixelCount) / pixelCount;

                // 2. Сэмплим исходный цвет экрана
                // Используем PointClamp, чтобы пиксели были четкими, а не размытыми
                float4 originalColor = SAMPLE_TEXTURE2D(_BlitTexture, sampler_PointClamp, pixelUV);

                // 3. Получаем значение из матрицы Байера
                // Используем input.texcoord * pixelCount, чтобы паттерн "прилип" к пикселям
                uint2 pixelPos = uint2(input.texcoord * pixelCount);
                float ditherValue = bayerDither[pixelPos.x % 4][pixelPos.y % 4];

                // Центрируем дизеринг (от -0.5 до 0.5)
                float ditherOffset = (ditherValue - 0.5) * _DitherSpread;

                // 4. Применяем дизеринг и уменьшаем глубину цвета
                // Сначала добавляем шум к исходному цвету
                float3 noisyColor = originalColor.rgb + ditherOffset;
                
                // Затем квантуем цвет (округляем до ближайшего уровня)
                // Формула: floor(x * levels) / levels
                float3 finalColor = floor(noisyColor * _ColorDepth) / _ColorDepth;

                return float4(finalColor, 1.0);
            }
            
            ENDHLSL
        }
    }
}