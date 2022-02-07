using CodeStage.AntiCheat.ObscuredTypes;
using System;
using UnityEngine;

namespace UltimateSurvival
{
    [Serializable]
    public class LootItem
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private ObscuredString m_ItemName;

#pragma warning restore 0649
        #endregion

        public ObscuredString ItemName => m_ItemName;
    }
}
