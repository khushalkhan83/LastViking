using Game.AI;
using Game.Models;
using UnityEngine;

namespace Game.Objectives.Data.Conditions.Static
{
    public class BreakWorldObjectConditionData : ConditionBaseData, IProgress<int>
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private WorldObjectID _worldObjectID;
        [SerializeField] private TargetID _targetID = TargetID.Player;
        [SerializeField] private int _breakCount;

#pragma warning restore 0649
        #endregion

        public WorldObjectID WorldObjectID => _worldObjectID;
        public TargetID TargetID => _targetID;
        public int Value => _breakCount;

        public override ConditionID ConditionID => ConditionID.BreakWorldObject;
    }
}
