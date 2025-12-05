using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MoveTo", story: "[Self] follows [Path] with a speed of [speed]", category: "Action", id: "c4edd160b20236211db2a4a1acbb704e")]
public partial class MoveToAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<List<Vector2Int>> Path;
    [SerializeReference] public BlackboardVariable<float> Speed;
    bool arrived = false;
    
    protected override Status OnStart()
    {
        FollowPath();
        return Status.Running;
    }
    
    protected override Status OnUpdate()
    {
        if (Path.Value.Count == 0)
            return Status.Running;
        
        if (Vector2.Distance(Self.Value.gameObject.transform.position, Path.Value.Last()) <= 0.5f)
        {
            return Status.Success;
        }
        else
        {
            return Status.Running;
        }
    }

    protected override void OnEnd()
    {
        
    }

    async Awaitable FollowPath()
    {
        foreach (Vector2Int _p in Path.Value)
        {
            Vector3 _startPos = Self.Value.transform.position;
            float elapsedTime = 0f;
            while (elapsedTime < Speed.Value)
            {
                Self.Value.transform.position = Vector3.Lerp(_startPos, new Vector3(_p.x,_p.y,-2), elapsedTime / (Speed.Value));
                elapsedTime += Time.deltaTime;
                await Awaitable.NextFrameAsync();
            }
        }

        arrived = true;
        return;
    }
}

