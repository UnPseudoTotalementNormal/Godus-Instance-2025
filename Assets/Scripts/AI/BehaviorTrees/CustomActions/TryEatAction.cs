using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Try eat", story: "[Self] try eat [Amount] of meat from [Village]", category: "Action", id: "db4cd5b9522c63720e22cd1da91dd4d1")]
public partial class TryEatAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<float> Amount;
    [SerializeReference] public BlackboardVariable<VillageManager> Village;
    

    protected override Status OnStart()
    {
        if (Self.Value.TryGetComponent(out HungerSystem.HungerComponent _hungerComponent))
        {
            VillageManager _villageManager = VillageManager.instance;
            int _meatCount = _villageManager.GetResourceAmount(ResourceType.Meat);
            if (_meatCount >= Amount.Value)
            {
                _villageManager.AddResource(ResourceType.Meat, (int)-Amount.Value);
                _hungerComponent.EatFood(_hungerComponent.GetMaxHunger());
                return Status.Success;
            }
            else
            {
                return Status.Failure;
            }
        }
        
        return Status.Failure;
    }
}

