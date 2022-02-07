using Core.StateMachine;
using Game.Audio;
using UnityEngine;

namespace Game.AI.Behaviours.Bear
{
    public class Death : BehaviourBase
    {
        public int AnimatorParametrIdDeath { get; } = Animator.StringToHash("Death");

        #region Data
#pragma warning disable 0649

        [SerializeField] private Animator _animator;
        [SerializeField] private float _duration;
        [SerializeField] private AudioID _audioID;
        [SerializeField] private OnceAudioThread _onceAudioThread;

#pragma warning restore 0649
        #endregion

        public AudioID AudioID => _audioID;
        public Animator Animator => _animator;
        public float Duration => _duration;
        public OnceAudioThread OnceAudioThread => _onceAudioThread;
        public float RemainingTime { get; private set; }

        public override void Begin()
        {
            RemainingTime = Duration;
            Animator.SetTrigger(AnimatorParametrIdDeath);
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
