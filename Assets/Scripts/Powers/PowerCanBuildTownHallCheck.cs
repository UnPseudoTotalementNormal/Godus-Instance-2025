namespace Powers
{
    public class PowerCanBuildTownHallCheck: PowerUseConditionComponent
    {
        public override bool CanUsePower()
        {
            return !PowerManager.instance.isTownHallSpawned;
        }
    }
}