namespace Core.States.Parametrs
{
    public class BoolParametr : ParametrBase
    {
        public bool Bool { get; protected set; }

        public void SetValue(bool @bool)
        {
            Bool = @bool;
        }
    }
}
