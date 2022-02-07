using CodeStage.AntiCheat.ObscuredTypes;
using Core.StateMachine;
using Game.AI;
using Game.StateMachine.Parametrs;
using UnityEngine;

namespace Game.StateMachine.Conditions
{
    public class TargetIDCondition : ConditionBase
    {
        #region Data
#pragma warning disable 0649

        [ObscuredID(typeof(TargetID))]
        [SerializeField] private ObscuredInt _targetID;
        [SerializeField] private TargetBase _target;

#pragma warning restore 0649
        #endregion

        public TargetID TargetID => (TargetID)(int)_targetID;
        public TargetBase Target => _target;

        public override bool IsTrue =>
            (
                TargetID == TargetID.None
                && Target.Target == null
            )
            ||
            (
                Target.Target != null
                && Target.Target.ID == TargetID
            );
    }
}
