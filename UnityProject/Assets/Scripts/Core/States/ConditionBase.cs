using UnityEngine;

namespace Core.StateMachine
{
    public abstract class ConditionBase : MonoBehaviour
    {
        public abstract bool IsTrue { get; }
    }
}
