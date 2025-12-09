using AudioSystem;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Utils;
using Utils.TimerSystem;

namespace Feedback.Wave
{
    
    public class FeedBackWave : MonoBehaviour, IFX
    {
        [Header("Wave Feedback Settings")]
        [SerializeField] private FeedbackData feedBackData;
        [SerializeField] private TMP_Text waveText;
        
        [Header("Slider Settings")]
        [SerializeField] private GameObject sliderObj;
        private Slider sliderComponent;
        [SerializeField] private float sliderAnimTranslateDuration = 1f;
        [SerializeField] private float sliderAnimCanvasDuration = 1f;
        
        private WaveInfo waveInfo = new();
        
        private int remainingEnemy = 0;
        private bool isSliderAreaActive = false;
        private Vector2 sliderInitialPosition;
        
        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;
        private float offsetY;
        
        private TimerSystem currentTimer;

        private void Start()
        {
            Assert.IsNotNull(sliderObj, $"<b>[FeedBackWave]</b> Slider GameObject is not assigned in the inspector.");
            sliderComponent = sliderObj.GetComponent<Slider>();
            Assert.IsNotNull(sliderComponent, $"<b>[FeedBackWave]</b> Slider component is missing on {sliderObj.name} GameObject.");

            waveText.text = "";
            waveText.enabled = false;
            sliderObj.SetActive(false);
            
            rectTransform = sliderObj.GetComponent<RectTransform>();
            canvasGroup = sliderObj.GetComponent<CanvasGroup>();
            Assert.IsNotNull(rectTransform, $"<b>[FeedBackWave]</b> RectTransform component is missing on {sliderObj.name} GameObject.");
            Assert.IsNotNull(canvasGroup, $"<b>[FeedBackWave]</b> CanvasGroup component is missing on {sliderObj.name} GameObject.");
            
            offsetY = rectTransform.rect.height;
            sliderInitialPosition = rectTransform.anchoredPosition;
        }

        
        private void OnEnable()
        {
            InitializeEvent();
        }

        private void OnDisable()
        {
            CleanupEvent();
        }

        private void OnDestroy()
        {
            CleanupEvent();
        }
        
        #region InitializeEvent
        private void InitializeEvent()
        {
            GameEvents.onWaveStarted += StartWave;
            GameEvents.onWaveEnded += EndWave;
            GameEvents.onWaveInfo += GetWaveInfo;
            GameEvents.onEnemyDeath += OnEnemyDeath;
            GameEvents.onStartTimerBetweenWave += StartTimer;
        }
        
        private void CleanupEvent()
        {
            GameEvents.onWaveStarted -= StartWave;
            GameEvents.onWaveEnded -= EndWave;
            GameEvents.onWaveInfo -= GetWaveInfo;
            GameEvents.onEnemyDeath -= OnEnemyDeath;
            GameEvents.onStartTimerBetweenWave -= StartTimer;
        }
        
        private void StartWave(int _currentWave = 0)
        { 
            waveInfo.currentWave = _currentWave;
            
            //StartFX();
            EnabledSlideBarRemainingEnemy(true);
            StartText($"Wave {waveInfo.currentWave}");
        }
        
        private void EndWave()
        {
            // Mettre le timer.
            EnabledSlideBarRemainingEnemy(false);
        }
        
        #endregion
                 
        #region Interface IFX
        public void StartFX()
        {
            StartVFX();
            StartSFX();
            StartShaderEffect();
        }

        public void StartVFX()
        {
            
        }
        
        public void StartSFX()
        {
            GameAudioManager.instance.PlayOneShot(feedBackData.startSFX);
        }
        
        
        public void StartShaderEffect()
        {
            
        }
        #endregion

        
        #region Calling by Events
        private void GetWaveInfo(WaveInfo _newInfo)
        {
            waveInfo = _newInfo;
            remainingEnemy = waveInfo.maxEnemiesInWave;
            SetMaxSliderRemainingEnemy(waveInfo.maxEnemiesInWave);
        }

        private void StartText(string _newInfo)
        {
            waveText.enabled = true;
            TextAnimationUtils.ChangeText(waveText, _newInfo, feedBackData.textDuration);
        }

        private void StartTimer(TimerSystem _timer)
        {
            Debug.Log($"{_timer} & {waveText}");
            waveText.enabled = true;
            currentTimer = TextAnimationUtils.StartTimerText(waveText, _timer, "Timer", "0.00");
        }
        
        private void OnEnemyDeath()
        {
            if (remainingEnemy <= 0)
                return;
            
            remainingEnemy--;
            UpdateSlideBarRemainingEnemy();
        }
        #endregion
        
        
        #region SlideBarRemainingEnemy
        private void EnabledSlideBarRemainingEnemy(bool _isEnable = false)
        {
            if (!_isEnable)
            {
                DisableSlideBarRemainingEnemy();
                return;
            }
            EnableSlideBarRemainingEnemy();
        }
        
        [ContextMenu("Enable Slide Bar Remaining Enemy")]
        private void EnableSlideBarRemainingEnemy()
        {
            if (!rectTransform || !canvasGroup)
                return;
         
            isSliderAreaActive = true;
            sliderObj.SetActive(true);
            
            canvasGroup.DOFade(1f, sliderAnimCanvasDuration)
                .SetEase(Ease.InBack);

            rectTransform.localScale = new Vector3(0f, 1f, 1f);
            rectTransform.DOScaleX(1f, sliderAnimTranslateDuration)
                .SetEase(Ease.InOutBack);
            
            rectTransform.anchoredPosition = new Vector2(sliderInitialPosition.x, sliderInitialPosition.y + offsetY);
            rectTransform.DOAnchorPos(sliderInitialPosition, sliderAnimTranslateDuration)
                .SetEase(Ease.InOutBack);
        }
        
        [ContextMenu("Disable Slide Bar Remaining Enemy")]
        private void DisableSlideBarRemainingEnemy()
        {
            if (!rectTransform || !canvasGroup)
                return;
            
            canvasGroup.DOFade(0f, sliderAnimCanvasDuration)
                .SetEase(Ease.InBack);

            rectTransform.localScale = new Vector3(1f, 1f, 1f);
            rectTransform.DOScaleX(0f, sliderAnimTranslateDuration)
                .SetEase(Ease.InBack);
            
            rectTransform.DOAnchorPosY(sliderInitialPosition.y + offsetY, sliderAnimTranslateDuration)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    canvasGroup.alpha = 1f;
                    rectTransform.anchoredPosition = sliderInitialPosition;
                    
                    sliderObj.SetActive(false);
                    isSliderAreaActive = false;
                });
        }

        private void SetMaxSliderRemainingEnemy(float _value)
        {
            sliderComponent.maxValue = _value;
            sliderComponent.value = _value;
        }
        
        private void UpdateSlideBarRemainingEnemy()
        {
            sliderComponent.value = remainingEnemy;
            // DOTween could be used here for smooth transition if needed on this slider update.
            
            
        }
        
        #endregion
    }
}