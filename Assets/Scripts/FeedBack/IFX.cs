namespace Feedback
{
    public interface IFX
    {
        void StartFX(int _currentWave);
        void StartVFX();
        void StartSFX();
        void StartTimer();
        void StartShaderEffect();
    }
}