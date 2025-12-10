using System;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    
    private float currentHealth;
    
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;

    public event Action<float> onDamaged;
    public event Action<float> onHealed;
    public event Action onDeath;
    
    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float _damage)
    {
        currentHealth -= _damage;
        if (currentHealth <= 0)
        {
            Die();
        }
        onDamaged?.Invoke(_damage);
    }
    
    public void Heal(float _healAmount)
    {
        currentHealth += _healAmount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        onHealed?.Invoke(_healAmount);
    }

    private void Die()
    {
        onDeath?.Invoke();
        Destroy(gameObject);
    }
}
