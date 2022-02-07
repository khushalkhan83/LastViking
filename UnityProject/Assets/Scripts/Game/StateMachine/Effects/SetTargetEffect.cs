using Core.StateMachine;
using Game.StateMachine.Parametrs;
using UnityEngine;

namespace Game.StateMachine.Effects
{
    public class SetTargetEffect : EffectBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private TargetBase _target;
        [SerializeField] private TargetDetection _targetDetection;

#pragma warning restore 0649
        #endregion

        public TargetBase Target => _target;
        public TargetDetection TargetDetection => _targetDetection;

        public override void Apply()
        {
            TargetDetection.SetTarget(Target?.Target);
        }
    }
}
