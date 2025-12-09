using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Hunt Target", story: "[Self] hunts [PathTarget] and stays at a range of [range]", category: "Action", id: "2228448d47bd20769be8c921961dface")]
public partial class HuntTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> PathTarget;
    [SerializeReference] public BlackboardVariable<float> Range;
    
    ResourceComponent targetRC;
    float gatheringTimer;
    bool resourceExhausted;
    
    protected override Status OnStart()
    {
        targetRC = PathTarget.Value.GetComponent<ResourceComponent>();
        targetRC.callback += CallbackReceiver;
        PathTarget.Value.GetComponent<BehaviorGraphAgent>().SetVariableValue("attacked", true);
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
    }

    void CallbackReceiver()
    {
        resourceExhausted = true;
    }
}

