using System;

namespace Game.Objectives
{
    public interface IConditionDataModel
    {
        ConditionDataTypeID ConditionDataTypeID { get; }

        event Action OnProgress;
    }
}
