using System.Linq;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Game.Models;

namespace Game.AI.BehaviorDesigner
{
    public class SetNextTaskPoint : Conditional
    {
        public SharedGameObjectList dayTasks;
        public SharedGameObjectList nightTasks;
        public SharedGameObjectList activeTasks;
        public SharedGameObject nextTaskPoint;

        private EnvironmentTimeModel EnvironmentTimeModel => ModelsSystem.Instance._environmentTimeModel;

        public override void OnStart()
        {
            base.OnStart();
        }

        public override TaskStatus OnUpdate()
        {
            activeTasks.Value = EnvironmentTimeModel.IsDayTime ? dayTasks.Value: nightTasks.Value;

            nextTaskPoint.Value = activeTasks.Value.FirstOrDefault();
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            dayTasks = null;
            nightTasks = null;
            activeTasks = null;
            nextTaskPoint = null;
        }
    }
}