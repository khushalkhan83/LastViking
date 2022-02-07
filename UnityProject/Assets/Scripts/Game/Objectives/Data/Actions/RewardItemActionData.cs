using UnityEngine;

namespace Game.Objectives.Actions
{
    public class RewardItemActionData : ActionBaseData
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private string _itemName;
        [SerializeField] private ushort _itemCount;

#pragma warning restore 0649
        #endregion

        public string ItemName => _itemName;
        public ushort ItemCount => _itemCount;

        public override ActionID ActionID { get; } = ActionID.RewardItem;
    }
}
