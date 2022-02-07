using Core.StateMachine;
using Game.Audio;
using UnityEngine;

namespace Game.AI.Behaviours.Skeleton
{
    public class Show : BehaviourBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private float _duration;
        [SerializeField] private AudioID _audioID;
        [SerializeField] private OnceAudioThread _onceAudioThread;

#pragma warning restore 0649
        #endregion

        public AudioID AudioID => _audioID;
        public float Duration => _duration;
        public OnceAudioThread OnceAudioThread => _onceAudioThread;

        public float TimeToEnd { get; private set; }

        public override void Begin()
        {
            TimeToEnd = Duration;
            OnceAudioThread.PlayOnce(AudioID);
        }

        public override void ForceEnd()
        {
            TimeToEnd = 0;
        }

        public override void Refresh()
        {
            TimeToEnd -= Time.deltaTime;

            if (TimeToEnd <= 0)
            {
                End();
            }
        }
    }
}
