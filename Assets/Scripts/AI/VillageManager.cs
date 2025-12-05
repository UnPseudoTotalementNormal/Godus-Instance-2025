using Unity.Behavior;
using Unity.Behavior.GraphFramework;
using UnityEngine;

public class VillageManager : MonoBehaviour
{
    TaskManager taskManager;
    ResourceManager resourceManager;

    [SerializeField] BehaviorGraphAgent villageBlackboard;
    [SerializeField] Vector2Int villageCenter;

    bool villageUnderAttack;

    void Start()
    {
        Debug.Log(villageBlackboard.GetVariable("RollCallActive", out var active));
        villageBlackboard.SetVariableValue("VillageCenter", villageCenter);
    }
    
    [ContextMenu("Roll call")]
    void RollCall()
    {
        villageUnderAttack = !villageUnderAttack;
        villageBlackboard.SetVariableValue("RollCallActive", villageUnderAttack);
    }
}
