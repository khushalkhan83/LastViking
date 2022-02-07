using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;
using Tooltip = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

namespace Game.AI.BehaviorDesigner
{
    [TaskCategory("Movement")]
    public class CanReachTarget : Conditional
    {
        public SharedGameObject targetObject;
        public SharedFloat reachDistance = 1;

        private NavMeshAgent navMeshAgent;
        
        public override void OnStart()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }
        public override TaskStatus OnUpdate()
        {
            if(navMeshAgent == null)
            {
                return TaskStatus.Failure;
            }

            NavMeshPath path = new NavMeshPath();
            if(targetObject.Value == null)
                return TaskStatus.Failure;

            navMeshAgent.CalculatePath(targetObject.Value.transform.position, path);

            if(path.status == NavMeshPathStatus.PathComplete)
            {
                return TaskStatus.Success;
            }
            else if(path.status == NavMeshPathStatus.PathPartial)
            {
                Vector3 lastPoint = path.corners[path.corners.Length - 1];

                if((lastPoint - targetObject.Value.transform.position).magnitude <= reachDistance.Value)
                {
                    return TaskStatus.Success;
                }
            }

            return TaskStatus.Failure;
        }

        public override void OnReset()
        {
            targetObject = null;
            reachDistance = 1;
        }
    }
}
