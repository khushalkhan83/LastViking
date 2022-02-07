using Core;
using Core.Controllers;
using Game.Audio;
using Game.Models;
using Game.Models.RemoteSettings;
using Game.Purchases;
using Game.Views;
using System.Linq;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers
{
    public class PickUpTimeDelayHealthCursorViewController : ViewControllerBase<PickUpTimeDelayHealthCursorView>
    {
        [Inject] public RemoteSettingsModel RemoteSettingsModel { get; private set; }
        [Inject] public InventoryViewModel InventoryViewModel { get; private set; }
        [Inject] public WorldObjectCreator WorldObjectCreator { get; private set; }
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public ActionsLogModel ActionsLogModel { get; private set; }
        [Inject] public StatisticsModel StatisticsModel { get; private set; }
        [Inject] public PurchasesModel PurchasesModel { get; private set; }
        [Inject] public InventoryModel InventoryModel { get; private set; }
        [Inject] public NetworkModel NetworkModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public HotBarModel HotBarModel { get; private set; }
        [Inject] public AudioSystem AudioSystem { get; private set; }
        [Inject] public ItemsDB ItemsDB { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }

        protected InventoryIsFillPopupView InventoryIsFillPopupView { get; private set; }
        protected RestorableObject RestorableObject { get; private set; }

        protected override void Show()
        {
            GameUpdateModel.OnUpdate += OnUpdate;

            RestorableObject = PlayerEventHandler.RaycastData.Value.GameObject.GetComponent<RestorableObject>();
            RestorableObject.Health.OnChangeHealth += OnChangeHealthHandler;
            UpdateView();
        }

        protected override void Hide()
        {
            GameUpdateModel.OnUpdate -= OnUpdate;
            RestorableObject.Health.OnChangeHealth -= OnChangeHealthHandler;
        }

        private void OnUpdate()
        {
            if(PlayerInput.Instance.AttackTap)
            {
                OnDownHandler();
            }
        }

        private void OnChangeHealthHandler() => UpdateView();

        private void OnDownHandler()
        {
            var itemPickUpTimeDelayModel = PlayerEventHandler.RaycastData.Value.GameObject.GetComponentInParent<ItemPickUpTimeDelayModel>();

            var itemFrom = GetItem(itemPickUpTimeDelayModel.ItemName, itemPickUpTimeDelayModel.ItemCount);
            var left = itemFrom.Count;

            if (itemFrom.IsCanStackable)
            {
                left = InventoryModel.ItemsContainer.AddItems(itemFrom.Id, itemFrom.Count);
                if (left > 0)
                {
                    left = HotBarModel.ItemsContainer.AddItems(itemFrom.Id, itemFrom.Count);
                }
            }
            else
            {
                var cellTo = GetEmptyCell(InventoryModel.ItemsContainer);
                if (cellTo == null)
                {
                    cellTo = GetEmptyCell(HotBarModel.ItemsContainer);
                }

                if (cellTo != null)
                {
                    left = 0;
                    cellTo.Item = itemFrom;
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

                itemPickUpTimeDelayModel.StartSpawn();
            }

            if (left > 0)
            {
                itemFrom.Count = left;
                DropItem(itemFrom);

                if (!InventoryViewModel.IsMaxExpandLevel)
                {
                    ShowInventoryFillPopup();
                }
            }
        }

        private void DropItem(SavableItem item)
        {
            var pTfm = PlayerEventHandler.transform;
            var itemPickup = WorldObjectCreator.Create(item.ItemData.WorldObjectID, pTfm.position + pTfm.forward + pTfm.up, Quaternion.identity).GetComponentInChildren<ItemPickup>();
            itemPickup.SetItemToAdd(item);
        }

        private SavableItem GetItem(string name, int count) => new SavableItem(ItemsDB.GetItem(name), count);

        private CellModel GetEmptyCell(ItemsContainer itemsContainer) => itemsContainer.Cells.FirstOrDefault(x => !x.IsHasItem);

        private void ShowInventoryFillPopup()
        {
            InventoryIsFillPopupView = ViewsSystem.Show<InventoryIsFillPopupView>(ViewConfigID.InventoryIsFillPopup);
            InventoryIsFillPopupView.OnBack += OnBackInventoryIsFillPopupViewHandler;
            InventoryIsFillPopupView.OnExpand += OnExpandInventoryIsFillPopupViewHandler;
        }

        private void HideInventoryFillPopup()
        {
            InventoryIsFillPopupView.OnBack -= OnBackInventoryIsFillPopupViewHandler;
            InventoryIsFillPopupView.OnExpand -= OnExpandInventoryIsFillPopupViewHandler;
            ViewsSystem.Hide(InventoryIsFillPopupView);
            InventoryIsFillPopupView = null;
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

        private void UpdateView() => UpdateView(RestorableObject.Health.Health, RestorableObject.Health.HealthMax);

        private void UpdateView(float health, float healthMax)
        {
            View.SetFillAmount(health / healthMax);
            View.SetText($"{health:F0}/{healthMax:F0}");
        }
    }
}
