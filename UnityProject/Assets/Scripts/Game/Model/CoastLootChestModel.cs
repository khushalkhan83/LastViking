using CodeStage.AntiCheat.ObscuredTypes;
using Core.Storage;
using System.Collections.Generic;
using UltimateSurvival;
using UnityEngine;
using System;

namespace Game.Models
{
    public class CoastLootChestModel : MonoBehaviour
    {
       
        [System.Serializable]
        public class Data : DataBase
        {
            public ObscuredLong TimeSpawnTicks;
            public List<SavableItem> LootItems;
            public ObscuredBool DoubleUsed;
            public ObscuredBool Spined;

            public void SetTimeSpawnTicks(long timeSpawnTicks)
            {
                TimeSpawnTicks = timeSpawnTicks;
                ChangeData();
            }

            public void SetLootImtems(List<SavableItem> itmes)
            {
                LootItems = itmes;
                ChangeData();
            }

            public void AddLootItem(SavableItem item)
            {
                LootItems.Add(item);
                item.StackChanged.AddListener(ItemCountChanged);
                ChangeData();
            }

            public void RemoveLootItem(SavableItem item)
            {
                LootItems.Remove(item);
                ChangeData();
            }

            public void ReplaceLootItem(int index, SavableItem newItem) {
                if (index < LootItems.Count)
                {
                    LootItems[index] = newItem;
                    ChangeData();
                }
            }

            public void ClearLootItems()
            {
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

            private void ItemCountChanged()
            {
                ChangeData();
            }
        }

        public event Action ChestOpen;

        [SerializeField] private float _durationRespawnSec;
        [SerializeField] Data _data;
        [SerializeField] private StorageModel _storageModel;

        private StorageModel StorageModel => _storageModel;
        private ItemsDB ItemsDB => ModelsSystem.Instance._itemsDB;

        public float DurationRespawnSec => _durationRespawnSec;
        public Data _Data => _data;
        public long TimeSpawnTicks
        {
            get { return _data.TimeSpawnTicks; }
            set { _data.SetTimeSpawnTicks(value); }
        }

        public List<SavableItem> LootItems
        {
            get { return _data.LootItems; }
            set { _data.SetLootImtems(value); }
        }

        public bool DoubleUsed
        {
            get { return _data.DoubleUsed; }
            set { _data.SetDoubleUsed(value); }
        }

        public bool Spined
        {
            get { return _data.Spined; }
            set { _data.SetSpined(value); }
        }

        public void AddLootItem(SavableItem item)
        {
            _data.AddLootItem(item);
        }

        public void RemoveLootItem(SavableItem item)
        {
            _data.RemoveLootItem(item);
        }

        public void ReplaceLootItem(int index, SavableItem newItem)
        {
            _data.ReplaceLootItem(index, newItem);
        }

        public void ClearLootItems()
        {
            _data.ClearLootItems();
        }

        public void OnChestOpen() {
            ChestOpen?.Invoke();
        }

        private void OnEnable() {
            StorageModel.TryProcessing(_data);
            foreach (var item in LootItems)
            {
                if (item.ItemData == null)
                {
                    item.ItemData = ItemsDB.GetItem(item.Id);
                }
            }
        }

    }
}
