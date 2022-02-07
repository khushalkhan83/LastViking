using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

namespace Game.AI
{
    public class Target : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [ObscuredID(typeof(TargetID))]
        [SerializeField] private ObscuredInt _targetID;

#pragma warning restore 0649
        #endregion

        public TargetID ID => (TargetID)(int)_targetID;
    }
}
