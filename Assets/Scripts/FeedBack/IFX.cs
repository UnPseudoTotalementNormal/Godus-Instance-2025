namespace Feedback
{
    public interface IFX
    {
        void StartVFX(int _currentWave);
        void StartSFX(int _currentWave);
        void StartTimer(int _currentWave);
        void StartShaderEffect(int _currentWave);
        void StartText(int _currentWave);
    }
}