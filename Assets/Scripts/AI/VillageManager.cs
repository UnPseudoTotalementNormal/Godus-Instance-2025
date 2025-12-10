using AI;
using Unity.Behavior;
using UnityEngine;

public class VillageManager : MonoBehaviour
{
    [SerializeField] BehaviorGraphAgent villageBlackboard;
    [SerializeField] Vector2Int villageCenter;

    bool villageUnderAttack;
    
    private VillageData villageData;

    void Awake()
    {
        villageData = new VillageData();
        
        villageBlackboard.SetVariableValue("VillageCenter", villageCenter);
        villageBlackboard.SetVariableValue("VillageManager", this);
    }
    
    private void Start()
    {
        villageData.Add(ResourceType.Meat,0);
        villageData.Add(ResourceType.Wood,0);
        villageData.Add(ResourceType.Stone,0);
        villageData.Add(ResourceType.Iron,0);
        villageData.Add(ResourceType.Glorp,0);
        
        villageData.AddMax(ResourceType.Meat,0);
        villageData.AddMax(ResourceType.Wood,0);
        villageData.AddMax(ResourceType.Stone,0);
        villageData.AddMax(ResourceType.Iron,0);
        villageData.AddMax(ResourceType.Glorp,0);
    }

    [ContextMenu("Roll call")]
    void RollCall()
    {
        villageUnderAttack = !villageUnderAttack;
        villageBlackboard.SetVariableValue("RollCallActive", villageUnderAttack);
    }

    public TaskType GetNewTask(Transform _caller, out GameObject _target)
    {
        if ((100 * (villageData.wood / villageData.maxWood)) >= 95)
        {
            wood -=wood*((wood / maxWood));
            _target = null;
            return TaskType.Building;
        }
        
        if ((100 * (villageData.meat / villageData.maxMeat)) <= 80)
        {
            _target = DetectResourceInRange(_caller, ResourceType.Meat);
            if (_target != null)
                return TaskType.Hunting;
        }

        if ((100 * (villageData.wood / villageData.maxWood)) <= 80)
        {
            _target = DetectResourceInRange(_caller, ResourceType.Wood);
            if (_target != null)
                return TaskType.Gathering;
        }

        if ((100 * (villageData.stone / villageData.maxStone)) <= 50)
        {
            _target = DetectResourceInRange(_caller, ResourceType.Stone);
            if (_target != null)
                return TaskType.Gathering;
        }

        if ((100 * (villageData.iron / villageData.maxIron)) <= 50)
        {
            _target = DetectResourceInRange(_caller, ResourceType.Iron);
            if (_target != null)
                return TaskType.Gathering;
        }

        if ((100 * (villageData.glorp / villageData.maxGlorp)) <= 50)
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
