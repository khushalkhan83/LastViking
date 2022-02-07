using System;
using UnityEngine;

namespace Core.StateMachine
{
    public abstract class BehaviourBase : MonoBehaviour
    {
        public event Action<BehaviourBase> OnEnd;

        public abstract void Begin();
        public abstract void ForceEnd();
        public abstract void Refresh();

        protected void End() => OnEnd?.Invoke(this);
    }
}
