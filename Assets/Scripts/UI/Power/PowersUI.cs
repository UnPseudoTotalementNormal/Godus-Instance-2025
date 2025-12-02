using System;
using System.Collections.Generic;
using DG.Tweening;
using Extensions;
using Powers;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Power
{
    public class PowersUI : MonoBehaviour
    {
        [SerializeField] private PowerUiObject powerUiObjectPrefab;
        [SerializeField] private PowerCategoryUiObject powerCategoryUiObjectPrefab;
        [SerializeField] private CanvasGroup powersGroupLayoutPrefab;

        [SerializeField] private Transform powersGroupsParent;
        [SerializeField] private HorizontalLayoutGroup powerCategoryLayout;
        
        private readonly List<PowerCategoryUiObject> powerCategoryUiObjects = new();
        
        private PowerCategoryUiObject currentPowerCategory;
        
        public event Action<PowerCategory> onPowerCategoryChanged;
        
        public void ShowPowerCategory(PowerCategoryUiObject _powerCategoryUiObject)
        {
            if (currentPowerCategory == _powerCategoryUiObject)
            {
                return;
            }
            
            if (currentPowerCategory != null)
            {
                currentPowerCategory.canvasGroup.DoHideGroup();
            }
            
            currentPowerCategory = _powerCategoryUiObject;
            currentPowerCategory.canvasGroup.DoShowGroup();
            onPowerCategoryChanged?.Invoke(currentPowerCategory.powerCategory);
        }
        
        private void Start()
        {
            onPowerCategoryChanged += (_) =>
            {
                PowerManager.instance.EquipPower(null);
            };
            SetupPowersUI(PowerManager.instance.availablePowers);
        }

        private void SetupPowersUI(List<Powers.Power> _availablePowers)
        {
            powerCategoryUiObjects.Clear();
            foreach (var _power in _availablePowers)
            {
                PowerCategoryUiObject _powerCategoryUiObject = GetOrCreatePowerCategoryUiObject(_power.powerCategory);
                
                PowerUiObject _powerUiObject = Instantiate(powerUiObjectPrefab, _powerCategoryUiObject.canvasGroup.transform);
                _powerUiObject.SetPower(_power);

                _powerUiObject.onClicked += OnPowerClicked;
            }
            
            ShowPowerCategory(powerCategoryUiObjects[0]);
        }

        private void OnPowerClicked(PowerUiObject _clickedPower)
        {
            PowerManager.instance.EquipPower(_clickedPower.power);
        }

        private PowerCategoryUiObject GetOrCreatePowerCategoryUiObject(PowerCategory _powerCategory)
        {
            foreach (var _existingCategoryUiObject in powerCategoryUiObjects)
            {
                if (_existingCategoryUiObject.powerCategory == _powerCategory)
                {
                    return _existingCategoryUiObject;
                }
            }
            
            CanvasGroup _canvasGroup = Instantiate(powersGroupLayoutPrefab, powersGroupsParent);
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            PowerCategoryUiObject _newCategoryUiObject = Instantiate(powerCategoryUiObjectPrefab, powerCategoryLayout.transform);
            _newCategoryUiObject.SetPowerCategory(_powerCategory, _canvasGroup, this);
            powerCategoryUiObjects.Add(_newCategoryUiObject);
            _newCategoryUiObject.onClicked += ShowPowerCategory;
            
            return _newCategoryUiObject;
        }
    }
}