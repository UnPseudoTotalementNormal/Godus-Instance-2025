#include "HeightSampling.hlsl"

float4 ApplyTileHeightVisuals(
    sampler2D mainTex,
    sampler2D heightTex,
    float2 uv,
    float3 worldPos,
    float4 color,

    float4 tileCount,
    float pixelsPerTile,

    float4 shadowsOffset,
    float4 lightDir,
    float minStep,
    float maxStep,
    float shadowStrength,
    float maxShadowOpacity,
    float tileHeightPerLevel,

    float currentHeightLevel
)
{
    float4 col = tex2D(mainTex, uv) * color;

    if (col.a <= 0)
        return col;

    float2 worldXY = worldPos.xy;
    float2 shadowSamplePos = worldXY + shadowsOffset.xy;

    float hCurrent = SampleHeight(heightTex, shadowSamplePos, tileCount, pixelsPerTile);

    // Division entière comme dans l'ancien code CG
    if ((currentHeightLevel / 255.0) * 20.0 < hCurrent)
    {
        clip(-1);
    }

    float2 lDir = lightDir.xy;
    float len = length(lDir);
    if (len < 1e-4)
        return col;

    lDir /= len;
    float2 stepDir = -lDir;
    float invTileHeight = 1.0 / tileHeightPerLevel;

    float hAbsDir = abs(lDir.x) + abs(lDir.y);
    int maxSteps = 200;
    int stepInterp = (int)lerp(64.0, (float)maxSteps, saturate(hAbsDir));

    float shadowAcc = 0.0;
    float lightSlope = invTileHeight;
    float densityFactor = 0.03;

    for (int s = 1; s <= stepInterp; s++)
    {
        float distFactor = saturate(s * minStep / 30.0);
        float currentStepSize = lerp(minStep, maxStep, distFactor * distFactor);
        float dist = s * currentStepSize;

        float2 samplePos = shadowSamplePos + stepDir * dist;
        float hCaster = SampleHeight(heightTex, samplePos, tileCount, pixelsPerTile);

        if (hCaster <= (currentHeightLevel / 255.0) * 20.0)
        {
            continue;
        }

        float heightDiff = hCaster - hCurrent;

        if (heightDiff > lightSlope * dist)
        {
            float invShadowLength = 1.0 / (heightDiff * tileHeightPerLevel);
            float falloff = saturate(1.0 - dist * invShadowLength);
            float density = saturate(heightDiff * densityFactor);

            shadowAcc += density * falloff;
        }
    }

    float shadowFactor = saturate(shadowAcc * shadowStrength);
    shadowFactor = min(shadowFactor, maxShadowOpacity);

    col.rgb *= (1.0 - shadowFactor);
    return col;
}

