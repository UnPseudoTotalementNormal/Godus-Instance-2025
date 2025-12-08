using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "GatherResource", story: "[Self] gathers [PathTarget]", category: "Action", id: "e86aae64195f7e38937405e6bfd3a81e")]
public partial class GatherResourceAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> PathTarget;
    [SerializeReference] public BlackboardVariable<VillageManager> villageManager;

    ResourceComponent targetRC;
    float gatheringTimer;
    bool resourceExhausted;
    
    protected override Status OnStart()
    {
        targetRC = PathTarget.Value.GetComponent<ResourceComponent>();
        targetRC.callback += CallbackReceiver;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (gatheringTimer >= targetRC.collectionDelay)
        {
            targetRC.OnCollect();
            gatheringTimer = 0f;
        }

        if (resourceExhausted)
            return Status.Success;
        gatheringTimer += Time.deltaTime;
        return Status.Running;
    }

    protected override void OnEnd()
    {
        villageManager.Value.AddResource(targetRC.resourceType, targetRC.collectionQuantity);
    }

    void CallbackReceiver()
    {
        resourceExhausted = true;
    }
}

