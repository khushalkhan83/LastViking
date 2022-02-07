using Core.StateMachine;
using Core.States.Parametrs;
using Game.Models;
using UnityEngine;

namespace Game.StateMachine.Behaviours
{
    public class TargetOutOfRange : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private float _outTime;
        [SerializeField] private TimerBase _targetOutTimer;

        [SerializeField] private ConditionBase[] _conditionUpdate;
        [SerializeField] private ConditionBase[] _conditionReset;
        [SerializeField] private EffectBase _activateEffect;

#pragma warning restore 0649
        #endregion

        public float OutTime => _outTime;

        public ConditionBase[] ConditionUpdate => _conditionUpdate;
        public ConditionBase[] ConditionReset => _conditionReset;
        private EffectBase ActivateEffect => _activateEffect;
        private TimerBase TimerBase => _targetOutTimer;

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
            if (ConditionReset.IsTrue())
            {
                TimerBase.ResetTimer();
            }

            if (ConditionUpdate.IsTrue())
            {
                TimerBase.Tick(Time.deltaTime);
            }

            if (TimerBase.Passed >= OutTime)
            {
                TimerBase.ResetTimer();
                ActivateEffect.Apply();
            }
        }
    }
}
