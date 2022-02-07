using UnityEngine;

namespace Game.Objectives.Data.Conditions.Static
{
    public class RaiseHealthConditionData : ConditionBaseData
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private int _targetHealth;

#pragma warning restore 0649
        #endregion

        public int TargetHealth => _targetHealth;

        public override ConditionID ConditionID => ConditionID.RaiseHealth;
    }
}
