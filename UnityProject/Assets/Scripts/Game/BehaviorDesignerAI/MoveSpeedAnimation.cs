using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Game.AI.BehaviorDesigner
{
    public class MoveSpeedAnimation : MonoBehaviour
    {
        public int AnimatorPropertyIdSpeedMove { get; } = Animator.StringToHash("Speed");

        #region Data
    #pragma warning disable 0649
        [SerializeField] private NavMeshAgent _navMeshAgent;
        [SerializeField] private Animator _animator;

    #pragma warning restore 0649
        #endregion

        public Animator Animator => _animator;
        public NavMeshAgent NavMeshAgent => _navMeshAgent;


        private float speed = 0;


        private void Update() 
        {
            float newSpeed = Vector3.Project(NavMeshAgent.desiredVelocity, transform.forward).magnitude;
            if(speed != newSpeed){
                speed = newSpeed;
                Animator.SetFloat(AnimatorPropertyIdSpeedMove, speed);
            }
        }

    }
}
