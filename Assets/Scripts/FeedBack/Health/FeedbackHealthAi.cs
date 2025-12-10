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
        private Color defaultFlashColor = Color.white;
        [SerializeField] private Color damageFlashColor = Color.white;
        [SerializeField] private Color healFlashColor = Color.green;

        [SerializeField] private SpriteRenderer spriteRenderer;
        private Material material;
        private int hitTimeID;
        private int flashDurationID;
        private int flashColorID;

        
        [Space(10)] [Header("    Text Feedback Settings")]
        [SerializeField] private float damageTextDuration = 2f;
        [SerializeField] private float healTextDuration = 2f;
        [SerializeField] private float textFontSize = 3f;
        [SerializeField] private Color damageColor = Color.red;
        [SerializeField] private Color healColor = Color.green;
        [SerializeField] private float endScaleValue = 0.6f;
        
        [Tooltip("Distance de déplacement vers le haut")]
        [SerializeField] private float textMoveDistance = 0.5f;
        
        [Tooltip("Offset de spawn du texte par rapport au coin supérieur droit du sprite")]
        [SerializeField] private Vector3 textSpawnOffset = new(0f, -0.25f, 0f); 
        
        private string damageTextPrefix = "-";
        private string healTextPrefix = "+";
        
        
        [Space(10)] [Header("    Scale Animation Settings")]
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
            healthComponent.onHealed += HandleHealed;
            healthComponent.onDamaged += HandleDamaged;
        }

        private void OnDisable()
        {
            healthComponent.onHealed -= HandleHealed;
            healthComponent.onDamaged -= HandleDamaged;
        }

        #region EventHandlers
        private void HandleDamaged(float damage)
        {
            PlayDamageFlash();
            PlayDamageScaleAnimation();
            PlayTakeDamageTextAnimation(damage);
        }

        private void HandleHealed(float healAmount)
        {
            PlayHealFlash();
            PlayHealTextAnimation(healAmount);
        }

        private void HandleDeath()
        {
            
        }
        #endregion
        
        #region Animations
        
        private void PlayDamageScaleAnimation()
        {
            transform.DOKill(true);
            Debug.Log("Playing damage animation");
            transform.DOPunchScale(Vector3.one * damagePunchScaleStrength, damagePunchScaleDuration, damagePunchScaleVibrato, damagePunchScaleElasticity);
        }
        
        private void PlayTakeDamageTextAnimation(float damage)
        {
            Bounds bounds = spriteRenderer.bounds;
            Vector3 spawnPosition = new Vector3(bounds.max.x, bounds.max.y, bounds.center.z) + textSpawnOffset;

            TextMeshPro damageText = new GameObject("DamageText Feedback").AddComponent<TextMeshPro>();
            damageText.transform.position = spawnPosition;
            
            damageText.text = $"{damageTextPrefix}{damage:F0}";
            damageText.color = damageColor;
            damageText.fontSize = textFontSize;
            damageText.alignment = TextAlignmentOptions.Center;
            damageText.sortingOrder = 100;

            Vector3 endPosition = spawnPosition + Vector3.up * textMoveDistance;
            
            Sequence sequence = DOTween.Sequence();
            sequence.Append(damageText.transform.DOMoveY(endPosition.y, damageTextDuration).SetEase(Ease.OutQuad));

            // Start Half Way
            sequence.Join(damageText.transform.DOScale(endScaleValue, damageTextDuration));
            sequence.Join(damageText.DOFade(0f, damageTextDuration * 0.5f).SetDelay(damageTextDuration * 0.5f));

            sequence.OnComplete(() => Destroy(damageText));
        }
        
        private void PlayHealTextAnimation(float healAmount)
        {
            Bounds bounds = spriteRenderer.bounds;
            Vector3 spawnPosition = new Vector3(bounds.max.x, bounds.max.y, bounds.center.z) + textSpawnOffset;
            
            TextMeshPro healText = new GameObject("HealthText Feedback").AddComponent<TextMeshPro>();
            healText.transform.position = spawnPosition;
            
            healText.text = $"{healTextPrefix}{healAmount:F0}";
            healText.color = healColor;
            healText.fontSize = textFontSize; 
            healText.alignment = TextAlignmentOptions.Center;
            healText.sortingOrder = 100;
            
            Vector3 endPosition = spawnPosition + Vector3.up * textMoveDistance;
            
            Sequence sequence = DOTween.Sequence();
            sequence.Append(healText.transform.DOMoveY(endPosition.y, damageTextDuration).SetEase(Ease.OutQuad));

            // Start Half Way
            sequence.Join(healText.transform.DOScale(endScaleValue, damageTextDuration));
            sequence.Join(healText.DOFade(0f, healTextDuration * 0.5f).SetDelay(healTextDuration * 0.5f));

            sequence.OnComplete(() => Destroy(healText.gameObject));
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
            material.SetColor(flashColorID, defaultFlashColor);
        }
        
        [ContextMenu("Test Damage Flash Effect")]
        private void PlayDamageFlash()
        {
            PlayDamageScaleAnimation();
            PlayTakeDamageTextAnimation(Random.Range(5, 20));
            
            material.SetColor(flashColorID, damageFlashColor);
            material.SetFloat(hitTimeID, Time.time);
        }
        
        [ContextMenu("Test Heal Flash Effect")]
        private void PlayHealFlash()
        {
            PlayHealTextAnimation(Random.Range(5, 20));
            
            material.SetColor(flashColorID, healFlashColor);
            material.SetFloat(hitTimeID, Time.time);
        }
        #endregion
    }
}