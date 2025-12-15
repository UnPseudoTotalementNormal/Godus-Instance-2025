using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SearchForTownHall", story: "[Self] gets the location of the TownHall", category: "Action", id: "c4c3d0d5a792813847bbdd34d0c3cb9c")]
public partial class SearchForTownHallAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    protected override Status OnStart()
    {
        Collider2D _collider = Physics2D.OverlapCircle(Self.Value.transform.position, 50f, LayerMask.GetMask("TownHall"));
        if (_collider != null)
        {
            Target.Value = _collider.gameObject;
            return Status.Success;
        }
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

