Shader "Unlit/TileHeightShadow"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _HeightTex ("Height Map", 2D) = "gray" {}

        _TileCount ("Tile Count", Vector) = (16,16,0,0)
        _PixelsPerTile ("Pixels Per Tile", Float) = 4

        _LightDir ("Light Direction (XY)", Vector) = (-1,1,0,0)
        _ShadowStrength ("Shadow Strength", Range(0,200)) = 1
        _MaxShadowOpacity("Max Shadow Opacity", Range(0,1)) = 0.8
        _MinLightMarchStepSize("Min Light March Step Size", Float) = 0.01
        _MaxLightMarchStepSize("Max Light March Step Size", Float) = 0.05
        _TileHeightPerLevelHeight ("Shadow Distance Per Height Level", Range(0,50)) = 1.0
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
            float4 _TileCount;
            float  _PixelsPerTile;

            float4 _LightDir;
            float  _MinLightMarchStepSize;
            float  _MaxLightMarchStepSize;
            float  _ShadowStrength;
            float  _MaxShadowOpacity;
            float  _TileHeightPerLevelHeight;

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

            inline float SampleHeight(float2 worldTilePos)
            {
                float2 texSize = _TileCount.xy * _PixelsPerTile;
                float2 uv = (worldTilePos * _PixelsPerTile) / texSize;
                return tex2D(_HeightTex, uv).r * 255.0;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                if (col.a <= 0) return col;

                float2 worldPos = i.worldPos.xy;
                float hCurrent = SampleHeight(worldPos);

                float2 lightDir = _LightDir.xy;
                float len = length(lightDir);
                if (len < 1e-4) return col;
                lightDir /= len;

                float2 stepDir = -lightDir;
                float stepSize = _MinLightMarchStepSize;
                float invTileHeight = 1.0 / _TileHeightPerLevelHeight;

                /*
                // Coarse test ----------------------------------------------------
                const int coarseSteps = 60;
                bool possibleBlocker = false;

                for (int s = 1; s <= coarseSteps; s++)
                {
                    float dist = s * (stepSize * 4.0);
                    float2 pos = worldPos + stepDir * dist;
                    float hCaster = SampleHeight(pos);

                    if (hCaster > hCurrent + dist * invTileHeight)
                    {
                        possibleBlocker = true;
                        break;
                    }
                }

                if (!possibleBlocker)
                    return col;
                */
                
                float hAbsDir = abs(lightDir.x) + abs(lightDir.y);
                int maxSteps = 350;
                float stepInterp = lerp(64.0, maxSteps, saturate(hAbsDir));

                //--------------------------------------------------------------
                // Physical shadow projection
                //--------------------------------------------------------------

                float shadowAcc = 0.0;
                float lightSlope = invTileHeight; // pente de la lumière
                float densityFactor = 0.03;
                
                for (int s = 1; s <= stepInterp; s++)
                {
                    float distFactor = saturate(s * stepSize / 30);
                    float currentStepSize = lerp(_MinLightMarchStepSize, _MaxLightMarchStepSize, distFactor * distFactor);
                    float dist = s * currentStepSize;
                    
                    float2 samplePos = worldPos + stepDir * dist;

                    float hCaster = SampleHeight(samplePos);
                    float heightDiff = hCaster - hCurrent;
                    

                    // condition physique : pente réelle > pente lumière
                    if (heightDiff > lightSlope * dist)
                    {
                        float invShadowLength = 1.0 / (heightDiff * _TileHeightPerLevelHeight);
                        float falloff = saturate(1.0 - dist * invShadowLength);
                        float density = saturate(heightDiff * densityFactor);

                        shadowAcc += density * falloff;
                    }
                }

                float shadowFactor = saturate(shadowAcc * _ShadowStrength);
                
                shadowFactor = min(shadowFactor, _MaxShadowOpacity);
                
                col.rgb *= (1.0 - shadowFactor);
                return col;
            }
            ENDCG
        }
    }
}
