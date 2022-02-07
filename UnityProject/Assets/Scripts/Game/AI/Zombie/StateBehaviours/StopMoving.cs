using Core.StateMachine;
using Game.Audio;
using Game.StateMachine.Behaviours;
using Game.StateMachine.Parametrs;
using UnityEngine;

namespace Game.AI.Behaviours.Zombie
{
    public class StopMoving: BehaviourBase
    {
        public int AnimatorPropertyIdSpeedMove { get; } = Animator.StringToHash("Speed");

        #region Data
#pragma warning disable 0649

        [SerializeField] private Movement _movement;
        [SerializeField] private Animator _animator;
        [SerializeField] private float _dampTime = 0.5f;
        [SerializeField] private float _stopSpeed = 0f;
        //[SerializeField] private AudioID _audioID;
        //[SerializeField] private OnceAudioThread OncePlay;

#pragma warning restore 0649
        #endregion

        //public AudioID AudioID => _audioID;
        public Movement Movement => _movement;
        public Animator Animator => _animator;
        public float DampTime => _dampTime;

        public override void Begin()
        {
            //OncePlay.PlayOnce(AudioID);
            Animator.transform.localRotation = Quaternion.identity;
            Animator.transform.localPosition = Vector3.zero;
        }

        public override void ForceEnd()
        {
            Movement.Stop();
            Animator.SetFloat(AnimatorPropertyIdSpeedMove, 0);
        }

        public override void Refresh()
        {
            Animator.transform.localRotation = Quaternion.identity;
            Animator.transform.localPosition = Vector3.zero;

            if (Animator.GetFloat(AnimatorPropertyIdSpeedMove) <= _stopSpeed)
            {
                Animator.SetFloat(AnimatorPropertyIdSpeedMove, 0);
                End();
            }
            else
            {
                Animator.SetFloat(AnimatorPropertyIdSpeedMove, 0, DampTime, Time.deltaTime);
            }
        }
    }
}
