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
    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

