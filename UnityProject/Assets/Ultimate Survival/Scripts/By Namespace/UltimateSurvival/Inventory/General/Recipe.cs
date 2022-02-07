using CodeStage.AntiCheat.ObscuredTypes;
using Game.Models;
using System;
using UnityEngine;

namespace UltimateSurvival
{
    [Serializable]
    public class RequiredItem
    {
        public string Name { get { return m_Name; } }

        public int Amount { get { return m_Amount; } }

        #region Data
#pragma warning disable 0649

        [SerializeField] private ObscuredString m_Name;

        [SerializeField] private ObscuredInt m_Amount;

#pragma warning restore 0649
        #endregion

        public RequiredItem() {}

        public RequiredItem(string name, int amount)
        {
               m_Name = name;
               m_Amount = amount; 
        }
    }

    [Serializable]
    public class Recipe
    {
        public int Duration => m_Duration;
        public CraftViewModel.CategoryID CategoryID => (CraftViewModel.CategoryID)(int)_categoryID;
        public RequiredItem[] RequiredItems => m_RequiredItems;
        public int CraftCount => _craftCount;

        #region Data
#pragma warning disable 0649

        [SerializeField] private ObscuredInt m_Duration = 1;

        [SerializeField] private RequiredItem[] m_RequiredItems;

        [ObscuredID(typeof(CraftViewModel.CategoryID))]
        [SerializeField] private ObscuredInt _categoryID;

        [SerializeField] private int _craftCount = 1;

#pragma warning restore 0649
        #endregion
    }
}
