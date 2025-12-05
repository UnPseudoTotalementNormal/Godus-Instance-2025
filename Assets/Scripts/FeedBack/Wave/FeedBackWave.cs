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
        
        private WavesManager wavesManager;
        
        private void Start()
        {
            waveText.enabled = false;
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
            GameEvents.onWaveStarted += StartVFX;
            GameEvents.onWaveStarted += StartSFX;
            GameEvents.onWaveStarted += StartTimer;
            GameEvents.onWaveStarted += StartShaderEffect;
            GameEvents.onWaveStarted += StartText;
        }
        private void CleanupEvent()
        {
            GameEvents.onWaveStarted -= StartVFX;
            GameEvents.onWaveStarted -= StartSFX;
            GameEvents.onWaveStarted -= StartTimer;
            GameEvents.onWaveStarted -= StartShaderEffect;
            GameEvents.onWaveStarted -= StartText;
        }
        #endregion
        
        public void StartVFX(int _currentWave = 0)
        {
            
        }
        
        public void StartSFX(int _currentWave = 0)
        {
            GameAudioManager.instance.PlayOneShot(feedBackData.startSFX);
        }
        
        public void StartTimer(int _currentWave = 0)
        {
            
        }
        
        public void StartShaderEffect(int _currentWave = 0)
        {
            
        }

        public void StartText(int _currentWave = 0)
        {
            waveText.enabled = true;
            waveText.text = "";

            string _fullText = $"Wave {_currentWave}";

            TextAnimationUtils.ChangeText(waveText, _fullText, feedBackData.textDuration);
        }
    }
}