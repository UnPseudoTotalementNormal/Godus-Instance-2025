using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "BuildAtLocation", story: "[Self] builds [building] at his current position", category: "Action", id: "52464b5d7569c11a7ef67b4d9c4489d0")]
public partial class BuildAtLocationAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Building;
    [SerializeReference] public BlackboardVariable<VillageManager> VillageManager;

    bool buildingFinished;
    
    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (buildingFinished)
        {
            VillageManager.Value.BuildAtLocation(Self.Value.transform, Building.Value);
            return Status.Success;
        }
        return Status.Running;
    }

    protected override void OnEnd()
    {
    }

    async Awaitable WaitForDelay()
    {
        await Awaitable.WaitForSecondsAsync(5);
        buildingFinished = true;
    }
}

