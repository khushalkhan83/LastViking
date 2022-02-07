using Core.StateMachine;
using Game.StateMachine.Behaviours;
using Game.StateMachine.Parametrs;
using UnityEngine;

namespace Game.AI.Behaviours.Skeleton
{
    public class RotateToTarget : BehaviourBase
    {
        public int AnimatorPropertyIdSpeedMove { get; } = Animator.StringToHash("Speed");

        #region Data
#pragma warning disable 0649

        [SerializeField] private Transform _root;
        [SerializeField] private TargetBase _target;
        [SerializeField] private Animator _animator;
        [SerializeField] private Movement _movement;
        [SerializeField] private float _speedAngle;
        [SerializeField] private float _epsilonAngle;

#pragma warning restore 0649
        #endregion

        public Transform Root => _root;
        public TargetBase Target => _target;
        public Animator Animator => _animator;
        public Movement Movement => _movement;
        public float SpeedAngle => _speedAngle;
        public float EpsilonAngle => _epsilonAngle;

        public override void Begin()
        {
            Animator.transform.localRotation = Quaternion.identity;
            Animator.transform.localPosition = Vector3.zero;
            Animator.SetFloat(AnimatorPropertyIdSpeedMove, Movement.SpeedSmoothNormalized);
        }

        public override void ForceEnd()
        {

        }

        Vector3 __to;
        Vector3 __from;
        Vector3 __forward;
        Vector3 __lookRotation;
        Quaternion __angle;
        Quaternion __lookTo;
        public override void Refresh()
        {
            Animator.SetFloat(AnimatorPropertyIdSpeedMove, Movement.SpeedSmoothNormalized);

            __to = Target.Target.transform.position;
            __from = Root.transform.position;
            __forward = Root.transform.forward;

            __from.y = 0;
            __to.y = 0;
            __forward.y = 0;

            __lookRotation = __to - __from;
            __angle = Quaternion.FromToRotation(__forward, __lookRotation);
            __lookTo = Root.rotation * __angle;
            Root.rotation = Quaternion.RotateTowards(Root.rotation, __lookTo, SpeedAngle * Time.deltaTime);

            //

            __forward = Root.transform.forward;
            __forward.y = 0;

            if (Vector3.Angle(__forward, __lookRotation) <= EpsilonAngle)
            {
                End();
            }
        }
    }
}
