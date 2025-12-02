using System;
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
        
        public event Action<PowerUiObject> onClicked;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(OnClicked);
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
    }
}