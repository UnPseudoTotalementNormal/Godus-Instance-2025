using Unity.Behavior;
using Unity.Behavior.GraphFramework;
using UnityEngine;

public class VillageManager : MonoBehaviour
{
    TaskManager taskManager;
    ResourceManager resourceManager;

    [SerializeField] BlackboardReference villageBlackboard;
    [SerializeField] Vector2Int villageCenter;

    bool villageUnderAttack;

    void Start()
    {
        villageBlackboard.SetVariableValue("VillageCenter", villageCenter);
    }
    
    [ContextMenu("Roll call")]
    void RollCall()
    {
        Debug.Log(villageBlackboard.GetVariable("RollCallActive", out var _variable));
        villageBlackboard.SetVariableValue("RollCallActive", true);
    }
    
    
}
