using System;
using Powers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Power
{
    public class PowerUiObject : MonoBehaviour
    {
        public Powers.Power power { get; private set; }
        
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private Image iconImage;
        [SerializeField] private Image imageCooldown;
        
        private ISelectionFeedback[] selectionFeedbacks;
        
        public event Action<PowerUiObject> onClicked;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(OnClicked);
            selectionFeedbacks = GetComponents<ISelectionFeedback>();
            PowerManager.instance.onPowerSelected += OnPowerSelected;
            PowerManager.instance.onPowerUnSelected += OnPowerUnSelected;
        }

        private void Update()
        {
            imageCooldown.fillAmount = power && power.isOnCooldown
                ? power.currentCooldownTime / power.powerCooldownDuration
                : 0f;
        }

        public void SetPower(Powers.Power _power)
        {
            power = _power;
            nameText.text = _power.powerName;
            iconImage.sprite = _power.powerIcon;
            
            iconImage.enabled = _power.powerIcon != null;
        }
        
        public void OnClicked()
        {
            onClicked?.Invoke(this);
        }
        
        private void OnPowerSelected(Powers.Power _selectedPower)
        {
            if (_selectedPower != power)
            {
                return;
            }
            
            foreach (var _feedback in selectionFeedbacks)
            {
                _feedback.ShowSelectionFeedback();
            }
        }
        
        private void OnPowerUnSelected(Powers.Power _unselectedPower)
        {
            if (_unselectedPower != power)
            {
                return;
            }
            
            foreach (var _feedback in selectionFeedbacks)
            {
                _feedback.HideSelectionFeedback();
            }
        }
    }
}