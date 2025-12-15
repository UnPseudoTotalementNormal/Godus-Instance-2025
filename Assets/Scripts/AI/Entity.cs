using System;
using AI;
using UnityEngine;

public class Entity : MonoBehaviour, ITeamComponent
{
    [field:SerializeField] public EntityTeam team { get; private set; }

    public event Action onDeath;
    
    [ContextMenu("DIE")]
    public void Die()
    {
        onDeath?.Invoke();
        Destroy(gameObject);
    }
}

