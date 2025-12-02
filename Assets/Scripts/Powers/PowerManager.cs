using UnityEngine;

namespace Powers
{
    public class PowerManager : MonoBehaviour
    {
        public static PowerManager instance { get; private set; }

        private Power currentPower;

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
        }

        public void EquipPower(Power _newPower)
        {
            if (currentPower == _newPower || !CanUsePower(_newPower))
            {
                return;
            }
            
            if (currentPower != null)
            {
                currentPower.Deactivate();
            }

            currentPower = _newPower;

            if (currentPower != null)
            {
                currentPower.Activate();
            }
        }

        public void UnequipCurrentPower()
        {
            if (currentPower != null)
            {
                currentPower.Deactivate();
                currentPower = null;
            }
        }

        public bool CanUsePower(Power _power)
        {
            return !_power.isOnCooldown && !_power.powerCategory.isOnCooldown;
        }
    }
}
