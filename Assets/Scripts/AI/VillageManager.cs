using System;
using AI;
using AYellowpaper.SerializedCollections;
using Unity.Behavior;
using UnityEngine;
using Random = UnityEngine.Random;

public class VillageManager : MonoBehaviour
{
    [SerializeField] BehaviorGraphAgent villageBlackboard;
    [SerializeField] public Vector2Int villageCenter;
    [SerializeField] GameObject buildingPrefab;
    [SerializeField] private int buildingWoodCost = 40;
    
    [SerializeField] private SerializedDictionary<ResourceType, int> upgradeCosts = new();
    
    [SerializeField] private int storageIncreasePerBuilding = 50;

    bool villageUnderAttack;
    
    private VillageData villageData; 

    void Awake()
    {
        GameEvents.onTownHallCreated -= NewVillageCenter;
        GameEvents.onStorageBuildingCreated -= OnNewStorageBuilding;
        
        GameEvents.onTownHallCreated += NewVillageCenter;
        GameEvents.onStorageBuildingCreated += OnNewStorageBuilding;
        
        villageData = new VillageData();
        
        villageData.Init();

        villageData.Add(ResourceType.Meat, 0);
        villageData.Add(ResourceType.Wood, 0);
        villageData.Add(ResourceType.Stone, 0);
        villageData.Add(ResourceType.Iron, 0);
        villageData.Add(ResourceType.Glorp, 0);

        villageBlackboard.SetVariableValue("VillageCenter", villageCenter);
        villageBlackboard.SetVariableValue("VillageManager", this);
    }

    private void Start()
    {
        villageData.Add(ResourceType.Meat, 0);
        villageData.Add(ResourceType.Wood, 0);
        villageData.Add(ResourceType.Stone, 0);
        villageData.Add(ResourceType.Iron, 0);
        villageData.Add(ResourceType.Glorp, 0);
        
        villageData.AddMax(ResourceType.Meat, 0);
        villageData.AddMax(ResourceType.Wood, 0);
        villageData.AddMax(ResourceType.Stone, 0);
        villageData.AddMax(ResourceType.Iron, 0);
        villageData.AddMax(ResourceType.Glorp, 0);
    }

    private void OnNewStorageBuilding()
    {
        //Increase max capacity for each resource by X
        villageData.AddMax(ResourceType.Meat,storageIncreasePerBuilding);
        villageData.AddMax(ResourceType.Wood,storageIncreasePerBuilding);
        villageData.AddMax(ResourceType.Stone,storageIncreasePerBuilding);
        villageData.AddMax(ResourceType.Iron,storageIncreasePerBuilding);
        villageData.AddMax(ResourceType.Glorp,storageIncreasePerBuilding);
    }

    private void OnDestroy()
    {
        GameEvents.onTownHallCreated -= NewVillageCenter;
        GameEvents.onStorageBuildingCreated -= OnNewStorageBuilding;
        villageData.OnDestroy();
    }

    private void NewVillageCenter(GameObject _newTownHall)
    {
        villageCenter = Vector2Int.FloorToInt(_newTownHall.transform.position);
        villageBlackboard.SetVariableValue("VillageCenter", villageCenter);
        Debug.Log("New Village Center is" + villageBlackboard.GetVariable("VillageCenter", out BlackboardVariable _d));
        Debug.Log(_d.ObjectValue);
    }

    [ContextMenu("Roll call")]
    void RollCall()
    {
        villageUnderAttack = !villageUnderAttack;
        villageBlackboard.SetVariableValue("RollCallActive", villageUnderAttack);
    }

    public TaskType GetNewTask(Transform _caller, out GameObject _target)
    {
        if (villageData == null)
        {
            Debug.LogWarning("No village data available");
            villageData = new();
            _target = null;
            return TaskType.Wandering;
        }
        
        if ((100 * (villageData.wood / villageData.maxWood)) >= 90)
        {
            villageData.Add(ResourceType.Wood,-buildingWoodCost);
            _target = null;
            return TaskType.Building;
        }
        
        var _resourcePercentages = new System.Collections.Generic.List<(ResourceType type, float percentage, float threshold)>
        {
            (ResourceType.Meat, 100f * ((float)villageData.meat / villageData.maxMeat), 80f),
            (ResourceType.Wood, 100f * ((float)villageData.wood / villageData.maxWood), 80f),
            (ResourceType.Stone, 100f * ((float)villageData.stone / villageData.maxStone), 50f),
            (ResourceType.Iron, 100f * ((float)villageData.iron / villageData.maxIron), 50f),
            (ResourceType.Glorp, 100f * ((float)villageData.glorp / villageData.maxGlorp), 50f)
        };
        
        _resourcePercentages.Sort((_a, _b) => _a.percentage.CompareTo(_b.percentage));
        
        foreach (var (_type, _percentage, _threshold) in _resourcePercentages)
        {
            if (_percentage <= _threshold)
            {
                _target = DetectResourceInRange(_caller, _type);
                if (_target != null)
                {
                    if (_type == ResourceType.Meat)
                        return TaskType.Hunting;
                    else
                        return TaskType.Gathering;
                }
            }
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

        if (Random.value <= 0.4f && (villageData.glorp >= 10 || villageData.iron >= 10 || villageData.stone >= 10 || villageData.wood >= 10))
        {
            Debug.Log(_caller.name + "wants to go shop !");
            //Make the AI go back to the village centre to upgrade
            return TaskType.Upgrading;
        }
        //Debug.Log("Wandering");
        return TaskType.Wandering;
    }
    
    public void AbortTask(Transform _caller, TaskType _taskType)
    {
        BehaviorGraphAgent _behaviourGraphAgent = _caller.GetComponent<BehaviorGraphAgent>();
        if (_behaviourGraphAgent == null)
        {
            return;
        }

        if (_taskType == TaskType.None)
        {
            return;
        }
        
        _behaviourGraphAgent.SetVariableValue("CurrentTask", TaskType.None);
        if (_behaviourGraphAgent.GetVariable("PathTarget", out BlackboardVariable _pathTargetVariable))
        {
            GameObject _pathTarget = _pathTargetVariable.ObjectValue as GameObject;
            if (_pathTarget != null)
            {
                switch (_taskType)
                {
                    case TaskType.Gathering:
                    case TaskType.Hunting:
                        ResourceComponent _resourceComponent = _pathTarget.GetComponentInParent<ResourceComponent>();
                        if (_resourceComponent != null)
                        {
                            _resourceComponent.collectible = true;
                            _resourceComponent.GetComponent<Collider2D>().enabled = true;
                        }
                        break;
                    case TaskType.Building: //refund building cost 
                        villageData.Add(ResourceType.Wood, buildingWoodCost);
                        break;
                }
            }
        }
    }

    GameObject DetectResourceInRange(Transform _origin, ResourceType _resourceType)
    {
        foreach (Collider2D _resource in Physics2D.OverlapCircleAll(_origin.position, 20, LayerMask.GetMask("Resource")))
        {
            ResourceComponent _resourceComponent = _resource.gameObject.GetComponentInParent<ResourceComponent>();
            if (!_resourceComponent) continue;
            if (_resourceComponent.resourceType != _resourceType || _resourceComponent.collectible == false) continue;
            _resource.GetComponent<Collider2D>().enabled = false;
            _resourceComponent.collectible = false;
            return _resource.gameObject;
        }
        //Debug.Log("No resource found");
        return null;
    }

    ResourceType DetectAllResourceInRange(Transform _origin, out GameObject _givenResource)
    {
        Collider2D[] _collider2Ds = Physics2D.OverlapCircleAll(_origin.position, 20, LayerMask.GetMask("Resource"));
        _givenResource = null;
        if (_collider2Ds.Length == 0)
            return ResourceType.Wood;
        int _randomIndex = Random.Range(0, _collider2Ds.Length-1);
        ResourceComponent _resourceComponent = _collider2Ds[_randomIndex].GetComponentInParent<ResourceComponent>();
        if (_resourceComponent)
        {
            if (_resourceComponent.collectible == false)
            {
                return ResourceType.Wood;
            }
        }
        else
        {
            return ResourceType.Wood;
        }
        _givenResource = _collider2Ds[_randomIndex].gameObject;
        _givenResource.gameObject.GetComponent<Collider2D>().enabled = false;
        return _collider2Ds[_randomIndex].GetComponentInParent<ResourceComponent>().resourceType;
    }

    public void AddResource(ResourceType _resource, int _amount)
    {
        Debug.Log("Added resource " + _resource + " with number of " + _amount);
        villageData.Add(_resource, _amount);
    }

    public int GetResourceAmount(ResourceType _resource)
    {
        return villageData.GetResourceValue(_resource);
    }

    public bool BuildAtLocation(Transform _position)
    {
        GameObject _newBuild = Instantiate(buildingPrefab);
        _newBuild.transform.position = _position.position;
        return true;
    }

    public bool CanUpgradeUnit(Entity _entity)
    {
        if (!_entity.TryGetComponent(out UpgradeStatsComponent _upgradeStats))
        {
            return false;
        }

        ResourceType _nextUpgrade = _upgradeStats.GetNextUpgrade();
        if (_nextUpgrade == ResourceType.None)
        {
            return false;
        }
        
        int _upgradeCost = upgradeCosts[_nextUpgrade];
        
        if (GetResourceAmount(_nextUpgrade) < _upgradeCost)
        {
            return false;
        }

        return true;
    }
    
    public void UpgradeEntity(Entity _entity)
    {
        if (!CanUpgradeUnit(_entity))
        {
            return;
        }

        if (!_entity.TryGetComponent(out UpgradeStatsComponent _upgradeStats))
        {
            return;
        }

        ResourceType _nextUpgrade = _upgradeStats.GetNextUpgrade();
        int _upgradeCost = upgradeCosts[_nextUpgrade];

        AddResource(_nextUpgrade, -_upgradeCost);
        _upgradeStats.ApplyUpgrade();
    }
}

[BlackboardEnum]
public enum TaskType
{
    Gathering,
    Building,
    Hunting,
    Wandering,
    Upgrading,
    None
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
    None,
}
