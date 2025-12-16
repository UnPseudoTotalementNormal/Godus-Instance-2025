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
    
    public static Action<ResourceType, int> onResourceValueRefreshed;
    public static Action<ResourceType, int> onResourceMaxValueRefreshed;
}