Shader "Custom/ScrollingShaderURP_WithColor"
{
    Properties
    {
        [MainTexture] _MainTex ("Texture", 2D) = "white" {}
        [MainColor] _BaseColor ("Color", Color) = (1, 1, 1, 1) // Цвет материала
        _MoveSpeed ("Move Speed", Vector) = (0.1, 0.1, 0, 0)
        _TextureScale ("Texture Scale", Float) = 1.0
    }

    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent" 
            "RenderPipeline" = "UniversalPipeline" 
            "PreviewType"="Plane" // Удобно для спрайтов
        }

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
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
                float4 color        : COLOR; // Цвет из SpriteRenderer / Vertex Color
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 uv           : TEXCOORD0;
                float4 color        : COLOR; 
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_mainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float2 _MoveSpeed;
                float _TextureScale;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);

                // Расчет UV с учетом масштаба и времени
                OUT.uv = IN.uv * _TextureScale + (_MoveSpeed * 0.1 * _Time.y);
                
                // Прокидываем цвет вершин (SpriteRenderer использует его)
                OUT.color = IN.color * _BaseColor;
                
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Выборка текстуры
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_mainTex, IN.uv);
                
                // Умножаем текстуру на итоговый цвет (Material Color * Sprite Color)
                return texColor * IN.color;
            }
            ENDHLSL
        }
    }
}