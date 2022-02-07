using UnityEngine;

namespace Game.Objectives.Data.Conditions.Static
{
    public class RaiseFoodConditionData : ConditionBaseData
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private int _targetFood;

#pragma warning restore 0649
        #endregion

        public int TargetFood => _targetFood;

        public override ConditionID ConditionID => ConditionID.RaiseFood;
    }
}
