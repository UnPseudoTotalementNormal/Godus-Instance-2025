using Mono.Cecil;
using Unity.Behavior;
using UnityEngine;

public class VillageManager : MonoBehaviour
{
    [SerializeField] BehaviorGraphAgent villageBlackboard;
    [SerializeField] Vector2Int villageCenter;

    bool villageUnderAttack;
    
    //Resources
    int wood;
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
        if ((100 * (wood / maxWood)) >= 95)
        {
            _target = null;
            return TaskType.Building;
        }
        
        if ((100 * (meat / maxMeat)) <= 80)
        {
            _target = DetectResourceInRange(_caller, ResourceType.Meat);
            if (_target != null)
                return TaskType.Hunting;
        }

        if ((100 * (wood / maxWood)) <= 80)
        {
            _target = DetectResourceInRange(_caller, ResourceType.Wood);
            if (_target != null)
                return TaskType.Gathering;
        }

        if ((100 * (stone / maxStone)) <= 50)
        {
            _target = DetectResourceInRange(_caller, ResourceType.Stone);
            if (_target != null)
                return TaskType.Gathering;
        }

        if ((100 * (iron / maxIron)) <= 50)
        {
            _target = DetectResourceInRange(_caller, ResourceType.Iron);
            if (_target != null)
                return TaskType.Gathering;
        }

        if ((100 * (glorp / maxGlorp)) <= 50)
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
            if (!_resource.gameObject.TryGetComponent(out ResourceComponent _resourceComponent)) continue;
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
