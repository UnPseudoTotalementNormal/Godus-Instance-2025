using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "UnitInfo", menuName = "Scriptable Objects/UnitInfo")]
public class UnitInfo : ScriptableObject
{
    [Header("Unit Info")]
    public string unitName;

    [FormerlySerializedAs("unitHP")] public string unitHp;
    
    [FormerlySerializedAs("unitATK")] public string unitAtk;
    
    public Sprite unitSprite;
    
    public List<UnitCost> cost;
    
    [Header("UnitPrefab")]
    public GameObject unitPrefab;

    void OnValidate()
    {
        if (cost.Count == 0)
        {
            cost = new List<UnitCost>();
            foreach (ResourceType _resourceType in Enum.GetValues(typeof(ResourceType)))
            {
                if (_resourceType == ResourceType.None)
                    continue;
                cost.Add(new UnitCost(_resourceType, 0));
            }
        }
    }
}

[System.Serializable]
public struct UnitCost
{
    public ResourceType resourceType;
    public int cost;

    public UnitCost(ResourceType _resourceType, int _cost)
    {
        this.resourceType = _resourceType;
        this.cost = _cost;
    }
}
