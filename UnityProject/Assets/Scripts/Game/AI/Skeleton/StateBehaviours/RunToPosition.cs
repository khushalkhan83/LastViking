using Core.StateMachine;
using Game.Audio;
using Game.StateMachine.Behaviours;
using UnityEngine;

namespace Game.AI.Behaviours.Skeleton
{
    public class RunToPosition : BehaviourBase
    {
        public int AnimatorPropertyIdSpeedMove { get; } = Animator.StringToHash("Speed");
        public int AnimatorPropertyIdMove { get; } = Animator.StringToHash("Move");

        #region Data
#pragma warning disable 0649

        [SerializeField] private Movement _movement;
        [SerializeField] private Animator _animator;

        [SerializeField] private float _stopDistance;
        [SerializeField] private float _stepSize;
        [SerializeField] private float _updateTargetTime = 5f;
#pragma warning restore 0649
        #endregion

        public AudioSystem AudioSystem => AudioSystem.Instance;

        public Movement Movement => _movement;
        public Animator Animator => _animator;
        public float StopDistance => _stopDistance;
        public float StepSize => _stepSize;
        public float UpdateTargetTime => _updateTargetTime;
        public GameObject GameObject;

        private Vector3 CurrentPosition { get; set; }

        public Vector3 TargetPosition { get; set; }

        private float timerUpdateTarget = 0;

        public override void Begin()
        {        
	        CurrentPosition = Movement.Position;
            Animator.transform.localRotation = Quaternion.identity;       
            Animator.transform.localPosition = Vector3.zero;
	        Movement.RunTo(TargetPosition, StopDistance);
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
                AudioSystem.PlayOnce(audioIdentifier.AudioID[Random.Range(0, audioIdentifier.AudioID.Length)], transform.position);
                CurrentPosition = Movement.Position;
            }           
            if (Vector3.Distance(Movement.Position, TargetPosition) <= StopDistance)
            {
                Movement.Stop();
                Animator.SetFloat(AnimatorPropertyIdSpeedMove, 0);
                End();
            }
            else
            {
                timerUpdateTarget += Time.deltaTime;
                if (timerUpdateTarget >= UpdateTargetTime)
                {
                    Movement.RunTo(TargetPosition, StopDistance);
                    timerUpdateTarget = 0;
                }
                Animator.SetFloat(AnimatorPropertyIdSpeedMove, Movement.SpeedSmoothNormalized);
            }
        }
    }
}
