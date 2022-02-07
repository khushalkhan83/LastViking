using System;

namespace Game.Objectives
{
    public class CountConditionDataModel : IConditionDataModel, IProgress<int>
    {
        public ConditionDataTypeID ConditionDataTypeID => ConditionDataTypeID.Int;

        public int Value { get; private set; }

        public event Action OnProgress;

        public CountConditionDataModel() { }

        public CountConditionDataModel(int count) => Value = count;

        public void Progress(int adjustment) => SetCount(Value + adjustment);

        public void SetCount(int count)
        {
            Value = count;
            OnProgress?.Invoke();
        }
    }
}
