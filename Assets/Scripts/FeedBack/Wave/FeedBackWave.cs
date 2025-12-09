using AudioSystem;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
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
        [SerializeField] private float sliderAnimTranslateDuration = 1f;
        [SerializeField] private float sliderAnimCanvasDuration = 1f;
        
        //private WaveInfo waveInfo ;
        private int currentWave = 0;
        
        private int totalEnemy = 0;
        private int remainingEnemy = 0;
        private bool isSliderAreaActive = false;
        private Vector2 sliderInitialPosition;
        
        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;
        private float offsetY;
        
        private void Start()
        {
            waveText.enabled = false;
            sliderArea.SetActive(false);
            
            rectTransform = sliderArea.GetComponent<RectTransform>();
            canvasGroup = sliderArea.GetComponent<CanvasGroup>();
            Assert.IsNotNull(rectTransform, $"<b>[FeedBackWave]</b> RectTransform component is missing on {sliderArea.name} GameObject.");
            Assert.IsNotNull(canvasGroup, $"<b>[FeedBackWave]</b> CanvasGroup component is missing on {sliderArea.name} GameObject.");
            
            offsetY = rectTransform.rect.height;
            sliderInitialPosition = rectTransform.anchoredPosition;
        }

        /*
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
        }*/
        
        public void StartFX(int _currentWave = 0)
        {
            currentWave = _currentWave;
            
            StartVFX();
            StartSFX();
            StartTimer();
            StartShaderEffect();
            StartText();
        }
        //#endregion
                 
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
            if (!rectTransform || !canvasGroup)
                return;
         
            isSliderAreaActive = true;
            sliderArea.SetActive(true);
            
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