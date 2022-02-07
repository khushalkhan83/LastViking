using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using Tooltip = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

namespace Game.AI.BehaviorDesigner
{
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}MoveTowardsIcon.png")]
    public class SprintAttackAction : Action
    {
        [Tooltip("The speed of the agent")]
        public SharedFloat speed;
        [Tooltip("Attack will be stopped after this time")]
        public SharedFloat attackTime = 1f;
         [Tooltip("The agent has arrived when the magnitude is less than this value")]
        public SharedFloat arriveDistance = 0.1f;
        [Tooltip("Should the agent be looking at the target position?")]
        public SharedBool lookAtTarget = true;
        [Tooltip("Max rotation delta if lookAtTarget is enabled")]
        public SharedFloat maxLookAtRotationDelta;
        [Tooltip("The GameObject that the agent is moving towards")]
        public SharedGameObject target;
        [Tooltip("If target is null then use the target position")]
        public SharedVector3 targetPosition;

        private Vector3 position;
        private float startTime;
        private float pauseTime;

        public override void OnStart()
        {
            base.OnStart();
            position = Target();
            startTime = Time.time;
            Owner.SendEvent("StartSprintAttack");
        }

        public override TaskStatus OnUpdate()
        {
            // Return a task status of success once we've reached the target
            if (Vector3.Magnitude(transform.position - position) < arriveDistance.Value) {
                return TaskStatus.Success;
            }

            if (startTime + attackTime.Value < Time.time) {
                return TaskStatus.Success;
            }

            // We haven't reached the target yet so keep moving towards it
            transform.position = Vector3.MoveTowards(transform.position, position, speed.Value * Time.deltaTime);
            if (lookAtTarget.Value && (position - transform.position).sqrMagnitude > 0.01f) {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(position - transform.position), maxLookAtRotationDelta.Value);
            }
            
            return TaskStatus.Running;
        }
        
        public override void OnEnd()
        {
            base.OnEnd();
            Owner.SendEvent("StopSprintAttack");
        }

        public override void OnPause(bool paused)
        {
            if (paused) {
                // Remember the time that the behavior was paused.
                pauseTime = Time.time;
            } else {
                // Add the difference between Time.time and pauseTime to figure out a new start time.
                startTime += (Time.time - pauseTime);
            }
        }


        // Return targetPosition if targetTransform is null
        private Vector3 Target()
        {
            if (target == null || target.Value == null) {
                return targetPosition.Value;
            }
            return target.Value.transform.position;
        }

        // Reset the public variables
        public override void OnReset()
        {
            attackTime = 1f;
            arriveDistance = 0.1f;
            lookAtTarget = true;
        }
    }
}
