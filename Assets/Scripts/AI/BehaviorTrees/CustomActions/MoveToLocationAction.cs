using System;
using System.Collections.Generic;
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
    [SerializeReference] public BlackboardVariable<List<Vector2Int>> Path;

    Pathfinding pathfinder;
    bool pathFound = false;
    
    protected override Status OnStart()
    {
        pathfinder = new Pathfinding();
        pathfinder.callback += PathfindingCallback;
        pathfinder.FindPath(new Vector2Int((int)Self.Value.transform.position.x, (int)Self.Value.transform.position.y), Location.Value);
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
        Path.Value = new();
        pathFound = true;
        foreach (Cell _cell in _path)
        {
            Path.Value.Add(_cell.position);
        }
    }
}

