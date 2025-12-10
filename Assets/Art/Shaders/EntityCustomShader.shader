Shader "Custom/EntityCustomShader"
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
        
        // Propriétés du flash
        _FlashColor ("Flash Color", Color) = (1,1,1,1)
        _FlashDuration ("Flash Duration", Float) = 0.1
        _HitTime ("Hit Time", Float) = -9999
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
            #include "HLSL/TileHeightVisuals.hlsl"
            #include "HLSL/SpriteHitFlashFunction.hlsl"

            // Propriétés du Sprite
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            // Propriétés de la carte de hauteur et de l'ombre
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
            
            // Propriétés du flash
            fixed4 _FlashColor;
            float _FlashDuration;
            float _HitTime;
            
            // Structures
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
            
            // Vertex Shader
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex   = UnityObjectToClipPos(v.vertex);
                o.uv       = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            // Fragment Shader
            fixed4 frag (v2f i) : SV_Target
            {
                // Appliquer d'abord les effets de hauteur et d'ombre
                fixed4 col = ApplyTileHeightVisuals(
                    _MainTex,
                    _HeightTex,
                    i.uv,
                    i.worldPos,
                    _Color,
                    _TileCount,
                    _PixelsPerTile,
                    _ShadowsOffset,
                    _LightDir,
                    _MinLightMarchStepSize,
                    _MaxLightMarchStepSize,
                    _ShadowStrength,
                    _MaxShadowOpacity,
                    _TileHeightPerLevelHeight,
                    _CurrentHeightLevel
                );
                
                col = ApplySpriteHitFlashAuto(col, _FlashColor.rgb, _HitTime, _FlashDuration);
                
                return col;
            }
            ENDCG
        }
    }
}