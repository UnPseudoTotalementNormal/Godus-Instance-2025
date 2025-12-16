using System;
using AI;
using UnityEngine;

public class Entity : MonoBehaviour, ITeamComponent
{
    [field:SerializeField] public EntityTeam team { get; private set; }

    public event Action onDeath;
    
    private HealthComponent healthComponent;
    
    private bool isDead = false;
    
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
        GameEvents.onNewEntitySpawned?.Invoke(this);
    }

    [ContextMenu("DIE")]
    public void Die()
    {
        isDead = true;
        onDeath?.Invoke();
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (!isDead)
        {
            onDeath?.Invoke();
        }
    }
}

