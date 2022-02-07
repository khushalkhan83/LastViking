using Game.Objectives.Data.Conditions.Static;
using System;

namespace Game.Objectives
{
    public class ConditionModel
    {
        public ConditionID ConditionID { get; }
        public IConditionDataModel ConditionDataModel { get; }
        public ConditionBaseData ConditionBaseData { get; }

        public event Action OnComplete;
        public event Action OnProgress;

        public bool IsComplete => Progress >= 1;

        public void Complete() => OnComplete?.Invoke();

        public ConditionModel(ConditionID conditionID, IConditionDataModel conditionDataModel, ConditionBaseData conditionBaseData)
        {
            ConditionID = conditionID;
            ConditionDataModel = conditionDataModel;
            ConditionBaseData = conditionBaseData;

            ConditionDataModel.OnProgress += ChangeProgress;
        }

        ~ConditionModel()
        {
            ConditionDataModel.OnProgress -= ChangeProgress;
        }

        private void ChangeProgress() => OnProgress?.Invoke();

        public float Progress
        {
            get
            {
                switch (ConditionDataModel)
                {
                    case IProgress<int> progress:
                        return (float)progress.Value / ((IProgress<int>)ConditionBaseData).Value;
                    case IProgress<bool> progress:
                        return progress.Value ? 1 : 0;
                }

                throw new Exception();
            }
        }
    }
}
