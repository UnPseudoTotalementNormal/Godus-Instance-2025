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
            villageData.Add(ResourceType.Wood,villageData.wood / villageData.maxWood);
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
        
        //Random chance to select a random task
        if (Random.value <= 0.2f)
        {
            Debug.Log("Selecting a random task for " + _caller.name);
            ResourceType _randTaskType = DetectAllResourceInRange(_caller, out GameObject _newRes);
            if (_newRes == null)
                return TaskType.Wandering;
            switch (_randTaskType)
            {
                case ResourceType.Wood: case ResourceType.Stone: case ResourceType.Iron: case ResourceType.Glorp:
                    _target = _newRes;
                    return TaskType.Gathering;
                case ResourceType.Meat:
                    _target = _newRes;
                    return TaskType.Hunting;
            }
        }
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

    ResourceType DetectAllResourceInRange(Transform _origin, out GameObject _givenResource)
    {
        Collider2D[] _collider2Ds = Physics2D.OverlapCircleAll(_origin.position, 20, LayerMask.GetMask("Resource"));
        _givenResource = null;
        if (_collider2Ds.Length == 0)
            return ResourceType.Wood;
        int _randomIndex = Random.Range(0, _collider2Ds.Length);
        _givenResource = _collider2Ds[_randomIndex].gameObject;
        _givenResource.gameObject.GetComponent<Collider2D>().enabled = false;
        return _collider2Ds[_randomIndex].GetComponent<ResourceComponent>().resourceType;
    }

    public void AddResource(ResourceType _resource, int _amount)
    {
        villageData.Add(_resource, _amount);
    }

    public int GetResourceAmount(ResourceType _resource)
    {
        switch (_resource)
        {
            case ResourceType.Wood:
                return villageData.wood;
            case ResourceType.Stone:
                return villageData.stone;
            case ResourceType.Iron:
                return villageData.iron;
            case ResourceType.Glorp:
                return villageData.glorp;
            case ResourceType.Meat:
                return villageData.meat;
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
