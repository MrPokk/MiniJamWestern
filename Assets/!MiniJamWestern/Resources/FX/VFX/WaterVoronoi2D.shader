Shader "Custom/WaterVoronoi2D"
{
    Properties
    {
        // Скрываем текстуру, чтобы SpriteRenderer сам её назначал
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1, 1, 1, 1)

        [Space(10)]
        [Header(Toon Water Colors)]
        [HDR] _WaterDark ("Dark Water", Color) = (0.1, 0.45, 0.8, 1)
        [HDR] _WaterLight ("Light Water", Color) = (0.2, 0.7, 0.9, 1)
        [HDR] _FoamColor ("Foam Color", Color) = (1, 1, 1, 1)

        [Space(10)]
        [Header(Wave Settings)]
        _Speed ("Wave Speed", Float) = 1.0
        _Scale ("Wave Scale", Float) = 8.0
        _Distortion ("Distortion Level", Float) = 0.6
        
        [Space(10)]
        [Header(Thresholds)]
        _MidThreshold ("Light Water Threshold", Range(0, 1)) = 0.5
        _FoamThreshold ("Foam Threshold", Range(0, 1)) = 0.75
    }

    SubShader
    {
        // Теги, необходимые для корректной работы со SpriteRenderer
        Tags 
        { 
            "Queue"="Transparent" 
            "RenderType"="Transparent" 
            "RenderPipeline"="UniversalPipeline" 
            "IgnoreProjector"="True" 
            "PreviewType"="Plane" 
            "CanUseSpriteAtlas"="True"
        }

        // Настройки блендинга для спрайтов
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        ZTest LEqual

        Pass
        {
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // Подключаем библиотеку URP
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
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                half4 _WaterDark;
                half4 _WaterLight;
                half4 _FoamColor;
                float _Speed;
                float _Scale;
                float _Distortion;
                float _MidThreshold;
                float _FoamThreshold;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                // Преобразуем координаты из локальных в экранные
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                // Передаем цвет вершин (из SpriteRenderer)
                OUT.color = IN.color;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Читаем альфа-канал оригинального спрайта (чтобы вода не выходила за его границы)
                half4 spriteColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);

                // --- ПРОЦЕДУРНАЯ ГЕНЕРАЦИЯ ВОЛН ---
                // Создаем 2 направления движения для эффекта "пересечения" волн
                float2 scroll1 = float2(_Time.y * _Speed, _Time.y * _Speed * 0.5);
                float2 scroll2 = float2(-_Time.y * _Speed * 0.7, _Time.y * _Speed * 0.3);

                // Искажаем UV с помощью синусоид
                float wave = sin((IN.uv.x * _Scale + scroll1.x) + sin(IN.uv.y * _Scale + scroll1.y) * _Distortion);
                wave += cos((IN.uv.y * _Scale + scroll2.y) + cos(IN.uv.x * _Scale + scroll2.x) * _Distortion);

                // Приводим волну примерно к диапазону от 0 до 1
                wave = wave * 0.25 + 0.5;

                // --- TOON ЭФФЕКТ (Резкие переходы) ---
                // Функция step(edge, x) возвращает 1, если x > edge, иначе 0.
                float lightWaterMask = step(_MidThreshold, wave);
                float foamMask = step(_FoamThreshold, wave);

                // Смешиваем цвета воды
                half4 finalColor = lerp(_WaterDark, _WaterLight, lightWaterMask);
                // Добавляем пену
                finalColor = lerp(finalColor, _FoamColor, foamMask);

                // --- ПРИМЕНЕНИЕ ДАННЫХ СПРАЙТА ---
                // Умножаем на цвет самого спрайта (Tint) и глобальный цвет материала
                finalColor *= IN.color * _Color;
                
                // Вода должна отображаться только там, где у спрайта есть пиксели (по альфе)
                finalColor.a *= spriteColor.a;

                return finalColor;
            }
            ENDHLSL
        }
    }
}