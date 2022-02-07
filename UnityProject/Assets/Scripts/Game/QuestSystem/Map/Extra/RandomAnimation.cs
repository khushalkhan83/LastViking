using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.QuestSystem.Map.Extra
{
    public class RandomAnimation : StateMachineBehaviour
    {
        [SerializeField] private string triggerTransitionName = default;
        [SerializeField] private int randomAnimationCount = default;
        [SerializeField] private float delayMinTime = default;
        [SerializeField] private float delayMaxTime = default;

        private float startTransitionTime;
        private float timeDelay;
        private bool isWaitingTransition = false;


        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            StartNextAnimationTransition(animator);
        }

        private void StartNextAnimationTransition(Animator animator)
        {
            startTransitionTime = Time.time;
            timeDelay = Random.Range(delayMinTime, delayMaxTime);
            isWaitingTransition = true;    
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            WaitingTransitionUpdate(animator);
        }

        private void WaitingTransitionUpdate(Animator animator)
        {
            if(isWaitingTransition && Time.time - startTransitionTime >= timeDelay)
            {
                int index = Random.Range(0, randomAnimationCount);
                animator.SetTrigger(triggerTransitionName + index);
                isWaitingTransition = false;
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

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
