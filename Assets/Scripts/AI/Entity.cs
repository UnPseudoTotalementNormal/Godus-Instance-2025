using System;
using AI;
using UnityEngine;

public class Entity : MonoBehaviour, ITeamComponent
{
    [field:SerializeField] public EntityTeam team { get; private set; }

    public event Action onDeath;
    
    private HealthComponent healthComponent;
    
    public void SetTeam(EntityTeam _team)
    {
        team = _team;
    }

    private void Start()
    {
        healthComponent = GetComponent<HealthComponent>();
        if (healthComponent)
        {
            healthComponent.onDeath += Die;
        }
    }

    [ContextMenu("DIE")]
    public void Die()
    {
        onDeath?.Invoke();
        Destroy(gameObject);
    }
}

