using BehaviorDesigner.Runtime.Tasks;
using Game.Models;
using Game.Models.AI;

namespace Game.AI.BehaviorDesigner
{
    [TaskCategory("Movement")]
    public class TargetExist : Conditional
    {
        private TargetsModel TargetsModel => ModelsSystem.Instance._targetsModel;

        public override TaskStatus OnUpdate()
        {
            if(TargetsModel.HasTargetsForAttack)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }   
    }
}
