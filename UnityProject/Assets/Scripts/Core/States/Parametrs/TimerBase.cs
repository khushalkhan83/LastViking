namespace Core.States.Parametrs
{
    public class TimerBase : ParametrBase
    {
        public float Passed { get; private set; }

        public void ResetTimer()
        {
            Passed = 0;
        }

        public void Tick(float time)
        {
            Passed += time;
        }
    }
}
