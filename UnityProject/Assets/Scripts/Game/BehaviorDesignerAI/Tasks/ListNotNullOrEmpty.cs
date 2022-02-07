using System;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;

namespace Game.AI.BehaviorDesigner
{
    public class ListNotNullOrEmpty : Conditional
    {
        public SharedGameObjectList list;

        public override void OnStart()
        {
            base.OnStart();
        }


        public override TaskStatus OnUpdate()
        {
            bool noData = list.Value == null || list.Value.Count == 0;
            return noData ? TaskStatus.Failure : TaskStatus.Success;
        }

        public override void OnReset()
        {
            list = null;

        }
    }
}