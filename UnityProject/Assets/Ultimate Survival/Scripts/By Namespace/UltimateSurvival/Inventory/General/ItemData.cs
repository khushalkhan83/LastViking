using CodeStage.AntiCheat.ObscuredTypes;
using Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UltimateSurvival
{
    [Serializable]
    public class ItemData
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private string m_Name;
        [ObscuredID(typeof(LocalizationKeyID))]
        [SerializeField] private ObscuredInt _displayNameKeyID;
        [ObscuredID(typeof(LocalizationKeyID))]
        [SerializeField] private ObscuredInt _descriptionKeyID;
        [ObscuredID(typeof(WorldObjectID))]
        [SerializeField] private ObscuredInt _worldObjectID = 22;
        [SerializeField] private ObscuredInt m_Id;
        [SerializeField] private ObscuredString m_Category;
        [SerializeField] private Sprite m_Icon;
        [SerializeField] private ObscuredInt m_StackSize = 1;
        [SerializeField] private List<ItemProperty.Value> m_PropertyValues;
        [SerializeField] private ObscuredBool m_IsBuildable;
        [SerializeField] private ObscuredBool m_IsCraftable;
        [SerializeField] private Recipe m_Recipe;
        [SerializeField] private ObscuredBool m_isUnlockable;
        [SerializeField] private RequiredItem[] m_UnlockablesItems;
        [SerializeField] private ObscuredBool m_IsUnlock;
        [ObscuredID(typeof(ItemRarity))]
        [SerializeField] private ObscuredInt _itemRarity;

#pragma warning restore 0649
        #endregion

        public int __id { set { m_Id = value; } }
        public string __category { set { m_Category = value; } }

        public bool IsBuildable => m_IsBuildable;
        public bool IsCraftable => m_IsCraftable;
        public bool IsUnlockable => m_isUnlockable;
        public int Id => m_Id;
        public int StackSize => m_StackSize;
        public string Name => m_Name;
        public string Category => m_Category;
        public LocalizationKeyID DisplayNameKeyID => (LocalizationKeyID)(int)_displayNameKeyID;
        public LocalizationKeyID DescriptionKeyID => (LocalizationKeyID)(int)_descriptionKeyID;
        public Sprite Icon => m_Icon;
        public WorldObjectID WorldObjectID => (WorldObjectID)(int)_worldObjectID;
        public List<ItemProperty.Value> PropertyValues => m_PropertyValues;
        public Recipe Recipe => m_Recipe;
        public RequiredItem[] UnlockablesItems => m_UnlockablesItems;
        public ItemRarity ItemRarity => (ItemRarity)(int)_itemRarity;

        public bool IsHasProperty(string name) => m_PropertyValues.Any(x => x.Name == name);

        public ItemProperty.Value GetProperty(string name) => m_PropertyValues.FirstOrDefault(x => x.Name == name);

        public bool TryGetProperty(string name, out ItemProperty.Value propertyValue) => (propertyValue = GetProperty(name)) != null;
    }
}
