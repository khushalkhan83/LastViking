using System;

namespace UltimateSurvival
{
    [Serializable]
    public class FloatChanged : ChangedValue<float>
    {
        public FloatChanged(float value) : base(value)
        {
        }
    }
}