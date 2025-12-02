using System;
using UnityEngine;

namespace Powers
{
    public class PowerCategory : MonoBehaviour
    {
        public string categoryName => gameObject.name;
        [field:SerializeField] public Sprite categoryIcon { get; private set; }
        
        public float currentCooldownTime { get; private set; }
        public bool isOnCooldown => currentCooldownTime > 0f;
        
        public event Action onCooldownStarted;
        public event Action onCooldownEnded;

        private void Update()
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
        
        public void StartCooldown(float _cooldownDuration)
        {
            currentCooldownTime = _cooldownDuration;
            onCooldownStarted?.Invoke();
        }
    }
}