using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Random = UnityEngine.Random;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Choose Point Around", story: "[Self] chooses a point in a circle of [radius] units around him and sets [PathTargetLocation]", category: "Action", id: "1b77c1acd34db4a89e6fd6af7cb7f1d9")]
public partial class ChoosePointAroundAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<float> Radius;
    [SerializeReference] public BlackboardVariable<Vector2Int> PathTargetLocation;

    protected override Status OnStart()
    {
        Vector2Int _selfLocation = new Vector2Int((int)Self.Value.transform.position.x, (int)Self.Value.transform.position.y);
        PathTargetLocation.Value = new Vector2Int((int)Random.Range(_selfLocation.x-Radius.Value,_selfLocation.x+Radius.Value), (int)Random.Range(_selfLocation.y-Radius.Value,_selfLocation.y+Radius.Value));
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

