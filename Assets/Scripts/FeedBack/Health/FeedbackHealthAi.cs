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
        [SerializeField] private Color damageFlashColor = Color.red;
        [SerializeField] private Color healFlashColor = Color.green;

        [SerializeField] private SpriteRenderer spriteRenderer;
        private Material material;
        private int hitTimeID;
        private int flashDurationID;
        private int flashColorID;

        
        [Space(10)] [Header("    Text Feedback Settings")]
        [SerializeField] private float damageTextDuration = 2f;
        [SerializeField] private float healTextDuration = 2f;
        [SerializeField] private float textFontSize = 12f;
        [SerializeField] private Color damageColor = Color.red;
        [SerializeField] private Color healColor = Color.green;
        [SerializeField] private float endScaleValue = 0.6f;
        
        [Tooltip("Distance de déplacement vers le haut")]
        [SerializeField] private float textMoveDistance = 2f;
        
        [Tooltip("Offset de spawn du texte par rapport au coin supérieur droit du sprite")]
        [SerializeField] private Vector3 textSpawnOffset = new(0f, 0f, 0f); 
        
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
        
        private void PlayTakeDamageTextAnimation(float _damage)
        {
            Bounds _bounds = spriteRenderer.bounds;
            Vector3 _spawnPosition = new Vector3(_bounds.max.x, _bounds.max.y, _bounds.center.z) + textSpawnOffset;

            TextMeshPro _damageText = new GameObject("DamageText Feedback").AddComponent<TextMeshPro>();
            _damageText.transform.position = _spawnPosition;
            
            _damageText.text = $"{damageTextPrefix}{_damage:F0}";
            _damageText.color = damageColor;
            _damageText.fontSize = textFontSize;
            _damageText.alignment = TextAlignmentOptions.Center;
            _damageText.sortingOrder = 100;

            Vector3 _endPosition = _spawnPosition + Vector3.up * textMoveDistance;
            
            Sequence _sequence = DOTween.Sequence();
            _sequence.Append(_damageText.transform.DOMoveY(_endPosition.y, damageTextDuration).SetEase(Ease.OutQuad));

            // Start Half Way
            _sequence.Join(_damageText.transform.DOScale(endScaleValue, damageTextDuration));
            _sequence.Join(_damageText.DOFade(0f, damageTextDuration * 0.5f).SetDelay(damageTextDuration * 0.5f));

            _sequence.OnComplete(() => Destroy(_damageText));
        }
        
        private void PlayHealTextAnimation(float _healAmount)
        {
            Bounds _bounds = spriteRenderer.bounds;
            Vector3 _spawnPosition = new Vector3(_bounds.max.x, _bounds.max.y, _bounds.center.z) + textSpawnOffset;
            
            TextMeshPro _healText = new GameObject("HealthText Feedback").AddComponent<TextMeshPro>();
            _healText.transform.position = _spawnPosition;
            
            _healText.text = $"{healTextPrefix}{_healAmount:F0}";
            _healText.color = healColor;
            _healText.fontSize = textFontSize; 
            _healText.alignment = TextAlignmentOptions.Center;
            _healText.sortingOrder = 100;
            
            Vector3 _endPosition = _spawnPosition + Vector3.up * textMoveDistance;
            
            Sequence _sequence = DOTween.Sequence();
            _sequence.Append(_healText.transform.DOMoveY(_endPosition.y, damageTextDuration).SetEase(Ease.OutQuad));

            // Start Half Way
            _sequence.Join(_healText.transform.DOScale(endScaleValue, damageTextDuration));
            _sequence.Join(_healText.DOFade(0f, healTextDuration * 0.5f).SetDelay(healTextDuration * 0.5f));

            _sequence.OnComplete(() => Destroy(_healText.gameObject));
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
            material.SetColor(flashColorID, damageFlashColor);
            material.SetFloat(hitTimeID, Time.time);
        }
        
        [ContextMenu("Test Heal Flash Effect")]
        private void PlayHealFlash()
        {
            material.SetColor(flashColorID, healFlashColor);
            material.SetFloat(hitTimeID, Time.time);
        }
        #endregion
    }
}