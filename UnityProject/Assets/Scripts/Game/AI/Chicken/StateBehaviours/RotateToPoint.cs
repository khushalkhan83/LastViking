using CodeStage.AntiCheat.ObscuredTypes;
using Core.StateMachine;
using Core.States.Parametrs;
using Game.StateMachine.Behaviours;
using UnityEngine;

namespace Game.AI.Behaviours.Chicken
{
    public class RotateToPoint : BehaviourBase
    {
        public int AnimatorPropertyIdSpeedMove { get; } = Animator.StringToHash("Speed");
        public int AnimatorPropertyIdMove { get; } = Animator.StringToHash("Move");
        #region Data
#pragma warning disable 0649

        [SerializeField] private Transform _root;
        [SerializeField] private PositionParametr _targetPosition;
        [SerializeField] private Animator _animator;
        [SerializeField] private Movement _movement;
        [SerializeField] private ObscuredFloat _speedAngle;
        [SerializeField] private ObscuredFloat _moveSpeedRotate = 0.25f;
        [SerializeField] private ObscuredFloat _epsilonAngle;

#pragma warning restore 0649
        #endregion

        public Transform Root => _root;
        public PositionParametr TargetPosition => _targetPosition;
        public Animator Animator => _animator;
        public Movement Movement => _movement;
        public ObscuredFloat SpeedAngle => _speedAngle;
        public ObscuredFloat MoveSpeedRotate => _moveSpeedRotate;
        public ObscuredFloat EpsilonAngle => _epsilonAngle;

        public override void Begin()
        {
            Animator.transform.localRotation = Quaternion.identity;
            Animator.transform.localPosition = Vector3.zero;
            Animator.SetTrigger(AnimatorPropertyIdMove);
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
            __to = TargetPosition.Position;
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

            Animator.SetFloat(AnimatorPropertyIdSpeedMove, MoveSpeedRotate);

            if (Vector3.Angle(__forward, __lookRotation) <= EpsilonAngle)
            {
                End();
            }
        }
    }
}
