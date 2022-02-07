using System.Linq;
using System;

namespace Game.Objectives
{
    public class ObjectiveModel
    {
        public ObjectiveID ObjectiveID { get; }
        public ObjectiveData ObjectiveData { get; }
        public ConditionModel[] Conditions { get; private set;}
        public bool CanReroll {get; }

        public delegate void ObjectiveAction(ObjectiveModel objectiveModel);

        public event ObjectiveAction OnComplete;
        public event ObjectiveAction OnProgress;
        public event ObjectiveAction OnBegin;
        public event ObjectiveAction OnEnd;
        public event ObjectiveAction OnReward;

        public bool IsComplete => Conditions.All(x => x.IsComplete);
        public float Progress => Conditions.Sum(x => x.Progress) / Conditions.Length;

        public int GetAllInt => Conditions.Select(x => x.ConditionBaseData).OfType<IProgress<int>>().Sum(x => x.Value);
        public int GetCurrentInt => Conditions.Select(x => x.ConditionDataModel).OfType<IProgress<int>>().Sum(x => x.Value);

        public void ResetConditions(ConditionModel[] conditions)
        {
            UnSubscribeConditions();
            Conditions = conditions;
            SubscribeConditions();
        }

        public ObjectiveModel(ObjectiveID objectiveID, ObjectiveData objectiveData, ConditionModel[] conditions, bool canReroll)
        {
            ObjectiveID = objectiveID;
            ObjectiveData = objectiveData;
            Conditions = conditions;
            CanReroll = canReroll;

            SubscribeConditions();
        }

        ~ObjectiveModel()
        {
            UnSubscribeConditions();
        }

        private void SubscribeConditions()
        {
            foreach (var condition in Conditions)
            {
                condition.OnComplete += OnCompleteConditionHandler;
                condition.OnProgress += OnProgressConditionHandler;
            }
        }

        private void UnSubscribeConditions()
        {
            foreach (var condition in Conditions)
            {
                condition.OnComplete -= OnCompleteConditionHandler;
                condition.OnProgress -= OnProgressConditionHandler;
            }
        }

        private void OnCompleteConditionHandler()
        {
            if (IsComplete)
            {
                OnComplete?.Invoke(this);
            }
        }

        private void OnProgressConditionHandler() => OnProgress?.Invoke(this);
        public void Begin() => OnBegin?.Invoke(this);
        public void End() => OnEnd?.Invoke(this);
        public void Reward() => OnReward?.Invoke(this);
    }
}
