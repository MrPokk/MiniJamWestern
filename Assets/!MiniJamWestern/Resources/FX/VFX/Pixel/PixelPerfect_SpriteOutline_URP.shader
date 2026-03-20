Shader "Custom/PixelPerfect_SpriteOutline_URP"
{
    Properties
    {
        [HideInInspector] _MainTex ("Sprite Texture", 2D) = "white" {}
        
        [Header(Outline Settings)]
        _OutlineColor ("Outline Color", Color) = (1, 1, 1, 1)
        _OutlinePixels ("Outline Width (Pixels)", Range(0, 5)) = 1[Header(Advanced)]
        _AlphaThreshold ("Alpha Threshold", Range(0.01, 1.0)) = 0.1
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent" 
            "Queue"="Transparent"
            "RenderPipeline"="UniversalPipeline"
            "PreviewType"="Plane"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _OutlineColor;
                float _OutlinePixels;
                float _AlphaThreshold;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            // Unity автоматически заполняет эту переменную данными о размере _MainTex
            float4 _MainTex_TexelSize; 

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                output.color = input.color;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // Получаем основной цвет спрайта
                half4 mainColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv) * input.color;
                
                // _MainTex_TexelSize.xy уже содержит (1.0 / width, 1.0 / height)
                float2 offset = _MainTex_TexelSize.xy * _OutlinePixels;

                // Считываем альфа-канал 8 соседних пикселей вокруг текущего
                half aUp    = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv + float2(0, offset.y)).a;
                half aDown  = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv + float2(0, -offset.y)).a;
                half aLeft  = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv + float2(-offset.x, 0)).a;
                half aRight = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv + float2(offset.x, 0)).a;

                half maxAlpha = max(max(aUp, aDown), max(aLeft, aRight));

                // Логика: если текущий пиксель прозрачный, а рядом есть непрозрачный — рисуем обводку
                half outlineMask = step(_AlphaThreshold, maxAlpha) * step(mainColor.a, _AlphaThreshold);

                half4 finalCol = mainColor;
                // Смешиваем основной цвет и обводку
                finalCol.rgb = lerp(mainColor.rgb, _OutlineColor.rgb, outlineMask);
                finalCol.a = max(mainColor.a, outlineMask * _OutlineColor.a * input.color.a);

                return finalCol;
            }
            ENDHLSL
        }
    }
}