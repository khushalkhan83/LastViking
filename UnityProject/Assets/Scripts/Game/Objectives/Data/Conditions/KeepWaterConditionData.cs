using UnityEngine;

namespace Game.Objectives.Data.Conditions.Static
{
    public class KeepWaterConditionData : ConditionBaseData, IProgress<int>
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private int _targetWater;
        [SerializeField] private int _days;

#pragma warning restore 0649
        #endregion

        public int TargetWater => _targetWater;
        public int Value => _days;

        public override ConditionID ConditionID => ConditionID.KeepWater;
    }
}
