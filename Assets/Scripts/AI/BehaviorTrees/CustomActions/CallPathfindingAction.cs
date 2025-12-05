using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Call Pathfinding", story: "[Agent] calls the Pathfinder towards [Target] and sets [Path]", category: "Action", id: "5896d84d319683835cfb37a396b3c215")]
public partial class CallPathfindingAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<List<Vector2Int>> Path;
    
    Pathfinding pathfinder;
    bool pathFound = false;
    
    protected override Status OnStart()
    {
        pathfinder = new Pathfinding();
        pathfinder.callback += PathfindingCallback;
        pathfinder.FindPath(new Vector2Int((int)Agent.Value.transform.position.x, (int)Agent.Value.transform.position.y), new Vector2Int((int)Target.Value.transform.position.x, (int)Target.Value.transform.position.y));
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
        Debug.Log("Path found for" + Agent.Value.name);
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

