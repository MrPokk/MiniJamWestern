Shader "Custom_VFX/Pixel/PixelSNES_CRT_Blit_PixelVignette"
{
    Properties
    {
        [Header(SNES Settings)]
        _PixelHeight ("Pixel Height", Float) = 224.0
        _ColorLevels ("Color Levels", Float) = 32.0

        [Header(CRT Geometry)]
        _Distortion ("Screen Curvature", Range(-0.2, 0.2)) = 0.03
        _Zoom ("Zoom", Range(0.8, 1.2)) = 1.0

        [Header(CRT Vignette)]
        _VignetteStrength ("Vignette Strength", Range(0, 5)) = 0.5
        _VignetteSize ("Vignette Size", Range(0, 1)) = 0.7

        [Header(CRT Artifacts)]
        _ScanlineIntensity ("Scanline Intensity", Range(0, 1)) = 0.15
        _Aberration ("Chromatic Aberration", Range(0, 0.02)) = 0.002
    }

    SubShader
    {
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

        float _PixelHeight;
        float _ColorLevels;
        float _Distortion;
        float _Zoom;
        float _VignetteStrength;
        float _VignetteSize;
        float _ScanlineIntensity;
        float _Aberration;

        float2 CurveUV(float2 uv)
        {
            float2 centered = uv - 0.5;
            float r2 = dot(centered, centered);
            float2 distorted = centered * (1.0 + _Distortion * r2) * _Zoom;
            return distorted + 0.5;
        }

        ENDHLSL

        Tags { "RenderType"="Opaque" }
        LOD 100
        ZWrite Off Cull Off
        Pass
        {
            Name "PixelSNES_CRT_Pass"

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            float4 Frag (Varyings input) : SV_Target
            {
                // 1. Apply CRT Curvature
                float2 uv = CurveUV(input.texcoord);

                // Border Cutout
                if (uv.x < 0.0 || uv.x > 1.0 || uv.y < 0.0 || uv.y > 1.0)
                {
                    return float4(0, 0, 0, 1);
                }

                // 2. Calculate Virtual Resolution
                float aspect = _ScreenParams.x / _ScreenParams.y;
                float2 virtualRes = float2(_PixelHeight * aspect, _PixelHeight);

                // 3. Pixelate UVs (Split for Aberration)
                float2 uvR = floor((uv + float2(_Aberration, 0.0)) * virtualRes) / virtualRes;
                float2 uvG = floor(uv * virtualRes) / virtualRes; // Base Pixel UV
                float2 uvB = floor((uv - float2(_Aberration, 0.0)) * virtualRes) / virtualRes;

                // 4. Sample Texture
                float4 color;
                color.r = SAMPLE_TEXTURE2D(_BlitTexture, sampler_PointClamp, uvR).r;
                color.g = SAMPLE_TEXTURE2D(_BlitTexture, sampler_PointClamp, uvG).g;
                color.b = SAMPLE_TEXTURE2D(_BlitTexture, sampler_PointClamp, uvB).b;
                color.a = 1.0;

                // 5. PIXEL VIGNETTE (Applied BEFORE Quantization)
                // We use 'uvG' (the pixelated coordinates) so the vignette follows the grid
                float2 d = uvG - 0.5;
                float dist = dot(d, d);
                // Adjust smoothstep based on size and strength
                float vignette = smoothstep(_VignetteSize, _VignetteSize - 0.4, dist * _VignetteStrength);
                color.rgb *= vignette;

                // 6. Quantize Colors (This will now "band" the vignette)
                color.rgb = floor(color.rgb * _ColorLevels) / _ColorLevels;

                // 7. Scanlines (Applied last, as an overlay)
                float scanline = sin(uv.y * virtualRes.y * PI * 2.0);
                float scanlineEffect = ((scanline + 1.0) * 0.5);
                scanlineEffect = lerp(1.0, scanlineEffect, _ScanlineIntensity);
                color.rgb *= scanlineEffect;

                return color;
            }

            ENDHLSL
        }
    }
}
