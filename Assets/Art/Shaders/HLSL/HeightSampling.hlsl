#ifndef HEIGHT_SAMPLING_INCLUDED
#define HEIGHT_SAMPLING_INCLUDED

float SampleHeight(sampler2D heightTex, float2 worldTilePos, float4 tileCount, float pixelsPerTile)
{
    float2 texSize = tileCount.xy * pixelsPerTile;
    float2 uv = (worldTilePos * pixelsPerTile) / texSize;
    return tex2D(heightTex, uv).r * 255.0;
}

#endif

