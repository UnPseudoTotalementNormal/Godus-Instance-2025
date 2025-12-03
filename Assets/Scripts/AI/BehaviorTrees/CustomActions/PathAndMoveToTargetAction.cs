using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "PathAndMoveToTarget", story: "[Self] finds a path and moves towards [PathTarget]", category: "Action", id: "4f59a97ac4e34339c916bae2a8a94b80")]
public partial class PathAndMoveToTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> PathTarget;

    Pathfinding pathfinder;
    List<Cell> PathToTarget;
    bool stepTaken = false;

    protected override Status OnStart()
    {
        pathfinder = new Pathfinding();
        pathfinder.callback += SetNewPath;
        pathfinder.FindPath(new Vector2Int((int)Self.Value.transform.position.x, (int)Self.Value.transform.position.y), new Vector2Int((int)PathTarget.Value.transform.position.x, (int)PathTarget.Value.transform.position.y));
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (!stepTaken)
        {
            FollowPath();
            stepTaken = true;
        }
        return Status.Running;
    }

    protected override void OnEnd()
    {
    }

    void SetNewPath(List<Cell> _path)
    {
        PathToTarget = _path;
    }
    
    async Awaitable FollowPath()
    {
        Vector3 _startPos = Self.Value.transform.position;
        float elapsedTime = 0f;
        while (elapsedTime < 1)
        {
            Self.Value.transform.position = Vector3.Lerp(_startPos, new Vector3(PathToTarget[1].position.x,PathToTarget[1].position.y,-2), elapsedTime / (1));
            elapsedTime += Time.deltaTime;
            await Awaitable.NextFrameAsync();
        }
        pathfinder.FindPath(new Vector2Int((int)Self.Value.transform.position.x, (int)Self.Value.transform.position.y), new Vector2Int((int)PathTarget.Value.transform.position.x, (int)PathTarget.Value.transform.position.y));
        stepTaken = false;
    }
}

