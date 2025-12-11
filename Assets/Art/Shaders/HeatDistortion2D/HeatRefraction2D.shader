Shader "Custom/URP/HeatRefraction2D"
{
    Properties
    {
        // Sprite fournie automatiquement par le SpriteRenderer (masque)
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "" {}
        
        _DistortionAmount ("Distortion", Range(0, 0.2)) = 0.08
        _NoiseScale ("Noise Scale", Range(0.1, 10)) = 3.0
        _Speed ("Speed", Range(0, 5)) = 1.5
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "CanUseSpriteAtlas"="True"
            "RenderPipeline"="UniversalRenderPipeline"
        }

        Pass
        {
            Name "UniversalForward"
            Tags { "LightMode"="UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // Sprite (masque)
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;

            // Texture opaque de la caméra (fond)
            TEXTURE2D(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);

            float _DistortionAmount;
            float _NoiseScale;
            float _Speed;

            // ---------- bruit procédural ----------

            float hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            float Noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);

                float a = hash21(i);
                float b = hash21(i + float2(1.0, 0.0));
                float c = hash21(i + float2(0.0, 1.0));
                float d = hash21(i + float2(1.0, 1.0));

                float2 u = f * f * (3.0 - 2.0 * f);
                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            float FBM(float2 p)
            {
                float value = 0.0;
                float amp   = 0.5;
                float freq  = 1.0;

                [unroll]
                for (int i = 0; i < 4; i++)
                {
                    value += Noise(p * freq) * amp;
                    freq  *= 2.0;
                    amp   *= 0.5;
                }

                return value;
            }

            // ---------- structures ----------

            struct Attributes
            {
                float3 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                float4 color      : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float4 color       : COLOR;
                float4 screenPos   : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            // ---------- vertex ----------

            Varyings vert (Attributes IN)
            {
                Varyings OUT;

                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
                OUT.uv    = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.color = IN.color;
                OUT.screenPos = ComputeScreenPos(OUT.positionHCS);

                return OUT;
            }

            // ---------- fragment ----------

            half4 frag (Varyings IN) : SV_Target
            {
                // alpha du masque (sprite)
                half4 maskSample = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                half alpha = maskSample.a * IN.color.a;
                if (alpha <= 0.001h)
                    discard;

                // UV écran (0–1)
                float2 screenUV = IN.screenPos.xy / IN.screenPos.w;

                // bruit animé
                float t = _Time.y * _Speed;
                float2 noiseUV = IN.uv * _NoiseScale + float2(t * 0.4, t * 0.6);
                float n = FBM(noiseUV);

                // offset pour la réfraction
                float2 offset = (n - 0.5) * _DistortionAmount;
                float2 distortedUV = screenUV + offset;

                // couleur du fond déformé
                half4 bgCol = SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, distortedUV);

                // légère teinte chaude pour visualiser la zone
                half4 heatTint = half4(1.0h, 0.7h, 0.3h, 1.0h);
                half heatStrength = 0.25h;
                bgCol.rgb = lerp(bgCol.rgb, heatTint.rgb, heatStrength * alpha);

                bgCol.a = alpha;
                return bgCol;
            }

            ENDHLSL
        }
    }
}
