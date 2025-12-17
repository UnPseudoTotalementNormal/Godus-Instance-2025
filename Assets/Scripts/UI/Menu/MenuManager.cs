using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;

namespace UI.Menu
{
    public class MenuManager : MonoBehaviour
    {

        [Header("Scene Settings")]
        [SerializeField] private string playGameSceneName = "GameScene";
        [SerializeField] private Button playButton;
        
        [Header("Press Any Key Animation")]
        [SerializeField] private string pressAnyKeyText = "Press Any Key";
        [SerializeField] private float textWriteDuration = 1.5f;
        [SerializeField] private float textScaleStrength = 1.1f;
        [SerializeField] private float textScaleDuration = 0.8f;
                
        [Header("Menu Settings")]
        [SerializeField] private float showMenuDuration = 0.5f;
        [SerializeField] private float inactivityTimeout = 90f;
        
        [Space(10)]
        [SerializeField] private GameObject mainMenu;
        [SerializeField] private GameObject settingsMenu;
        [SerializeField] private GameObject creditsMenu;
        
        [Space(10)]
        [SerializeField] private GameObject panel;
        [SerializeField] private GameObject textPressAnyKeyObj;
        
        [Space(10)]
        [SerializeField] private CanvasGroup panelCanvasGroup;
        [SerializeField] private MenuNavigationHandler navigationHandler;
        private CanvasGroup textPressAnyKeyCanvasGroup;

        public bool isMenuShown;
        private bool inputsEnabled;
        private float inactivityTimer;
        private GameObject currentActiveMenu;
        private Tween textScaleTween;
        private TextMeshProUGUI textOfObj;
        
        private void Awake()
        {
            mainMenu.SetActive(false);
            settingsMenu.SetActive(false);
            creditsMenu.SetActive(false);
            
            panel.SetActive(false);
            textPressAnyKeyObj.SetActive(true);
        }
        
        private void Start()
        {
            InputManager.instance.onAnyKeyPressStarted += ShowMainMenu;
            
            textOfObj = textPressAnyKeyObj.GetComponent<TextMeshProUGUI>();
            Assert.IsNotNull(textOfObj, $"<b>[MenuManager]</b> Text");
            
            Assert.IsNotNull(navigationHandler, $"<b>[MenuManager]</b> MenuNavigationHandler reference is not assigned in the inspector.");
            Assert.IsNotNull(panelCanvasGroup, $"<b>[MenuManager]</b> Panel CanvasGroup reference is not assigned in the inspector.");
            Assert.IsNotNull(textPressAnyKeyObj, $"<b>[MenuManager]</b> Text 'Press Any Key' GameObject reference is not assigned in the inspector.");
            Assert.IsNotNull(playButton, $"<b>[MenuManager]</b> Play Button reference is not assigned in the inspector.");
            
            textPressAnyKeyCanvasGroup = textPressAnyKeyObj.GetComponent<CanvasGroup>();
            if (!textPressAnyKeyCanvasGroup)
                textPressAnyKeyCanvasGroup = textPressAnyKeyObj.AddComponent<CanvasGroup>();
            
            playButton.onClick.AddListener(() => SceneManager.LoadScene(playGameSceneName));
            currentActiveMenu = mainMenu;
            
            InitializePressAnyKeyText();
            
            navigationHandler.onNavigationStarted += DisableInputs;
            navigationHandler.onNavigationCompleted += EnableInputs;
            navigationHandler.onMenuChanged += UpdateCurrentActiveMenu;
            
            InputManager.instance.onLeftMouseButtonPressStarted += ResetInactivityTimer;
            InputManager.instance.onRightMouseButtonPressStarted += ResetInactivityTimer;
            InputManager.instance.onMiddleMouseButtonPressStarted += ResetInactivityTimer;
        }

        private void ShowMainMenu()
        {
            if (isMenuShown)
                return;
            
            panel.SetActive(true);
            
            GameObject menuToShow = currentActiveMenu != null ? currentActiveMenu : mainMenu;
            menuToShow.SetActive(true);
            
            CanvasGroup menuCanvasGroup = menuToShow.GetComponent<CanvasGroup>();
            InputManager.instance.onAnyKeyPressStarted -= ShowMainMenu;
            Sequence showSequence = DOTween.Sequence();
            
            if (panelCanvasGroup)
            {
                panelCanvasGroup.alpha = 0;
                showSequence.Append(panelCanvasGroup.DOFade(1, showMenuDuration));
            }
            
            if (menuCanvasGroup)
            {
                menuCanvasGroup.alpha = 0;
                showSequence.Join(menuCanvasGroup.DOFade(1, showMenuDuration));
            }
            
            if (textPressAnyKeyCanvasGroup)
            {
                showSequence.Join(textPressAnyKeyCanvasGroup.DOFade(0, showMenuDuration));
            }
            
            showSequence.OnComplete(() =>
            {
                textPressAnyKeyObj.SetActive(false);
                isMenuShown = true;
                
                if (menuCanvasGroup)
                {
                    menuCanvasGroup.interactable = true;
                    menuCanvasGroup.blocksRaycasts = true;
                }
                
                inactivityTimer = 0f;
                
                textScaleTween?.Kill();
            });
            
            showSequence.Play();
        }
        
        private void Update()
        {
            if (!isMenuShown)
                return;
            
            inactivityTimer += Time.deltaTime;
            
            // If inactivity timeout is reached, hide menu
            if (inactivityTimer >= inactivityTimeout)
            {
                HideMenu();
            }
        }
        
        private void HideMenu()
        {
            isMenuShown = false;
            inactivityTimer = 0f;
            
            GameObject menuToHide = currentActiveMenu;
            if (!menuToHide)
                menuToHide = mainMenu;
            
            CanvasGroup menuCanvasGroup = menuToHide.GetComponent<CanvasGroup>();
            
            Sequence hideSequence = DOTween.Sequence();
            
            if (menuCanvasGroup)
            {
                menuCanvasGroup.interactable = false;
                hideSequence.Append(menuCanvasGroup.DOFade(0, showMenuDuration));
            }
            
            if (panelCanvasGroup)
            {
                hideSequence.Join(panelCanvasGroup.DOFade(0, showMenuDuration));
            }
            
            textPressAnyKeyObj.SetActive(true);
            if (textPressAnyKeyCanvasGroup)
            {
                textPressAnyKeyCanvasGroup.alpha = 0;
                hideSequence.Join(textPressAnyKeyCanvasGroup.DOFade(1, showMenuDuration));
            }
            
            hideSequence.OnComplete(() =>
            {
                panel.SetActive(false);
                menuToHide.SetActive(false);
                
                // Also disable other menus if they are active
                if (mainMenu != menuToHide && mainMenu.activeSelf)
                    mainMenu.SetActive(false);
                if (settingsMenu != menuToHide && settingsMenu.activeSelf)
                    settingsMenu.SetActive(false);
                if (creditsMenu != menuToHide && creditsMenu.activeSelf)
                    creditsMenu.SetActive(false);
                
                InputManager.instance.onAnyKeyPressStarted += ShowMainMenu;
                
                InitializePressAnyKeyText();
            });
            
            hideSequence.Play();
        }
        
        private void ResetInactivityTimer()
        {
            if (isMenuShown)
            {
                inactivityTimer = 0f;
            }
        }
        
        private void UpdateCurrentActiveMenu(GameObject menu)
        {
            currentActiveMenu = menu;
        }
        
        private void DisableInputs()
        {
            inputsEnabled = false;
        }
        
        private void EnableInputs()
        {
            inputsEnabled = true;
        }
        
        private void StartTextScaleAnimation()
        {
            textScaleTween?.Kill();
            
            textPressAnyKeyObj.transform.localScale = Vector3.one;
            
            textScaleTween = textPressAnyKeyObj.transform
                .DOScale(textScaleStrength, textScaleDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }
        
        private void InitializePressAnyKeyText()
        {
            textOfObj.text = "";
            
            TextAnimationUtils.ChangeText(textOfObj, pressAnyKeyText, textWriteDuration, () =>
            {
                StartTextScaleAnimation();
            });
        }
        
        private void OnDestroy()
        {
            textScaleTween?.Kill();
            
            if (InputManager.instance != null)
            {
                InputManager.instance.onAnyKeyPressStarted -= ShowMainMenu;
                InputManager.instance.onLeftMouseButtonPressStarted -= ResetInactivityTimer;
                InputManager.instance.onRightMouseButtonPressStarted -= ResetInactivityTimer;
                InputManager.instance.onMiddleMouseButtonPressStarted -= ResetInactivityTimer;
            }
            
            if (navigationHandler != null)
            {
                navigationHandler.onNavigationStarted -= DisableInputs;
                navigationHandler.onNavigationCompleted -= EnableInputs;
                navigationHandler.onMenuChanged -= UpdateCurrentActiveMenu;
            }
            
            if (playButton != null)
            {
                playButton.onClick.RemoveAllListeners();
            }
        }
    }
}