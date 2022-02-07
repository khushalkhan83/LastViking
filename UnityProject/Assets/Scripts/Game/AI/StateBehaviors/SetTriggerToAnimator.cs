using Core.StateMachine;
using Game.Audio;
using UnityEngine;

namespace Game.AI.Behaviours
{
    public class SetTriggerToAnimator : BehaviourBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private string _triggerName;
        [SerializeField] private Animator _animator;
        [SerializeField] private float _duration;

#pragma warning restore 0649
        #endregion

        public string TriggerName => _triggerName;
        public float Duration => _duration;
        public Animator Animator => _animator;

        public float RemainingTime { get; private set; }

        public override void Begin()
        {
            RemainingTime = Duration;
            Animator.SetTrigger(TriggerName);
        }

        public override void ForceEnd()
        {

        }

        public override void Refresh()
        {
            RemainingTime -= Time.deltaTime;

            if (RemainingTime <= 0)
            {
                End();
            }
        }
    }
}
