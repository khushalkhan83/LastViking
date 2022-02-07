using Game.AI;
using System;

namespace Game.StateMachine.Parametrs
{
    public class TargetDetection : TargetBase
    {
        private Target target;
        public override Target Target => target;

        public event Action OnChangeTarget;

        public void SetTarget(Target target)
        {
            this.target = target;
            OnChangeTarget?.Invoke();
        }
    }
}
