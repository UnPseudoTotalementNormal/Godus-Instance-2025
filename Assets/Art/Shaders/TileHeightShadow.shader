Shader "Unlit/TileHeightShadow"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _HeightTex ("Height Map", 2D) = "gray" {}
        _TileCount ("Tile Count", Vector) = (16,16,0,0)
        _LightDir ("Light Direction", Vector) = (-1,1,0,0)
        _ShadowStrength ("Shadow Strength", Range(0,5)) = 1
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
            float4 _LightDir;       // xy = direction
            float _ShadowStrength;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv      : TEXCOORD0;
                float4 vertex  : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Couleur de base de la tuile
                fixed4 col = tex2D(_MainTex, i.uv);
                if (col.a <= 0) return col;

                float2 tileCount = _TileCount.xy;

                // Hypothèse simple : 1 unité monde = 1 tile,
                // et ta grille commence en (0,0).
                float2 tileCoord = floor(i.worldPos.xy);

                // UV vers la heightmap : 1 pixel = 1 tile
                float2 uvHeight = (tileCoord + 0.5) / tileCount;
                fixed4 hCurrentColor = tex2D(_HeightTex, uvHeight);
                float hCurrent = hCurrentColor.r * 255.0;

                // Direction de la lumière → on projette l'ombre dans la direction opposée
                float2 lightDir = normalize(_LightDir.xy);
                float2 casterTileCoord = tileCoord - lightDir;
                float2 casterUvHeight = (casterTileCoord + 0.5) / tileCount;
                fixed4 hCasterColor = tex2D(_HeightTex, casterUvHeight);
                float hCaster = hCasterColor.r * 255.0;

                // Différence de hauteur : si > 0 → ombre
                float delta = max(0.0, hCaster - hCurrent);
                float shadowFactor = saturate(delta * _ShadowStrength);

                // Assombrissement (tu peux ajuster le 0.5)
                col.rgb *= (1.0 - shadowFactor * 0.5);

                return col;
            }
            ENDCG
        }
    }
}
