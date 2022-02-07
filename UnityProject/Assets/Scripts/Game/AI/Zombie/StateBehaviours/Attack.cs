using Core.StateMachine;
using UnityEngine;

namespace Game.AI.Behaviours.Zombie
{
    public class Attack : BehaviourBase
    {
        public int AnimatorParametrIdAttack { get; } = Animator.StringToHash("Attack");

        #region Data
#pragma warning disable 0649

        [SerializeField] private Animator _animator;
        [SerializeField] private float _timeAttack;

#pragma warning restore 0649
        #endregion

        public Animator Animator => _animator;
        public float TimeAttack => _timeAttack;

        public float TimeToEnd { get; private set; }

        public override void Begin()
        {
            Animator.transform.localRotation = Quaternion.identity;
            Animator.transform.localPosition = Vector3.zero;
            Animator.SetTrigger(AnimatorParametrIdAttack);
            TimeToEnd = TimeAttack;
        }

        public override void ForceEnd()
        {
        }

        public override void Refresh()
        {
            TimeToEnd -= Time.deltaTime;

            if (TimeToEnd <= 0)
            {
                End();
            }
        }
    }
}
