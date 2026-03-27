Shader "Custom_VFX/Pixel/PixelDither_Monochrome"
{
    Properties
    {
        [Header(Settings)]
        _PixelDensity ("Pixel Density", Float) = 256.0
        _Spread ("Dither Spread", Range(0, 1)) = 0.5

        [Header(Colors)]
        _ColorDark ("Dark Color", Color) = (0.05, 0.05, 0.2, 1) // Deep Blue
        _ColorLight ("Light Color", Color) = (0.8, 0.9, 1.0, 1) // White-ish Blue
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 100
        ZWrite Off Cull Off

        Pass
        {
            Name "Dither1BitPass"

            HLSLPROGRAM
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #pragma vertex Vert
            #pragma fragment Frag


            CBUFFER_START(UnityPerMaterial)
                float _PixelDensity;
                float _Spread;
                float4 _ColorDark;
                float4 _ColorLight;
            CBUFFER_END

            // 4x4 Bayer Matrix for Ordered Dithering
            static const float4x4 bayerDither = float4x4(
                 0.0,  8.0,  2.0, 10.0,
                12.0,  4.0, 14.0,  6.0,
                 3.0, 11.0,  1.0,  9.0,
                15.0,  7.0, 13.0,  5.0
            ) / 16.0;

            float4 Frag (Varyings input) : SV_Target
            {
                float aspect = _ScreenParams.x / _ScreenParams.y;
                float2 pixelCount = float2(_PixelDensity * aspect, _PixelDensity);
                
                float2 uv = floor(input.texcoord * pixelCount) / pixelCount;

                float4 color = SAMPLE_TEXTURE2D(_BlitTexture, sampler_PointClamp, uv);

                float luminance = dot(color.rgb, float3(0.299, 0.587, 0.114));

                uint2 pixelPos = uint2(input.texcoord * pixelCount);
                float threshold = bayerDither[pixelPos.x % 4][pixelPos.y % 4];

                float ditherResult = luminance + (threshold - 0.5) * _Spread;
                
                float3 finalColor = step(0.5, ditherResult) == 0.0 ? _ColorDark.rgb : _ColorLight.rgb;

                return float4(finalColor, 1.0);
            }
            
            ENDHLSL
        }
    }
}