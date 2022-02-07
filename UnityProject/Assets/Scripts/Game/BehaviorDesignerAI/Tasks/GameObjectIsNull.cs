using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;
using Tooltip = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

namespace Game.AI.BehaviorDesigner
{
    [TaskCategory("Custom")]
    public class GameObjectIsNull : Conditional
    {
        public SharedGameObject objectToCheck;

        public override TaskStatus OnUpdate()
        {
            if (objectToCheck == null)
                return TaskStatus.Success;
            if (objectToCheck.Value == null)
                return TaskStatus.Success;

            if(objectToCheck.Value.activeSelf == false)
                return TaskStatus.Success;

            return TaskStatus.Failure;
        }
    }
}
