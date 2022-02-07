using UnityEngine;

namespace Game.Objectives.Data.Conditions.Static
{
    public class RepairAnyItemConditionData : ConditionBaseData, IProgress<int>
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private int _repairCount;

#pragma warning restore 0649
        #endregion

        public int Value => _repairCount;

        public override ConditionID ConditionID => ConditionID.RepairAnyItem;
    }
}
