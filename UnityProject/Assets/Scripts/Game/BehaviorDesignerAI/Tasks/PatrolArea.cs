using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;
using UnityEngine;
using UnityEngine.AI;
using Tooltip = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

namespace Game.AI.BehaviorDesigner
    {
    [TaskDescription("Patrol around in the specified area using the Unity NavMesh.")]
    [TaskCategory("Movement")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}PatrolIcon.png")]
    public class PatrolArea : NavMeshMovement
    {
        [Tooltip("The length of time that the agent should pause when arriving at a waypoint")]
        public SharedFloat waypointPauseDuration = 0;
        
        [Tooltip("The length of time that the agent should pause when arriving at a waypoint")]
        public SharedBool patrolForever = false;

        [Tooltip("The length of time that the agent should pause when arriving at a waypoint")]
        public SharedInt patrolWaypointsCount = 0;
        public SharedGameObject areaCenter;
        public SharedFloat areaRadius;

        // The current index that we are heading towards within the waypoints array
        private Vector3? currentWaypoint = null; 
        private float waypointReachedTime;
        private int waypointsReached = 0;

        public override void OnStart()
        {
            base.OnStart();
            currentWaypoint = null;
            waypointsReached = 0;
            waypointReachedTime = -1;
        }

        // Patrol around the different waypoints specified in the waypoint array. Always return a task status of running. 
        public override TaskStatus OnUpdate()
        {
            if (currentWaypoint == null) 
            {
                currentWaypoint = GetRandomWaypoint();
                SetDestination((Vector3)currentWaypoint);
            }
            if (HasArrived()) 
            {
                if(!patrolForever.Value)
                {
                    waypointsReached++;
                    if(waypointsReached >= patrolWaypointsCount.Value)
                    {
                        return TaskStatus.Success;
                    }
                }

                if (waypointReachedTime == -1) {
                    waypointReachedTime = Time.time;
                }
                // wait the required duration before switching waypoints.
                if (waypointReachedTime + waypointPauseDuration.Value <= Time.time) 
                {
                    currentWaypoint = GetRandomWaypoint();
                    SetDestination((Vector3)currentWaypoint);
                    waypointReachedTime = -1;
                }
            }

            return TaskStatus.Running;
        }
        
        public override void OnReset()
        {
            base.OnReset();
            waypointPauseDuration = 0;
            patrolForever = false;
            patrolWaypointsCount = 0;
        }

        private Vector3 GetRandomWaypoint()
        {
            var position = areaCenter.Value.transform.position + Random.insideUnitSphere.normalized * Random.Range(0.0f, areaRadius.Value);

            if (NavMesh.SamplePosition(position, out var hit, areaRadius.Value * 1.2f, NavMesh.AllAreas))
            {   
                NavMeshPath path = new NavMeshPath();
                navMeshAgent.CalculatePath(hit.position, path);
                if(path.status == NavMeshPathStatus.PathComplete)
                {
                    return hit.position;
                }
            }
            return areaCenter.Value.transform.position;
        }

        public override void OnDrawGizmos()
        {
    #if UNITY_EDITOR
            var oldColor = UnityEditor.Handles.color;
            if(areaCenter != null)
            {
                UnityEditor.Handles.color = Color.yellow;
                UnityEditor.Handles.DrawWireDisc(areaCenter.Value.transform.position, Owner.transform.up, areaRadius.Value);
            }

            if(currentWaypoint != null)
            {
                UnityEditor.Handles.color = Color.yellow;
                UnityEditor.Handles.SphereHandleCap(0, (Vector3)currentWaypoint, Quaternion.identity, 0.5f, EventType.Repaint);
            }
            UnityEditor.Handles.color = oldColor;
    #endif
        }

    }
}
