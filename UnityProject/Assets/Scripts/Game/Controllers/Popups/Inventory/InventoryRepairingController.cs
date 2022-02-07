using Core;
using Core.Controllers;
using Game.Models;
using Game.Purchases;
using Game.Views;
using System;
using System.Collections.Generic;
using UltimateSurvival;
using static Game.Models.InventoryOperationsModel;

namespace Game.Controllers
{
    /* Split logic : view logic + repair logic. Reapir logic can be reused by other enteties
     */

    public class InventoryRepairingController : IInventoryRepairingController, IController
    {
        [Inject] public InventoryRepairingModel InventoryRepairingModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public RepairViewModel RepairViewModel { get; private set; }
        [Inject] public RepairingItemsModel RepairingItemsModel { get; private set; }
        [Inject] public PurchasesModel PurchasesModel { get; private set; }
        [Inject] public InventoryOperationsModel InventoryOperationsModel { get; private set; }

        protected RepairView RepairView { get; private set; }

        void IController.Enable() 
        {
            InventoryRepairingModel.OnShowRepairPopup += ShowRepairPopup;
            InventoryRepairingModel.OnHideRepairPopup += HideRepairPopup;
        }

        void IController.Start() 
        {
        }

        void IController.Disable()
        {
            InventoryRepairingModel.OnShowRepairPopup -= ShowRepairPopup;
            InventoryRepairingModel.OnHideRepairPopup -= HideRepairPopup;
        }

        private void ShowRepairPopup()
        {
            RepairView = ViewsSystem.Show<RepairView>(ViewConfigID.Repair);
            RepairView.OnClose += OnBackRepairPopupViewHandler;
            RepairView.OnRepairWatch += OnRepairWatchPopupViewHandler;
            RepairView.OnRepairGold += OnRepairGoldPopupViewHandler;
        }

        private void HideRepairPopup()
        {
            if(RepairView != null)
            {
                RepairView.OnClose -= OnBackRepairPopupViewHandler;
                RepairView.OnRepairWatch -= OnRepairWatchPopupViewHandler;
                RepairView.OnRepairGold -= OnRepairGoldPopupViewHandler;
                ViewsSystem.Hide(RepairView);
                RepairView = null;
            }
        }

        private void OnBackRepairPopupViewHandler() => HideRepairPopup();

        private void OnRepairWatchPopupViewHandler()
        {
            var item = RepairViewModel.CurrentItem;
            var repairCount = item.GetProperty("Repair count");
            var repairIndex = repairCount.Int.Default - repairCount.Int.Current;

            if (item.ItemData.Recipe == null) return;
            List<ItemInfo> repairPrice = GetRepairPrice(item, repairIndex);

            InventoryOperationsModel.RemoveItemsFromPlayer(repairPrice);
            RepairingItemsModel.AddItem(item);

            HideRepairPopup();
        }

        private List<ItemInfo> GetRepairPrice(SavableItem item, int repairIndex)
        {
            List<ItemInfo> repairPrice = new List<ItemInfo>();
            var part = RepairingItemsModel.PartRequiresResources(repairIndex);

            //TODO: create extension to convert required items to item info (easy to make error here)
            foreach (var requiredItem in item.ItemData.Recipe.RequiredItems)
            {
                int count = (int)Math.Ceiling(part * requiredItem.Amount);
                repairPrice.Add(new ItemInfo(requiredItem.Name, count));
            }

            return repairPrice;
        }

        private PurchaseID GetRepairGoldPurchaseId(SavableItem _item, int repairIndex)
        {
            if (_item.HasProperty("ItemLowQuality"))
                return RepairingItemsModel.RepairPurchaseLow(repairIndex);
            if (_item.HasProperty("ItemMidQuality"))
                return RepairingItemsModel.RepairPurchaseMid(repairIndex);
            if (_item.HasProperty("ItemHighQuality"))
                return RepairingItemsModel.RepairPurchaseHigh(repairIndex);

            return RepairingItemsModel.RepairPurchaseLow(repairIndex);
        }

        private void OnRepairGoldPopupViewHandler()
        {
            var item = RepairViewModel.CurrentItem;
            var repairCount = item.GetProperty("Repair count");
            var repairIndex = repairCount.Int.Default - repairCount.Int.Current;

            PurchaseID purchaseID = GetRepairGoldPurchaseId(item, repairIndex);
            PurchasesModel.Purchase(purchaseID, OnPurchaseGoldRepairItem);
        }

        private void OnPurchaseGoldRepairItem(PurchaseResult purchaseResult)
        {
            if (purchaseResult == PurchaseResult.Successfully)
            {
                var item = RepairViewModel.CurrentItem;
                RepairingItemsModel.RepairItemInstantly(item);

                HideRepairPopup();
            }
            else
            {
                ViewsSystem.Show<PurchasesView>(ViewConfigID.Purchases);
            }
        }
    }
}
