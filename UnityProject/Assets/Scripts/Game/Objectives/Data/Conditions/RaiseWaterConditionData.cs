using UnityEngine;

namespace Game.Objectives.Data.Conditions.Static
{
    public class RaiseWaterConditionData : ConditionBaseData
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private int _targetWater;

#pragma warning restore 0649
        #endregion

        public int TargetWater => _targetWater;

        public override ConditionID ConditionID => ConditionID.RaiseWater;
    }
}
