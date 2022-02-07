using Core.StateMachine;
using Game.StateMachine.Behaviours;
using Game.StateMachine.Parametrs;
using UnityEngine;

namespace Game.AI.Behaviours.Boar
{
    public class RunToTarget : BehaviourBase
    {
        public int AnimatorPropertyIdSpeedMove { get; } = Animator.StringToHash("Speed");
        public int AnimatorPropertyIdMove { get; } = Animator.StringToHash("Move");

        #region Data
#pragma warning disable 0649

        [SerializeField] private Movement _movement;
        [SerializeField] private TargetBase _target;
        [SerializeField] private Animator _animator;

        [SerializeField] private float _stopDistance;

#pragma warning restore 0649
        #endregion

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

            float dist = Vector3.Distance(GetVectorWithoutY(Movement.Position), GetVectorWithoutY(Target.Target.transform.position));

            // check slow distance
            if (dist <= StopDistance)
            {
                Movement.Stop();
                Animator.SetFloat(AnimatorPropertyIdSpeedMove, 0);
                End();
            }
            else
            {
                Movement.RunTo(Target.Target.transform.position, StopDistance);
                Animator.SetFloat(AnimatorPropertyIdSpeedMove, Movement.SpeedSmoothNormalized);
            }
        }

        private Vector3 GetVectorWithoutY(Vector3 vec)
        {
            vec.y = 0;
            return vec;
        }
    }
}
