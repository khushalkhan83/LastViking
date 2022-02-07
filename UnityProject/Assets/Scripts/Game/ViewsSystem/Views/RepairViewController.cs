using Core;
using Core.Controllers;
using Game.Audio;
using Game.Models;
using Game.Purchases;
using Game.Views;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UltimateSurvival;
using System;

namespace Game.Controllers
{
    public class RepairViewController : ViewControllerBase<RepairView>
    {
        [Inject] public AudioSystem AudioSystem { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public ItemsDB ItemsDB { get; private set; }
        [Inject] public RepairViewModel RepairViewModel { get; private set; }
        [Inject] public InventoryModel InventoryModel { get; private set; }
        [Inject] public HotBarModel HotBarModel { get; private set; }
        [Inject] public RepairingItemsModel RepairingItemsModel { get; private set; }
        [Inject] public PurchasesModel PurchasesModel { get; private set; }
        [Inject] public LocalizationModel LocalizationModel { get; private set; }

        protected override void Show()
        {
            LocalizationModel.OnChangeLanguage += SetLocalization;
            View.OnClose += OnCloseHandler;
            View.OnRepairWatch += OnRepairHandler;
            View.OnRepairGold += OnRepairGoldHandler;

            AudioSystem.PlayOnce(AudioID.WindowOpen);
            UpdateItemRepairInfo(RepairViewModel.CurrentItem);

            SetLocalization();
        }

        protected override void Hide()
        {
            LocalizationModel.OnChangeLanguage -= SetLocalization;
            View.OnClose -= OnCloseHandler;
            View.OnRepairWatch -= OnRepairHandler;
            View.OnRepairGold -= OnRepairGoldHandler;
        }

        private void OnCloseHandler()
        {
            AudioSystem.PlayOnce(AudioID.Button);
        }

        private void OnRepairHandler()
        {
            AudioSystem.PlayOnce(AudioID.PickUp);
        }

        private void OnRepairGoldHandler()
        {
            AudioSystem.PlayOnce(AudioID.PickUp);
        }

        private void UpdateItemRepairInfo(SavableItem _item)
        {
            var repairCount = _item.GetProperty("Repair count");
            int repairIndex = repairCount.Int.Default - repairCount.Int.Current;
            float part = RepairingItemsModel.PartRestoredDuratility(repairIndex);
            View.SetTextTitlePart(string.Format(LocalizationModel.GetString(LocalizationKeyID.RepairPopup_Title02), (int)(part * 100)));

            if (_item.HasProperty("ItemLowQuality"))
            {
                UpdateRepairButtonsInfoLow(repairIndex);
            }
            else if (_item.HasProperty("ItemMidQuality"))
            {
                UpdateRepairButtonsInfoMid(repairIndex);
            }
            else if (_item.HasProperty("ItemHighQuality"))
            {
                UpdateRepairButtonsInfoHigh(repairIndex);
            }
            else
            {
                UpdateRepairButtonsInfoLow(repairIndex);
            }

            bool goldRepair = RepairingItemsModel.OnlyGoldRepair(repairIndex);
            View.SetRepairResourcesPartVisible(!goldRepair);
            if (!goldRepair)
                RefreshResourceCellViews(_item);
        }

        private void UpdateRepairButtonsInfoLow(int repairIndex)
        {
            PurchaseID purchaseID = RepairingItemsModel.RepairPurchaseLow(repairIndex);
            var info = PurchasesModel.GetInfo<IPurchaseCoinInfo>(purchaseID);

            if (info != null)
            {
                View.SetCountText(info.CoinCost.ToString());
                View.SetCountVisible(true);
                View.SetWatchIconVisible(false);
                View.SetGoldIconVisible(true);
            }
            else
            {
                View.SetCountVisible(false);
                View.SetWatchIconVisible(true);
                View.SetGoldIconVisible(false);
            }
        }

        private void UpdateRepairButtonsInfoMid(int repairIndex)
        {
            PurchaseID purchaseID = RepairingItemsModel.RepairPurchaseMid(repairIndex);
            var info = PurchasesModel.GetInfo<IPurchaseCoinInfo>(purchaseID);

            if (info != null)
            {
                View.SetCountText(info.CoinCost.ToString());
                View.SetCountVisible(true);
                View.SetWatchIconVisible(false);
                View.SetGoldIconVisible(true);
            }
            else
            {
                View.SetCountVisible(false);
                View.SetWatchIconVisible(true);
                View.SetGoldIconVisible(false);
            }
        }

        private void UpdateRepairButtonsInfoHigh(int repairIndex)
        {
            PurchaseID purchaseID = RepairingItemsModel.RepairPurchaseHigh(repairIndex);
            var info = PurchasesModel.GetInfo<IPurchaseCoinInfo>(purchaseID);

            if (info != null)
            {
                View.SetCountText(info.CoinCost.ToString());
                View.SetCountVisible(true);
                View.SetWatchIconVisible(false);
                View.SetGoldIconVisible(true);
            }
            else
            {
                View.SetCountVisible(false);
                View.SetWatchIconVisible(true);
                View.SetGoldIconVisible(false);
            }
        }

        private void RefreshResourceCellViews(SavableItem _item)
        {
            var cellId = 0;
            var items = _item.ItemData.Recipe.RequiredItems;

            var repairCount = _item.GetProperty("Repair count");
            int repairIndex = repairCount.Int.Default - repairCount.Int.Current;
            float part = RepairingItemsModel.PartRequiresResources(repairIndex);

            {
                for (; cellId < items.Length; cellId++)
                {
                    var itemResource = ItemsDB.GetItem(items[cellId].Name);
                    var resourcesData = GetResourceCelllData(cellId, itemResource, (int)Math.Ceiling(items[cellId].Amount * part));

                    View.ResourceCells[cellId].SetData(resourcesData);
                }

                for (; cellId < View.ResourceCells.Length; cellId++)
                {
                    View.ResourceCells[cellId].SetData(ResourceCellData.Empty);
                }
            }

            bool canRepair = IsHasResources(items, part);
            StartCoroutine(SetButton(canRepair));
        }

        private ResourceCellData GetResourceCelllData(int cellId, ItemData item, int count)
        {
            var countCurrent = InventoryModel.ItemsContainer.GetItemsCount(item.Id) + HotBarModel.ItemsContainer.GetItemsCount(item.Id);

            return new ResourceCellData()
            {
                ItemId = item.Id,
                CellId = cellId,
                Icon = item.Icon,
                Message = GetResourceMessage(count, countCurrent),
                IsActive = true,
                ItemRarity = item.ItemRarity,
                IsComponent = item.Category == "Components",
            };
        }

        private bool IsHasResources(IEnumerable<RequiredItem> items, float resPart) => items.All(item => InventoryModel.ItemsContainer.GetItemsCount(ItemsDB.GetItem(item.Name).Id) + HotBarModel.ItemsContainer.GetItemsCount(ItemsDB.GetItem(item.Name).Id) >= (int)Math.Ceiling(item.Amount * resPart));

        private IEnumerator SetButton(bool canRepair)
        {
            yield return null;
            View.RepairButton.SetActive(canRepair);
            View.RepairButtonDisabled.SetActive(!canRepair);
        }

        private string GetResourceMessage(int count, int countCurrent)
        {
            if (countCurrent < count)
            {
                return $"<color=#FF0000FF>{countCurrent}/{count}</color>";

            }
            else
            {
                return $"<color=#FFECC4FF>{countCurrent}/{count}</color>";
            }
        }

        private void SetLocalization()
        {
            View.SetTextTitleRepair(LocalizationModel.GetString(LocalizationKeyID.RepairPopup_Title01));
            View.SetTextButtonReapirTitle(LocalizationModel.GetString(LocalizationKeyID.InventoryMenu_RepairBtn));
            View.SetTextButtonReapirInactiveTitle(LocalizationModel.GetString(LocalizationKeyID.InventoryMenu_RepairBtn));
            View.SetTextButtonReapirGoldTitle(LocalizationModel.GetString(LocalizationKeyID.InventoryMenu_RepairBtn));
            View.SetInfo(LocalizationModel.GetString(LocalizationKeyID.RepairPopup_NoResReq));
        }
    }
}
