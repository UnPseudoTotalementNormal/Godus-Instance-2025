using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "GetUpgrade", story: "[Self] asks [VillageManager] for an upgrade", category: "Action", id: "1826f3a3c38a5265239250253de460b8")]
public partial class GetUpgradeAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<VillageManager> VillageManager;

    protected override Status OnStart()
    {
        Debug.Log("Get upgrade action for" + Self.Value.name);
        VillageManager.Value.UpgradeEntity(Self.Value.GetComponent<Entity>());
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

