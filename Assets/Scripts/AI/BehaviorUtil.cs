using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;

public static class BehaviorUtil
{
    //Not used but still kept if necessary
    public static bool GetVariable<TValue>(this RuntimeBlackboardAsset _self, string _variableName, out TValue _variableValue) where TValue : new()
    {
        foreach (BlackboardVariable _kvp in _self.Blackboard.Variables)
        {
            if (_kvp.Name == _variableName)
            {
                _variableValue = (TValue)_kvp.ObjectValue;
                return true;
            }
        }
        _variableValue = default(TValue);
        return false;
    }

    public static bool SetVariable<TValue>(this RuntimeBlackboardAsset _self, string _variableName, TValue _value)
    {
        foreach (BlackboardVariable _kvp in _self.Blackboard.Variables)
        {
            if (_kvp.Name == _variableName)
            {
                _kvp.ObjectValue = _value;
                return true;
            }
        }
        return false;
    }
}
