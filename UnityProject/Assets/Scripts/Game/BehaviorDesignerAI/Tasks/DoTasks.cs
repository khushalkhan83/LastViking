using UnityEngine;
using System.Linq;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Game.Models;

namespace Game.AI.BehaviorDesigner
{
    public class DoTasks : Conditional
    {
        public SharedGameObjectList list;
        public SharedGameObject root;
        public SharedBool loopTasks;

        private NPCTask activeTask;
        private float timeLeft;
        private bool forceTaskCompleate;

        private EnvironmentTimeModel EnvironmentTimeModel => ModelsSystem.Instance._environmentTimeModel;

        #region Task
        public override void OnStart()
        {
            base.OnStart();

            activeTask = list.Value.FirstOrDefault().GetComponent<NPCTask>();

            activeTask.Do(root.Value.GetComponent<NPCContext>());
            SetEndTaskTime();

            UnSubscribe();
            Subscribe();
        }

        public override TaskStatus OnUpdate()
        {
            if(forceTaskCompleate)
            {
                forceTaskCompleate = false;
                HandleCompleatedTask();
                return TaskStatus.Success;
            }

            if(activeTask.Endless) 
                return TaskStatus.Running;

            if(TaskEnded())
            {
                HandleCompleatedTask();
                return TaskStatus.Success;
            }
            else return TaskStatus.Running;
        }

        public override void OnReset()
        {
            list = null;
            root = null;
            activeTask = null;
            timeLeft = 0;
            forceTaskCompleate = false;
            UnSubscribe();
        }
            
        #endregion

        private void Subscribe()
        {
            EnvironmentTimeModel.OnDayTimeChanged += OnDayTimeChanged;
        }

        private void UnSubscribe()
        {
            EnvironmentTimeModel.OnDayTimeChanged -= OnDayTimeChanged;
        }

        private void OnDayTimeChanged()
        {
            if(activeTask == null) return;

            if(activeTask.CompleateOnDayTimeChanged)

            forceTaskCompleate = true;
        }

        private void HandleCompleatedTask()
        {
            activeTask.Finish();
            
            list.Value.Remove(activeTask.gameObject);

            if(loopTasks.Value == true)
            {
                list.Value.Add(activeTask.gameObject);
            }
        }
        
        private void SetEndTaskTime()
        {
            timeLeft = activeTask.Duration;
        }

        private bool TaskEnded()
        {
            timeLeft -= Time.deltaTime;
            return timeLeft <= 0;
        }
    }
}