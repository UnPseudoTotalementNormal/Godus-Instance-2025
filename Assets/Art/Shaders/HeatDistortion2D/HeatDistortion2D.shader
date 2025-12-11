Shader "Custom/URP/HeatDistortionSprite2D"
{
    Properties
    {
        // Sprite fournie par le SpriteRenderer
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "" {}

        _DistortionAmount ("Distortion", Range(0, 0.1)) = 0.03
        _NoiseScale ("Noise Scale", Range(0.1, 10)) = 2.0
        _Speed ("Speed", Range(0, 5)) = 1.0
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
            Name "Universal2D"
            Tags { "LightMode"="Universal2D" }

            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // Sprite
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;

            float _DistortionAmount;
            float _NoiseScale;
            float _Speed;

            // ----------- bruit procédural -----------

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
                float4 color      : COLOR;      // couleur du SpriteRenderer
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float4 color       : COLOR;
            };

            // ---------- vertex ----------

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.color = IN.color;
                return OUT;
            }

            // ---------- fragment ----------

            half4 frag (Varyings IN) : SV_Target
            {
                // couleur de base du sprite
                half4 spriteCol = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * IN.color;
                half alpha = spriteCol.a;
                if (alpha <= 0.001h)
                    discard;

                // bruit animé
                float t = _Time.y * _Speed;
                float2 noiseUV = IN.uv * _NoiseScale + float2(t * 0.4, t * 0.6);
                float n = FBM(noiseUV);

                // offset de distorsion
                float2 offset = (n - 0.5) * _DistortionAmount;

                // on redéforme les UV pour échantillonner la sprite
                float2 distortedUV = IN.uv + offset;
                half4 distortedSprite = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, distortedUV) * IN.color;

                return half4(distortedSprite.rgb, alpha);
            }

            ENDHLSL
        }
    }
}
