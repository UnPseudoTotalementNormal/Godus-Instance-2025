using System;
using HungerSystem;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Get current hunger", story: "Assign [Self] hunger to [Variable]", category: "Action", id: "8ad84413874246d6eac5fd2ac71d07ea")]
public partial class GetCurrentHungerAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<float> Variable;

    protected override Status OnStart()
    {
        if (Self.Value.TryGetComponent(out HungerComponent _hungerComponent))
        {
            Variable.Value = _hungerComponent.GetHunger();
            return Status.Success;
        }
        
        return Status.Failure;
    }

}

