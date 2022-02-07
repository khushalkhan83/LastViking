using UnityEngine;

namespace Game.Objectives.Data.Conditions.Static
{
    public abstract class ConditionBaseData : ScriptableObject
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private ushort _id;

#pragma warning restore 0649
        #endregion

        public ushort Id => _id;

        public abstract ConditionID ConditionID { get; }
    }
}
