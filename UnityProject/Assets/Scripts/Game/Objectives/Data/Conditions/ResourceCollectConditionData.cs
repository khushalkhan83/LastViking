using UnityEngine;

namespace Game.Objectives.Data.Conditions.Static
{
    public class ResourceCollectConditionData : ConditionBaseData, IProgress<int>
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private string _resourceName;
        [SerializeField] private int _collectCount;

#pragma warning restore 0649
        #endregion

        public string ResourceName => _resourceName;
        public int Value => _collectCount;

        public override ConditionID ConditionID => ConditionID.ResourceCollect;
    }
}
