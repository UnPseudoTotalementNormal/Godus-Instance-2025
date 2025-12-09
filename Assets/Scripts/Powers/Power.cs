using System;
using TileSystemSpace;
using UnityEngine;

namespace Powers
{
    public abstract class Power : MonoBehaviour
    {
        public string powerName => gameObject.name;
        [field:SerializeField] public Sprite powerIcon { get; private set; }
        
        public int tileRadius = 0;
        public TileSystem.RadiusMode radiusMode = TileSystem.RadiusMode.Diamond;
        
        public PowerCategory powerCategory => GetComponentInParent<PowerCategory>();
        
        [field:SerializeField] public float powerCooldownDuration { get; protected set; }  = 5f;
        [field:SerializeField] public float categoryCooldownDuration { get; protected set; }  = 2f;
        public float currentCooldownTime { get; protected set; }
        public bool isOnCooldown => currentCooldownTime > 0f;
        
        public event Action onPowerActivated;
        public event Action onPowerDeactivated;
        
        public event Action onCooldownStarted;
        public event Action onCooldownEnded;
        
        public bool isPowerActive { get; protected set; } = false;

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
            if (isPowerActive)
            {
                return;
            }
            
            isPowerActive = true;
            onPowerActivated?.Invoke();
        }

        public virtual void Deactivate()
        {
            if (!isPowerActive)
            {
                return;
            }
            
            isPowerActive = false;
            
            if (ShouldStartCooldownOnDeactivate())
            {
                StartPowerCooldown(powerCooldownDuration);
                powerCategory.StartCooldown(categoryCooldownDuration);
            }
            onPowerDeactivated?.Invoke();
        }
        
        public abstract bool ShouldStartCooldownOnDeactivate();
        
        public void StartPowerCooldown(float _cooldownDuration)
        {
            currentCooldownTime = _cooldownDuration;
            onCooldownStarted?.Invoke();
        }
    }
}