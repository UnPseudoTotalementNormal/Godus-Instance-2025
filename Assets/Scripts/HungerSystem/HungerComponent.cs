using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace HungerSystem
{
    public class HungerComponent : MonoBehaviour
    {
        [SerializeField] private HealthComponent healthComponent;

        [SerializeField] private float maxHunger = 100;
        private float hunger = 100;
        [SerializeField] private float starvePerSecond = 1;

        [SerializeField] private float damageTickTime = 1;
        private float damageTimer = 0;
        [SerializeField] private float damagePerTick = 5;
        
        public float GetHunger() => hunger;
        public float GetMaxHunger() => maxHunger;
        

        private void Reset()
        {
            healthComponent = GetComponent<HealthComponent>();
        }

        private void Start()
        {
            hunger = maxHunger;
            damageTimer = damageTickTime;
        }

        public void EatFood(float _regainHunger)
        {
            hunger = Mathf.Clamp(hunger + _regainHunger, 0, maxHunger);
        }
    
        void Update()
        {
            if (hunger > 0)
            {
                hunger = Mathf.Clamp(hunger - starvePerSecond * Time.deltaTime, 0, maxHunger);
            }
            else
            {
                damageTimer -= Time.deltaTime;
                if (damageTimer > 0)
                {
                    return;
                }
                damageTimer = damageTickTime;
                healthComponent.TakeDamage(damagePerTick);
            }
        }
    }
}
