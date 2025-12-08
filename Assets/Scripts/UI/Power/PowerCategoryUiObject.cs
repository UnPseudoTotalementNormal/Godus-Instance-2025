using System;
using Powers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Power
{
    public class PowerCategoryUiObject : MonoBehaviour
    {
        private PowersUI powersUI;
        
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private Image imageCooldown;
        
        public PowerCategory powerCategory { get; private set; }
        public CanvasGroup canvasGroup { get; private set; }
        
        private ISelectionFeedback[] selectionFeedbacks;
        
        public event Action<PowerCategoryUiObject> onClicked;

        private float currentCooldownDuration;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(OnClicked);
            selectionFeedbacks = GetComponents<ISelectionFeedback>();
        }

        private void Update()
        {
            imageCooldown.fillAmount = powerCategory && powerCategory.isOnCooldown
                ? powerCategory.currentCooldownTime / currentCooldownDuration
                : 0f;
        }

        public void SetPowerCategory(PowerCategory _powerCategory, CanvasGroup _canvasGroup, PowersUI _powersUi)
        {
            powerCategory = _powerCategory;
            canvasGroup = _canvasGroup;
            
            iconImage.sprite = _powerCategory.categoryIcon;
            nameText.text = _powerCategory.categoryName;

            iconImage.enabled = _powerCategory.categoryIcon != null;
            
            if (selectionFeedbacks == null || selectionFeedbacks.Length == 0)
            {
                selectionFeedbacks = GetComponents<ISelectionFeedback>();
            }
            
            powersUI = _powersUi;
            
            powersUI.onPowerCategoryChanged += OnPowerCategoryChanged;
            
            powerCategory.onCooldownStarted += (_cooldownDuration) =>
            {
                currentCooldownDuration = _cooldownDuration;
            };
        }
        
        public void OnClicked()
        {
            onClicked?.Invoke(this);
        }
        
        private void OnPowerCategoryChanged(PowerCategory _setCategory)
        {
            bool _show = _setCategory == powerCategory;

            foreach (var _selectionFeedback in selectionFeedbacks)
            {
                if (_show)
                {
                    _selectionFeedback.ShowSelectionFeedback();
                    continue;
                }
                _selectionFeedback.HideSelectionFeedback();
            }
        }
    }
}