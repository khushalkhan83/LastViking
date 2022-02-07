using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers
{
    public class CollectCellDataController : ICollectCellDataController, IController
    {
        [Inject] public CollectCellDataModel CollectCellDataModel { get; private set; }
        [Inject] public RepairingItemsModel RepairingItemsModel { get; private set; }
        [Inject] public PlayerConsumeModel PlayerConsumeModel { get; private set; }
        [Inject] public ContainersModel ContainersModel { get; private set; }
        [Inject] public InventoryDragAndDropModel InventoryDraAndDropModel {get;set;}

        private InventoryCellsViewModelBase InventoryCellsViewModel => InventoryDraAndDropModel.InventoryCellsViewModel;

        void IController.Enable() 
        {
            CollectCellDataModel.OnCollectCellData += CollectCellData;
            CollectCellDataModel.OnCollectConsumeCellData += CollectConsumeCellData;
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            CollectCellDataModel.OnCollectCellData -= CollectCellData;
            CollectCellDataModel.OnCollectConsumeCellData -= CollectConsumeCellData;
        }

        private CellData CollectCellData(CellModel cell, int containerId)
        {
            var icon = (Sprite)null;
            var count = (int?)null;
            var durability = (float?)null;
            var sips = (float?)null;
            var progressConsumable = (float?)null;
            var progressRepairing = (float?)null;
            var disable = false;
            var itemRarity = ItemRarity.Eazy;
            var isComponent = false;

            var isConsumable = PlayerConsumeModel.IsInProgress
                    && ContainersModel.GetContainerID(PlayerConsumeModel.Container) == (ContainerID)containerId
                    && cell.Id == PlayerConsumeModel.CellId;

            SavableItem item;
            if (isConsumable)
            {
                item = PlayerConsumeModel.Item;
            }
            else
            {
                item = cell.Item;
            }

            if (cell.IsHasItem || isConsumable)
            {

                icon = item.ItemData.Icon;

                if (item.Count > 1)
                {
                    count = item.Count;
                }

                if (item.TryGetProperty("Durability", out var durabilityProperty))
                {
                    durability = durabilityProperty.Float.Ratio;
                }

                if (item.TryGetProperty("Sips", out var sipsProperty))
                {
                    sips = sipsProperty.Int.Ratio;
                }

                if (isConsumable)
                {
                    progressConsumable = PlayerConsumeModel.RemainingTimeNormalized;
                }

                if (RepairingItemsModel.TryFindRepairItemInfo(item, out var info))
                {
                    progressRepairing = info.RemainingTime / info.AllTime;
                }

                var consuming = item.IsCanConsume() && PlayerConsumeModel.IsInProgress;
                disable = consuming || item.IsBroken() || DisableItemWithoutProperties(item);

                itemRarity = item.ItemData.ItemRarity;
                isComponent = item.ItemData.Category == "Components";
            }

            return new CellData
            {
                Id = cell.Id,
                ContainerId = containerId,
                Icon = icon,
                Count = count,
                Durability = durability,
                ProgressConsumable = progressConsumable,
                ProgressRepairing = progressRepairing,
                Sips = sips,
                IsDisable = disable,
                ItemRarity = itemRarity,
                IsComponent = isComponent
            };
        }

        private CellData CollectConsumeCellData(SavableItem item, int containerId, int cellId)
        {
            var count = (int?)null;
            var durability = (float?)null;
            var sips = (float?)null;

            if (item.Count > 1)
            {
                count = item.Count;
            }

            if (item.TryGetProperty("Durability", out var durabilityProperty))
            {
                durability = durabilityProperty.Float.Ratio;
            }

            if (item.TryGetProperty("Sips", out var sipsProperty))
            {
                sips = sipsProperty.Int.Ratio;
            }

            return new CellData
            {
                Id = cellId,
                ContainerId = containerId,
                Icon = item.ItemData.Icon,
                Count = count,
                Durability = durability,
                ProgressConsumable = PlayerConsumeModel.RemainingTimeNormalized,
                Sips = sips,
                ItemRarity = item.ItemData.ItemRarity,
                IsComponent = item.ItemData.Category == "Components"
            };
        }

        private bool DisableItemWithoutProperties(SavableItem item)
        {
            if(InventoryCellsViewModel.DisableItemsWithoutProperties && InventoryCellsViewModel.ActiveItemsProperties.Length > 0)
            {
                foreach(string prop in InventoryCellsViewModel.ActiveItemsProperties)
                {
                    if(item.HasProperty(prop))
                        return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
