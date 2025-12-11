using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.Serialization;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "LookAroundForTarget", story: "[Self] looks around for a target of faction [Faction]", category: "Action", id: "9fef01a4d135449317f00d0c949fc738")]
public partial class LookAroundForTargetAction : Action
{
    [FormerlySerializedAs("Self")] [SerializeReference] public BlackboardVariable<GameObject> self;
    [FormerlySerializedAs("Faction")] [SerializeReference] public BlackboardVariable<Factions> faction;
    [FormerlySerializedAs("Target")] [SerializeReference] public BlackboardVariable<GameObject> target;
    [SerializeReference] public BlackboardVariable<AiState> aiState;

    protected override Status OnStart()
    {
        Collider2D[] _around = Physics2D.OverlapCircleAll(self.Value.transform.position, 20f);
        float closest = Int32.MaxValue;
        GameObject newTarget = null;
        foreach (Collider2D _hit in _around)
        {
            if (_hit.TryGetComponent(out BehaviorGraphAgent _agent))
            {
                if (_agent.GetVariable("Faction", out BlackboardVariable _faction) == false)
                    continue;
                if (_faction.ValueEquals(faction) && Vector3.Distance(self.Value.transform.position, _agent.transform.position) <=closest)
                {
                    //Debug.Log("Target found !!!");
                    closest = Vector3.Distance(self.Value.transform.position, _agent.transform.position);
                    newTarget = _agent.gameObject;
                    target.Value = _agent.gameObject;
                    aiState.Value = AiState.Attacking;
                }
            }
        }
        if (newTarget == null)
            aiState.Value = AiState.Free;
        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

