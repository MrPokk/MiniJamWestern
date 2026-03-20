Shader "Custom/Watery_Balatro_Outline_URP"
{
    Properties
    {
        [MainTexture] _MainTex ("Sprite Texture", 2D) = "white" {}
        _BaseColor ("Water Tint", Color) = (0.5, 0.8, 1, 1)
        
        [Header(Outline Settings)]
        _OutlineColor ("Outline Color", Color) = (1, 1, 1, 1)
        _OutlinePixels ("Outline Width (Pixels)", Range(0, 5)) = 1
        _AlphaThreshold ("Alpha Threshold", Range(0.01, 1.0)) = 0.1

        [Header(Tilt and Physics)]
        _Tilt ("Tilt Amount (XY)", Vector) = (0,0,0,0)
        _Perspective ("Perspective Strength", Range(0, 0.5)) = 0.1
        _WaveStretch ("Liquid Stretch", Range(0, 0.2)) = 0.05

        [Header(Water Ripples)]
        _WarpStrength ("Refraction Warp", Range(0, 0.05)) = 0.01
        _WarpFreq ("Warp Frequency", Range(1, 20)) = 8.0
        _WarpSpeed ("Warp Speed", Range(0.1, 5)) = 1.0

        [Header(Caustics Shine)]
        _CausticColor ("Caustic Color", Color) = (1, 1, 1, 1)
        _CausticIntensity ("Caustic Intensity", Range(0, 2)) = 1.0
        _CausticScale ("Caustic Scale", Range(1, 20)) = 5.0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline" }
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

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_TexelSize;

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half4 _OutlineColor;
                float _OutlinePixels;
                float _AlphaThreshold;
                float4 _Tilt;
                float _Perspective;
                float _WaveStretch;
                float _WarpStrength;
                float _WarpFreq;
                float _WarpSpeed;
                half4 _CausticColor;
                float _CausticIntensity;
                float _CausticScale;
            CBUFFER_END

            float3 mod289(float3 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
            float2 mod289(float2 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }

            float GetCaustics(float2 uv, float time) {
                uv *= _CausticScale;
                float2 p = mod289(uv);
                float res = sin(p.x + time) + sin(p.y + time);
                res += sin((p.x + p.y) * 0.5 + time * 1.2);
                return saturate(pow(abs(res), 6.0) * 0.05);
            }

            Varyings vert(Attributes input)
            {
                Varyings output;
                float3 pos = input.positionOS.xyz;
                
                float tiltX = pos.y * _Tilt.x * _Perspective;
                float tiltY = pos.x * _Tilt.y * _Perspective;
                pos.x += tiltX; 
                pos.y += tiltY;

                float movementSpeed = length(_Tilt.xy);
                float wave = sin(_Time.y * _WarpSpeed + pos.y * _WarpFreq) * _WaveStretch * movementSpeed;
                pos.x += wave;

                output.positionCS = TransformObjectToHClip(pos);
                output.uv = input.uv;
                output.color = input.color;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float time = _Time.y * _WarpSpeed;

                // 1. Искажение координат (Warp)
                float2 warp = float2(
                    sin(input.uv.y * _WarpFreq + time),
                    cos(input.uv.x * _WarpFreq + time)
                ) * _WarpStrength;
                warp *= (1.0 + length(_Tilt.xy) * 8.0);
                
                float2 warpedUV = input.uv + warp;

                // 2. Основной цвет спрайта
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, warpedUV);
                half4 mainColor = texColor * _BaseColor * input.color;

                // 3. Логика Pixel Perfect Outline (на искаженных координатах)
                float2 offset = _MainTex_TexelSize.xy * _OutlinePixels;
                
                half aUp    = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, warpedUV + float2(0, offset.y)).a;
                half aDown  = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, warpedUV + float2(0, -offset.y)).a;
                half aLeft  = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, warpedUV + float2(-offset.x, 0)).a;
                half aRight = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, warpedUV + float2(offset.x, 0)).a;

                half maxAlpha = max(max(aUp, aDown), max(aLeft, aRight));
                half outlineMask = step(_AlphaThreshold, maxAlpha) * step(mainColor.a, _AlphaThreshold);

                // Применяем обводку
                mainColor.rgb = lerp(mainColor.rgb, _OutlineColor.rgb, outlineMask);
                mainColor.a = max(mainColor.a, outlineMask * _OutlineColor.a * input.color.a);

                // 4. Каустика (Блики воды)
                float2 causticUV = input.uv + _Tilt.xy * 0.15;
                float c1 = GetCaustics(causticUV + warp, time);
                float c2 = GetCaustics(causticUV * 1.2 - warp, time * 0.8);
                
                half3 finalCaustic = (c1 + c2) * _CausticColor.rgb * _CausticIntensity;
                mainColor.rgb += finalCaustic * mainColor.a;

                return mainColor;
            }
            ENDHLSL
        }
    }
}