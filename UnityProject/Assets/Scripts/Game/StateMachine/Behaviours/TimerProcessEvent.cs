using Core.StateMachine;
using Core.States.Parametrs;
using Game.Models;
using UnityEngine;

namespace Game.StateMachine.Behaviours
{
    public class TimerProcessEvent : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private float _durationToApply;
        [SerializeField] private TimerBase _timer;
        [SerializeField] private ConditionBase[] _conditionBegin;
        [SerializeField] private ConditionBase[] _conditionEnd;
        [SerializeField] private EffectBase[] _beginEffects;
        [SerializeField] private EffectBase[] _endEffects;
        [SerializeField] private EffectBase[] _activateEffects;

#pragma warning restore 0649
        #endregion

        public float DurationToApply => _durationToApply;
        public ConditionBase[] ConditionBegin => _conditionBegin;
        public ConditionBase[] ConditionEnd => _conditionEnd;
        public EffectBase[] BeginEffects => _beginEffects;
        public EffectBase[] EndEffects => _endEffects;
        public EffectBase[] ActivateEffects => _activateEffects;
        public TimerBase Timer => _timer;

        public bool IsRunTimer { get; private set; }

        private GameUpdateModel GameUpdateModel => ModelsSystem.Instance._gameUpdateModel;

        private void OnEnable()
        {
            GameUpdateModel.OnUpdate += OnUpdate;
        }

        private void OnDisable()
        {
            GameUpdateModel.OnUpdate -= OnUpdate;
        }

        private void OnUpdate()
        {
            if (IsRunTimer)
            {
                if (ConditionEnd.IsTrue())
                {
                    IsRunTimer = false;
                    Timer.ResetTimer();
                    EndEffects?.Apply();
                }
            }
            else
            {
                if (ConditionBegin.IsTrue())
                {
                    IsRunTimer = true;
                    BeginEffects?.Apply();
                }
            }

            if (IsRunTimer)
            {
                Timer.Tick(Time.deltaTime);
            }

            if (Timer.Passed >= DurationToApply)
            {
                IsRunTimer = false;
                Timer.ResetTimer();
                ActivateEffects?.Apply();
            }
        }
    }
}
