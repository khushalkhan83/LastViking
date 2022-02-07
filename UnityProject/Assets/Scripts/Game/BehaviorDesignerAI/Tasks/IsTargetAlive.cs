using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Game.Models;

namespace Game.AI.BehaviorDesigner
{
    public class IsTargetAlive : Conditional
    {
        public SharedGameObject objectToCheck;

        public override TaskStatus OnUpdate()
        {
            IHealth healthModel = objectToCheck.Value.GetComponentInParent<IHealth>();
            if (healthModel != null && !healthModel.IsDead)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }
    }
}
