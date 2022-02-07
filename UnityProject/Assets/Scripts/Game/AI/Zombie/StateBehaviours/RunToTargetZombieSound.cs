using Core.StateMachine;
using Game.Audio;
using Game.StateMachine.Behaviours;
using Game.StateMachine.Parametrs;
using UnityEngine;

namespace Game.AI.Behaviours.Zombie
{
    public class RunToTargetZombieSound : BehaviourBase // cant find usage of this class in project
    {
        public int AnimatorPropertyIdSpeedMove { get; } = Animator.StringToHash("Speed");
        public int AnimatorPropertyIdMove { get; } = Animator.StringToHash("Move");

        #region Data
#pragma warning disable 0649

        [SerializeField] private Movement _movement;
        [SerializeField] private TargetBase _target;
        [SerializeField] private Animator _animator;
        [SerializeField] private AudioID _audioID;
        [SerializeField] private float _stopDistance;

#pragma warning restore 0649
        #endregion

        public AudioSystem AudioSystem => AudioSystem.Instance;

        public Movement Movement => _movement;
        public TargetBase Target => _target;
        public Animator Animator => _animator;
        public float StopDistance => _stopDistance;

        public override void Begin()
        {
            Animator.transform.localRotation = Quaternion.identity;
            Animator.transform.localPosition = Vector3.zero;
            Movement.RunTo(Target.Target.transform.position, StopDistance);
            Animator.SetFloat(AnimatorPropertyIdSpeedMove, Movement.SpeedSmoothNormalized);
            Animator.SetTrigger(AnimatorPropertyIdMove);
            AudioSystem.PlayOnce(_audioID);
        }

        public override void ForceEnd()
        {
            Movement.Stop();
            Animator.SetFloat(AnimatorPropertyIdSpeedMove, Movement.SpeedSmoothNormalized);
        }

        public override void Refresh()
        {
            Animator.transform.localRotation = Quaternion.identity;
            Animator.transform.localPosition = Vector3.zero;

            if (Vector3.Distance(Movement.Position, Target.Target.transform.position) <= StopDistance)
            {
                Movement.Stop();
                End();
            }
            else
            {
                Movement.RunTo(Target.Target.transform.position, StopDistance);
            }

            Animator.SetFloat(AnimatorPropertyIdSpeedMove, Movement.SpeedSmoothNormalized);
        }
    }
}
