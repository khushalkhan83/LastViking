using cakeslice;
using CodeStage.AntiCheat.ObscuredTypes;
using Core.Storage;
using Core.Views;
using Game.Models;
using Game.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UltimateSurvival
{
    public class TreasureLootObject : InteractableObject, IData, IOutlineTarget
    {
        [Serializable]
        public class Data : DataBase
        {
            public List<SavableItem> LootItems;
            public ObscuredBool DoubleUsed;
            public ObscuredBool Spined;
            public ObscuredLong TimeSpawnTicks;
            public void AddLootItem(SavableItem item) {
                LootItems.Add(item);
                item.StackChanged.AddListener(ItemCountChanged);
                ChangeData();
            }

            public void SetLootImtems(List<SavableItem> itmes)
            {
                LootItems = itmes;
                ChangeData();
            }

            public void RemoveLootItem(SavableItem item)
            {
                LootItems.Remove(item);
                ChangeData();
            }
            public void ReplaceLootItem(int index, SavableItem newItem)
            {
                if (index < LootItems.Count)
                {
                    LootItems[index] = newItem;
                    ChangeData();
                }
            }

            public void ClearLootItems() {
                LootItems.Clear();
                ChangeData();
            }
            public void SetDoubleUsed(bool value)
            {
                DoubleUsed = value;
                ChangeData();
            }
            public void SetSpined(bool value)
            {
                Spined = value;
                ChangeData();
            }
            public void SetTimeSpawnTicks(long timeSpawnTicks)
            {
                TimeSpawnTicks = timeSpawnTicks;
                ChangeData();
            }

            private void ItemCountChanged() {
                ChangeData();
            }
        }

        public event Action OnOpen;
        public event Action OnClose;
        public event Action OnDataInitialize;
        public event Action<IOutlineTarget> OnUpdateRendererList;

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] private TreasureID _treasureID;
        [SerializeField] List<Renderer> _renderers;
        [SerializeField] private ObscuredFloat _respawnSec;

#pragma warning restore 0649
        #endregion

        public TreasureID TreasureID => _treasureID;
        public virtual float RespawnSec => _respawnSec;

        public ItemsDB ItemsDB => ModelsSystem.Instance._itemsDB;
        public StorageModel StorageModel => ModelsSystem.Instance._storageModel;
        public TreasureLootModel TreasureLootModel => ModelsSystem.Instance._treasureLootModel;

        public virtual List<SavableItem> LootItems {
            get { return _data.LootItems; }
            set { _data.SetLootImtems(value); }
        }
        public virtual bool DoubleUsed
        {
            get { return _data.DoubleUsed; }
            set { _data.SetDoubleUsed(value); }
        }
        public virtual bool Spined
        {
            get { return _data.Spined; }
            set { _data.SetSpined(value); }
        }
        public virtual long TimeSpawnTicks
        {
            get { return _data.TimeSpawnTicks; }
            set { _data.SetTimeSpawnTicks(value); }
        }
        public IEnumerable<IUnique> Uniques
        {
            get
            {
                yield return _data;
            }
        }

        public virtual void GenerateItems()
        {
            ClearLootItems();
            var items = TreasureLootModel.GetLootItems(TreasureID);
            foreach (var item in items) {
                AddLootItem(item);
            }
        }

        public virtual void AddLootItem(SavableItem item) {
            _data.AddLootItem(item);
        }

        public virtual void RemoveLootItem(SavableItem item) {
            _data.RemoveLootItem(item);
        }

        public virtual void ReplaceLootItem(int index, SavableItem newItem) {
            _data.ReplaceLootItem(index, newItem);
        }

        public virtual void ClearLootItems() {
            _data.ClearLootItems();
        }

        protected virtual void InitializeData() 
        {
            StorageModel.TryProcessing(_data);
            foreach (var item in LootItems)
            {
                if (item.ItemData == null)
                {
                    item.ItemData = ItemsDB.GetItem(item.Id);
                }
            }
        }

        public void UUIDInitialize()
        {
            InitializeData();
            OnDataInitialize?.Invoke();
        }

        public List<Renderer> GetRenderers()
        {
            return _renderers;
        }

        public bool IsUseWeaponRange()
        {
            return false;
        }

        public int GetColor()
        {
            return 1;
        }
    }
}
