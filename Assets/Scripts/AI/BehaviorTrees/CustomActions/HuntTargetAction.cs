using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Hunt Target", story: "[Self] hunts [PathTarget]", category: "Action", id: "2228448d47bd20769be8c921961dface")]
public partial class HuntTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> PathTarget;
    [SerializeReference] public BlackboardVariable<VillageManager> VillageManager;
    
    ResourceComponent targetRC;
    float gatheringTimer;
    bool resourceExhausted;
    
    protected override Status OnStart()
    {
        resourceExhausted = false;
        gatheringTimer = 0f;
        targetRC = PathTarget.Value.GetComponentInParent<ResourceComponent>();
        targetRC.callback += CallbackReceiver;
        PathTarget.Value.GetComponentInParent<BehaviorGraphAgent>().SetVariableValue("attacked", true);
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (resourceExhausted)
            return Status.Success;
        if (targetRC == null)
        {
            PathTarget.Value = null;
            return Status.Failure;
        }
        if (gatheringTimer >= targetRC.collectionDelay)
        {
            targetRC.OnCollect();
            gatheringTimer = 0f;
        }

        gatheringTimer += Time.deltaTime;
        return Status.Running;
    }

    protected override void OnEnd()
    {
    }

    void CallbackReceiver()
    {
        resourceExhausted = true;
        Debug.Log("huntResourceAction: End");
        global::VillageManager.instance.AddResource(targetRC.resourceType, targetRC.collectionQuantity);
        PathTarget.Value = null;
    }
}

