Shader "Custom/URP_MultiStageShine"
{
    Properties
    {
        [Header(Base Settings)]
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Angle_deg ("Base Angle (Deg)", Range(-90, 90)) = 30.0
        _Brightness ("Shine Brightness", Range(0, 5)) = 2.5
        _Band_Soft ("Band Softness", Range(0, 0.3)) = 0.06

        [Header(Staging)]
        _Stage_Count ("Stage Count", Int) = 2
        _Stage1_Duration ("S1 Duration", Range(0.01, 10)) = 1.0
        _Stage2_Duration ("S2 Duration", Range(0.01, 10)) = 1.5
        _Stage3_Duration ("S3 Duration", Range(0.01, 10)) = 1.0

        _Stage1_Width ("S1 Width", Range(0, 0.5)) = 0.12
        _Stage2_Width ("S2 Width", Range(0, 0.5)) = 0.12
        _Stage3_Width ("S3 Width", Range(0, 0.5)) = 0.12

        [ToggleUI] _Stage1_LtoR ("S1 Left to Right", Float) = 1
        [ToggleUI] _Stage2_LtoR ("S2 Left to Right", Float) = 0
        [ToggleUI] _Stage3_LtoR ("S3 Left to Right", Float) = 1

        [Header(Animation Mode)]
        [ToggleUI] _One_Shot ("One Shot", Float) = 0
        _OneShot_Delay ("One Shot Delay", Range(0, 10)) = 0.0
        _Phase_Offset ("Loop Phase Offset", Range(0, 1)) = 0.0
        _Shine_Pause ("Pause Between Shines", Range(0, 10)) = 0.4

        [Header(Wiggle)]
        [ToggleUI] _Wiggle_Angle ("Wiggle Angle", Float) = 0
        _Wiggle_Amount_deg ("Wiggle Amount", Range(0, 45)) = 15.0
        
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent" 
            "RenderPipeline"="UniversalPipeline"
        }

        // Соответствие blend_premul_alpha из Godot
        Blend One OneMinusSrcAlpha
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
                float4 _MainTex_ST;
                float _Angle_deg;
                float _Brightness;
                float _Band_Soft;
                int _Stage_Count;
                float _Stage1_Duration, _Stage2_Duration, _Stage3_Duration;
                float _Stage1_Width, _Stage2_Width, _Stage3_Width;
                float _Stage1_LtoR, _Stage2_LtoR, _Stage3_LtoR;
                float _One_Shot, _OneShot_Delay, _Phase_Offset, _Shine_Pause;
                float _Wiggle_Angle, _Wiggle_Amount_deg;
                float4 _RendererColor;
            CBUFFER_END

            float2 rotate_uv(float2 uv, float2 center, float deg)
            {
                float a = radians(deg);
                float c = cos(a);
                float s = sin(a);
                float2x2 r = float2x2(c, -s, s, c);
                return mul(r, uv - center) + center;
            }

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                output.color = input.color * _RendererColor;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float4 base = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv) * input.color;
                
                // 1. Тайминги
                int N = clamp(_Stage_Count, 1, 3);
                float T1 = max(0.0001, _Stage1_Duration);
                float T2 = (N >= 2) ? max(0.0001, _Stage2_Duration) : 0.0;
                float T3 = (N >= 3) ? max(0.0001, _Stage3_Duration) : 0.0;

                float P = max(0.0, _Shine_Pause);
                float pause_count = (_One_Shot > 0.5) ? (float(N) - 1.0) : float(N);
                
                float Ttotal = T1 + T2 + T3 + P * pause_count;
                Ttotal = max(Ttotal, 0.0001);

                float t01;
                if (_One_Shot > 0.5) {
                    float t = max(0.0, _Time.y - _OneShot_Delay);
                    t01 = saturate(t / Ttotal);
                } else {
                    t01 = frac(_Phase_Offset + _Time.y / Ttotal);
                }
                float tsec = t01 * Ttotal;

                bool finished_one_shot = (_One_Shot > 0.5 && t01 >= 1.0);

                // 2. Определение текущей стадии
                float s1_start = 0.0;
                float s1_end = T1;
                float p1_end = s1_end + P;

                float s2_start = (N >= 2) ? p1_end : -1.0;
                float s2_end = (N >= 2) ? (s2_start + T2) : -1.0;
                float p2_end = (N >= 2) ? (s2_end + P) : -1.0;

                float s3_start = (N >= 3) ? p2_end : -1.0;
                float s3_end = (N >= 3) ? (s3_start + T3) : -1.0;

                int stage = 0;
                float t_local = 0.0;
                float T_stage = 1.0;

                if (tsec >= s1_start && tsec < s1_end) {
                    stage = 1; T_stage = T1; t_local = tsec - s1_start;
                } else if (N >= 2 && tsec >= s2_start && tsec < s2_end) {
                    stage = 2; T_stage = T2; t_local = tsec - s2_start;
                } else if (N >= 3 && tsec >= s3_start && tsec < s3_end) {
                    stage = 3; T_stage = T3; t_local = tsec - s3_start;
                }

                // Если пауза или конец OneShot - выводим чистую текстуру (premultiplied)
                if (finished_one_shot || stage == 0) {
                    return half4(base.rgb * base.a, base.a);
                }

                float p = saturate(t_local / T_stage);

                // 3. Угол и Виггл
                float angle_eff = _Angle_deg;
                if (_Wiggle_Angle > 0.5) {
                    float wig = sin(PI * (2.0 * p - 1.0));
                    angle_eff += _Wiggle_Amount_deg * wig;
                }

                float2 ruv = rotate_uv(input.uv, float2(0.5, 0.5), angle_eff);
                float rad = radians(angle_eff);

                // 4. Границы спрайта в повернутом пространстве
                float cos_a = abs(cos(rad));
                float sin_a = abs(sin(rad));
                float min_x = 0.5 - 0.5 * (cos_a + sin_a);
                float max_x = 0.5 + 0.5 * (cos_a + sin_a);

                // 5. Параметры полосы
                float width = _Stage1_Width;
                bool dir_now = (_Stage1_LtoR > 0.5);
                if (stage == 2) { width = _Stage2_Width; dir_now = (_Stage2_LtoR > 0.5); }
                else if (stage == 3) { width = _Stage3_Width; dir_now = (_Stage3_LtoR > 0.5); }

                float half_w = max(width * 0.5, 0.0001);
                float soft = max(_Band_Soft, 0.00001);
                float pad = half_w + soft;

                float start_x = min_x - pad;
                float end_x = max_x + pad;
                float center = dir_now ? lerp(start_x, end_x, p) : lerp(end_x, start_x, p);

                // 6. Маска блика
                float dist = abs(ruv.x - center);
                float band = 0.0;
                if (_Band_Soft <= 0.0) {
                    band = step(dist, half_w);
                } else {
                    band = 1.0 - smoothstep(half_w, half_w + soft, dist);
                }
                
                band *= base.a;

                // 7. Финальный цвет (Premultiplied Alpha)
                half3 lit_rgb = base.rgb + half3(1, 1, 1) * (band * _Brightness);
                return half4(lit_rgb * base.a, base.a);
            }
            ENDHLSL
        }
    }
}