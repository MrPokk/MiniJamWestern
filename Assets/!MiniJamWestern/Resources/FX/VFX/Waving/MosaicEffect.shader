Shader "Custom/MosaicEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [HideInInspector] _TileSize ("Размер мозаики", Float) = 500
        _Speed ("Скорость", Range(0.1, 3)) = 1
        [KeywordEnum(Right, Left, Up, Down, Right_Up, Right_Down, Left_Up, Left_Down)]
        _Direction ("Направление", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _DIRECTION_RIGHT _DIRECTION_LEFT _DIRECTION_UP _DIRECTION_DOWN _DIRECTION_RIGHT_UP _DIRECTION_RIGHT_DOWN _DIRECTION_LEFT_UP _DIRECTION_LEFT_DOWN
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4    _MainTex_ST;
            float     _TileSize;
            float     _Speed;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float  offset = _Time.y * _Speed;
                float2 direction = float2(0, 0);

                #ifdef _DIRECTION_RIGHT
                direction = float2(1, 0);
                #elif _DIRECTION_LEFT
                        direction = float2(-1, 0);
                #elif _DIRECTION_UP
                        direction = float2(0, 1);
                #elif _DIRECTION_DOWN
                        direction = float2(0, -1);
                #elif _DIRECTION_RIGHT_UP
                        direction = normalize(float2(1, 1));
                #elif _DIRECTION_RIGHT_DOWN
                        direction = normalize(float2(1, -1));
                #elif _DIRECTION_LEFT_UP
                        direction = normalize(float2(-1, 1));
                #elif _DIRECTION_LEFT_DOWN
                        direction = normalize(float2(-1, -1));
                #endif

                const float TILE_SIZE = 500;
                float2      uv = i.uv * TILE_SIZE;
                float2      mosaicUV = floor(uv) / TILE_SIZE;

                mosaicUV += offset * direction;
                mosaicUV = frac(mosaicUV);

                return tex2D(_MainTex, mosaicUV);
            }
            ENDCG
        }
    }
}