using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AnimalMoveTo", story: "[Self] follow [path] with speed of [speed] and stops if [attacked]", category: "Action", id: "2223409a7f7a2801dd8f7a4c52d6281f")]
public partial class AnimalMoveToAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<List<Vector2Int>> Path;
    [SerializeReference] public BlackboardVariable<float> Speed;
    [SerializeReference] public BlackboardVariable<bool> Attacked;
    bool arrived = false;
    
    protected override Status OnStart()
    {
        FollowPath();
        return Status.Running;
    }
    
    protected override Status OnUpdate()
    {
        if (Attacked.Value)
            return Status.Success;
       
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
            float _elapsedTime = 0f;
            while (_elapsedTime < Speed.Value)
            {
                Self.Value.transform.position = Vector3.Lerp(_startPos, new Vector3(_p.x,_p.y,-2), _elapsedTime / (Speed.Value));
                _elapsedTime += Time.deltaTime;
                await Awaitable.NextFrameAsync();
            }
        }

        arrived = true;
        return;
    }
}

