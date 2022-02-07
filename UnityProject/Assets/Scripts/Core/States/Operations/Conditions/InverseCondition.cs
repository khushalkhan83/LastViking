using UnityEngine;

namespace Core.StateMachine.Operations.Conditions
{
    public class InverseCondition : ConditionBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private ConditionBase _condition;

#pragma warning restore 0649
        #endregion

        public ConditionBase Condition => _condition;

        public override bool IsTrue => !Condition.IsTrue;
    }
}
