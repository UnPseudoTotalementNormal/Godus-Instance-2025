using AudioSystem;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Utils;

namespace Feedback.Wave
{
    
    public class FeedBackWave : MonoBehaviour, IFX
    {
        [Header("Wave Feedback Settings")]
        [SerializeField] private FeedbackData feedBackData;
        [SerializeField] private TMP_Text waveText;
        
        [Header("Slider Settings")]
        [SerializeField] private GameObject sliderArea;
        
        //private WaveInfo waveInfo ;
        private int currentWave = 0;
        
        private int totalEnemy = 0;
        private int remainingEnemy = 0;
        private bool isSliderAreaActive = false;
        private Vector2 sliderInitialPosition;
        
        
        private void Start()
        {
            waveText.enabled = false;
            sliderArea.SetActive(false);
            
            
            RectTransform rectTransform = sliderArea.GetComponent<RectTransform>();
            
            if (rectTransform == null)
                return;
            
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
            GameEvents.onWaveStarted += StartFX;
            
            GameEvents.onEnabledSlideBarRemainingEnemy += EnabledSlideBarRemainingEnemy;
            GameEvents.onUpdateSlideBarRemainingEnemy += UpdateSlideBarRemainingEnemy;
            GameEvents.onEnableSlideBarRemainingEnemy += EnableSlideBarRemainingEnemy;
            GameEvents.onDisableSlideBarRemainingEnemy += DisableSlideBarRemainingEnemy;
        }
        
        private void CleanupEvent()
        {
            GameEvents.onWaveStarted -= StartFX;
            
            GameEvents.onEnabledSlideBarRemainingEnemy -= EnabledSlideBarRemainingEnemy;
            GameEvents.onUpdateSlideBarRemainingEnemy -= UpdateSlideBarRemainingEnemy;
            GameEvents.onEnableSlideBarRemainingEnemy -= EnableSlideBarRemainingEnemy;
            GameEvents.onDisableSlideBarRemainingEnemy -= DisableSlideBarRemainingEnemy;
        }
        
        public void StartFX(int _currentWave = 0)
        {
            currentWave = _currentWave;
            
            StartVFX();
            StartSFX();
            StartTimer();
            StartShaderEffect();
            StartText();
        }
        #endregion
                 
        #region Interface IFX
        public void StartVFX()
        {
            
        }
        
        public void StartSFX()
        {
            GameAudioManager.instance.PlayOneShot(feedBackData.startSFX);
        }
        
        public void StartTimer()
        {
            
        }
        
        public void StartShaderEffect()
        {
            
        }
        #endregion

        private void StartText(int _currentWave = 0)
        {
            waveText.enabled = true;
            waveText.text = "";

            string _fullText = $"Wave {_currentWave}";

            TextAnimationUtils.ChangeText(waveText, _fullText, feedBackData.textDuration);
        }

        
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
            isSliderAreaActive = true;
            sliderArea.SetActive(true);
            
            RectTransform _rectTransform = sliderArea.GetComponent<RectTransform>();
            CanvasGroup _canvasGroup = sliderArea.GetComponent<CanvasGroup>();
            
            if (!_rectTransform || !_canvasGroup)
                return;
            
            _canvasGroup.DOFade(1f, feedBackData.textDuration)
                .SetEase(Ease.InBack);

            float _offsetY = _rectTransform.rect.height;

            _rectTransform.anchoredPosition = new Vector2(sliderInitialPosition.x, sliderInitialPosition.y + _offsetY);
            _rectTransform.DOAnchorPos(sliderInitialPosition, feedBackData.textDuration)
                .SetEase(Ease.InOutBack);
        }
        
        [ContextMenu("Disable Slide Bar Remaining Enemy")]
        private void DisableSlideBarRemainingEnemy()
        {
            RectTransform _rectTransform = sliderArea.GetComponent<RectTransform>();
            CanvasGroup _canvasGroup = sliderArea.GetComponent<CanvasGroup>();
            
            if (!_rectTransform || !_canvasGroup)
                return;
            
            _canvasGroup.DOFade(0f, feedBackData.textDuration)
                .SetEase(Ease.InBack);
            
            float _offsetY = _rectTransform.rect.height;

            _rectTransform.DOAnchorPosY(sliderInitialPosition.y + _offsetY, feedBackData.textDuration)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    _canvasGroup.alpha = 1f;
                    _rectTransform.anchoredPosition = sliderInitialPosition;
                    
                    sliderArea.SetActive(false);
                    isSliderAreaActive = false;
                });
        }

        private void SetMaxSliderRemainingEnemy(float _value)
        {
            
        }
        
        private void UpdateSlideBarRemainingEnemy()
        {
            // Mettre a jour la variable `remainingEnemy`
            // remainingEnemy = waveInfo.currentEnemyCount;
        }
        #endregion
    }
}