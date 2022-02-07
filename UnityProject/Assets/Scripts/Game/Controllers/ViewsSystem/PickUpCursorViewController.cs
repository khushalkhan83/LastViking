using Core;
using Core.Controllers;
using Extensions;
using Game.Audio;
using Game.Models;
using Game.Models.RemoteSettings;
using Game.Purchases;
using Game.Views;
using System.Linq;
using UltimateSurvival;

namespace Game.Controllers
{
    public class PickUpCursorViewController : ViewControllerBase<PickUpCursorView>
    {
        [Inject] public RemoteSettingsModel RemoteSettingsModel { get; private set; }
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public InventoryViewModel InventoryViewModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public ActionsLogModel ActionsLogModel { get; private set; }
        [Inject] public PurchasesModel PurchasesModel { get; private set; }
        [Inject] public StatisticsModel StatisticsModel { get; private set; }
        [Inject] public InventoryModel InventoryModel { get; private set; }
        [Inject] public NetworkModel NetworkModel { get; private set; }
        [Inject] public PickUpsModel PickUpsModel { get; private set; }
        [Inject] public HotBarModel HotBarModel { get; private set; }
        [Inject] public AudioSystem AudioSystem { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public ItemsDB ItemsDB { get; private set; }
        [Inject] public InputModel InputModel { get; private set; }

        protected InventoryIsFillPopupView InventoryIsFillPopupView { get; private set; }

        public DestroyItemTimeDelay DestroyItemTimeDelay { get; private set; }

        private void OnInputItemPickUpHandler()
        {
            var itemPickUp = PlayerEventHandler.RaycastData.Value?.GameObject.CheckNull()?.GetComponentInChildren<ItemPickup>();

            if(itemPickUp == null)
                return;

            var itemFrom = itemPickUp.ItemToAdd;
            var left = itemFrom.Count;
             
            if (itemFrom.IsCanStackable)
            {
                if (itemFrom.HasProperty("CraftResultDestinationInventory"))
                {
                    left = InventoryModel.ItemsContainer.AddItemsData(itemFrom, left);

                    if (left > 0)
                    {
                        left = HotBarModel.ItemsContainer.AddItemsData(itemFrom, left);
                    }
                }
                else
                {
                    left = HotBarModel.ItemsContainer.AddItemsData(itemFrom, left);

                    if (left > 0)
                    {
                        left = InventoryModel.ItemsContainer.AddItemsData(itemFrom, left);
                    }
                }
            }
            else
            {
                var cellTo = GetTargetCell();

                if (cellTo != null)
                {
                    left = 0;
                    cellTo.Item = itemFrom;
                }
            }

            CellModel GetTargetCell()
            {
                if (itemFrom.HasProperty("CraftResultDestinationInventory"))
                {
                    return GetEmptyCell(InventoryModel.ItemsContainer) ?? GetEmptyCell(HotBarModel.ItemsContainer);
                }
                else
                {
                    return GetEmptyCell(HotBarModel.ItemsContainer) ?? GetEmptyCell(InventoryModel.ItemsContainer);
                }
            }

            var added = itemFrom.Count - left;

            if (added > 0)
            {
                AudioSystem.PlayOnce(AudioID.PickUp);

                var item = new SavableItem(itemFrom)
                {
                    Count = added
                };

                var itemData = ItemsDB.GetItem(item.Name);
                ActionsLogModel.SendMessage(new MessageInventoryGatheredData(item.Count, itemData));
                StatisticsModel.GatherResource((uint)item.Count);
                PickUpsModel.PickUp(item.Name, added);
            }

            if (left > 0)
            {
                itemPickUp.ItemToAdd.Count = left;

                if (!InventoryViewModel.IsMaxExpandLevel)
                {
                    ShowInventoryFillPopup();
                }
            }
            else
            {
                itemPickUp.PickUp();
                View.OnInteract -= OnDownHandler;
            }
        }

        protected override void Show()
        {
            GameUpdateModel.OnUpdate += OnUpdate;
            DestroyItemTimeDelay = PlayerEventHandler.RaycastData.Value.GameObject.GetComponentInParent<DestroyItemTimeDelay>();
            InputModel.OnInput.AddListener(PlayerActions.ItemPickUp,OnDownHandler);
            if (DestroyItemTimeDelay != null)
            {
                View.SetTimerVisible(true);
                View.SetFillAmount(DestroyItemTimeDelay.WaitingTime / DestroyItemTimeDelay.DestroyTime);
            }
            else
            {
                View.SetTimerVisible(false);
            }
        }

        protected override void Hide()
        {
            GameUpdateModel.OnUpdate -= OnUpdate;
            View.SetTimerVisible(false);
            InputModel.OnInput.RemoveListener(PlayerActions.ItemPickUp,OnDownHandler);
        }

        private void OnUpdate()
        {
            if (DestroyItemTimeDelay != null)
            {
                View.SetFillAmount(DestroyItemTimeDelay.WaitingTime / DestroyItemTimeDelay.DestroyTime);
                View.SetTimerText(TimeToString(DestroyItemTimeDelay.WaitingTime));
            }

            if(PlayerInput.Instance.AttackTap)
            {
                OnInputItemPickUpHandler();
            }
        }

        private string TimeToString(float time) => System.TimeSpan.FromSeconds(time).ToString(@"mm\:ss");

        private void OnDownHandler()
        {
            OnInputItemPickUpHandler();
        }

        private CellModel GetEmptyCell(ItemsContainer itemsContainer) => itemsContainer.Cells.FirstOrDefault(x => !x.IsHasItem);

        private void ShowInventoryFillPopup()
        {
            InventoryIsFillPopupView = ViewsSystem.Show<InventoryIsFillPopupView>(ViewConfigID.InventoryIsFillPopup);
            InventoryIsFillPopupView.OnBack += OnBackInventoryIsFillPopupViewHandler;
            InventoryIsFillPopupView.OnExpand += OnExpandInventoryIsFillPopupViewHandler;
        }

        private void HideInventoryFillPopup()
        {
            if(InventoryIsFillPopupView != null)
            {
                InventoryIsFillPopupView.OnBack -= OnBackInventoryIsFillPopupViewHandler;
                InventoryIsFillPopupView.OnExpand -= OnExpandInventoryIsFillPopupViewHandler;
                ViewsSystem.Hide(InventoryIsFillPopupView);
                InventoryIsFillPopupView = null;
            }
        }

        private void OnExpandInventoryIsFillPopupViewHandler()
        {
            HideInventoryFillPopup();
            OnExpandInventoryHandler();
        }

        private void OnBackInventoryIsFillPopupViewHandler()
        {
            HideInventoryFillPopup();
        }

        private void OnExpandInventoryHandler()
        {
            var watchPurchase = PurchasesModel.GetInfo<IWatchPurchase>(PurchaseID.InventoryExpandWatch);
            var remouteSettingsInventory = RemoteSettingsModel.Get<InventoryRemoteSettings>(RemoteSettingID.Inventory);
            var isShowExpandInventoryWatch = !remouteSettingsInventory.IsBuySlotsGold && (watchPurchase.IsCanPurchase || NetworkModel.IsHasConnection);

            if (isShowExpandInventoryWatch)
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
                InventoryModel.ItemsContainer.AddCells(InventoryViewModel.CellsExpanded);

                InventoryViewModel.ExpandCellsOnce();
            }
        }

        private void OnPurchaseInventoryExpandGold(PurchaseResult purchaseResult)
        {
            if (purchaseResult == PurchaseResult.Successfully)
            {
                InventoryModel.ItemsContainer.AddCells(InventoryViewModel.CellsExpanded);

                InventoryViewModel.ExpandCellsOnce();
            }
            else
            {
                ViewsSystem.Show<PurchasesView>(ViewConfigID.Purchases);
            }
        }
    }
}
