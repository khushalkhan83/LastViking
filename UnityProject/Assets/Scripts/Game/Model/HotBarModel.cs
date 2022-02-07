using CodeStage.AntiCheat.ObscuredTypes;
using Core.Storage;
using System;
using UltimateSurvival;
using UnityEngine;

namespace Game.Models
{
    public class HotBarModel : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase
        {
            public ObscuredInt EquipCellId;
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

            public void SetEquipCellId(int id)
            {
                EquipCellId = id;
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;

        [SerializeField] private StorageModel _storageModel;

        [SerializeField] private int _defaltItemID;

#pragma warning restore 0649
        #endregion

        public ItemsContainer ItemsContainer => _data.ItemsContainer;
        private ItemsDB ItemsDB => ModelsSystem.Instance._itemsDB;

        private StorageModel StorageModel => _storageModel;

        public event Action OnChangeEquipCell;
        public event Action OnChangeEquipItem;

        public int DefaltItemID => _defaltItemID;

        public int EquipCellIdLast { get; private set; }

        public int EquipCellId
        {
            get => _data.EquipCellId;
            set => _data.SetEquipCellId(value);
        }

        public CellModel EquipCell => ItemsContainer.GetCell(EquipCellId);

        public void Equp(int cellId)
        {
            EquipCellIdLast = EquipCellId;
            EquipCellId = cellId;

            ItemsContainer.GetCell(EquipCellIdLast).OnChange -= OnChangeEquipItemHandler;
            ItemsContainer.GetCell(EquipCellId).OnChange += OnChangeEquipItemHandler;

            ChangeEquipItem();
            ChangeEquipCell();
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

            if(EquipCell.IsHasItem && cellModel.Item != null && !cellModel.Item.IsBroken())
            {
                ChangeEquipItem();
            }
            else
            {
                Equp(0);
            }

        }

        private void ChangeEquipItem() => OnChangeEquipItem?.Invoke();
        private void ChangeEquipCell() => OnChangeEquipCell?.Invoke();

        private void OnEnable()
        {
            StorageModel.TryProcessing(_data);
            SetDefaultItem();
            ItemsContainer.OnLoad();
            _data.Initialize();

            if(EquipCell.IsHasItem && !EquipCell.Item.IsBroken())
            {
                ItemsContainer.GetCell(EquipCellId).OnChange += OnChangeEquipItemHandler;
                ChangeEquipItem();
                ChangeEquipCell();
            }
            else
            {
                Equp(0);
            }
        }

        public void SetDefaultItem()
        {
            ItemsContainer.SetCell(0, _defaltItemID, 1);
        }

        public bool IsCanPlayAudio;
        public void SetCanPlayAudio(bool isCanPlay) => IsCanPlayAudio = isCanPlay;
    }
}
