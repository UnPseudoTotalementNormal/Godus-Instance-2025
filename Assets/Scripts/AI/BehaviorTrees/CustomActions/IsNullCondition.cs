using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Is not null", story: "Is [pathTarget] not null ?", category: "Conditions", id: "08152389092521f146a5ab1c7252f7f3")]
public partial class IsNullCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> PathTarget;

    public override bool IsTrue()
    {
        if (PathTarget.Value == null)
            return false;
        return true;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
