using System;
using System.Collections.Generic;
using System.Linq;
using Core.Storage;
using CustomeEditorTools;
using NaughtyAttributes;
using UltimateSurvival;
using UnityEngine;

namespace Game.Models
{
    public class InventoryEquipmentModel : InitableModel<InventoryEquipmentModel.Data>
    {
        [Serializable]
        public class Data : DataBase
        {
            public ItemsContainer ItemsContainer;

            public void Initialize()
            {
                ItemsContainer.OnAddCell += OnAddCell;
                ItemsContainer.OnAddItems += OnAddItems;
                ItemsContainer.OnChangeCell += OnChangeCell;
            }

            private void OnChangeCell(CellModel cell) => ChangeData();
            private void OnAddItems(int itemId, int count) => ChangeData();
            private void OnAddCell(CellModel cell) => ChangeData();
        }

        [SerializeField] private Data _data = default;
        [SerializeField] private IntEquipmentCategoryDictionary slotCategoeySettings = default;

        public event Action OnChangeEquipItem;

        protected override Data DataBase => _data;
        public ItemsContainer ItemsContainer => _data.ItemsContainer;


        protected override void OnInited() 
        {
            _data.Initialize();
            ItemsContainer.OnLoad();

            foreach(var cell in ItemsContainer.Cells)
            {
                cell.OnChange += OnChangeEquipItemHandler;
            }
        }

        private void OnChangeEquipItemHandler(CellModel cellModel)
        {
            if
            (
                cellModel.IsHasItem && cellModel.Item.TryGetProperty("Durability", out _)
                && cellModel.Item.TryGetProperty("Repair count", out var repairCount)
            )
            {
                if (cellModel.Item.IsBroken())
                {
                    if (repairCount.Int.Current <= 0)
                    {
                        ItemsContainer.RemoveItemsFromCell(cellModel.Id, 1);
                    }
                }
            }

            ChangeEquipItem();
        }

        private void ChangeEquipItem() => OnChangeEquipItem?.Invoke();

        public EquipmentCategory GetSlotCategory(int slotID)
        {
            if(slotCategoeySettings.TryGetValue(slotID, out var category))
            {
                return category;
            }
            return EquipmentCategory.None;
        }

        public int GetSlotIdByCategory(EquipmentCategory category)
        {
            foreach(var slotCategoey in slotCategoeySettings)
            {
                if(slotCategoey.Value == category)
                {
                    return slotCategoey.Key;
                }
            }
            return 0;
        }

        public List<SavableItem> GetAllEquipedItems() => ItemsContainer.Cells.Where(c => c.IsHasItem).Select(c => c.Item).ToList();
        public List<SavableItem> GetNotBrokenEquipedItems() => ItemsContainer.Cells.Where(c => c.IsHasItem && c.Item.TryGetProperty("Durability", out var durability) && durability.Float.Current > 0f).Select(c => c.Item).ToList();

    }

    public enum EquipmentCategory
    {
        None = 0,
        Head = 1,
        Chest = 2,
        Shoes = 3,
    }
}
