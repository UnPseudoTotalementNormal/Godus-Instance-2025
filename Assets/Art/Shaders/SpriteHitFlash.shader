Shader "Custom/SpriteHitFlash_TimeBased"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _FlashColor ("Flash Color", Color) = (1,1,1,1)
        _FlashDuration ("Flash Duration", Float) = 0.1
        _HitTime ("Hit Time", Float) = -9999
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _FlashColor;
            float _FlashDuration;
            float _HitTime;
            // _Time est déjà fourni par Unity, pas besoin de le déclarer

            struct appdata
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex   = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                o.color    = v.color * _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.texcoord) * i.color;

                // temps écoulé depuis le dernier hit
                float elapsed = _Time.y - _HitTime;

                // si elapsed < 0 ou > durée, pas de flash
                float flashAmount = 0;
                if (elapsed >= 0 && elapsed <= _FlashDuration)
                {
                    // 1 au moment du hit, 0 à la fin
                    float t = saturate(elapsed / _FlashDuration);
                    flashAmount = 1.0 - t;
                }

                col.rgb = lerp(col.rgb, _FlashColor.rgb, flashAmount);
                return col;
            }
            ENDCG
        }
    }
}
