using System;
using Powers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Power
{
    public class PowerCategoryUiObject : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text nameText;
        
        public PowerCategory powerCategory { get; private set; }
        public CanvasGroup canvasGroup { get; private set; }
        
        public event Action<PowerCategoryUiObject> onClicked;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(OnClicked);
        }

        public void SetPowerCategory(PowerCategory _powerCategory, CanvasGroup _canvasGroup)
        {
            powerCategory = _powerCategory;
            canvasGroup = _canvasGroup;
            
            iconImage.sprite = _powerCategory.categoryIcon;
            nameText.text = _powerCategory.categoryName;

            iconImage.enabled = _powerCategory.categoryIcon != null;
        }
        
        public void OnClicked()
        {
            onClicked?.Invoke(this);
        }
    }
}