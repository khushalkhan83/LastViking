using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;
using UnityEngine;
using UnityEngine.AI;
using Tooltip = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;
using System.Collections.Generic;

namespace Game.AI.BehaviorDesigner
{
    [TaskDescription("Check to see if the any obstacle is within the distance specified of the current agent.")]
    [TaskCategory("Movement")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}WithinDistanceIcon.png")]
    public class ObstacleWithinDistance : Conditional
    {
        [Tooltip("The LayerMask of the objects that we are searching for")]
        public LayerMask objectLayerMask;
        [Tooltip("The distance that the object needs to be within")]
        public SharedFloat magnitude = 5;
        [Tooltip("If true, the object must be within line of sight to be within distance. For example, if this option is enabled then an object behind a wall will not be within distance even though it may " +
                 "be physically close to the other object")]
        public SharedBool lineOfSight;
        [Tooltip("The LayerMask of the objects to ignore when performing the line of sight check")]
        public LayerMask ignoreLayerMask = 1 << LayerMask.NameToLayer("Ignore Raycast");
        [Tooltip("The raycast offset relative to the pivot position")]
        public SharedVector3 offset;
        [Tooltip("The target raycast offset relative to the pivot position")]
        public SharedVector3 targetOffset;
        [Tooltip("The object variable that will be set when a object is found what the object is")]
        public SharedGameObject returnedObject;

        private Collider[] colliders = new Collider[10];
        private int collidersCount;

        // distance * distance, optimization so we don't have to take the square root
        private float sqrMagnitude;
        Vector3 direction;

        public override void OnStart()
        {
            sqrMagnitude = magnitude.Value * magnitude.Value;
        }

        // returns success if any object is within distance of the current object. Otherwise it will return failure
        public override TaskStatus OnUpdate()
        {
            if (transform == null)
                return TaskStatus.Failure;

            collidersCount = Physics.OverlapSphereNonAlloc(transform.position, magnitude.Value, colliders, objectLayerMask.value);
            if (collidersCount > 0)
            {
                float closestDistace = float.MaxValue;
                GameObject closestObject = null;
                for(int i = 0; i < collidersCount; i++)
                {
                    if(IsWithinDistance(colliders[i].gameObject, out float distanceSqr))
                    {
                        if(distanceSqr < closestDistace)
                        {
                            closestDistace = distanceSqr;
                            closestObject = colliders[i].gameObject;
                        }
                    }
                }
                
                if(closestObject != null)
                {
                    var target = closestObject.GetComponentInParent<Target>();
                    if(target != null)
                    {
                        returnedObject.Value = target.gameObject;
                        return TaskStatus.Success;
                    }
                }
            }

            return TaskStatus.Failure;
        }

        private bool IsWithinDistance(GameObject gameObject, out float distanceSqr)
        {
            direction = gameObject.transform.position - (transform.position + offset.Value);
            distanceSqr = Vector3.SqrMagnitude(direction);
            if (distanceSqr < sqrMagnitude)
            {
                if (lineOfSight.Value)
                {
                    if (MovementUtility.LineOfSight(transform, offset.Value, gameObject, targetOffset.Value, false, ignoreLayerMask.value))
                    {
                        return true;
                    }
                }
                return true;
            }
            return false;
        }
        public override void OnReset()
        {
            objectLayerMask = 0;
            magnitude = 5;
            lineOfSight = true;
            ignoreLayerMask = 1 << LayerMask.NameToLayer("Ignore Raycast");
            offset = Vector3.zero;
            targetOffset = Vector3.zero;
        }

        // Draw the seeing radius
        public override void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (Owner == null || magnitude == null) {
                return;
            }
            var oldColor = UnityEditor.Handles.color;
            UnityEditor.Handles.color = Color.yellow;
            UnityEditor.Handles.DrawWireDisc(Owner.transform.position, Owner.transform.up, magnitude.Value);
            UnityEditor.Handles.color = oldColor;
#endif
        }
    }
}
