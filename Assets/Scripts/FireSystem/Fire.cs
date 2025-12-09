using System;
using UnityEngine;

namespace FireSystem
{
    public class Fire : MonoBehaviour
    {
        [field:SerializeField] public float damagePerSecond { get; private set; }

        private float lastDamageTime = -Mathf.Infinity;

        private void Update()
        {
            if (Time.time >= lastDamageTime + 1f)
            {
                ApplyFireDamage();
                lastDamageTime = Time.time;
            }
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