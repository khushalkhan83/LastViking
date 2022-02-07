using System.Collections.Generic;

namespace Core.StateMachine
{
    static public class ConditionExtensions
    {
        static public bool IsTrue(this ConditionBase[] conditions)
        {
            for (int i = 0; i < conditions.Length; i++)
            {
                if (!conditions[i].IsTrue) return false;
            }
            return true;
        }
    }
}
