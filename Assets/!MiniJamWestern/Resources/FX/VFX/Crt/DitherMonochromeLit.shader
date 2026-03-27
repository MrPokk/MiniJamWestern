Shader "Custom_VFX/Object/Lit/DitherMonochromeLit"
{
    Properties
    {
        [MainTexture] _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _ColorDark ("Dark Color", Color) = (0.0, 0.0, 0.1, 1)
        _ColorLight ("Light Color", Color) = (0.8, 0.9, 1.0, 1)
        _DitherScale ("Dither Scale", Float) = 1.0
        _LightThreshold ("Light Threshold", Range(-1, 1)) = 0.0
        
        [Header(Alpha Settings)]
        _AlphaClip ("Alpha Clip Threshold", Range(0, 1)) = 0.5
        [Toggle]_USE_ALPHA_CLIP ("Use Alpha Clip", Float) = 1
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "RenderPipeline"="UniversalPipeline" 
            "Queue"="Transparent"
        }
        
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            Name "DitherLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma shader_feature _USE_ALPHA_CLIP_ON
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _ColorDark;
                float4 _ColorLight;
                float _DitherScale;
                float _LightThreshold;
                float _AlphaClip;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            static const float4x4 bayerDither = float4x4(
                 0.0,  8.0,  2.0, 10.0,
                12.0,  4.0, 14.0,  6.0,
                 3.0, 11.0,  1.0,  9.0,
                15.0,  7.0, 13.0,  5.0
            ) / 16.0;

            Varyings Vert(Attributes input)
            {
                Varyings output;
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInput.positionCS;
                output.screenPos = ComputeScreenPos(output.positionCS);
                
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, float4(0,0,0,0));
                output.normalWS = normalInput.normalWS;
                
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                
                return output;
            }

            float4 Frag(Varyings input) : SV_Target
            {
                float4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                
                // Alpha clip
                #ifdef _USE_ALPHA_CLIP_ON
                    clip(texColor.a - _AlphaClip);
                #endif
                
                Light mainLight = GetMainLight();
                float3 lightDir = normalize(mainLight.direction);
                float3 normal = normalize(input.normalWS);

                float NdotL = dot(normal, lightDir);
                float brightness = Luminance(texColor.rgb) * NdotL;

                // Screen-space dither
                float2 screenUV = input.screenPos.xy / input.screenPos.w * _ScreenParams.xy;
                uint2 pixelPos = uint2(screenUV / _DitherScale);
                float threshold = bayerDither[pixelPos.x % 4][pixelPos.y % 4];
                
                float ditherResult = brightness + (threshold - 0.5);
                
                // Lerp between colors based on dither
                float lerpValue = step(_LightThreshold, ditherResult);
                float3 finalColor = lerp(_ColorDark.rgb, _ColorLight.rgb, lerpValue);
                
                // Alpha from texture or lerp between colors' alpha
                float finalAlpha = texColor.a * lerp(_ColorDark.a, _ColorLight.a, lerpValue);

                return float4(finalColor, finalAlpha);
            }
            ENDHLSL
        }
    }
}