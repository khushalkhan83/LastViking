using Core.StateMachine;
using Game.Audio;
using UnityEngine;

namespace Game.AI.Behaviours.Skeleton
{
    public class Death : BehaviourBase
    {

        #region Data
#pragma warning disable 0649

        [SerializeField] private float _duration;
        [SerializeField] private AudioID _audioID;

#pragma warning restore 0649
        #endregion
        public AudioSystem AudioSystem => AudioSystem.Instance;

        public AudioID AudioID => _audioID;
        public float Duration => _duration;

        public float RemainingTime { get; private set; }

        public override void Begin()
        {
            RemainingTime = Duration;
            AudioSystem.PlayOnce(AudioID, transform.position);
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
