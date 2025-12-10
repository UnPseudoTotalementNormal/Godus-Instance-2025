using System;
using System.Collections.Generic;
using System.Linq;
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
    Vector2Int targetPosition;
    int stepIndex = 0;
    float stepTime = 0.5f;
    bool canStep = false;
    bool hasToPath = false;
    protected override Status OnStart()
    {
        targetPosition = new Vector2Int((int)PathTarget.Value.transform.position.x, (int)PathTarget.Value.transform.position.y);
        pathfinder = new Pathfinding();
        pathfinder.callback += SetNewPath;
        
        GetNewPath();
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Vector2.Distance(Self.Value.gameObject.transform.position, targetPosition) <= 0.5f)
        {
            return Status.Success;
        }
        if (canStep)
            if (HasTargetMoved())
            {
                targetPosition = new Vector2Int((int)PathTarget.Value.transform.position.x, (int)PathTarget.Value.transform.position.y);
                hasToPath = true;
                GetNewPath();
                stepIndex = 1;
                return Status.Running;
            }
            else
            {
                FollowPath();
            }
        return Status.Running;
    }

    protected override void OnEnd()
    {
    }

    void GetNewPath()
    {
        Vector2Int newPosition = new Vector2Int((int)Self.Value.transform.position.x, (int)Self.Value.transform.position.y);
        pathfinder.FindPath(newPosition, targetPosition);
    }
    
    void SetNewPath(List<Cell> _path)
    {
        PathToTarget = _path;
        
        Vector2 currentPos = Self.Value.transform.position;
        float minDist = float.MaxValue;
        int closestIndex = 0;

        for (int i = 0; i < PathToTarget.Count; i++)
        {
            float d = Vector2.Distance(currentPos, PathToTarget[i].position);
            if (d < minDist)
            {
                minDist = d;
                closestIndex = i;
            }
        }

        stepIndex = closestIndex;
        canStep = true;
        
        for (int i = 0; i < PathToTarget.Count - 1; i++)
        {
            Vector3 a = new Vector3(PathToTarget[i].position.x, PathToTarget[i].position.y, -1);
            Vector3 b = new Vector3(PathToTarget[i + 1].position.x, PathToTarget[i + 1].position.y, -1);

            Debug.DrawLine(a, b, Color.cyan, 5.0f);
        }
    }
    
    bool HasTargetMoved()
    {
        Vector2Int currentTargetPosition = new Vector2Int((int)PathTarget.Value.transform.position.x, (int)PathTarget.Value.transform.position.y);
        
        return currentTargetPosition != targetPosition;
    }
    
    async Awaitable FollowPath()
    {
        if (!canStep && stepIndex > 0)
            return;
        
        canStep = false;
        
        Vector3 _startPos = Self.Value.transform.position;
        float elapsedTime = 0f;
        while (elapsedTime < stepTime)
        {
            Self.Value.transform.position = Vector3.Lerp(_startPos, new Vector3(PathToTarget[stepIndex].position.x,PathToTarget[stepIndex].position.y,-2), elapsedTime / (stepTime));
            elapsedTime += Time.deltaTime;
            if (hasToPath)
                break;
            await Awaitable.NextFrameAsync();
        }
        canStep = true;
        stepIndex++;
    }
}

