using Core.StateMachine;
using Game.StateMachine.Parametrs;
using UnityEngine;

namespace Game.StateMachine.Conditions
{
    public class HasTargetCondition : ConditionBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private TargetBase _target;

#pragma warning restore 0649
        #endregion

        public override bool IsTrue => _target.Target != null;
    }
}
