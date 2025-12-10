Shader "Custom/EntityHeightShadow"
{
    Properties
    {
        // Propriétés standard du Sprite
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        // Propriétés de la carte de hauteur (Height Map)
        _HeightTex ("Height Map", 2D) = "gray" {}
        _TileCount ("Tile Count", Vector) = (16,16,0,0)
        _PixelsPerTile ("Pixels Per Tile", Float) = 4
        
        // Propriétés de l'ombre
        _ShadowsOffset ("Shadows Offset (XY)", Vector) = (-0.5,-0.5,0,0) // Point d'échantillonnage de la base du sprite
        _LightDir ("Light Direction (XY)", Vector) = (-1,1,0,0) // Direction de la lumière
        _ShadowStrength ("Shadow Strength", Range(0,200)) = 1
        _MaxShadowOpacity("Max Shadow Opacity", Range(0,1)) = 0.8
        _MinLightMarchStepSize("Min Light March Step Size", Float) = 0.01
        _MaxLightMarchStepSize("Max Light March Step Size", Float) = 0.05
        _TileHeightPerLevelHeight ("Shadow Distance Per Height Level", Range(0,50)) = 1.0
        
        _CurrentHeightLevel ("Current Height Level", Float) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "CanUseSpriteAtlas"="True"
            "PreviewType"="Plane"
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

            // Propriétés du Sprite
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            // Propriétés de la carte de hauteur et de l'ombre (Copie de TileHeightShadow.shader [cite: 5, 6])
            sampler2D _HeightTex;
            float4 _TileCount;
            float  _PixelsPerTile;
            
            float4 _ShadowsOffset;

            float4 _LightDir;
            float  _MinLightMarchStepSize;
            float  _MaxLightMarchStepSize;
            float  _ShadowStrength;
            float  _MaxShadowOpacity;
            float  _TileHeightPerLevelHeight;
            
            float _CurrentHeightLevel;
            
            // Structures (Copie de TileHeightShadow.shader [cite: 7, 8])
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
            
            // Vertex Shader (Copie de TileHeightShadow.shader [cite: 9, 10, 11])
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex   = UnityObjectToClipPos(v.vertex);
                o.uv       = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz; // Obtient la position du pixel dans le monde
                return o;
            }

            // Fonction pour échantillonner la hauteur (Copie de TileHeightShadow.shader [cite: 12, 13])
            inline float SampleHeight(float2 worldTilePos)
            {
                // Normaliser la position du monde aux coordonnées UV de la texture de hauteur
                float2 texSize = _TileCount.xy * _PixelsPerTile;
                float2 uv = (worldTilePos * _PixelsPerTile) / texSize;
                
                // La texture est en R8, donc la hauteur est stockée dans le canal R, multipliée par 255
                return tex2D(_HeightTex, uv).r * 255.0; 
            }

            // Fragment Shader
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                
                // Sortir si le fragment est transparent
                if (col.a <= 0) return col; // 

                float2 worldPos = i.worldPos.xy;

                
                float2 shadowSamplePos = worldPos + _ShadowsOffset.xy;
                float hCurrent = SampleHeight(shadowSamplePos); // Hauteur de la tuile sous l'entité
                float hTileUnder = SampleHeight(worldPos);

                if ((_CurrentHeightLevel / 255) * 20 < hCurrent) 
                {
                    clip(-1); // Rejeter le fragment
                }
                
                
                // --------------------------------------------------------------
                // Calcul d'ombre (Ray Marching)
                // --------------------------------------------------------------

                
                
                float2 lightDir = _LightDir.xy; // [cite: 15]
                float len = length(lightDir);
                if (len < 1e-4) return col;
                lightDir /= len;
                
                float2 stepDir = -lightDir; // [cite: 16]
                float invTileHeight = 1.0 / _TileHeightPerLevelHeight; // [cite: 16]
                
                float hAbsDir = abs(lightDir.x) + abs(lightDir.y);
                int maxSteps = 200;
                float stepInterp = lerp(64.0, maxSteps, saturate(hAbsDir)); // [cite: 23]

                // Physical shadow projection (Copie de TileHeightShadow.shader [cite: 24])
                float shadowAcc = 0.0; // [cite: 25]
                float lightSlope = invTileHeight; // pente de la lumière [cite: 25]
                float densityFactor = 0.03;

                for (int s = 1; s <= stepInterp; s++) // [cite: 26]
                {
                    float distFactor = saturate(s * _MinLightMarchStepSize / 30.0);
                    float currentStepSize = lerp(_MinLightMarchStepSize, _MaxLightMarchStepSize, distFactor * distFactor); // [cite: 27]
                    float dist = s * currentStepSize;

                    float2 samplePos = shadowSamplePos + stepDir * dist; // [cite: 28]

                    float hCaster = SampleHeight(samplePos);
                    
                    // Ignorer les ombres provenant de tuiles plus basses ou égales à la hauteur actuelle de l'entité
                    if (hCaster <= (_CurrentHeightLevel / 255.0) * 20.0)
                    {
                        continue;
                    }
                    
                    float heightDiff = hCaster - hCurrent; // Différence de hauteur par rapport à la base du sprite

                    // Condition physique : pente réelle > pente lumière [cite: 29]
                    if (heightDiff > lightSlope * dist)
                    {
                        float invShadowLength = 1.0 / (heightDiff * _TileHeightPerLevelHeight); // [cite: 29]
                        float falloff = saturate(1.0 - dist * invShadowLength); // [cite: 30]
                        float density = saturate(heightDiff * densityFactor);

                        shadowAcc += density * falloff; // [cite: 30]
                    }
                }

                float shadowFactor = saturate(shadowAcc * _ShadowStrength); // [cite: 31]
                shadowFactor = min(shadowFactor, _MaxShadowOpacity); // [cite: 32]
                
                // Application de l'ombre à la couleur du sprite
                col.rgb *= (1.0 - shadowFactor); // [cite: 32]
                return col; // [cite: 33]
            }
            ENDCG
        }
    }
}