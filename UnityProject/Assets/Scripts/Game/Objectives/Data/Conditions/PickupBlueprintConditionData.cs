using UnityEngine;

namespace Game.Objectives.Data.Conditions.Static
{
    public class PickupBlueprintConditionData : ConditionBaseData, IProgress<int>
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private int _blueprintCount;

#pragma warning restore 0649
        #endregion

        public int Value => _blueprintCount;

        public override ConditionID ConditionID => ConditionID.PickupBlueprint;
    }
}
