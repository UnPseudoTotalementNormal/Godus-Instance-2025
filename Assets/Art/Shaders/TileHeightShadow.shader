Shader "Unlit/TileHeightShadow"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _HeightTex ("Height Map", 2D) = "gray" {}

        _TileCount ("Tile Count", Vector) = (16,16,0,0)
        _PixelsPerTile ("Pixels Per Tile", Float) = 4

        _LightDir ("Light Direction (XY)", Vector) = (-1,1,0,0)
        _ShadowStrength ("Shadow Strength", Range(0,5)) = 1
        _MaxShadowDistance ("Max Shadow Distance (tiles)", Range(0,32)) = 8
        _HeightFalloff ("Height Falloff per tile", Range(0,2)) = 0.2
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
            float  _PixelsPerTile;  // résolution par tuile

            float4 _LightDir;       // xy = direction lumière
            float  _ShadowStrength;
            float  _MaxShadowDistance;
            float  _HeightFalloff;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv       : TEXCOORD0;
                float4 vertex   : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex   = UnityObjectToClipPos(v.vertex);
                o.uv       = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            // worldTilePos.xy = position en "coordonnées tuile" CONTINUES
            // (1 unité monde = 1 tuile)
            float SampleHeight(float2 worldTilePos)
            {
                float2 tileCount = _TileCount.xy;
                float  pixels    = _PixelsPerTile;
                float2 texSize   = tileCount * pixels;

                // Coord continue -> UV dans la heightmap sur-échantillonnée
                float2 uv = (worldTilePos * pixels + 0.5) / texSize;

                // R8 : rouge = hauteur / 255
                return tex2D(_HeightTex, uv).r * 255.0;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                if (col.a <= 0) return col;

                // Position actuelle en coordonnées "tuile" CONTINUES
                // (pas de floor → permet des diagonales lisses)
                float2 worldPos = i.worldPos.xy;

                // Hauteur à la position exacte du fragment
                float hCurrent = SampleHeight(worldPos);

                // Direction lumière
                float2 lightDir = _LightDir.xy;
                float len = length(lightDir);
                if (len < 1e-4)
                    return col; // pas de lumière => pas d'ombre

                lightDir /= len;

                // On remonte vers le soleil (raymarch)
                float2 stepDir  = -lightDir;   // vers la lumière
                float  stepSize = 0.25;        // < 1 tuile pour du vrai raymarch

                float maxDelta = 0.0;

                // Bornes
                int   maxSteps = 128;
                float maxDist  = _MaxShadowDistance;

                for (int s = 1; s <= maxSteps; s++)
                {
                    float dist = s * stepSize;
                    if (dist > maxDist)
                        break;

                    float2 samplePos = worldPos + stepDir * dist;
                    float  hCaster   = SampleHeight(samplePos);

                    // On réduit l'influence avec la distance (HeightFalloff)
                    float delta = (hCaster - hCurrent) - dist * _HeightFalloff;

                    maxDelta = max(maxDelta, delta);
                }

                float shadowFactor = saturate(maxDelta * _ShadowStrength);

                // Assombrissement (0.5 = profondeur max de l’ombre)
                col.rgb *= (1.0 - shadowFactor);

                return col;
            }
            ENDCG
        }
    }
}
