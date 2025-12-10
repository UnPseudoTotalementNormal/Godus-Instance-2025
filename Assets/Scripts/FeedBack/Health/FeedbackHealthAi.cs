using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.Assertions;

namespace Feedback.Health
{
    [RequireComponent(typeof(HealthComponent))]
    public class FeedbackHealthAi : MonoBehaviour, IFX
    {
        [Header("    Health Feedback Settings")]
        [SerializeField] [Range(1, 5)] private float flashDuration = 1f;
        [SerializeField] private Color flashColor = Color.white;

        [SerializeField] private SpriteRenderer spriteRenderer;
        private Material material;
        private int hitTimeID;
        private int flashDurationID;
        private int flashColorID;

        [Space(10)]
        
        [Header("    Text Feedback Settings")]
        [SerializeField] private TextMeshPro healthText;
        
        [Space(10)]
        
        [Header("    Scale Animation Settings")]
        [SerializeField] private float damagePunchScaleDuration = 1f;
        [SerializeField] private float damagePunchScaleStrength = 1.2f;
        [SerializeField] private int damagePunchScaleVibrato = 5;
        [SerializeField] private float damagePunchScaleElasticity = 1f;
        
        [Header("Reference")]
        private HealthComponent healthComponent;
        
        

        private void Awake()
        {
            healthComponent = GetComponent<HealthComponent>();
            Assert.IsNotNull(healthComponent, $"<b>[FeedbackHealthAi]</b> HealthComponent is missing on {gameObject.name}.");
            Assert.IsNotNull(spriteRenderer, $"<b>[FeedbackHealthAi]</b> SpriteRenderer is missing on {gameObject.name}.");
            
            InitializeShaderProperties();
        }

        private void OnEnable()
        {
            healthComponent.onDamaged += HandleDamaged;
        }

        private void OnDisable()
        {
            healthComponent.onDamaged -= HandleDamaged;
        }

        #region EventHandlers
        private void HandleDamaged(float damage)
        {
            PlayFlash();
            PlayDamageAnimation();
        }

        private void HandleHealed(float healAmount)
        {

        }

        private void HandleDeath()
        {
            
        }
        #endregion
        
        #region Animations
        
        private void PlayDamageAnimation()
        {
            Debug.Log("Playing damage animation");
            transform.DOPunchScale(Vector3.one * damagePunchScaleStrength, damagePunchScaleDuration, damagePunchScaleVibrato, damagePunchScaleElasticity);
        }
        
        private void PlayHealTextAnimation()
        {
            // TODO : implement heal text animation
            // Style cookieClicker text a instantier sur la position de l'ennemi en world space, pour ensuite faire une translation vers le haut avec un fade out
        }
        
        #endregion
        
        #region ShaderProperties
        private void InitializeShaderProperties()
        {
            // IMPORTANT : instancier le material pour ne pas partager l'effet entre tous les ennemis
            material = Instantiate(spriteRenderer.material);
            spriteRenderer.material = material;

            hitTimeID = Shader.PropertyToID("_HitTime");
            flashDurationID = Shader.PropertyToID("_FlashDuration");
            flashColorID = Shader.PropertyToID("_FlashColor");

            material.SetFloat(flashDurationID, flashDuration);
            material.SetColor(flashColorID, flashColor);
        }
        
        [ContextMenu("Test Flash Effect")]
        private void PlayFlash()
        {
            PlayDamageAnimation();
            Debug.Log($"<b>[FeedbackHealthAi]</b> Playing flash effect at time: {Time.time}");
            material.SetFloat(hitTimeID, Time.time);
        }
        #endregion
    }
}