using System;
using TileSystemSpace;
using UnityEngine;

namespace AI
{
    public class AttackComponent : MonoBehaviour
    {
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private float attackDamage = 10f;
        [SerializeField] private float attackCooldown = 1f;
        [SerializeField] private float attackKnockbackForce = 1f;
        
        private float lastAttackTime = -Mathf.Infinity;
        
        public event Action onAttack;
        
        public virtual bool IsInAttackRange(Vector2 _target)
        {
            float _distanceToTarget = Vector2.Distance(transform.position, _target);
            return _distanceToTarget <= attackRange;
        }
        public bool IsInAttackRange(HealthComponent _target) => IsInAttackRange(_target.transform.position);
        public bool IsInAttackRange(Transform _target) => IsInAttackRange(_target.transform.position);
        
        public bool IsCooldownReady()
        {
            return Time.time >= lastAttackTime + attackCooldown;
        }

        public bool TryAttack(HealthComponent _targetHealth)
        {
            if (!IsInAttackRange(_targetHealth))
            {
                return false;
            }
            
            if (!IsCooldownReady())
            {
                return false;
            }
            
            Attack(_targetHealth);
            return true;
        }
        
        public void ResetAttackCooldown()
        {
            lastAttackTime = -Mathf.Infinity;
        }
        
        protected virtual void Attack(HealthComponent _targetHealth)
        {
            _targetHealth.TakeDamage(attackDamage);
            lastAttackTime = Time.time;
            onAttack?.Invoke();
        }
    }
}