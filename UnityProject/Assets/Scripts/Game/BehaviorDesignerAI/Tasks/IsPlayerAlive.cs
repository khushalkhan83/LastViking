using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Game.Models;
using UnityEngine;
using UnityEngine.AI;
using Tooltip = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

namespace Game.AI.BehaviorDesigner
{
    public class IsPlayerAlive : Conditional
    {
        private IHealth PlayerHealth => ModelsSystem.Instance._playerHealthModel;

        public override TaskStatus OnUpdate()
        {
            if (PlayerHealth.Health > 0)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }
    }
}
