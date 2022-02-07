using UnityEngine;

namespace Game.Objectives.Actions
{
    public class ChoiseItemActionData : ActionBaseData
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private string _itemName;

#pragma warning restore 0649
        #endregion

        public string ItemName => _itemName;

        public override ActionID ActionID { get; } = ActionID.ChoiseItem;
    }
}
