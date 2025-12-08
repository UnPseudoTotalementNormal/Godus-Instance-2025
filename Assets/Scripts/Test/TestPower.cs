using Powers;

namespace Test
{
    public class TestPower : Power
    {
        public override bool ShouldStartCooldownOnDeactivate()
        {
            return true;
        }
    }
}