using Core.StateMachine;
using Core.States.Parametrs;
using Game.Audio;
using Game.StateMachine.Behaviours;
using UnityEngine;

namespace Game.AI.Behaviours.Chicken
{
    public class RunOffPlayer : BehaviourBase
    {
        public int AnimatorPropertyIdMove { get; } = Animator.StringToHash("Move");
        public int AnimatorPropertyIdSpeedMove { get; } = Animator.StringToHash("Speed");

        #region Data
#pragma warning disable 0649

        [SerializeField] private Movement _movement;
        [SerializeField] private PositionParametr _targetPosition;
        [SerializeField] private Animator _animator;
        [SerializeField] private float _reachDistance;
        [SerializeField] private AudioID _audioID;
        [SerializeField] private OnceAudioThread _onceAudioThread;

#pragma warning restore 0649
        #endregion

        public Movement Movement => _movement;
        public PositionParametr TargetPosition => _targetPosition;
        public Animator Animator => _animator;
        public float ReachDistance => _reachDistance;
        public AudioID AudioID => _audioID;
        public OnceAudioThread OnceAudioThread => _onceAudioThread;

        public override void Begin()
        {
            Animator.transform.localRotation = Quaternion.identity;
            Animator.transform.localPosition = Vector3.zero;
            Movement.RunTo(TargetPosition.Position);
            Animator.SetFloat(AnimatorPropertyIdSpeedMove, Movement.SpeedSmoothNormalized);                                                                       
            Animator.SetTrigger(AnimatorPropertyIdMove);
            OnceAudioThread.PlayOnce(AudioID);
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

            if (Vector3.Distance(Movement.Position, TargetPosition.Position) <= ReachDistance)
            {
                Movement.Stop();
                Animator.SetFloat(AnimatorPropertyIdSpeedMove, 0);
                End();
            }
            else
            {
                Movement.RunTo(TargetPosition.Position);
                Animator.SetFloat(AnimatorPropertyIdSpeedMove, Movement.SpeedSmoothNormalized);
            }
        }
    }
}
