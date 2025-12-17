using System;
using AudioSystem;
using DG.Tweening;
using FMODUnity;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace UI.Menu
{
    public class MenuNavigationHandler : MonoBehaviour
    {
        [SerializeField] private MenuManager menuManager;
        
        [Header("Audio")]
        [SerializeField] private EventReference sfxClick;
        
        [Header("Animation Settings")]
        [SerializeField] private float fadeDuration = 0.3f;
        
        [Header("Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button quitButton;
        [Space(5)]
        [SerializeField] private Button backToMainFromSettings;
        [SerializeField] private Button backToMainFromCredits;
        
        [Header("Menu GameObjects")]
        [SerializeField] private GameObject mainMenu;
        [SerializeField] private GameObject settingsMenu;
        [SerializeField] private GameObject creditsMenu;
        
        private CanvasGroup mainMenuCanvasGroup;
        private CanvasGroup settingsMenuCanvasGroup;
        private CanvasGroup creditsMenuCanvasGroup;
        
        
        private bool isAnimating;
        
        public event Action onNavigationStarted;
        public event Action onNavigationCompleted;
        public event Action<GameObject> onMenuChanged;

        private void Start()
        {
            GetOrAddCanvasGroups();
            SetupButtons();
            InitializeCanvasGroups();
        }

        private void GetOrAddCanvasGroups()
        {
            if (mainMenu != null)
            {
                mainMenuCanvasGroup = mainMenu.GetComponent<CanvasGroup>();
                if (!mainMenuCanvasGroup)
                    mainMenuCanvasGroup = mainMenu.AddComponent<CanvasGroup>();
                
                Assert.IsNotNull(mainMenuCanvasGroup, $"<b>[MenuNavigationHandler]</b> Main Menu CanvasGroup reference is not assigned in the inspector.");
            }
            
            if (settingsMenu != null)
            {
                settingsMenuCanvasGroup = settingsMenu.GetComponent<CanvasGroup>();
                if (!settingsMenuCanvasGroup)
                    settingsMenuCanvasGroup = settingsMenu.AddComponent<CanvasGroup>();
                
                Assert.IsNotNull(settingsMenuCanvasGroup, $"<b>[MenuNavigationHandler]</b> Settings Menu CanvasGroup reference is not assigned in the inspector.");
            }
            
            if (creditsMenu != null)
            {
                creditsMenuCanvasGroup = creditsMenu.GetComponent<CanvasGroup>();
                if (!creditsMenuCanvasGroup)
                    creditsMenuCanvasGroup = creditsMenu.AddComponent<CanvasGroup>();
                
                Assert.IsNotNull(creditsMenuCanvasGroup, $"<b>[MenuNavigationHandler]</b> Credits Menu CanvasGroup reference is not assigned in the inspector.");
            }
        }

        private void SetupButtons()
        {
            // Main Menu buttons
            Assert.IsNotNull(settingsButton, $"<b>[MenuNavigationHandler]</b> Settings Button reference is not assigned in the inspector.");
            settingsButton.onClick.AddListener(() =>
            {
                GameAudioManager.instance.PlayOneShot(sfxClick);
                NavigateToMenu(mainMenuCanvasGroup, settingsMenuCanvasGroup);
            });
            
            Assert.IsNotNull(creditsButton, $"<b>[MenuNavigationHandler]</b> Credits Button reference is not assigned in the inspector.");
            creditsButton.onClick.AddListener(() =>
            {
                GameAudioManager.instance.PlayOneShot(sfxClick);
                NavigateToMenu(mainMenuCanvasGroup, creditsMenuCanvasGroup);
            });
            
            Assert.IsNotNull(quitButton, $"<b>[MenuNavigationHandler]</b> Quit Button reference is not assigned in the inspector.");
            quitButton.onClick.AddListener(() =>
            {
                GameAudioManager.instance.PlayOneShot(sfxClick);
                QuitGame();
            });
            
            // Back buttons
            Assert.IsNotNull(backToMainFromSettings, $"<b>[MenuNavigationHandler]</b> Back To Main From Settings Button reference is not assigned in the inspector.");
            backToMainFromSettings.onClick.AddListener(() =>
            {
                GameAudioManager.instance.PlayOneShot(sfxClick);
                NavigateToMenu(settingsMenuCanvasGroup, mainMenuCanvasGroup);
            });
            
            Assert.IsNotNull(backToMainFromCredits, $"<b>[MenuNavigationHandler]</b> Back To Main From Credits Button reference is not assigned in the inspector.");
            backToMainFromCredits.onClick.AddListener(() =>
            {
                GameAudioManager.instance.PlayOneShot(sfxClick);
                NavigateToMenu(creditsMenuCanvasGroup, mainMenuCanvasGroup);
            });
        }

        private void InitializeCanvasGroups()
        {
            settingsMenu.SetActive(false);
            settingsMenuCanvasGroup.alpha = 0;
            settingsMenuCanvasGroup.interactable = false;
            settingsMenuCanvasGroup.blocksRaycasts = false;
        
            creditsMenu.SetActive(false);
            creditsMenuCanvasGroup.alpha = 0;
            creditsMenuCanvasGroup.interactable = false;
            creditsMenuCanvasGroup.blocksRaycasts = false;
            
            mainMenu.SetActive(true);
            mainMenuCanvasGroup.alpha = 1;
            mainMenuCanvasGroup.interactable = true;
            mainMenuCanvasGroup.blocksRaycasts = true;
            
            onMenuChanged?.Invoke(mainMenu);
        }

        private void NavigateToMenu(CanvasGroup fromMenu, CanvasGroup toMenu)
        {
            if (isAnimating || fromMenu == null || toMenu == null)
                return;
            
            isAnimating = true;
            onNavigationStarted?.Invoke();

            fromMenu.interactable = false;
            toMenu.gameObject.SetActive(true);
            toMenu.alpha = 0;
            
            
            Sequence navigationSequence = DOTween.Sequence();
            
            navigationSequence.Append(fromMenu.DOFade(0, fadeDuration));
            navigationSequence.Join(toMenu.DOFade(1, fadeDuration));
            navigationSequence.OnComplete(() =>
            {
                fromMenu.gameObject.SetActive(false);
                fromMenu.blocksRaycasts = false;
                
                toMenu.interactable = true;
                toMenu.blocksRaycasts = true;
                
                onMenuChanged?.Invoke(toMenu.gameObject);
                
                isAnimating = false;
                onNavigationCompleted?.Invoke();
            });
            
            navigationSequence.Play();
        }

        private void QuitGame()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }

        private void OnDestroy()
        {
            // Clean up button listeners
            if (settingsButton != null)
                settingsButton.onClick.RemoveAllListeners();
            
            if (creditsButton != null)
                creditsButton.onClick.RemoveAllListeners();
            
            if (quitButton != null)
                quitButton.onClick.RemoveAllListeners();
            
            if (backToMainFromSettings != null)
                backToMainFromSettings.onClick.RemoveAllListeners();
            
            if (backToMainFromCredits != null)
                backToMainFromCredits.onClick.RemoveAllListeners();
        }
    }
}

