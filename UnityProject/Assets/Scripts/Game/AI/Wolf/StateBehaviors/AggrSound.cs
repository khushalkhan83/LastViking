using Core.StateMachine;
using Game.Audio;
using UnityEngine;

namespace Game.AI.Behaviours.Boar
{
    public class AggrSound : BehaviourBase
    {
        public int AnimatorPropertyIdAggr { get; } = Animator.StringToHash("Aggr");

        #region Data
#pragma warning disable 0649

        [SerializeField] private Animator _animator;
        [SerializeField] private float _duration;
        [SerializeField] private AudioID[] _audioIDs;
        [SerializeField] private OnceAudioThread _onceAudioThread;

#pragma warning restore 0649
        #endregion

        public Animator Animator => _animator;
        public float Duration => _duration;
        public AudioID[] AudioIDs => _audioIDs;
        public OnceAudioThread OnceAudioThread => _onceAudioThread;

        private AudioID RandomSound => AudioIDs[UnityEngine.Random.Range(0, AudioIDs.Length)];

        public float RemainingTime { get; private set; }

        public override void Begin()
        {
            Animator.SetTrigger(AnimatorPropertyIdAggr);
            RemainingTime = Duration;
            OnceAudioThread.PlayOnce(RandomSound);
        }

        public override void ForceEnd()
        {
            Animator.ResetTrigger(AnimatorPropertyIdAggr);
        }

        public override void Refresh()
        {
            RemainingTime -= Time.deltaTime;

            if (RemainingTime <= 0)
            {
                //Animator.ResetTrigger(AnimatorPropertyIdAggr);
                End();
            }
        }
    }
}
