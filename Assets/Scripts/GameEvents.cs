using System;
using UnityEngine;
using Utils.TimerSystem;

public static class GameEvents
{
    public static Action<int> onWaveStarted;
    public static Action onWaveEnded;
    public static Action onGameOver;
    
    public static Action<GameObject> onTownHallCreated;
    public static Action onTownHallDestroy;
    
    public static Action onStorageBuildingCreated;
    public static Action onStorageBuildingDestroyed;
    
    public static Action<Entity> onNewEntitySpawned;
    public static Action<Entity> onAlienDeath;
    public static Action<Entity> onEnemyDeath;
    public static Action<Entity> onEntityDeath;
    public static Action<WaveInfo> onWaveInfo;
  
    public static Action<TimerSystem> onStartTimerBetweenWave;

    public static Action onNecessaryPathMade;
    
    public static Action<ResourceType, int> onResourceValueRefreshed;
    public static Action<ResourceType, int> onResourceMaxValueRefreshed;

#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoadMethod]
    private static void ResetStaticEvents()
    {
        UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(UnityEditor.PlayModeStateChange _state)
    {
        if (_state == UnityEditor.PlayModeStateChange.ExitingPlayMode)
        {
            onWaveStarted = null;
            onWaveEnded = null;
            onGameOver = null;
            onTownHallCreated = null;
            onTownHallDestroy = null;
            onStorageBuildingCreated = null;
            onStorageBuildingDestroyed = null;
            onNewEntitySpawned = null;
            onAlienDeath = null;
            onEnemyDeath = null;
            onEntityDeath = null;
            onWaveInfo = null;
            onStartTimerBetweenWave = null;
            onNecessaryPathMade = null;
            onResourceValueRefreshed = null;
            onResourceMaxValueRefreshed = null;
        }
    }
#endif
}