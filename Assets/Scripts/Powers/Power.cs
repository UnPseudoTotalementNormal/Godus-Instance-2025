using System;
using UnityEngine;

namespace Powers
{
    public abstract class Power : MonoBehaviour
    {
        public string powerName => gameObject.name;
        [field:SerializeField] public Sprite powerIcon { get; private set; }
        public PowerCategory powerCategory => GetComponentInParent<PowerCategory>();
        
        public float currentCooldownTime { get; protected set; }
        public bool isOnCooldown => currentCooldownTime > 0f;
        
        public event Action onPowerActivated;
        public event Action onPowerDeactivated;
        
        public event Action onCooldownStarted;
        public event Action onCooldownEnded;

        public virtual void Update()
        {
            if (isOnCooldown)
            {
                currentCooldownTime -= Time.deltaTime;
                if (currentCooldownTime <= 0f)
                {
                    currentCooldownTime = 0f;
                    onCooldownEnded?.Invoke();
                }
            }
        }

        public virtual void Activate()
        {
            onPowerActivated?.Invoke();
        }

        public virtual void Deactivate()
        {
            onPowerDeactivated?.Invoke();
        }
        
        public void StartCooldown(float _cooldownDuration)
        {
            currentCooldownTime = _cooldownDuration;
            onCooldownStarted?.Invoke();
        }
    }
}