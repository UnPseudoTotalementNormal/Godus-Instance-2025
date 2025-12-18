using System;
using System.Collections.Generic;
using System.Linq;
using AI;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MoveToLocation", story: "[Self] calls the pahtfinder towards [location] and sets [Path]", category: "Action", id: "f999d70c92d8995e65de19d42cfea12a")]
public partial class MoveToLocationAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<Vector2Int> Location;
    [SerializeReference] public BlackboardVariable<PathHolder> Path;
    [SerializeReference] public BlackboardVariable<VillageManager> vManager;
    Pathfinding pathfinder;
    bool pathFound = false;
    
    protected override Status OnStart()
    {
        if (pathfinder == null)
        {
            pathfinder = new Pathfinding();
            pathfinder.callback += PathfindingCallback;
        }

        pathFound = false;
        PathHolder _pathHolder = Path.Value;
        
        if (!_pathHolder.askForRecalculation
            && !_pathHolder.hasMapBeenChangedSinceLastPathCalculation 
            && _pathHolder.targetObject == null
            && _pathHolder.waypoints.Count > 0
            && _pathHolder.waypoints[^1] == Location.Value)
        {
            // Path is still valid
            pathFound = true;
            return Status.Success;
        }
        
        _pathHolder.targetObject = null;
        pathfinder.FindPath(Vector2Int.RoundToInt(Self.Value.transform.position), Location.Value);
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (pathFound)
            return Status.Success;
        return Status.Running;
    }

    protected override void OnEnd()
    {
        //Debug.Log("Path found for" + Self.Value.name);
    }

    void PathfindingCallback(List<Cell> _path)
    {
        pathFound = true;
        Path.Value.SetPath(_path.Select(_c => _c.position).ToList());
    }
}

