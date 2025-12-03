Shader "Unlit/TileHeightShadow"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _HeightTex ("Height Map", 2D) = "gray" {}
        _TileCount ("Tile Count", Vector) = (16,16,0,0)
        _PixelsPerTile ("Pixels Per Tile", Float) = 4
        _LightDir ("Light Direction", Vector) = (1,-1,0,0)
        _ShadowStrength ("Shadow Strength", Range(0,10)) = 1.5
        [KeywordEnum(None, HeightMap, ShadowOnly, UVCoords)] _DebugMode ("Debug Mode", Float) = 0
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "CanUseSpriteAtlas"="True"
        }

        // Standard pour un shader 2D transparent
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _HeightTex;
            float4 _TileCount;      // x = width, y = height
            float _PixelsPerTile;   // Résolution de la heightmap
            float4 _LightDir;       // xy = direction
            float _ShadowStrength;
            float _DebugMode;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv      : TEXCOORD0;
                float4 vertex  : SV_POSITION;
                float2 worldPos : TEXCOORD1; // Position monde précise (pas arrondie)
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                // Obtenir la position monde sans arrondir
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldPos = worldPos.xy;
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                if (col.a <= 0) return col;

                float2 tileCount = _TileCount.xy;
                float2 tilePos = i.worldPos; // Position monde précise (pas arrondie)
                float2 uvHeight = tilePos / tileCount;
                
                float hCurrent = tex2D(_HeightTex, uvHeight).r * 255.0;

                // DEBUG MODES
                if (_DebugMode > 0.5 && _DebugMode < 1.5) // HeightMap
                {
                    col.rgb = hCurrent / 10.0;
                    return col;
                }
                if (_DebugMode > 2.5 && _DebugMode < 3.5) // UVCoords
                {
                    col.rgb = float3(uvHeight.x, uvHeight.y, 0);
                    return col;
                }

                // Direction de la lumière normalisée
                float2 lightDir = normalize(_LightDir.xy);
                
                // Calcul des ombres
                float shadowFactor = 0.0;
                
                for (int s = 1; s <= 12; s++)
                {
                    float dist = float(s) * 0.4;
                    float2 casterUV = (tilePos - lightDir * dist) / tileCount;
                    
                    if (casterUV.x < 0 || casterUV.x > 1 || casterUV.y < 0 || casterUV.y > 1)
                        continue;
                    
                    float hCaster = tex2D(_HeightTex, casterUV).r * 255.0;
                    float heightDiff = hCaster - hCurrent;
                    
                    if (heightDiff > 0.1)
                    {
                        // Calcul simplifié : plus la différence de hauteur est grande, plus l'ombre est forte
                        float shadow = heightDiff * 0.1; // Conversion niveau -> intensité d'ombre
                        shadowFactor = max(shadowFactor, shadow);
                    }
                }

                // Appliquer la force d'ombre contrôlable
                shadowFactor = shadowFactor * _ShadowStrength * 5;

                // DEBUG MODE 2 : Shadow Only
                if (_DebugMode > 1.5 && _DebugMode < 2.5)
                {
                    col.rgb = shadowFactor;
                    return col;
                }

                // Application de l'ombre - ici on peut aller jusqu'à noir complet
                col.rgb = lerp(col.rgb, float3(0, 0, 0), shadowFactor);

                return col;
            }
            ENDCG
        }
    }
}
