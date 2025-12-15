using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager instance { get; private set; }
    
    public List <Entity> alienUnit = new List<Entity>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void Enter()
    {
        
    }  
}
