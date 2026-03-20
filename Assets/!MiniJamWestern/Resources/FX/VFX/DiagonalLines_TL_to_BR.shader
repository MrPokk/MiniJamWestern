Shader "UI/Custom/DiagonalLinesBackground"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        [Header(Lines Settings)]
        _Angle ("Line Angle", Range(0, 360)) = 45.0
        
        _LineColor ("Line Color", Color) = (1, 1, 1, 1)
        _BgColor ("Background Color", Color) = (0, 0, 0, 1) 
        
        _Density ("Lines Density", Range(1, 150)) = 20.0
        _Thickness ("Line Thickness", Range(0.01, 0.99)) = 0.5
        _Speed ("Movement Speed", Range(-5.0, 5.0)) = 1.0

        // Системные параметры UI
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
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

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode] 
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float2 uv           : TEXCOORD0;
                float4 color        : COLOR;
                float4 worldPosition: TEXCOORD1;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                half4 _LineColor;
                half4 _BgColor;
                float _Angle;      // Добавили переменную угла
                float _Density;
                float _Thickness;
                float _Speed;
                float4 _ClipRect;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            Varyings vert(Attributes input)
            {
                Varyings output;
                
                output.worldPosition = mul(GetObjectToWorldMatrix(), input.positionOS);
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                output.color = input.color * _Color; 
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);

                // --- ЛОГИКА НАПРАВЛЕНИЯ ЛИНИЙ ---
                
                // 1. Переводим градусы в радианы
                float rad = _Angle * (3.14159265359 / 180.0);
                
                // 2. Считаем синус и косинус для матрицы поворота (sincos - быстрая встроенная функция HLSL)
                float s, c;
                sincos(rad, s, c);
                
                // 3. Смещаем UV к центру (0.5, 0.5), чтобы линии вращались вокруг центра объекта
                float2 centeredUV = input.uv - 0.5;
                
                // 4. Вращаем координаты. Для отрисовки полос нам нужна только одна ось (например, X)
                float rotatedX = centeredUV.x * c - centeredUV.y * s;

                // --- ФОРМИРОВАНИЕ ПАТТЕРНА ---
                
                // Используем повернутую ось X вместо диагонали (uv.x + uv.y)
                float value = rotatedX * _Density + _Time.y * _Speed;
                float pattern = frac(value);

                // lineMask будет равен 0 на линии и 1 на фоне
                half lineMask = step(_Thickness, pattern);

                // Смешиваем цвет линии и фона
                half4 finalColor = lerp(_LineColor, _BgColor, lineMask);

                // Умножаем на базовую текстуру и цвет компонента Image 
                finalColor *= texColor * input.color;

                #if UNITY_UI_CLIP_RECT
                    half2 clipUV = (input.worldPosition.xy - _ClipRect.xy) / (_ClipRect.zw - _ClipRect.xy);
                    half clipAlpha = saturate(clipUV.x) * saturate(clipUV.y) * saturate(1.0 - clipUV.x) * saturate(1.0 - clipUV.y);
                    finalColor.a *= step(0.5, clipAlpha);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                    clip (finalColor.a - 0.001);
                #endif

                return finalColor;
            }
            ENDHLSL
        }
    }
}