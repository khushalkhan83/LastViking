using Core.StateMachine;
using Game.Audio;
using UnityEngine;

namespace Game.AI.Behaviours
{
    public class SetTriggerToAnimatorZombie : BehaviourBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private string _triggerName;
        [SerializeField] private Animator _animator;
        [SerializeField] private float _duration;
        [SerializeField] private AudioID _audioID;
        [SerializeField] private OnceAudioThread _onceAudioThread;

#pragma warning restore 0649
        #endregion

        public AudioID AudioID => _audioID;
        public string TriggerName => _triggerName;
        public float Duration => _duration;
        public Animator Animator => _animator;
        public OnceAudioThread OnceAudioThread => _onceAudioThread;

        public float RemainingTime { get; private set; }

        public override void Begin()
        {
            RemainingTime = Duration;
            Animator.SetTrigger(TriggerName);
            OnceAudioThread.PlayOnce(AudioID);
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
