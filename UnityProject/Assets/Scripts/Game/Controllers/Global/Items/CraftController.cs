using Core;
using Core.Controllers;
using Game.Models;
using Game.Models.RemoteSettings;
using Game.Purchases;
using Game.Views;
using UltimateSurvival;
using UnityEngine;
using static Game.Models.InventoryOperationsModel;
using System.Linq;

namespace Game.Controllers
{
    public class CraftController : ICraftController, IController
    {
        [Inject] public RemoteSettingsModel RemoteSettingsModel { get; private set; }
        [Inject] public InventoryViewModel InventoryViewModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public PurchasesModel PurchasesModel { get; private set; }
        [Inject] public InventoryModel InventoryModel { get; private set; }
        [Inject] public NetworkModel NetworkModel { get; private set; }
        [Inject] public StorageModel StorageModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public CraftModel CraftModel { get; private set; }
        [Inject] public ItemsDB ItemsDB { get; private set; }
        [Inject] public InventoryOperationsModel InventoryOperationsModel { get; private set; }
        [Inject] public DropItemModel DropItemModel { get; private set; }

        public InventoryIsFillPopupView InventoryIsFillPopupView { get; private set; }

        private bool IsExpandInventoryButtonWatch => !RemoteSettingsInventory.IsBuySlotsGold && (WatchPurchaseInventoryExpand.IsCanPurchase || NetworkModel.IsHasConnection);

        private InventoryRemoteSettings RemoteSettingsInventory => RemoteSettingsModel.Get<InventoryRemoteSettings>(RemoteSettingID.Inventory);

        private IWatchPurchase WatchPurchaseInventoryExpand => PurchasesModel.GetInfo<IWatchPurchase>(PurchaseID.InventoryExpandWatch);

        void IController.Enable()
        {
            CraftModel.Init();

            CraftModel.OnCraftedItem += OnCraftedItemHandler;
            GameUpdateModel.OnUpdate += OnUpdate;

            DropCached();
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
            GameUpdateModel.OnUpdate -= OnUpdate;
            CraftModel.OnCraftedItem -= OnCraftedItemHandler;
        }

        private void OnUpdate() => CraftModel.CraftProcess(Time.deltaTime);

        private void OnCraftedItemHandler(int itemId)
        {
            var itemData = ItemsDB.GetItem(itemId);

            AddedItemDestinationPriority destinationPriority = itemData.IsHasProperty("CraftResultDestinationInventory") ? AddedItemDestinationPriority.Inventory : AddedItemDestinationPriority.HotBar;
            InventoryOperationsModel.ItemConfig config = new InventoryOperationsModel.ItemConfig(null, destinationPriority);

            AddItemsResult result = InventoryOperationsModel.TryAddItemsToPlayer(itemData,itemData.Recipe.CraftCount,config);

            if(!result.allAdded)
            {
                int left = result.notAddedItems.FirstOrDefault().Count;
                SavableItem savableItem = new SavableItem(itemData, left);

                if (InventoryViewModel.IsMaxExpandLevel)
                {
                    DropItemModel.DropItem(savableItem);
                    // CraftModel.PushCraftCompleted(item); // TODO: check this method
                }
                else
                {
                    // TODO: check this method
                    CraftModel.PushCraftCompleted(savableItem);
                    OpenInventoryFillPopup();
                }
            }
        }

        private void DropCached()
        {
            for (var i = 0; i < CraftModel.CountCachedCraftedItems; i++)
            {
                SavableItem savableItem = CraftModel.PopCraftCompleted();
                if (savableItem.ItemData == null) {
                    savableItem.ItemData = ItemsDB.GetItem(savableItem.Id);
                }
                DropItemModel.DropItem(savableItem);
            }
        }

        // TODO: reuse Full Inventory Controller
        private void OpenInventoryFillPopup()
        {
            if (InventoryIsFillPopupView == null)
            {
                InventoryIsFillPopupView = ViewsSystem.Show<InventoryIsFillPopupView>(ViewConfigID.InventoryIsFillPopup);
                InventoryIsFillPopupView.OnExpand += OnExpandInventoryIsFillPopupViewHandler;
                InventoryIsFillPopupView.OnBack += OnBackInventoryIsFillPopupViewHandler;
            }
        }

        private void CloseInventoryIsFillPopup()
        {
            InventoryIsFillPopupView.OnBack -= OnBackInventoryIsFillPopupViewHandler;
            InventoryIsFillPopupView.OnExpand -= OnExpandInventoryIsFillPopupViewHandler;

            ViewsSystem.Hide(InventoryIsFillPopupView);
            InventoryIsFillPopupView = null;
        }

        private void OnBackInventoryIsFillPopupViewHandler()
        {
            CloseInventoryIsFillPopup();

            DropCached();
        }

        private void OnExpandInventoryIsFillPopupViewHandler()
        {
            CloseInventoryIsFillPopup();

            if (IsExpandInventoryButtonWatch)
            {
                PurchasesModel.Purchase(PurchaseID.InventoryExpandWatch, OnPurchaseInventoryExpandWatch);
            }
            else
            {
                PurchasesModel.Purchase(PurchaseID.PlayerInventoryExpandGold, OnPurchaseInventoryExpandGold);
            }
        }

        private void OnPurchaseInventoryExpandWatch(PurchaseResult purchaseResult)
        {
            if (purchaseResult == PurchaseResult.Successfully)
            {
                PurchaseInventory();
            }
        }

        private void OnPurchaseInventoryExpandGold(PurchaseResult purchaseResult)
        {
            if (purchaseResult == PurchaseResult.Successfully)
            {
                PurchaseInventory();
            }
            else
            {
                ViewsSystem.Show<PurchasesView>(ViewConfigID.Purchases);
            }
        }

        private void PurchaseInventory()
        {
            InventoryModel.ItemsContainer.AddCells(InventoryViewModel.CellsExpanded);

            InventoryViewModel.ExpandCellsOnce();

            for (var index = 0; index < CraftModel.CountCachedCraftedItems; index++)
            {
                var item = CraftModel.PopCraftCompleted();
                var result = InventoryOperationsModel.TryAddItemsToPlayer(item.ItemData,item.Count);

                if(!result.allAdded)
                    break;
            }

            if (CraftModel.CountCachedCraftedItems > 0)
            {
                if (InventoryViewModel.IsMaxExpandLevel)
                {
                    DropCached();
                }
                else
                {
                    OpenInventoryFillPopup();
                }
            }
        }
    }
}
