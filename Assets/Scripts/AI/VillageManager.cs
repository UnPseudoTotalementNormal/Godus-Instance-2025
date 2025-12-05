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

    void Start()
    {
        Debug.Log(villageBlackboard.GetVariable("RollCallActive", out var active));
        villageBlackboard.SetVariableValue("VillageCenter", villageCenter);
        villageBlackboard.SetVariableValue("VillageManager", this);
    }
    
    [ContextMenu("Roll call")]
    void RollCall()
    {
        villageUnderAttack = !villageUnderAttack;
        villageBlackboard.SetVariableValue("RollCallActive", villageUnderAttack);
    }

    public TaskType GetNewTask(out GameObject _target)
    {
        _target = null; // Remove when all functions have been created
        if ((100 * (wood / maxWood)) >= 95)
        {
            return TaskType.Building;
        }
        
        if ((100 * (meat / maxMeat)) <= 80)
        {
            return TaskType.Hunting;
        }

        if ((100 * (wood / maxWood)) <= 80)
        {
            return TaskType.Gathering;
        }

        if ((100 * (stone / maxStone)) <= 50)
        {
            return TaskType.Gathering;
        }

        if ((100 * (iron / maxIron)) <= 50)
        {
            return TaskType.Gathering;
        }

        if ((100 * (glorp / maxGlorp)) <= 50)
        {
            return TaskType.Gathering;
        }

        return TaskType.Wandering;
    }
}

public enum TaskType
{
    Gathering,
    Building,
    Hunting,
    Wandering,
}
