using Core.StateMachine;
using Core.States.Parametrs;
using UnityEngine;

namespace Game.StateMachine.Conditions
{
    public class TimeRemainingCondition : ConditionBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private TimerBase _timer;
        [SerializeField] private float _remainingTime;

#pragma warning restore 0649
        #endregion

        public TimerBase Timer => _timer;
        public float RemainingTime => _remainingTime;

        public override bool IsTrue => Timer.Passed >= RemainingTime;
    }
}
