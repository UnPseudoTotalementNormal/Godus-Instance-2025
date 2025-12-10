using System;
using UnityEngine;

namespace FireSystem
{
    public class Fire : MonoBehaviour
    {
        [field:SerializeField] public float damagePerSecond { get; private set; }
        [field: SerializeField] public float spreadChancePerSecond { get; private set; } = 0;
        [field:SerializeField] public float minLifetime { get; private set; } = 5f;
        [field:SerializeField] public float maxLifetime { get; private set; } = 6f;

        private float lastDamageTime = -Mathf.Infinity;

        public float lifeTime { get; private set; }
        private float lifeTimer;
        
        public event Action onFireExtinguished; 

        private void Awake()
        {
            lastDamageTime = Time.time;
            lifeTime = UnityEngine.Random.Range(minLifetime, maxLifetime);
        }

        private void Update()
        {
            lifeTimer += Time.deltaTime;
            
            if (Time.time >= lastDamageTime + 1f)
            {
                ApplyFireDamage();
                lastDamageTime = Time.time;
            }
            if (lifeTimer >= lifeTime)
            {
                ExtinguishFire();
            }
        }
        
        public void ExtinguishFire()
        {
            onFireExtinguished?.Invoke();
            Destroy(gameObject);
        }

        private void ApplyFireDamage()
        {
            Collider2D[] _colliders = Physics2D.OverlapCircleAll(transform.position, 0.5f);
            foreach (Collider2D _collider in _colliders)
            {
                HealthComponent _healthComponent = _collider.GetComponentInParent<HealthComponent>();
                if (_healthComponent != null)
                {
                    _healthComponent.TakeDamage(damagePerSecond);
                }
            }
        }
    }
}