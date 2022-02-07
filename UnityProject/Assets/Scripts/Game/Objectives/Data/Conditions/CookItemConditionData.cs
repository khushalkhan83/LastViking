using UnityEngine;

namespace Game.Objectives.Data.Conditions.Static
{
    public class CookItemConditionData : ConditionBaseData, IProgress<int>
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private string _itemName;
        [SerializeField] private int _cookCount;

#pragma warning restore 0649
        #endregion

        public string ItemName => _itemName;
        public int Value => _cookCount;

        public override ConditionID ConditionID => ConditionID.CookItem;
    }
}
