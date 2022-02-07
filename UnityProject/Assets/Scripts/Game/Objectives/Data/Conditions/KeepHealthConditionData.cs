using UnityEngine;

namespace Game.Objectives.Data.Conditions.Static
{
    public class KeepHealthConditionData : ConditionBaseData, IProgress<int>
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private int _targetHealth;
        [SerializeField] private int _days;

#pragma warning restore 0649
        #endregion

        public int TargetHealth => _targetHealth;
        public int Value => _days;

        public override ConditionID ConditionID => ConditionID.KeepHealth;
    }
}
