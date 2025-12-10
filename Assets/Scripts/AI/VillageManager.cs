using Mono.Cecil;
using Unity.Behavior;
using UnityEngine;

public class VillageManager : MonoBehaviour
{
    [SerializeField] BehaviorGraphAgent villageBlackboard;
    [SerializeField] Vector2Int villageCenter;

    bool villageUnderAttack;
    
    //Resources
    [SerializeField] int wood;
    int maxWood = 50;
    int stone;
    int maxStone = 50;
    int iron;
    int maxIron = 50;
    int glorp;
    int maxGlorp = 50;
    int meat;
    int maxMeat = 50;

    void Awake()
    {
        villageBlackboard.SetVariableValue("VillageCenter", villageCenter);
        villageBlackboard.SetVariableValue("VillageManager", this);
    }
    
    [ContextMenu("Roll call")]
    void RollCall()
    {
        villageUnderAttack = !villageUnderAttack;
        villageBlackboard.SetVariableValue("RollCallActive", villageUnderAttack);
    }

    public TaskType GetNewTask(Transform _caller, out GameObject _target)
    {
        if ((100 * ((float)wood / maxWood)) >= 95)
        {
            wood -=wood*((wood / maxWood));
            _target = null;
            return TaskType.Building;
        }
        
        if ((100 * ((float)meat / maxMeat)) <= 80)
        {
            _target = DetectResourceInRange(_caller, ResourceType.Meat);
            if (_target != null)
                return TaskType.Hunting;
        }

        if ((100 * ((float)wood / maxWood)) <= 80)
        {
            _target = DetectResourceInRange(_caller, ResourceType.Wood);
            if (_target != null)
                return TaskType.Gathering;
        }

        if ((100 * ((float)stone / maxStone)) <= 50)
        {
            _target = DetectResourceInRange(_caller, ResourceType.Stone);
            if (_target != null)
                return TaskType.Gathering;
        }

        if ((100 * ((float)iron / maxIron)) <= 50)
        {
            _target = DetectResourceInRange(_caller, ResourceType.Iron);
            if (_target != null)
                return TaskType.Gathering;
        }

        if ((100 * ((float)glorp / maxGlorp)) <= 50)
        {
            _target = DetectResourceInRange(_caller, ResourceType.Glorp);
            if (_target != null)
                return TaskType.Gathering;
        }
        _target = null;
        return TaskType.Wandering;
    }

    GameObject DetectResourceInRange(Transform _origin, ResourceType _resourceType)
    {

        foreach (Collider2D _resource in Physics2D.OverlapCircleAll(_origin.position, 20, LayerMask.GetMask("Resource")))
        {
            if (!_resource.gameObject.TryGetComponent(out ResourceComponent _resourceComponent)) continue;
            if (_resourceComponent.resourceType != _resourceType) continue;
            _resource.GetComponent<Collider2D>().enabled = false;
            return _resource.gameObject;
        }
        return null;
    }

    public void AddResource(ResourceType _resource, int _amount)
    {
        switch (_resource)
        {
            case ResourceType.Wood:
                wood += _amount;
                break;
            case ResourceType.Stone:
                stone += _amount;
                break;
            case ResourceType.Iron:
                iron += _amount;
                break;
            case ResourceType.Glorp:
                glorp += _amount;
                break;
            case ResourceType.Meat:
                meat += _amount;
                break;
        }
    }

    public int GetResourceAmount(ResourceType _resource)
    {
        switch (_resource)
        {
            case ResourceType.Wood:
                return wood;
            case ResourceType.Stone:
                return stone;
            case ResourceType.Iron:
                return iron;
            case ResourceType.Glorp:
                return glorp;
            case ResourceType.Meat:
                return meat;
            default:
                return -1;
        }
    }
}

[BlackboardEnum]
public enum TaskType
{
    Gathering,
    Building,
    Hunting,
    Wandering,
}

[BlackboardEnum]
public enum AiState
{
    Free,
    Attacking,
}

[BlackboardEnum]
public enum Factions
{
    Villager,
    Enemy
}

public enum ResourceType
{
    Wood,
    Stone,
    Iron,
    Glorp,
    Meat,
}
