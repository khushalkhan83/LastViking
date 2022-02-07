using Core.StateMachine;
using UnityEngine;

namespace Game.AI.Behaviours.Boar
{
    public class IdleBreathe : BehaviourBase
    {
        public int AnimatorPropertyIdIdle { get; } = Animator.StringToHash("IdleBreathe");

        #region Data
#pragma warning disable 0649

        [SerializeField] private Animator _animator;
        [SerializeField] private float _duration;

#pragma warning restore 0649
        #endregion

        public Animator Animator => _animator;
        public float Duration => _duration;

        public float RemainingTime { get; private set; }

        public override void Begin()
        {
            Animator.SetTrigger(AnimatorPropertyIdIdle);
            RemainingTime = Duration;
        }

        public override void ForceEnd()
        {
            Animator.ResetTrigger(AnimatorPropertyIdIdle);
        }

        public override void Refresh()
        {
            RemainingTime -= Time.deltaTime;

            if (RemainingTime <= 0)
            {
                Animator.ResetTrigger(AnimatorPropertyIdIdle);
                End();
            }
        }
    }
}
