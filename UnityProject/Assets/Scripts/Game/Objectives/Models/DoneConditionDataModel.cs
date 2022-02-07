using System;

namespace Game.Objectives
{
    public class DoneConditionDataModel : IConditionDataModel, IProgress<bool>
    {
        public ConditionDataTypeID ConditionDataTypeID => ConditionDataTypeID.Bool;

        public bool Value { get; private set; }

        public event Action OnProgress;

        public DoneConditionDataModel() { }

        public DoneConditionDataModel(bool isSuccess) => Value = isSuccess;

        public void Progress(bool value)
        {
            Value = value;
            OnProgress?.Invoke();
        }
    }
}
