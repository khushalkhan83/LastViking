using UnityEngine;

namespace Game.Objectives.Data.Conditions.Static
{
    public class ReachScoreLeadboardConditionData : ConditionBaseData, IProgress<int>
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private int _targetScore;

#pragma warning restore 0649
        #endregion

        public int Value => _targetScore;

        public override ConditionID ConditionID => ConditionID.ReachScoreLB;
    }
}
