using Game.Models;
using Game.Models.AI;
using System.Collections.Generic;
using System.Linq;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;
using Game.VillageBuilding;

namespace Game.AI.BehaviorDesigner
{
    [TaskCategory("Movement")]
    public class GetClosestTarget : Conditional
    {
        public SharedGameObject self;
        public SharedGameObject returnedObject;
        public TargetID targetID = TargetID.Obstacle;

        private TargetsModel TargetsModel => ModelsSystem.Instance._targetsModel;

        private NavMeshAgent navMeshAgent;

        public override void OnStart()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }
        public override TaskStatus OnUpdate()
        {
            if(self == null)
            {
                returnedObject.Value = null;
                return TaskStatus.Failure;
            }

            var target = GetClosest(TargetsModel.TownHallPartTargets, TargetsModel.Towers, TargetsModel.Walls, self.Value.transform);

            if(target == null )
            {
                returnedObject.Value = null;
                return TaskStatus.Failure;
            }
            else
            {
                returnedObject.Value = target.gameObject;
                return TaskStatus.Success;
            }
        }

       
        private Target GetClosest(List<Target> townHallPartTargets, List<DestroyableBuilding> towersTargets, List<DestroyableBuilding> wallsTargets, Transform mainPosition)
        {
            if(townHallPartTargets == null || townHallPartTargets.Count == 0)
            {
                return null;
            }

            float sqrShortestDistance = float.MaxValue;
            Target answer = null;
            Target target;


            for(int i = 0; i < townHallPartTargets.Count; i++)
            {
                target = townHallPartTargets[i];
                float sqrDistance = (target.transform.position - mainPosition.position).sqrMagnitude;
                if(sqrDistance < sqrShortestDistance)
                {
                    sqrShortestDistance = sqrDistance;
                    answer = target;
                }
            }

            foreach(var towerTarget in towersTargets)
            {
                if(towerTarget.BuildingHealthModel.IsDead)
                    continue;

                float sqrDistance = (towerTarget.transform.position - mainPosition.position).sqrMagnitude;
                if(sqrDistance < sqrShortestDistance)
                {
                    sqrShortestDistance = sqrDistance;
                    answer = towerTarget.Target;
                }
            }

            NavMeshPath path = new NavMeshPath();
            navMeshAgent.CalculatePath(answer.transform.position, path);

            if(path.status != NavMeshPathStatus.PathComplete)
            {
                Vector3 lastPathPoint;
                if(path.corners.Length > 0)
                {
                    lastPathPoint = path.corners[path.corners.Length - 1];
                }
                else
                {
                    lastPathPoint = transform.position;
                }

                sqrShortestDistance = float.MaxValue;
                if(wallsTargets != null)
                {
                    for(int i = 0; i < wallsTargets.Count; i++)
                    {
                        if(wallsTargets[i].BuildingHealthModel.IsDead)
                            continue;

                        target = wallsTargets[i].Target;
                        float sqrDistance = (target.transform.position - lastPathPoint).sqrMagnitude;
                        if(sqrDistance < sqrShortestDistance)
                        {
                            sqrShortestDistance = sqrDistance;
                            answer = target;
                        }
                    }
                }
            }

            return answer;
        }      
    }
}
