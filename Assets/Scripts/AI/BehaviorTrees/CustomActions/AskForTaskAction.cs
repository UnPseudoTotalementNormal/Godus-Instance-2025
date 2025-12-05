using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Ask for Task", story: "Ask [VillageManager] for a new [task]", category: "Action", id: "51e0926e754fd4c45642012fd93a6e6e")]
public partial class AskForTaskAction : Action
{
    [SerializeReference] public BlackboardVariable<VillageManager> VillageManager;
    [SerializeReference] public BlackboardVariable<TaskType> Task;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    protected override Status OnStart()
    {
        VillageManager.Value.GetNewTask(out var _target);
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

