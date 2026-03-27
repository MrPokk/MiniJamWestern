Shader "Custom/CRT_Fullscreen_URP"
{
    Properties
    {
        [Header(Curvature)]
        _Distortion ("Distortion Strength", Range(-0.5, 0.5)) = 0.1
        _Zoom ("Zoom", Range(0.5, 1.5)) = 1.0
        
        [Header(Scanlines)]
        _ScanlineIntensity ("Scanline Intensity", Range(0, 1)) = 0.5
        _ScanlineCount ("Scanline Count", Float) = 240
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off

        Pass
        {
            Name "CRT_PostProcess"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            // Используем библиотеки URP для доступа к _BlitTexture
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float _Distortion;
                float _Zoom;
                float _ScanlineIntensity;
                float _ScanlineCount;
            CBUFFER_END

            // Функция искривления (Выпуклость)
            float2 CurveUV(float2 uv)
            {
                float2 centered = uv - 0.5;
                float r2 = dot(centered, centered);
                // Масштабируем координаты от центра
                float2 distorted = centered * (1.0 + _Distortion * r2) * _Zoom;
                return distorted + 0.5;
            }

            float4 Frag (Varyings input) : SV_Target
            {
                // 1. Получаем искривленные координаты экрана
                float2 uv = CurveUV(input.texcoord);
                
                // Проверка на черные полосы по краям "телевизора"
                if (uv.x < 0.0 || uv.x > 1.0 || uv.y < 0.0 || uv.y > 1.0) 
                    return float4(0, 0, 0, 1);

                // 2. Берем изображение экрана (BlitTexture)
                // Используем sampler_LinearClamp для мягкого сглаживания при искажении
                float4 color = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv);

                // 3. Добавляем сканлайны (полоски)
                // Используем PI * 2 для четкого повторения линий
                float scanline = sin(uv.y * _ScanlineCount * PI * 2.0);
                
                // Смягчаем эффект сканлайнов
                float scanIntensity = (scanline + 1.0) * 0.5;
                float multiplier = lerp(1.0, 1.0 - _ScanlineIntensity * 0.5, scanIntensity);
                
                // Дополнительное затемнение каждой второй линии (эффект более глубоких полос)
                if (scanline < 0) {
                    multiplier *= (1.0 - _ScanlineIntensity * 0.5);
                }

                color.rgb *= multiplier;

                return color;
            }
            ENDHLSL
        }
    }
}