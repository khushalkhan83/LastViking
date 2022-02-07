using UnityEngine;

namespace Game.Objectives.Data.Conditions.Static
{
    public class PickupItemConditionData : ConditionBaseData, IProgress<int>
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private string _itemName;
        [SerializeField] private int _pickupCount;

#pragma warning restore 0649
        #endregion

        public string ItemName => _itemName;
        public int Value => _pickupCount;

        public override ConditionID ConditionID => ConditionID.PickupItem;
    }
}
