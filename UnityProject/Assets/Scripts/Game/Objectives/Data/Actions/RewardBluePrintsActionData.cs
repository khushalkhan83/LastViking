using UnityEngine;

namespace Game.Objectives.Actions
{
    public class RewardBluePrintsActionData : ActionBaseData
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private ushort _bluePrintsReward;

#pragma warning restore 0649
        #endregion

        public ushort BluePrintsReward => _bluePrintsReward;

        public override ActionID ActionID { get; } = ActionID.BluePrintsReward;
    }
}
