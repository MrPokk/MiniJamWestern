Shader "Custom/Balatro_Card_Perspective_URP"
{
    Properties
    {
        [MainTexture] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        [Header(Tilt Settings)]
        _Tilt ("Tilt Amount (XY)", Vector) = (0,0,0,0) 
        _Perspective ("Perspective Strength", Range(0, 0.5)) = 0.1
        
        [Header(Holographic Shine)]
        _HoloIntensity ("Holo Intensity", Range(0, 2)) = 0.5
        _ShineWidth ("Shine Width", Range(0.01, 1)) = 0.1
        _HoloColor ("Holo Color", Color) = (1, 1, 1, 1)

        [Header(Chromatic Aberration)]
        _Distortion ("Edge Distortion", Range(0, 0.05)) = 0.01
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

            CBUFFER_START(UnityPerMaterial)
                // УДАЛЕНО: _MainTex_ST. Это "лечит" 2D SRP Batcher.
                half4 _Color;
                float4 _Tilt;
                float _Perspective;
                float _HoloIntensity;
                float _ShineWidth;
                half4 _HoloColor;
                float _Distortion;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                
                float3 pos = input.positionOS.xyz;
                
                // Эффект перспективы
                float tiltX = pos.y * _Tilt.x * _Perspective;
                float tiltY = pos.x * _Tilt.y * _Perspective;
                
                pos.x += tiltX; 
                pos.y += tiltY;
                pos.z += (abs(pos.x) + abs(pos.y)) * _Perspective;

                output.positionCS = TransformObjectToHClip(pos);
                
                // ЗАМЕНЕНО: Просто передаем UV. TRANSFORM_TEX удален.
                output.uv = input.uv;
                
                output.color = input.color;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // Chromatic Aberration
                float2 shift = float2(_Tilt.y, -_Tilt.x) * _Distortion;
                
                half r = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv + shift).r;
                half g = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv).g;
                half b = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv - shift).b;
                half a = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv).a;

                half4 texColor = half4(r, g, b, a) * _Color * input.color;

                // Holographic Shine
                float shinePos = input.uv.x + input.uv.y;
                float movement = (_Tilt.x + _Tilt.y) * 2.0;
                float mask = smoothstep(0.5 - _ShineWidth, 0.5, frac(shinePos - movement)) * 
                             (1.0 - smoothstep(0.5, 0.5 + _ShineWidth, frac(shinePos - movement)));

                half3 rainbow = half3(
                    sin(movement + 0.0) * 0.5 + 0.5,
                    sin(movement + 2.0) * 0.5 + 0.5,
                    sin(movement + 4.0) * 0.5 + 0.5
                );

                half3 holoEffect = rainbow * mask * _HoloIntensity;
                texColor.rgb += holoEffect * texColor.a;

                return texColor;
            }
            ENDHLSL
        }
    }
}