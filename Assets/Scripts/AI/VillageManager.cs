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
        foreach (Collider _resource in Physics.OverlapSphere(_origin.position, 20, LayerMask.GetMask("Resource")))
        {
            if (!_resource.gameObject.TryGetComponent(out ResourceComponent _resourceComponent))
                continue;
            
            _resource.GetComponent<Collider>().enabled = false;
            return _resource.gameObject;
        }
        return null;
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

public enum ResourceType
{
    Wood,
    Stone,
    Iron,
    Glorp,
    Meat,
}
