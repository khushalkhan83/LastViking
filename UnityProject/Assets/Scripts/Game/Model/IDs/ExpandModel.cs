using CodeStage.AntiCheat.ObscuredTypes;
using System;
using UnityEngine;

namespace Game.Models
{

    [Serializable]
    public class ExpandModel
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private ObscuredInt _expandLevel;
        [SerializeField] private ObscuredInt _expandLevelMax;

#pragma warning restore 0649
        #endregion

        public event Action<int> OnExpand;

        public int ExpandLevel => _expandLevel;

        public int ExpandLevelMax => _expandLevelMax;

        public bool IsMaxLevel => ExpandLevel == ExpandLevelMax;

        public void ExpandCells()
        {
            ++_expandLevel;

            OnExpand?.Invoke(ExpandLevel);
        }

        public void Initialization(int expandLevel, int expandLevelMax)
        {
            _expandLevel = expandLevel;
            _expandLevelMax = expandLevelMax;
        }
    }
}
