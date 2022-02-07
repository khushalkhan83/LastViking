using Core.StateMachine;
using Game.Audio;
using Game.StateMachine.Behaviours;
using Game.StateMachine.Parametrs;
using UnityEngine;

namespace Game.AI.Behaviours.Zombie
{
    public class RunToTransform : BehaviourBase
    {
        public int AnimatorPropertyIdSpeedMove { get; } = Animator.StringToHash("Speed");
        public int AnimatorPropertyIdMove { get; } = Animator.StringToHash("Move");

        #region Data
#pragma warning disable 0649

        [SerializeField] private Movement _movement;
        [SerializeField] private TransformProvider _target;
        [SerializeField] private Animator _animator;
        [SerializeField] private OnceAudioThread _onceAudioThread;

        [SerializeField] private float _stopDistance;
        [SerializeField] private float _stepSize;

#pragma warning restore 0649
        #endregion
        public AudioSystem AudioSystem => AudioSystem.Instance;

        public Movement Movement => _movement;
        public TransformProvider Target => _target;
        public Animator Animator => _animator;
        public OnceAudioThread OnceAudioThread => _onceAudioThread;
        public float StopDistance => _stopDistance;
        public float StepSize => _stepSize;

        public GameObject GameObject;

        private Vector3 CurrentPosition { get; set; }

        public override void Begin()
        {
            CurrentPosition = Movement.Position;
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

            if (Vector3.Distance(Movement.Position, CurrentPosition) > StepSize)
            {
                var audioIdentifier = GameObject.GetComponent<AudioIdentifier>();
                OnceAudioThread.PlayOnce(audioIdentifier.AudioID[Random.Range(0, audioIdentifier.AudioID.Length)]);
                CurrentPosition = Movement.Position;
            }
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
