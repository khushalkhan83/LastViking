using Core.StateMachine;
using Core.States.Parametrs;
using UnityEngine;

namespace Game.StateMachine.Conditions
{
    public class IsTrueBoolParametrCondition : ConditionBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private BoolParametr _boolParametr;

#pragma warning restore 0649
        #endregion

        public BoolParametr BoolParametr => _boolParametr;

        public override bool IsTrue => BoolParametr.Bool;
    }
}
