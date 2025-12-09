using System;
using System.Collections.Generic;
using UnityEngine;

namespace Powers
{
    public class PowerManager : MonoBehaviour
    {
        public static PowerManager instance { get; private set; }

        public readonly List<Power> availablePowers = new();
        
        private Power currentPower;
        
        public event Action<Power> onPowerUnSelected;
        public event Action<Power> onPowerSelected;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
            }
            
            RetrieveAndSetPowers();
        }

        private void RetrieveAndSetPowers()
        {
            var _powers = GetComponentsInChildren<Power>();
            availablePowers.Clear();
            foreach (var _power in _powers)
            {
                availablePowers.Add(_power);
            }
        }

        public void EquipPower(Power _newPower)
        {
            if (currentPower == _newPower)
            {
                UnequipCurrentPower();
                return;
            }
            
            if (currentPower != null)
            {
                UnequipCurrentPower();
            }

            if (!CanUsePower(_newPower))
            {
                return;
            }

            currentPower = _newPower;

            if (currentPower != null)
            {
                currentPower.onPowerDeactivated += UnequipCurrentPower;
                currentPower.Activate();
                onPowerSelected?.Invoke(currentPower);
            }
        }

        public void UnequipCurrentPower()
        {
            var _power = currentPower;
            if (_power != null)
            {
                _power.Deactivate();
                currentPower.onPowerDeactivated -= UnequipCurrentPower;
                currentPower = null;
                onPowerUnSelected?.Invoke(_power);
            }
        }

        public bool CanUsePower(Power _power)
        {
            if (_power == null)
            {
                return false;
            }
            return !_power.isOnCooldown && !_power.powerCategory.isOnCooldown;
        }
    }
}
