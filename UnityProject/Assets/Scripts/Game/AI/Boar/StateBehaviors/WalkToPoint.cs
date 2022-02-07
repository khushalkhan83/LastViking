using Core.StateMachine;
using Core.States.Parametrs;
using Game.StateMachine.Behaviours;
using UnityEngine;

namespace Game.AI.Behaviours.Boar
{
    public class WalkToPoint : BehaviourBase
    {
        public int AnimatorPropertyIdSpeedMove { get; } = Animator.StringToHash("Speed");
        public int AnimatorPropertyIdMove { get; } = Animator.StringToHash("Move");

        #region Data
#pragma warning disable 0649

        [SerializeField] private Movement _movement;
        [SerializeField] private PositionParametr _targetPosition;
        [SerializeField] private Animator _animator;
        [SerializeField] private float _reachDistance;

#pragma warning restore 0649
        #endregion

        public Movement Movement => _movement;
        public PositionParametr TargetPosition => _targetPosition;
        public Animator Animator => _animator;
        public float ReachDistance => _reachDistance;

        public override void Begin()
        {
            Animator.transform.localRotation = Quaternion.identity;
            Animator.transform.localPosition = Vector3.zero;
            Movement.WalkTo(TargetPosition.Position);
            Animator.SetFloat(AnimatorPropertyIdSpeedMove, Movement.SpeedSmoothNormalized);
            Animator.SetTrigger(AnimatorPropertyIdMove);
        }

        public override void ForceEnd()
        {
            Movement.Stop();
            Animator.ResetTrigger(AnimatorPropertyIdMove);
            Animator.SetFloat(AnimatorPropertyIdSpeedMove, Movement.SpeedSmoothNormalized);
        }

        public override void Refresh()
        {
            Animator.transform.localRotation = Quaternion.identity;
            Animator.transform.localPosition = Vector3.zero;

            if (Vector3.Distance(Movement.Position, TargetPosition.Position) <= ReachDistance)
            {
                Movement.Stop();
                Animator.SetFloat(AnimatorPropertyIdSpeedMove, 0);
                Animator.ResetTrigger(AnimatorPropertyIdMove);
                End();
            }
            else
            {
                Movement.WalkTo(TargetPosition.Position);
                Animator.SetFloat(AnimatorPropertyIdSpeedMove, Movement.SpeedSmoothNormalized);
            }
        }
    }
}
