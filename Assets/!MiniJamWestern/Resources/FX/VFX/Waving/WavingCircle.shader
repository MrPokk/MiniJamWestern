Shader "Custom/PixelPerfect_WavingCircle_Elka_URP"
{
    Properties
    {
        [HideInInspector] _MainTex ("Sprite Texture", 2D) = "white" {} 
        
        [Header(Size and Proportions)]
        _AspectRatio ("Aspect Ratio (Width / Height)", Float) = 1.0[Header(Colors)]
        _Color ("Color", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)[Header(Shape Parameters)]
        // Теперь вы указываете толщину строго в количестве пикселей!
        _OutlinePixels ("Outline Width (in Pixels)", Range(0, 10)) = 1
        _Radius ("Radius", Range(0, 2)) = 0.5[Header(Wave Settings)]
        _WaveAmplitude ("Wave Amplitude", Range(0, 0.5)) = 0.1
        _WaveFrequency ("Wave Frequency", Float) = 5.0
        _WaveSpeed ("Wave Speed", Float) = 2.0
        _RotationSpeed ("Rotation Speed", Float) = 1.0
        _ElkaSharpness ("Elka Sharpness", Range(1, 10)) = 3.0
        _MaxAngle ("Max Angle", Range(0.1, 32)) = 6.28 
        
        [Header(Pixelation)]
        _PixelSize ("Pixel Density", Range(1, 200)) = 50
    }
    
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent" 
            "Queue"="Transparent"
            "RenderPipeline"="UniversalPipeline"
            "IgnoreProjector"="True"
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
                float _AspectRatio;
                half4 _Color;
                half4 _OutlineColor;
                float _OutlinePixels;
                float _Radius;
                float _WaveAmplitude;
                float _WaveFrequency;
                float _WaveSpeed;
                float _RotationSpeed;
                float _PixelSize;
                float _ElkaSharpness;
                float _MaxAngle;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv; 
                output.color = input.color; 
                return output;
            }

            float limitedAtan2(float y, float x, float maxAngle)
            {
                float angle = atan2(y, x);
                angle = angle < 0.0 ? angle + TWO_PI : angle;
                return min(angle, maxAngle);
            }

            half4 frag(Varyings input) : SV_Target
            {
                float2 centeredUV = input.uv * 2.0 - 1.0;
                centeredUV.x *= _AspectRatio;

                // 1. Вычисляем размер одного "пикселя" в нашей сетке
                float pixelStep = 1.0 / _PixelSize;

                // 2. Идеальная пикселизация со сдвигом в ЦЕНТР макро-пикселя (устраняет асимметрию круга)
                float2 pixelatedUV = (floor(centeredUV * _PixelSize) + 0.5) * pixelStep;
                
                float distance = length(pixelatedUV);
                float angle = limitedAtan2(pixelatedUV.y, pixelatedUV.x, _MaxAngle);
                
                float timeFactor = _Time.y * _WaveSpeed * 2.0;
                float rotationTime = _Time.y * _RotationSpeed;

                float elkaWave = abs(frac(angle * _WaveFrequency / PI + timeFactor) * 2.0 - 1.0);
                elkaWave = pow(elkaWave, _ElkaSharpness);
                
                float wavingRadius = _Radius + _WaveAmplitude * elkaWave * (0.5 + 0.5 * sin(angle * 5.0 + rotationTime));
                
                // 3. Вычисляем физическую толщину обводки в пикселях
                float actualOutlineWidth = _OutlinePixels * pixelStep;
                
                // 4. Используем step() вместо smoothstep(). 
                // Это дает 100% жесткие края (идеальный Pixel Art) без полупрозрачной грязи.
                float circleAlpha = step(distance, wavingRadius);
                float outlineAlpha = step(distance, wavingRadius + actualOutlineWidth) - circleAlpha;
                
                // Применяем цвета
                half4 baseColor = _Color * input.color;
                half4 col = baseColor;
                col.a *= circleAlpha;
                
                half4 outlineCol = _OutlineColor;
                outlineCol.a *= outlineAlpha * input.color.a; 
                
                // Если мы попали в зону обводки (outlineAlpha == 1), заменяем цвет на цвет обводки
                col = lerp(col, outlineCol, outlineAlpha);
                
                return col;
            }
            ENDHLSL
        }
    }
}