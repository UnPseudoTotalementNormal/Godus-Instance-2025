using System;
using UnityEngine;

public class StorageBuilding : MonoBehaviour
{
    private void Start()
    {
        GameEvents.onStorageBuildingCreated?.Invoke();
    }

    private void OnDestroy()
    {
        GameEvents.onStorageBuildingDestroyed?.Invoke();
    }
}
