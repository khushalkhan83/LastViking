using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using Tooltip = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

namespace Game.AI.BehaviorDesigner
{
    [TaskCategory("Unity/Animator")]
    [TaskDescription("Sets a trigger parameter to active or inactive. Returns Success.")]
    public class SetRandomTrigger : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The name of the parameter")]
        public SharedString[] paramaterNames;

        private Animator animator;
        private GameObject prevGameObject;

        public override void OnStart()
        {
            var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
            if (currentGameObject != prevGameObject) {
                animator = currentGameObject.GetComponent<Animator>();
                prevGameObject = currentGameObject;
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (animator == null) {
                Debug.LogWarning("Animator is null");
                return TaskStatus.Failure;
            }

            animator.SetTrigger(paramaterNames[Random.Range(0, paramaterNames.Length)].Value);

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            paramaterNames = null;
        }
    }
}
