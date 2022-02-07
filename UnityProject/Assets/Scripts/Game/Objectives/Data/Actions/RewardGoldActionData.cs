using UnityEngine;

namespace Game.Objectives.Actions
{
    public class RewardGoldActionData : ActionBaseData
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private ushort _goldReward;

#pragma warning restore 0649
        #endregion

        public ushort GoldReward => _goldReward;

        public override ActionID ActionID { get; } = ActionID.RewardGold;
    }
}
