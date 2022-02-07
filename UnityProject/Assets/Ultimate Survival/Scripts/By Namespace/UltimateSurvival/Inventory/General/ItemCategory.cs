using System;
using UnityEngine;

namespace UltimateSurvival
{
    [Serializable]
    public class ItemCategory
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private string m_Name;
        [SerializeField] private ItemData[] m_Items;

#pragma warning restore 0649
        #endregion

        public string Name => m_Name;
        public ItemData[] Items => m_Items;
    }
}
