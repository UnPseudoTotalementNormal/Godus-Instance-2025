namespace Powers
{
    public class PowerUseTownHallCheck : PowerUseConditionComponent
    {
        public override bool CanUsePower()
        {
            return PowerManager.instance.hasTownHallSpawned;
        }
    }
}