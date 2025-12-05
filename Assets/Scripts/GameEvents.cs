using System;

public static class GameEvents
{
    public static Action<int> onWaveStarted;
    public static Action onWaveEnded;
    public static Action onEnemyDeath;
    public static Action<WaveInfo> onWaveInfo;
    public static Action<bool> onEnabledSlideBarRemainingEnemy;
    public static Action onEnableSlideBarRemainingEnemy;
    public static Action onDisableSlideBarRemainingEnemy;
}