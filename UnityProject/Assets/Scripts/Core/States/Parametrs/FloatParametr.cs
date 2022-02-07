namespace Core.States.Parametrs
{
    public class FloatParametr : ParametrBase
    {
        public float Float { get; protected set; }

        public void SetValue(float @float)
        {
            Float = @float;
        }
    }
}
