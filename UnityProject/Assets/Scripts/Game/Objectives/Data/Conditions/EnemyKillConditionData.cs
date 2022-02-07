using UnityEngine;

namespace Game.Objectives.Data.Conditions.Static
{
    public class EnemyKillConditionData : ConditionBaseData, IProgress<int>
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private EnemyID _enemyID;
        [SerializeField] private int _killCount;

#pragma warning restore 0649
        #endregion

        public EnemyID EnemyID => _enemyID;
        public int Value => _killCount;

        public override ConditionID ConditionID => ConditionID.EnemyKill;
    }
}
