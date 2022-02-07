using System;
using UnityEngine;

namespace UltimateSurvival.Debugging
{
    [Serializable]
    public class ItemMeta
    {
        public string Name { get { return m_Name; } }
        public int Count { get { return m_Count; } }

        #region Data
#pragma warning disable 0649

        [SerializeField] private string m_Name;

        [Clamp(1, 9999)]
        [SerializeField] private int m_Count = 1;

#pragma warning restore 0649
        #endregion

    }
}
