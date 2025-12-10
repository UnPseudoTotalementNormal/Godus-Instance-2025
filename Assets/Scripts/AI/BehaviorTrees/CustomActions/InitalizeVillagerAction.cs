using AI;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "InitalizeVillager", story: "[Self] gets references of it's [HealthComponent] and [AttackComponent]", category: "Action", id: "d5c38443cb92fc12addee00e7f4ab48b")]
public partial class InitalizeVillagerAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<HealthComponent> HealthComponent;
    [SerializeReference] public BlackboardVariable<AttackComponent> AttackComponent;

    protected override Status OnStart()
    {
        HealthComponent.Value = Self.Value.GetComponent<HealthComponent>();
        AttackComponent.Value = Self.Value.GetComponent<AttackComponent>();
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

