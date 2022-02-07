using CodeStage.AntiCheat.ObscuredTypes;
using Core.Storage;
using System.Collections.Generic;
using UltimateSurvival;
using UnityEngine;

namespace Game.Models
{
    public class TreasureHuntLootChestModel : MonoBehaviour
    {
        [System.Serializable]
        public class Data : DataBase
        {
            public List<SavableItem> LootItems;

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

            public void ReplaceLootItem(int index, SavableItem newItem)
            {
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
            private void ItemCountChanged()
            {
                ChangeData();
            }
        }

        [SerializeField] Data _data;
        [SerializeField] TreasureID _treasureID;

        public Data _Data => _data;
        public TreasureID TreasureID => _treasureID;
        public List<SavableItem> LootItems
        {
            get { return _data.LootItems; }
            set { _data.SetLootImtems(value); }
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

        private void OnEnable()
        {
            ClearLootItems();
        }

    }
}
