using UnityEngine;

namespace Game.Objectives.Data.Conditions.Static
{
    public class KillKrakenConditionData : ConditionBaseData, IProgress<int>
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private int _health;

#pragma warning restore 0649
        #endregion

        public int Value => _health;

        public override ConditionID ConditionID => ConditionID.KillKraken;
    }
}
