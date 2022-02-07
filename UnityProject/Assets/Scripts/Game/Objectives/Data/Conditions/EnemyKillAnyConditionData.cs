using UnityEngine;

namespace Game.Objectives.Data.Conditions.Static
{
    public class EnemyKillAnyConditionData : ConditionBaseData, IProgress<int>
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private EnemyID[] _enemyIDs;
        [SerializeField] private int _killCount;

#pragma warning restore 0649
        #endregion

        public EnemyID[] EnemyIDs => _enemyIDs;
        public int Value => _killCount;

        public override ConditionID ConditionID => ConditionID.EnemyKillAny;
    }
}
