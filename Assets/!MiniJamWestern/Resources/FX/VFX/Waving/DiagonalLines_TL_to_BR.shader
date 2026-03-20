Shader "Custom/DiagonalLines_TL_to_BR"
{
    Properties
    {
        [HideInInspector] _MainTex ("Sprite Texture", 2D) = "white" {}
        
        [Header(Lines Settings)]
        _LineColor ("Line Color", Color) = (1, 1, 1, 1)
        _BgColor ("Background Color", Color) = (0, 0, 0, 0)
        
        _Density ("Lines Density", Range(1, 50)) = 10.0
        _Thickness ("Line Thickness", Range(0.01, 0.99)) = 0.5
        _Speed ("Movement Speed", Range(-5.0, 5.0)) = 1.0
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
                half4 _LineColor;
                half4 _BgColor;
                float _Density;
                float _Thickness;
                float _Speed;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                // Поддержка цвета вершины (например, от компонента SpriteRenderer или Image)
                output.color = input.color; 
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // Считываем основную текстуру (полезно, если вешаем на спрайт)
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                
                // Математика диагонали: uv.x + uv.y дает линии слева-сверху вправо-вниз
                // Если нужно слева-снизу вправо-вверх, используйте (uv.x - uv.y)
                float diagonal = input.uv.x + input.uv.y;
                
                // Умножаем на плотность и добавляем время для анимации движения
                float value = diagonal * _Density + _Time.y * _Speed;
                
                // frac() оставляет только дробную часть (от 0.0 до 1.0), создавая повторения
                float pattern = frac(value);

                // step() возвращает 1, если pattern > _Thickness, и 0 в противном случае.
                // Получается жесткая маска линий
                half lineMask = step(_Thickness, pattern);

                // Смешиваем цвет линии и цвет фона по маске
                half4 finalColor = lerp(_LineColor, _BgColor, lineMask);

                // Умножаем на цвет текстуры и вершины (чтобы работала прозрачность спрайта и Tint)
                finalColor *= texColor * input.color;

                return finalColor;
            }
            ENDHLSL
        }
    }
}