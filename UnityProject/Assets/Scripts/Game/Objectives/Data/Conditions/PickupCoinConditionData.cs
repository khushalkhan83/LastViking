using UnityEngine;

namespace Game.Objectives.Data.Conditions.Static
{
    public class PickupCoinConditionData : ConditionBaseData, IProgress<int>
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private int _coinCount;

#pragma warning restore 0649
        #endregion

        public int Value => _coinCount;

        public override ConditionID ConditionID => ConditionID.PickupCoin;
    }
}
