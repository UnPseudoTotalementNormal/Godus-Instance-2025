#ifndef SPRITE_HIT_FLASH_INCLUDED
#define SPRITE_HIT_FLASH_INCLUDED

// Fonction HLSL pour calculer l'effet de flash sur un sprite
// Paramètres :
//   baseColor : la couleur de base du sprite (texture * tint)
//   flashColor : la couleur du flash
//   hitTime : le temps (_Time.y) auquel le hit s'est produit
//   currentTime : le temps actuel (_Time.y)
//   flashDuration : la durée du flash en secondes
// Retourne : la couleur finale avec l'effet de flash appliqué
float4 ApplySpriteHitFlash(
    float4 baseColor,
    float3 flashColor,
    float hitTime,
    float currentTime,
    float flashDuration
)
{
    // Temps écoulé depuis le dernier hit
    float elapsed = currentTime - hitTime;

    // Calcul du montant de flash (0 = pas de flash, 1 = flash complet)
    float flashAmount = 0.0;
    if (elapsed >= 0.0 && elapsed <= flashDuration)
    {
        // Interpolation linéaire : 1 au moment du hit, 0 à la fin
        float t = saturate(elapsed / flashDuration);
        flashAmount = 1.0 - t;
    }

    // Application du flash sur les canaux RGB uniquement
    float4 result = baseColor;
    result.rgb = lerp(baseColor.rgb, flashColor, flashAmount);
    
    return result;
}

// Version simplifiée qui utilise directement _Time.y
float4 ApplySpriteHitFlashAuto(
    float4 baseColor,
    float3 flashColor,
    float hitTime,
    float flashDuration
)
{
    return ApplySpriteHitFlash(baseColor, flashColor, hitTime, _Time.y, flashDuration);
}

#endif // SPRITE_HIT_FLASH_INCLUDED

