using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    [TaskDescription("Flee from the target specified using the Unity NavMesh.")]
    [TaskCategory("Movement")]
    [HelpURL("https://www.opsive.com/support/documentation/behavior-designer-movement-pack/")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}FleeIcon.png")]
    public class Flee : NavMeshMovement
    {
        [Tooltip("The agent has fleed when the magnitude is greater than this value")]
        public SharedFloat fleedDistance = 20;
        [Tooltip("Minimum flee time")]
        public SharedFloat fleedTime = 2;
        [Tooltip("The distance to look ahead when fleeing")]
        public SharedFloat lookAheadDistance = 5;
        [Tooltip("Find random point in this radius if can't find point ahead")]
        public SharedFloat randomPointRadius = 8;
        [Tooltip("The GameObject that the agent is fleeing from")]
        public SharedGameObject target;

        private bool hasMoved;
        private float startFleeTime;

        public override void OnStart()
        {
            base.OnStart();

            hasMoved = false;
            startFleeTime = Time.time;

            SetDestination(Target());
        }

        // Flee from the target. Return success once the agent has fleed the target by moving far enough away from it
        // Return running if the agent is still fleeing
        public override TaskStatus OnUpdate()
        {
            if (Time.time - startFleeTime > fleedTime.Value
                && Vector3.Magnitude(transform.position - target.Value.transform.position) > fleedDistance.Value) 
            {
                return TaskStatus.Success;
            }

            if (HasArrived()) {
                if (!hasMoved) {
                    return TaskStatus.Failure;
                }
                if (!SetDestination(Target())) {
                    return TaskStatus.Failure;
                }
                hasMoved = false;
            } else {
                // If the agent is stuck the task shouldn't continue to return a status of running.
                var velocityMagnitude = Velocity().sqrMagnitude;
                if (hasMoved && velocityMagnitude <= 0f) {
                    return TaskStatus.Failure;
                }
                hasMoved = velocityMagnitude > 0f;
            }

            return TaskStatus.Running;
        }

        // Flee in the opposite direction
        private Vector3 Target()
        {
            Vector3 forwardPoint = transform.position + (transform.position - target.Value.transform.position).normalized * lookAheadDistance.Value;
            if(SamplePosition(forwardPoint) && CanReachPoint(forwardPoint))
            {
                return forwardPoint;
            }

            return GetRandomFleePoint();
        }

        private bool CanReachPoint(Vector3 point)
        {
            var path = new NavMeshPath();
            if(NavMesh.CalculatePath(transform.position, point, NavMesh.AllAreas, path))
            {
                return path.status == NavMeshPathStatus.PathComplete;
            }
            return false;
        }

        private Vector3 GetRandomFleePoint()
        {
            var position = transform.position + Random.insideUnitSphere.normalized * Random.Range(0.0f, randomPointRadius.Value);
            if (NavMesh.SamplePosition(position, out var hit, randomPointRadius.Value * 1.2f, NavMesh.AllAreas))
            {
                return hit.position;
            }
            return transform.position;
        }


        // Return false if the position isn't valid on the NavMesh.
        protected override bool SetDestination(Vector3 destination)
        {
            if (!SamplePosition(destination)) {
                return false;
            }
            return base.SetDestination(destination);
        }

        // Reset the public variables
        public override void OnReset()
        {
            base.OnReset();

            fleedDistance = 20;
            lookAheadDistance = 5;
            target = null;
        }
    }
}