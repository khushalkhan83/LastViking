using Core.StateMachine;
using UnityEngine;

namespace Game.AI.States.Behaviours.Zombie
{
    public class OnHit : BehaviourBase
    {
        public int AnimatorParametrIdReceiveHit { get; } = Animator.StringToHash("ReceiveHit");

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
            Animator.SetTrigger(AnimatorParametrIdReceiveHit);
            RemainingTime = Duration;
        }

        public override void ForceEnd()
        {
            RemainingTime = 0;
        }

        public override void Refresh()
        {
            if (RemainingTime > 0)
            {
                RemainingTime -= Time.deltaTime;
            }

            if (RemainingTime < 0)
            {
                RemainingTime = 0;
            }

            if (RemainingTime == 0)
            {
                End();
            }
        }
    }
}
