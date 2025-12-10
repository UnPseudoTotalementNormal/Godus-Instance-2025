using System;
using AI;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Attack", story: "[Self] attacks [PathTarget]", category: "Action", id: "387649064c43fbd469c1ef936af444a3")]
public partial class AttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> PathTarget;
    [SerializeReference] public BlackboardVariable<AttackComponent> AttackC;

    HealthComponent enemyHealthC;
    protected override Status OnStart()
    {
        enemyHealthC = PathTarget.Value.GetComponent<HealthComponent>();
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (PathTarget.Value == null)
            return Status.Success;
        AttackC.Value.TryAttack(enemyHealthC);
        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

