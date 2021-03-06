using System.Collections;
using System.Collections.Generic;
using Game.Models;
using UnityEngine;

namespace Game.AI.Behaviours.Kraken.StageMachine.AnimatorBased
{
    public class CheckHealth : StateMachineBehaviour
    {
        [SerializeField] private string _animatorBoolName = "IsDead";
        private IHealth _health;


        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            if (_health == null)
                _health = animator.GetComponentInChildren<IHealth>();

        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator,stateInfo,layerIndex);
            animator.SetBool(_animatorBoolName, _health.IsDead);
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        // override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        // {

        // }

        // OnStateMove is called right after Animator.OnAnimatorMove()
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that processes and affects root motion
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK()
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that sets up animation IK (inverse kinematics)
        //}
    }
}