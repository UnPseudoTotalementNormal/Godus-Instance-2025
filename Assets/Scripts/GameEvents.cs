using System;
using Utils.TimerSystem;

public static class GameEvents
{
    public static Action<int> onWaveStarted;
    public static Action onWaveEnded;
    
    public static Action onEnemyDeath;
    public static Action<WaveInfo> onWaveInfo;

    public static Action<TimerSystem> onStartTimerBetweenWave;
}