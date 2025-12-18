using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Abort task", story: "[Self] ask [VillageManager] to abort [CurrentTask]", category: "Action", id: "aa5c467ec86e32c243d51a798e13301e")]
public partial class AbortTaskAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<VillageManager> VillageManager;
    [SerializeReference] public BlackboardVariable<TaskType> CurrentTask;

    protected override Status OnStart()
    {
        global::VillageManager.instance.AbortTask(Self.Value.transform, CurrentTask.Value);
        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

