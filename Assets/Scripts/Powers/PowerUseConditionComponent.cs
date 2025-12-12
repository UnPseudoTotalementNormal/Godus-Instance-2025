using UnityEngine;

namespace Powers
{
    public abstract class PowerUseConditionComponent : MonoBehaviour
    {
        public abstract bool CanUsePower();
    }
}