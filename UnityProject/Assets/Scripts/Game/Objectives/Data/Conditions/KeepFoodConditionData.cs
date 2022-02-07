using UnityEngine;

namespace Game.Objectives.Data.Conditions.Static
{
    public class KeepFoodConditionData : ConditionBaseData, IProgress<int>
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private int _targetFood;
        [SerializeField] private int _days;

#pragma warning restore 0649
        #endregion

        public int TargetFood => _targetFood;
        public int Value => _days;

        public override ConditionID ConditionID => ConditionID.KeepFood;
    }
}
