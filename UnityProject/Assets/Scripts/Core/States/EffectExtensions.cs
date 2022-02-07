using System.Collections.Generic;

namespace Core.StateMachine
{
    static public class EffectExtensions
    {
        static public void Apply(this IEnumerable<EffectBase> effects)
        {
            foreach (var effect in effects)
            {
                effect.Apply();
            }
        }
    }
}
