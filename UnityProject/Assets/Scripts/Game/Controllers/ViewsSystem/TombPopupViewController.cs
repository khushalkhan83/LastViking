using Core;
using Core.Controllers;
using Game.Audio;
using Game.Models;
using Game.Models.RemoteSettings;
using Game.Purchases;
using Game.Views;
using System.Collections.Generic;
using System.Linq;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers
{
    public class TombPopupViewController : ViewControllerBase<TombPopupView>
    {
        [Inject] public AudioSystem AudioSystem { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public LocalizationModel LocalizationModel { get; private set; }
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public GameTimeModel GameTimeModel { get; private set; }
        [Inject] public SheltersModel SheltersModel { get; private set; }
        [Inject] public PurchasesModel PurchasesModel { get; private set; }
        [Inject] public InventoryModel InventoryModel { get; private set; }
        [Inject] public HotBarModel HotBarModel { get; private set; }
        [Inject] public InventoryViewModel InventoryViewModel { get; private set; }
        [Inject] public ItemsDB ItemsDB { get; private set; }
        [Inject] public TombsModel TombsModel { get; private set; }
        [Inject] public PlayerProfileModel PlayerProfileModel { get; private set; }
        [Inject] public CoinsModel CoinsModel { get; private set; }
        [Inject] public RemoteSettingsModel RemoteSettingsModel { get; private set; }
        [Inject] public NetworkModel NetworkModel { get; private set; }
        [Inject] public PurchaseComplitedModel PurchaseComplitedModel { get; private set; }
        [Inject] public InventoryOperationsModel AddItemToPlayerModel { get; private set; }
        [Inject] public InputModel InputModel { get; private set; }
        [Inject] public BonusItemsModel BonusItemsModel { get; private set; }

        protected TombModel TombModel { get; private set; }
        protected InventoryIsFillPopupView InventoryIsFillPopupView { get; private set; }

        protected override void Show()
        {
            TombModel = PlayerEventHandler.RaycastData.Value.GameObject.GetComponentInParent<TombModel>();
            View.OnClose += OnCloseHandler;
            View.OnTakeGold += OnTakeGoldHandler;
            View.OnTakeWatch += OnTakeWatchHandler;
            InputModel.OnInput.AddListener(PlayerActions.Cancel,OnCloseHandler);

            LocalizationModel.OnChangeLanguage += OnChangeLanguageHandler;

            NetworkModel.OnInternetConnectionStateChange += OnInternetConnectionStateChangeHandler;
            NetworkModel.UpdateInternetConnectionStatus();

            Initialize();

            SetLocalization();
        }

        protected override void Hide()
        {
            View.OnClose -= OnCloseHandler;
            View.OnTakeGold -= OnTakeGoldHandler;
            View.OnTakeWatch -= OnTakeWatchHandler;
            InputModel.OnInput.RemoveListener(PlayerActions.Cancel,OnCloseHandler);

            NetworkModel.OnInternetConnectionStateChange -= OnInternetConnectionStateChangeHandler;

            LocalizationModel.OnChangeLanguage -= OnChangeLanguageHandler;
        }

        private void OnInternetConnectionStateChangeHandler()
        {
            if (NetworkModel.IsHasConnection)
            {
                PurchasesModel.GetInfo<IWatchPurchase>(PurchaseID.TombExitVideoInter).Prepere();
            }
        }

        private void Initialize()
        {
            var isHasOtherItems = View.TombItemCellViews.Length <= TombModel.Items.Count;
            var isHasShelter = TombModel.ShelterID != ShelterModelID.None;
            var purchase = PurchasesModel.GetInfo<IPurchaseCoinInfo>(PurchaseID.TombTakeItemsGold);
            bool hasBonus = true;

            View.SetTextKilledValue(TombModel.PlayerKilled.ToString());
            View.SetTextPlayerName(TombModel.PlayerName);
            View.SetImagePlayer(PlayerProfileModel.PlayerAvatar);
            View.SetPlayerName(PlayerProfileModel.PlayerName);
            View.SetTextTakeGoldButtonValue(purchase.CoinCost.ToString());
            View.SetIsVisibleOtherItemsInfo(isHasOtherItems);
            View.SetIsVisibleShelterInfo(isHasShelter);
            View.SetIsVisibleShelterNone(!isHasShelter);
            View.SetIsVisibleBonus(hasBonus);
            View.SetIsVisibleTakeGoldObject(hasBonus);

            if (hasBonus)
            {
                var item = BonusItemsModel.GetBonusItem();
                View.SetImageBonusItem(ItemsDB.GetItem(item).Icon);
            }

            if (isHasOtherItems)
            {
                View.SetTextOtherItemsCountValue($"+{TombModel.Items.Count - View.TombItemCellViews.Length}");
            }

            InitializeItems();
        }

        private void InitializeItems()
        {
            TombItemCellView tombItemCellView;
            SavableItem item;
            bool isVisibleCounter;
            int i;

            for (i = 0; i < View.TombItemCellViews.Length && i < TombModel.Items.Count; i++)
            {
                tombItemCellView = View.TombItemCellViews[i];
                item = TombModel.Items[i];

                isVisibleCounter = item.Count > 1;

                tombItemCellView.SetIsVisible(true);
                tombItemCellView.SetIsVisibleCount(isVisibleCounter);
                tombItemCellView.SetImageItem(ItemsDB.GetItem(item.Id).Icon);

                if (isVisibleCounter)
                {
                    tombItemCellView.SetTextCountItems(item.Count.ToString());
                }
            }

            for (; i < View.TombItemCellViews.Length; i++)
            {
                View.TombItemCellViews[i].SetIsVisible(false);
            }
        }

        private void OnTakeWatchHandler()
        {
            AudioSystem.PlayOnce(AudioID.PickUp);

            if (TryTombTakeItemsWatch() && !PurchaseComplitedModel.AnyPurchaseComplited)
            {
                PurchasesModel.Purchase(PurchaseID.TombExitVideoInter, default);
            }
        }

        private void OnTakeGoldHandler()
        {
            AudioSystem.PlayOnce(AudioID.PickUp);
            var tombTakeItemsGold = PurchasesModel.GetInfo<IPurchaseCoinInfo>(PurchaseID.TombTakeItemsGold);
            if (tombTakeItemsGold.CoinCost > CoinsModel.Coins)
            {
                ViewsSystem.Show<PurchasesView>(ViewConfigID.Purchases);
            }
            else
            {
                if (!HasSpaceForItems(1) && !InventoryViewModel.IsMaxExpandLevel)
                {
                    OpenInventoryFillPopup();
                }
                else
                {
                    PurchasesModel.Purchase(PurchaseID.TombTakeItemsGold, OnPurchaseTombTakeItemsGoldHandler);
                }
            }
        }

        private bool TryTombTakeItemsWatch()
        {
            var isAdded = AddItemsToPlayer(TombModel.Items, out var added);
            if (isAdded)
            {
                TombModel.ClearTombItems();
                ViewsSystem.Hide(View);
                TombModel.DestroyTomb();

                return true;
            }

            RemoveItemsFromTomb(added);
            Initialize();
            if (!InventoryViewModel.IsMaxExpandLevel)
            {
                OpenInventoryFillPopup();
            }

            return false;
        }

        private void OnPurchaseTombTakeItemsGoldHandler(PurchaseResult purchaseResult)
        {
            if (purchaseResult == PurchaseResult.Successfully)
            {
                Transform tombTransform = TombModel != null ? TombModel.transform : null;

                var item = BonusItemsModel.GetBonusItem();
                AddItemToPlayerModel.AddItemToPlayer(item, 1,tombTransform);
                BonusItemsModel.ReceiveBonus();

                bool bonusesLeft = true;
                if (!bonusesLeft)
                {
                    View.SetIsVisibleTakeGoldObject(true);
                }

                bool isAdded = AddItemsToPlayer(TombModel.Items, out var added);
                if (isAdded)
                {
                    TombModel.ClearTombItems();
                    ViewsSystem.Hide(View);
                    TombModel.DestroyTomb();

                    return;
                }
                else
                {
                    RemoveItemsFromTomb(added);
                    Initialize();
                    if (!InventoryViewModel.IsMaxExpandLevel)
                    {
                        OpenInventoryFillPopup();
                    }
                }
            }
        }

        private bool HasSpaceForItems(int count) => InventoryModel.ItemsContainer.GetEmptyCellsCount() + HotBarModel.ItemsContainer.GetEmptyCellsCount() >= count;

        private void RemoveItemsFromTomb(IEnumerable<SavableItem> toRemove)
        {
            foreach (var item in toRemove)
            {
                TombModel.RemoveTombItem(item);
            }
        }

        private bool AddItemsToPlayer(IEnumerable<SavableItem> items, out IEnumerable<SavableItem> added)
        {
            var addedItems = new List<SavableItem>();
            foreach (var item in items)
            {
                var left = InventoryModel.ItemsContainer.AddItemsData(item, item.Count);
                if (left > 0)
                {
                    left = HotBarModel.ItemsContainer.AddItemsData(item, item.Count);
                    if (left > 0)
                    {
                        added = addedItems;
                        return false;
                    }
                }
                addedItems.Add(item);
            }

            added = addedItems;
            return true;
        }

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
            if (InventoryIsFillPopupView != null)
            {
                InventoryIsFillPopupView.OnBack -= OnBackInventoryIsFillPopupViewHandler;
                InventoryIsFillPopupView.OnExpand -= OnExpandInventoryIsFillPopupViewHandler;

                ViewsSystem.Hide(InventoryIsFillPopupView);
                InventoryIsFillPopupView = null;
            }
        }

        private void OnBackInventoryIsFillPopupViewHandler() => CloseInventoryIsFillPopup();

        private void OnExpandInventoryIsFillPopupViewHandler()
        {
            CloseInventoryIsFillPopup();

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
        }

        private void OnChangeLanguageHandler() => SetLocalization();

        private void SetLocalization()
        {
            View.SetTextBonus(LocalizationModel.GetString(LocalizationKeyID.LoseMenu_Bonus));
            View.SetTextKilled(LocalizationModel.GetString(LocalizationKeyID.LoseMenu_Killed));
            View.SetTextKilledValue(TombModel.PlayerKilled.ToString());
            View.SetTextTakeWatchButton(LocalizationModel.GetString(LocalizationKeyID.LoseMenu_SaveMeBtn));
            View.SetTextTakeGoldButton(string.Format(LocalizationModel.GetString(LocalizationKeyID.LoseMenu_SaveMeBtn), TombModel.Items.Count));
            View.SetTextTimeAlive(LocalizationModel.GetString(LocalizationKeyID.LoseMenu_Time));
            View.SetTextTimeAliveValue(string.Format(LocalizationModel.GetString(LocalizationKeyID.LoseMenu_Day), GameTimeModel.GetDays(TombModel.PlayerLifeTime), GameTimeModel.GetHours(TombModel.PlayerLifeTime)));

            var isHasShelter = TombModel.ShelterID != ShelterModelID.None;

            if (isHasShelter)
            {
                View.SetTextShelter(LocalizationModel.GetString(SheltersModel.GetComponentsInChildren<ShelterModel>().Where(x => x.ShelterID == TombModel.ShelterID).First().NameKeyID));
                View.SetTextShelterValue(string.Format(LocalizationModel.GetString(LocalizationKeyID.LoseMenu_ShelterLvl), TombModel.ShelterLevel + 1));
            }
            else
            {
                View.SetTextShelterNone(LocalizationModel.GetString(LocalizationKeyID.LoseMenu_ShelterNone));
            }
        }

        private void OnCloseHandler()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            ViewsSystem.Hide(View);
        }
    }
}
