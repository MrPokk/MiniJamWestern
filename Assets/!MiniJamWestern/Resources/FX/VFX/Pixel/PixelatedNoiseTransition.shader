Shader "Custom/UI/PixelatedNoiseTransition"
{
    Properties
    {[PerRendererData] _MainTex ("Texture", 2D) = "white" {}
        _Progress ("Progress", Range(0.0, 1.0)) = 0.0
        _Speed ("Speed", Float) = 0.1
        _PixelationX ("Pixelation X", Float) = 2.0
        _PixelationY ("Pixelation Y", Float) = 2.0
        _Zoom ("Zoom", Float) = 2.0
        
        _BackgroundThreshold ("Background Threshold", Float) = 0.0
        _ColorThreshold ("Color Threshold", Float) = 0.24
        _TransitionColor ("Color", Color) = (0,0,0,1)
        
        _Seed ("Seed", Float) = 0.0
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Overlay" 
            "RenderPipeline"="UniversalPipeline"
        }
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        ZTest Always
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            float _Progress;
            float _Speed;
            float _PixelationX;
            float _PixelationY;
            float _Zoom;
            float _BackgroundThreshold;
            float _ColorThreshold;
            float4 _TransitionColor;
            float _Seed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            float rand(float2 n) 
            { 
                return frac(sin(dot(n, float2(12.9898 + _Seed, 4.1414 - _Seed))) * (43758.5453 + _Seed * 1000.0));
            }

            float noise(float2 p)
            {
                float2 ip = floor(p);
                float2 u = frac(p);
                u = u * u * (3.0 - 2.0 * u);

                float res = lerp(
                    lerp(rand(ip), rand(ip + float2(1.0, 0.0)), u.x),
                    lerp(rand(ip + float2(0.0, 1.0)), rand(ip + float2(1.0, 1.0)), u.x), u.y);
                    
                return res * res;
            }

            float fbm(float2 p)
            {
                float f = 0.0;
                float timeVar = _Time.y * _Speed - _Seed;
                
                // В HLSL матрицы объявляются по строкам, используем mul()
                float2x2 mtx = float2x2(0.80, -0.60, 0.60, 0.80);
                
                f += 0.500000 * noise( p + timeVar ); p = mul(mtx, p) * 2.02;
                f += 0.031250 * noise( p );           p = mul(mtx, p) * 2.01;
                f += 0.250000 * noise( p );           p = mul(mtx, p) * 2.03;
                f += 0.125000 * noise( p );           p = mul(mtx, p) * 2.01;
                f += 0.062500 * noise( p );           p = mul(mtx, p) * 2.04;
                f += 0.015625 * noise( p + sin(timeVar) );

                return f / 0.96875;
            }

            float pattern(float2 p)
            {
                return fbm( p + fbm( p + fbm( p ) ) );
            }

            float4 colormap(float x, float2 uv) 
            {
                // Маска для диагонального проявления
                float mask = max(0.0, min(-abs(_Progress * 4.0 - uv.x - uv.y - 1.0) + 1.0, 1.0) * 2.0);
                x *= mask;
                
                if (x < _BackgroundThreshold) 
                { 
                    return float4(0.0, 0.0, 0.0, 0.0);
                }
                else if (x < _ColorThreshold) 
                { 
                    float factor = round((x - _BackgroundThreshold) / (_ColorThreshold - _BackgroundThreshold));
                    return lerp(float4(0.0, 0.0, 0.0, 0.0), _TransitionColor, factor);
                }
                else 
                {
                    return _TransitionColor;
                }
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 pixelation = float2(_PixelationX, _PixelationY);
                // В Unity ScreenParams.xy - это разрешение экрана. Это аналог 1.0 / SCREEN_PIXEL_SIZE в Godot.
                float2 modifier = _ScreenParams.xy / pixelation;
                
                float2 uv = floor(i.uv * modifier) / modifier;
                float shade = pattern(uv * _Zoom);
                
                return colormap(shade, uv);
            }
            ENDCG
        }
    }
}