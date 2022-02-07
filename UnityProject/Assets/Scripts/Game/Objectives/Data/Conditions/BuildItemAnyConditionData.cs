using UnityEngine;

namespace Game.Objectives.Data.Conditions.Static
{
    public class BuildItemAnyConditionData : ConditionBaseData, IProgress<int>
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private string[] _itemNames;
        [SerializeField] private int _buildCount;

#pragma warning restore 0649
        #endregion

        public string[] ItemNames => _itemNames;
        public int Value => _buildCount;

        public override ConditionID ConditionID => ConditionID.BuildItemAny;
    }
}
