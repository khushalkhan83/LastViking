using UnityEngine;

namespace Game.Objectives.Data.Conditions.Static
{
    public class ReachPlaceLeaderboardConditionData : ConditionBaseData, IProgress<int>
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private int _targetUpPlace;

#pragma warning restore 0649
        #endregion

        public int Value => _targetUpPlace;

        public override ConditionID ConditionID => ConditionID.ReachPlaceLB;
    }
}
