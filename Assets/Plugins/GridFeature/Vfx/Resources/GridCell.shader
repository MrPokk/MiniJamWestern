Shader "Custom/GridCell"
{
    Properties {
        _FillColor ("Fill Color", Color) = (1,1,1,0.1)
        _BorderColor ("Border Color", Color) = (1,1,1,1)
        _Thickness ("Thickness", Range(0.0, 0.5)) = 0.05
    }
    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            fixed4 _FillColor;
            fixed4 _BorderColor;
            float _Thickness;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // Вычисляем маску рамки (1.0 на краях, 0.0 в центре)
                float2 b = step(i.uv, _Thickness) + step(1.0 - _Thickness, i.uv);
                float mask = saturate(b.x + b.y);
                
                // Смешиваем цвета: если mask=0 берем Fill, если mask=1 берем Border
                fixed4 res = lerp(_FillColor, _BorderColor, mask);
                
                return res;
            }
            ENDCG
        }
    }
}