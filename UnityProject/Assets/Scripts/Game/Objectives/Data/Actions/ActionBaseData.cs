using UnityEngine;

namespace Game.Objectives
{
    public abstract class ActionBaseData : ScriptableObject
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private ushort _id;

#pragma warning restore 0649
        #endregion

        public ushort Id => _id;

        abstract public ActionID ActionID { get; }
    }
}
