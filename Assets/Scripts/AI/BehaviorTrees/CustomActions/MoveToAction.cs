using System;
using System.Collections.Generic;
using AI;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MoveTo", story: "[Self] follows [Path] with a speed of [speed]", category: "Action", id: "c4edd160b20236211db2a4a1acbb704e")]
public partial class MoveToAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<PathHolder> Path;
    [SerializeReference] public BlackboardVariable<float> Speed;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<bool> CheckForTarget;
    
    private int currentPathIndex = 0;
    
    protected override Status OnStart()
    {
        currentPathIndex = 0;
        
        if (Path.Value == null || Path.Value.waypoints.Count == 0)
            return Status.Failure;
            
        Path.Value.onPathChanged += OnPathChanged;
        return Status.Running;
    }

    private void OnPathChanged()
    {
        if (Path.Value != null && Path.Value.waypoints.Count > 1)
        {
            currentPathIndex = 1;
        }
        else
        {
            currentPathIndex = 0;
        }
    }

    protected override Status OnUpdate()
    {
        if (Target.Value == null && CheckForTarget.Value)
            return Status.Success;
            
        if (Path.Value == null || Path.Value.waypoints.Count == 0)
            return Status.Failure;
        
        if (currentPathIndex >= Path.Value.waypoints.Count)
            return Status.Success;
        
        Vector2Int _currentTarget = Path.Value.waypoints[currentPathIndex];
        Vector3 _targetPos = new Vector3(_currentTarget.x, _currentTarget.y, -2);
        
        Self.Value.transform.position = Vector3.MoveTowards(Self.Value.transform.position, _targetPos, Speed.Value * Time.deltaTime);
        
        if (Vector3.Distance(Self.Value.transform.position, _targetPos) < 0.01f)
        {
            Self.Value.transform.position = _targetPos;
            currentPathIndex++;
            
            if (currentPathIndex >= Path.Value.waypoints.Count)
            {
                return Status.Success;
            }
        }
        
        return Status.Running;
    }

    protected override void OnEnd()
    {
        currentPathIndex = 0;
        if (Path.Value != null)
        {
            Path.Value.onPathChanged -= OnPathChanged;
        }
    }
}

