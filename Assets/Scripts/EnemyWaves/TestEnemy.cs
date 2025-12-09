using System;
using UnityEngine;

public class TestEnemy : Entity
{
    public event Action onDeath;
    
    [ContextMenu("DIE")]
    public void Die()
    {
        onDeath?.Invoke();
        Destroy(gameObject);
    }
}
