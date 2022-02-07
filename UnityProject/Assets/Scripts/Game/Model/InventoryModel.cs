using Core.Storage;
using CustomeEditorTools;
using NaughtyAttributes;
using System;
using UnityEngine;

namespace Game.Models
{
    public class InventoryModel : MonoBehaviour
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

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;

        [SerializeField] private StorageModel _storageModel;
        [SerializeField] private InventoryViewModel _inventoryViewModel;
        [SerializeField] private int _cellsCountDefault;

        [SerializeField] private ItemsGroup _initialItemsGroup;
        [SerializeField] private ItemsGroup _debugItemsGroup;

#pragma warning restore 0649
        #endregion

        private int CellsCountDefault => _cellsCountDefault;
        public ItemsContainer ItemsContainer => _data.ItemsContainer;
        private ItemsGroup InitialItemsGroup => _initialItemsGroup;
        private ItemsGroup DebugItemsGroup => _debugItemsGroup;
        private StorageModel StorageModel => _storageModel;
        private InventoryViewModel InventoryViewModel => _inventoryViewModel;

        private ItemsDB ItemsDB => ModelsSystem.Instance._itemsDB;
        private EditorGameSettings EditorGameSettings => EditorGameSettings.Instance;

        private void OnEnable()
        {
            StorageModel.TryProcessing(InventoryViewModel._Data);
            
            if (!StorageModel.TryProcessing(_data))
            {
                for (int i = 0; i < InventoryViewModel.TotalExpandedCountLevel; i++)
                {
                    ItemsContainer.AddCells(InventoryViewModel.CellsExpanded);
                }

                foreach (var item in InitialItemsGroup.Items)
                {
                    ItemsContainer.AddItems(ItemsDB.GetItem(item.Name).Id, item.Count);
                }

                if(EditorGameSettings.AddDebugItemsOnGameStart)
                {
                    AddDebugItems();
                }
            }
            _data.Initialize();
            ItemsContainer.OnLoad();
        }

        public void Reset()
        {
            int extraCount = ItemsContainer.CountCells - CellsCountDefault;
            int expandedCount = InventoryViewModel.ForeverExpandedCountLevel * InventoryViewModel.CellsExpanded;
            ItemsContainer.RemoveLastCells(extraCount - expandedCount);
        }

        //TODO: remove dublicate code in ItemsActionAddItems
        [Button("Add debug items")] void AddDebugItems()
        {
            foreach (var item in DebugItemsGroup.Items)
            {
                ItemsContainer.AddItems(ItemsDB.GetItem(item.Name).Id, item.Count);
            }
        }
    }
}
