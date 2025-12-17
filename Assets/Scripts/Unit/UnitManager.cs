using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager instance { get; private set; }
    
    public HashSet<Entity> alienUnits = new();
    public HashSet<Entity> humanUnits = new();
    public HashSet<Entity> neutralUnits = new();
    public HashSet<Entity> entities = new();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        GameEvents.onNewEntitySpawned += RegisterUnit;
    }

    private void OnDestroy()
    {
        GameEvents.onNewEntitySpawned -= RegisterUnit;
    }

    public void RegisterUnit(Entity _entity)
    {
        entities.Add(_entity);
        if (_entity.team == AI.EntityTeam.Alien)
        {
            alienUnits.Add(_entity);
            
        }
        else if (_entity.team == AI.EntityTeam.Human)
        {
            humanUnits.Add(_entity);
        }
        else
        {
            neutralUnits.Add(_entity);
        }
        _entity.onDeath += () => OnEntityDeath(_entity);
    }

    private void OnEntityDeath(Entity _entity)
    {
        entities.Remove(_entity);
        alienUnits.Remove(_entity);
        humanUnits.Remove(_entity);
        if (_entity.team == AI.EntityTeam.Alien)
        {
            GameEvents.onAlienDeath?.Invoke(_entity);
        }
        else if (_entity.team == AI.EntityTeam.Human)
        {
            GameEvents.onEnemyDeath?.Invoke(_entity);
        }
        GameEvents.onEntityDeath?.Invoke(_entity);
    }
}
