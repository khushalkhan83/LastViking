using System.Collections.Generic;
using UnityEngine;

namespace Game.AI.Behaviours.Kraken.StageMachine.AnimatorBased
{
    public class Refresh : StateMachineBehaviour
    {
        [SerializeField] private List<BoolParamData> boolParams;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            boolParams.ForEach(x => animator.SetBool(x.paramName, x.value));
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

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

        [System.Serializable]
        private class ParamData<T>
        {
            public T value;
            public string paramName;
        }

        [System.Serializable]
        private class BoolParamData : ParamData<bool> { }
    }
}
