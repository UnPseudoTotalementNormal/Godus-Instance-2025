using UnityEngine;
using System;
using Powers;

public class TownHallSpawn : MonoBehaviour
{
    private void Start()
    {
        PowerManager.instance.hasTownHallSpawned = true;
        PowerManager.instance.isTownHallSpawned = true;
        GameEvents.onTownHallCreated?.Invoke();
    }

    private void OnDestroy()
    {
        PowerManager.instance.isTownHallSpawned = false;
        GameEvents.onTownHallDestroy?.Invoke();
    }
}
