using CodeStage.AntiCheat.ObscuredTypes;
using Core.StateMachine;
using Game.Audio;
using Game.StateMachine.Behaviours;
using Game.StateMachine.Parametrs;
using UnityEngine;

namespace Game.AI.Behaviours.Bear
{
    public class RunToTarget : BehaviourBase
    {
        public int AnimatorPropertyIdSpeedMove { get; } = Animator.StringToHash("Speed");
        public int AnimatorPropertyIdMove { get; } = Animator.StringToHash("Move");

        #region Data
#pragma warning disable 0649

        [SerializeField] private Movement _movement;
        [SerializeField] private TargetDetection _target;
        [SerializeField] private Animator _animator;
        [SerializeField] private ObscuredFloat _reachDistance;
        [SerializeField] private AudioID _audioID;

#pragma warning restore 0649
        #endregion

        public Movement Movement => _movement;
        public TargetDetection Target => _target;
        public Animator Animator => _animator;
        public ObscuredFloat ReachDistance => _reachDistance;

        public Vector3 TargetPosition => Target.Target.transform.position;

        public override void Begin()
        {
            Animator.transform.localRotation = Quaternion.identity;
            Animator.transform.localPosition = Vector3.zero;
            Animator.SetTrigger(AnimatorPropertyIdMove);
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

            if (Vector3.Distance(Movement.Position, Target.Target.transform.position) <= ReachDistance)
            {
                Movement.Stop();
                Animator.SetFloat(AnimatorPropertyIdSpeedMove, 0);
                End();
            }
            else
            {
                MoveTo(TargetPosition);
            }
        }

        private void MoveTo(Vector3 to)
        {
            Movement.RunTo(to);
            Animator.SetFloat(AnimatorPropertyIdSpeedMove, Movement.SpeedSmoothNormalized);
        }
    }
}
