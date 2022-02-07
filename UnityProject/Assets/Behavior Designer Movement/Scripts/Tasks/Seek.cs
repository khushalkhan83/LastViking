using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    [TaskDescription("Seek the target specified using the Unity NavMesh.")]
    [TaskCategory("Movement")]
    [HelpURL("https://www.opsive.com/support/documentation/behavior-designer-movement-pack/")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}SeekIcon.png")]
    public class Seek : NavMeshMovement
    {
        [Tooltip("The GameObject that the agent is seeking")]
        public SharedGameObject target;
        [Tooltip("If target is null then use the target position")]
        public SharedVector3 targetPosition;

        private Vector3 targetPoint;

        public override void OnStart()
        {
            base.OnStart();
            targetPoint = Target();
            SetDestination(targetPoint);
        }

        // Seek the destination. Return success once the agent has reached the destination.
        // Return running if the agent hasn't reached the destination yet
        public override TaskStatus OnUpdate()
        {
            if (HasArrived()) {
                return TaskStatus.Success;
            }

            Vector3 newTargetPoint = Target();
            if(IsTargePointChanged(newTargetPoint))
            {
                targetPoint = newTargetPoint;
                SetDestination(targetPoint);
            }

            return TaskStatus.Running;
        }
        
        // Return targetPosition if target is null
        private Vector3 Target()
        {
            if (target.Value != null) {
                return target.Value.transform.position;
            }
            return targetPosition.Value;
        }

        public override void OnReset()
        {
            base.OnReset();
            target = null;
            targetPosition = Vector3.zero;
        }

        private bool IsTargePointChanged(Vector3 newTargetPoint)
        {
            return Mathf.Abs(targetPoint.x - newTargetPoint.x) > 0.01f || Mathf.Abs(targetPoint.z - newTargetPoint.z) > 0.01f;
        }
    }
}