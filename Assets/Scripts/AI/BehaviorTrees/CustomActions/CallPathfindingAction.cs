using System;
using System.Collections.Generic;
using System.Linq;
using AI;
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
    [SerializeReference] public BlackboardVariable<PathHolder> Path;
    
    Pathfinding pathfinder;
    bool pathFound = false;
    
    protected override Status OnStart()
    {
        if (pathfinder == null)
        {
            pathfinder = new Pathfinding();
            pathfinder.callback += PathfindingCallback;
        }
        pathfinder.FindPath(Vector2Int.RoundToInt(Agent.Value.transform.position), new Vector2Int((int)Target.Value.transform.position.x, (int)Target.Value.transform.position.y));
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
        //Debug.Log("Path found for" + Agent.Value.name);
    }

    void PathfindingCallback(List<Cell> _path)
    {
        pathFound = true;
        //Debug.Log("Path found for " + Agent.Value.name + " to " + Target.Value.name);
        Path.Value.SetPath(_path.Select(_c => _c.position).ToList());
    }
}

