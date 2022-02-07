using Core;
using Core.Controllers;
using Core.Views;
using Game.Audio;
using Game.Models;
using Game.Models.RemoteSettings;
using Game.Purchases;
using Game.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using UltimateSurvival;
using UnityEngine;
using UnityEngine.EventSystems;
using static Game.Models.InventoryOperationsModel;
using CategoryID = Game.Models.CraftViewModel.CategoryID;
using CellInfo = Game.Models.CraftViewModel.CellInfo;
using ContainerID = Game.Models.CraftViewModel.ContainerID;

namespace Game.Controllers
{
    public class CraftViewController : WindowViewControllerBase<CraftView>
    {
        [Inject] public InfoPanelMedicineViewModel InfoPanelMedicineViewModel { get; private set; }
        [Inject] public InfoPanelDefenceViewModel InfoPanelDefenceViewModel { get; private set; }
        [Inject] public InfoPanelWeaponViewModel InfoPanelWeaponViewModel { get; private set; }
        [Inject] public InfoPanelToolViewModel InfoPanelToolViewModel { get; private set; }
        [Inject] public InfoPanelFoodViewModel InfoPanelFoodViewModel { get; private set; }
        [Inject] public RemoteSettingsModel RemoteSettingsModel { get; private set; }
        [Inject] public InfoPanelViewModel InfoPanelViewModel { get; private set; }
        [Inject] public LocalizationModel LocalizationModel { get; private set; }
        [Inject] public PurchasesModel PurchasesModel { get; private set; }
        [Inject] public InventoryModel InventoryModel { get; private set; }
        [Inject] public CraftViewModel CraftViewModel { get; private set; }
        [Inject] public NetworkModel NetworkModel { get; private set; }
        [Inject] public HotBarModel HotBarModel { get; private set; }
        [Inject] public CoinsModel CoinsModel { get; private set; }
        [Inject] public CraftModel CraftModel { get; private set; }
        [Inject] public ItemsDB ItemsDB { get; private set; }
        [Inject] public BluePrintsModel BluePrintsModel { get; private set; }
        [Inject] public SheltersModel SheltersModel { get; private set; }
        [Inject] public InventoryOperationsModel InventoryOperationsModel { get; private set; }

        protected IList<CraftCellView> CraftCellViews { get; } = new List<CraftCellView>();

        protected AddCellGoldView AddCellGoldView { get; private set; }
        protected AddCellWatchView AddCellWatchView { get; private set; }
        protected BoostButtonGoldView BoostButtonGoldView { get; private set; }
        protected BoostButtonWatchView BoostButtonWatchView { get; private set; }

        protected bool IsAddQueueCellButtonWatch => GetIsAddQueueCellButtonWatch(WatchPurchaseCraftSlotBuy);
        protected bool IsBoostButtonWatch => GetIsBoostButtonWatch(WatchPurchaseCraftBoost);

        protected bool GetIsAddQueueCellButtonWatch(IWatchPurchase purchase) => !RemoteSettingsCraft.IsBuySlotGold && (purchase.IsCanPurchase || NetworkModel.IsHasConnection);
        protected bool GetIsBoostButtonWatch(IWatchPurchase watchPurchase) => !RemoteSettingsCraft.IsBoostGold && (watchPurchase.IsCanPurchase || NetworkModel.IsHasConnection);

        protected CraftRemoteSettings RemoteSettingsCraft => RemoteSettingsModel.Get<CraftRemoteSettings>(RemoteSettingID.Craft);

        protected IWatchPurchase WatchPurchaseCraftSlotBuy => PurchasesModel.GetInfo<IWatchPurchase>(PurchaseID.CraftSlotBuyWatch);
        protected IWatchPurchase WatchPurchaseCraftBoost => PurchasesModel.GetInfo<IWatchPurchase>(PurchaseID.CraftBoostWatch);

        private int shelterLevel;
        private bool isCanStudy;
        private bool isCanStudyDisable;

        private IEnumerable<ItemData> GetCurrentCategoryItems() => CraftModel.GetItemsByCategory(CraftViewModel.CategorySelected);

        #region MonoBehaviour
        // TODO: check this logic
        private void Update()
        {
            FinalizationQueueViews();
            InitializationQueueViews();
        }
        #endregion


        protected override void Show()
        {
            base.Show();

            InputModel.OnInput.AddListener(PlayerActions.UIMenu_CategoryLeft,OnSlectCategoryLeftHandler);
            InputModel.OnInput.AddListener(PlayerActions.UIMenu_CategoryRight,OnSlectCategoryRightHandler);

            AudioSystem.PlayOnce(AudioID.WindowOpen);
            LocalizationModel.OnChangeLanguage += SetLocalization;

            View.OnClose += OnCloseHandler;
            View.SetIsVisibleDefaultDescription(true);

            View.OnClickCategoryAll += OnClickCategoryAllHandler;
            View.OnClickCategoryItems += OnClickCategoryItemsHandler;
            View.OnClickCategoryTools += OnClickCategoryToolsHandler;
            View.OnClickCategoryDefence += OnClickCategoryDefenceHandler;
            View.OnClickCategoryMedical += OnClickCategoryMedicalHandler;
            View.OnClickCategoryWeapons += OnClickCategoryWeaponsHandler;

            View.OnCancel += OnClickCancelHandler;
            View.OnCraft += OnClickCraftHandler;
            View.OnStudy += OnClickStudyHandler;
            View.OnAddCoins += OnAddCoinsHandler;

            CraftModel.OnUpgrade += OnUpgradeCraftHandler;
            CraftModel.OnChangeCount += OnChangeCountCraftHandler;

            CraftModel.OnStartBoost += OnStartBoostHandler;
            CraftModel.OnEndBoost += OnEndBoostHandler;
            CraftModel.OnUpdateBoostRemainingTime += OnUpdateBoostRemainingTimeHandler;

            InventoryModel.ItemsContainer.OnChangeCell += OnChangeCellInventoryHandler;

            CraftViewModel.OnChangeCatgory += OnChangeCategoryHandler;
            CraftViewModel.OnChangeSelected += OnChangeSelectedHandler;
            CraftViewModel.OnGetCell += OnGetCellHandler;
            CraftViewModel.OnGetCraftButton += OnGetCraftButtonHandler;
            CraftViewModel.OnGetBoostButton += OnGetBoostButtonHandler;

            CoinsModel.OnChange += OnChangeCoins;
            BluePrintsModel.OnChange += OnChangeBluePrints;

            InitializationCraftViews();
            InitializationQueueViews();

            foreach (var item in View.ResourceCells)
            {
                item.OnPointDown_ += OnPointDownResourceViewHandler;
            }

            NetworkModel.OnInternetConnectionStateChange += HandleInternetConnection;
            NetworkModel.UpdateInternetConnectionStatus();

            OnChangeSelectedHandler(CraftViewModel.SelectedCell);

            ShowBoostButton();
            ShowAddQueueCellButton();

            UpdateCoins();
            UpdateBluePrints();

            SetLocalization();

            if (SheltersModel.ShelterModel != null)
            {
                if (SheltersModel.ShelterModel.Level > CraftViewModel.ShelterLevelMax)
                {
                    CraftViewModel.SetShelterMax(SheltersModel.ShelterModel.Level);
                }
            }
        }

        protected override void Hide()
        {
            base.Hide();
            View.OnClose -= OnCloseHandler;

            InputModel.OnInput.RemoveListener(PlayerActions.UIMenu_CategoryLeft,OnSlectCategoryLeftHandler);
            InputModel.OnInput.RemoveListener(PlayerActions.UIMenu_CategoryRight,OnSlectCategoryRightHandler);

            View.OnClickCategoryAll -= OnClickCategoryAllHandler;
            View.OnClickCategoryItems -= OnClickCategoryItemsHandler;
            View.OnClickCategoryTools -= OnClickCategoryToolsHandler;
            View.OnClickCategoryDefence -= OnClickCategoryDefenceHandler;
            View.OnClickCategoryMedical -= OnClickCategoryMedicalHandler;
            View.OnClickCategoryWeapons -= OnClickCategoryWeaponsHandler;

            View.OnCancel -= OnClickCancelHandler;
            View.OnCraft -= OnClickCraftHandler;
            View.OnStudy -= OnClickStudyHandler;
            View.OnAddCoins -= OnAddCoinsHandler;

            NetworkModel.OnInternetConnectionStateChange -= HandleInternetConnection;

            LocalizationModel.OnChangeLanguage -= SetLocalization;

            CraftModel.OnUpgrade -= OnUpgradeCraftHandler;
            CraftModel.OnChangeCount -= OnChangeCountCraftHandler;
            CraftModel.OnStartBoost -= OnStartBoostHandler;
            CraftModel.OnEndBoost -= OnEndBoostHandler;
            CraftModel.OnUpdateBoostRemainingTime -= OnUpdateBoostRemainingTimeHandler;

            CoinsModel.OnChange -= OnChangeCoins;
            BluePrintsModel.OnChange -= OnChangeBluePrints;

            InventoryModel.ItemsContainer.OnChangeCell -= OnChangeCellInventoryHandler;

            CraftViewModel.OnChangeCatgory -= OnChangeCategoryHandler;
            CraftViewModel.OnChangeSelected -= OnChangeSelectedHandler;
            CraftViewModel.OnGetCell -= OnGetCellHandler;
            CraftViewModel.OnGetCraftButton -= OnGetCraftButtonHandler;
            CraftViewModel.OnGetBoostButton -= OnGetBoostButtonHandler;

            FinalizationCraftViews();
            FinalizationQueueViews();

            foreach (var item in View.ResourceCells)
            {
                item.OnPointDown_ -= OnPointDownResourceViewHandler;
            }

            HideAddQueueCellButton();
            HideBoostButton();

            HideDescrition();
            View.SetIsVisibleDefaultDescription(false);
        }

        public IView DescriptiuonView { get; private set; }

        private void UpdateDesctiption<T>(ViewConfigID viewConfigID, IDataViewController data = null) where T : Component, IView
        {
            if (!(DescriptiuonView is T))
            {
                HideDescrition();
                ShowDescrition<T>(viewConfigID, data);
            }
        }

        private void ShowDescrition<T>(ViewConfigID viewConfigID, IDataViewController data = null) where T : Component, IView
        {
            DescriptiuonView = ViewsSystem.Show<T>(viewConfigID, View.Container, data);
            ((T)DescriptiuonView).transform.localPosition = Vector3.zero;
        }

        private void HideDescrition()
        {
            if (DescriptiuonView != null)
            {
                ViewsSystem.Hide(DescriptiuonView);
                DescriptiuonView = null;
            }
        }

        private void HideDefaultDescription() => View.SetIsVisibleDefaultDescription(false);

        private void HideBoostButton()
        {
            HideBoostButtonGold();
            HideBoostButtonWatch();
        }

        private void HideBoostButtonWatch()
        {
            if (BoostButtonWatchView != null)
            {
                BoostButtonWatchView.OnClick -= OnClickBoostHandler;
                ViewsSystem.Hide(BoostButtonWatchView);
                BoostButtonWatchView = null;
            }
        }

        private void HideBoostButtonGold()
        {
            if (BoostButtonGoldView != null)
            {
                BoostButtonGoldView.OnClick -= OnClickBoostHandler;
                ViewsSystem.Hide(BoostButtonGoldView);
                BoostButtonGoldView = null;
            }
        }

        private void UpdateBoostButon()
        {
            HideBoostButton();
            ShowBoostButton();
        }

        private void ShowBoostButton()
        {
            var watchPurchase = WatchPurchaseCraftBoost;
            if (GetIsBoostButtonWatch(watchPurchase))
            {
                watchPurchase.Prepere();
                ShowBoostButtonWatch();
            }
            else
            {
                ShowBoostButtonGold();
            }
        }

        private void ShowBoostButtonWatch()
        {
            BoostButtonWatchView = ViewsSystem.Show<BoostButtonWatchView>(ViewConfigID.BoostWatchButton, View.ContainerBoostButtonView);
            BoostButtonWatchView.transform.localPosition = Vector3.zero;
            BoostButtonWatchView.OnClick += OnClickBoostHandler;
            BoostButtonWatchView.SetIsInteractable(!CraftModel.IsBoostNow);
            BoostButtonWatchView.SetIsVisibleHandler(CraftModel.IsBoostNow);
            BoostButtonWatchView.SetFillAmount(GetBoostFillAmmount());
            BoostButtonWatchView.SetText(LocalizationModel.GetString(LocalizationKeyID.CraftMenu_BoostBtn));
        }

        private void ShowBoostButtonGold()
        {
            BoostButtonGoldView = ViewsSystem.Show<BoostButtonGoldView>(ViewConfigID.BoostGoldButton, View.ContainerBoostButtonView);
            BoostButtonGoldView.transform.localPosition = Vector3.zero;
            BoostButtonGoldView.OnClick += OnClickBoostHandler;
            BoostButtonGoldView.SetIsInteractable(!CraftModel.IsBoostNow);
            BoostButtonGoldView.SetIsVisibleHandler(CraftModel.IsBoostNow);
            BoostButtonGoldView.SetFillAmount(GetBoostFillAmmount());
            UpdateBoostButtonGoldText();
            BoostButtonGoldView.SetText(LocalizationModel.GetString(LocalizationKeyID.CraftMenu_BoostBtn));
        }

        private void UpdateBoostButtonGoldText() => BoostButtonGoldView.SetCount(PurchasesModel.GetInfo<IPurchaseCoinInfo>(PurchaseID.CraftBoostGold).CoinCost.ToString());

        private float GetBoostFillAmmount()
        {
            if (CraftModel.IsBoostNow)
            {
                return CraftModel.BoostRemainingTime / CraftModel.BoostTime;
            }

            return 1;
        }

        private void HandleInternetConnection() => UpdateRemoteButton();

        private void UpdateRemoteButton()
        {
            UpdateBoostButon();
            UpdateAddQueueCellButton();
        }

        private void UpdateAddQueueCellButton()
        {
            HideAddQueueCellButton();
            ShowAddQueueCellButton();
        }

        private void ShowAddQueueCellButton()
        {
            if (!CraftModel.IsMaxLevelUpgrade)
            {
                var watchPurchase = WatchPurchaseCraftSlotBuy;
                if (GetIsAddQueueCellButtonWatch(watchPurchase))
                {
                    watchPurchase.Prepere();
                    ShowAddQueueCellButtonWatch();
                }
                else
                {
                    ShowAddQueueCellButtonGold();
                }
            }
        }

        private void ShowAddQueueCellButtonGold()
        {
            AddCellGoldView = ViewsSystem.Show<AddCellGoldView>(ViewConfigID.AddCellGold, View.ContainerAddCraftCellButton);
            AddCellGoldView.transform.localPosition = Vector3.zero;
            AddCellGoldView.OnClick += OnClickAddQueueCellHandler;
            AddCellGoldView.SetTextGoldValue(PurchasesModel.GetInfo<IPurchaseCoinInfo>(PurchaseID.CraftSlotBuyGold).CoinCost.ToString());
        }

        private void ShowAddQueueCellButtonWatch()
        {
            AddCellWatchView = ViewsSystem.Show<AddCellWatchView>(ViewConfigID.AddCellWatch, View.ContainerAddCraftCellButton);
            AddCellWatchView.transform.localPosition = Vector3.zero;
            AddCellWatchView.OnClick += OnClickAddQueueCellHandler;
        }

        private void HideAddQueueCellButton()
        {
            HideAddQueueCellButtonGold();
            HideAddQueueCellButtonWatch();
        }

        private void HideAddQueueCellButtonWatch()
        {
            if (AddCellWatchView != null)
            {
                AddCellWatchView.OnClick -= OnClickAddQueueCellHandler;
                ViewsSystem.Hide(AddCellWatchView);
                AddCellWatchView = null;
            }
        }

        private void HideAddQueueCellButtonGold()
        {
            if (AddCellGoldView != null)
            {
                AddCellGoldView.OnClick -= OnClickAddQueueCellHandler;
                ViewsSystem.Hide(AddCellGoldView);
                AddCellGoldView = null;
            }
        }

        private void OnAddCoinsHandler() => ViewsSystem.Show<PurchasesView>(ViewConfigID.Purchases);

        private void OnChangeCoins() => UpdateCoins();

        private void OnChangeBluePrints() => UpdateBluePrints();

        private void UpdateCoins() => View.SetCoins(CoinsModel.Coins.ToString());

        private void UpdateBluePrints() => View.SetBluePrints(BluePrintsModel.BluePrints.ToString());

        private void OnPointDownResourceViewHandler(ResourceCellView view, PointerEventData eventData)
        {
            if (view.ItemId >= 0)
            {
                CraftViewModel.SelectCell(ContainerID.Resource, view.CellId, view.ItemId);

                if (view.ItemId == CraftViewModel.SelectedCellLast.itemId)
                {
                    var craftCellView = CraftCellViews.FirstOrDefault(x => x.ItemId == view.ItemId);
                    if (craftCellView != null)
                    {
                        CraftViewModel.SelectCell(ContainerID.Craft, craftCellView.CellId, craftCellView.ItemId);
                    }
                }
            }
        }

        private void OnUpdateBoostRemainingTimeHandler() => BoostButtonFillAmount(CraftModel.BoostRemainingTime / CraftModel.BoostTime);

        private void OnEndBoostHandler()
        {
            BoostButtonIsVisibleHandler(false);
            BoostButtonFillAmount(1);
            BoostButtonIsInteractable(true);
        }

        private void OnStartBoostHandler()
        {
            AudioSystem.PlayOnce(AudioID.BoostButton);
            BoostButtonIsVisibleHandler(true);
            BoostButtonFillAmount(1);
            BoostButtonIsInteractable(false);
        }

        private void BoostButtonIsInteractable(bool isVisible)
        {
            BoostButtonGoldView?.SetIsInteractable(isVisible);
            BoostButtonWatchView?.SetIsInteractable(isVisible);
        }

        private void BoostButtonIsVisibleHandler(bool isVisible)
        {
            BoostButtonGoldView?.SetIsVisibleHandler(isVisible);
            BoostButtonWatchView?.SetIsVisibleHandler(isVisible);
        }

        private void BoostButtonFillAmount(float amount)
        {
            BoostButtonGoldView?.SetFillAmount(amount);
            BoostButtonWatchView?.SetFillAmount(amount);
        }

        private void OnClickBoostHandler()
        {
            if (IsBoostButtonWatch)
            {
                PurchasesModel.Purchase(PurchaseID.CraftBoostWatch, OnPurchaseBoostWatch);
            }
            else
            {
                PurchasesModel.Purchase(PurchaseID.CraftBoostGold, OnPurchaseBoostGold);
            }
        }

        private void OnPurchaseBoostWatch(PurchaseResult purchaseResult)
        {
            if (purchaseResult == PurchaseResult.Successfully)
            {
                CraftModel.Boost();
            }
        }

        private void OnPurchaseBoostGold(PurchaseResult purchaseResult)
        {
            if (purchaseResult == PurchaseResult.Successfully)
            {
                CraftModel.Boost();
            }
            else
            {
                ViewsSystem.Show<PurchasesView>(ViewConfigID.Purchases);
            }
        }

        private void OnChangeCellInventoryHandler(CellModel cell)
        {
            var craftableItems = GetCurrentCategoryItems();

            var cellId = 0;
            foreach (var item in craftableItems)
            {
                CraftCellViews[cellId].SetData(GetCraftCellData(item, cellId));
                ++cellId;
            }
        }

        private void FinalizationQueueViews()
        {
            foreach (var queueCellView in View.QueueCells)
            {
                queueCellView.OnPointDown_ -= OnPointDownQueueViewHandler;
            }
        }

        private void FinalizationCraftViews()
        {
            foreach (var craftCellView in CraftCellViews)
            {
                ViewsSystem.Hide(craftCellView);
                craftCellView.OnPointDown_ -= OnPointDownCtaftViewHandler;
            }
            CraftCellViews.Clear();
        }

        private void InitializationCraftViews()
        {
            IEnumerable<ItemData> craftableItems = GetCurrentCategoryItems();

            var cellId = 0;
            foreach (var item in craftableItems)
            {
                var instance = ViewsSystem.Show<CraftCellView>(ViewConfigID.CraftCell, View.ContainerCraftItems);

                instance.SetData(GetCraftCellData(item, cellId));
                instance.OnPointDown_ += OnPointDownCtaftViewHandler;

                CraftCellViews.Add(instance);

                ++cellId;
            }
        }

        private void OnChangeCountCraftHandler()
        {
            View.SetIsVisibleBoostButton(CraftModel.Queue.Count > 0 || CraftModel.Craft.Count > 0);
            UpdateBoostButtonGoldText();

            if (CraftViewModel.SelectedCell.containerID == ContainerID.Queue)
            {
                if (CraftViewModel.SelectedCell.cellId < CraftModel.CountCells)
                {
                    var cellId = CraftViewModel.SelectedCell.cellId;
                    if (cellId < CraftModel.Craft.Count)
                    {
                        CraftViewModel.SelectCell(CraftViewModel.SelectedCell.containerID, CraftViewModel.SelectedCell.cellId, CraftModel.Craft[cellId].ItemData.Id);
                    }
                    else
                    {
                        CraftViewModel.SelectCell(CraftViewModel.SelectedCell.containerID, CraftViewModel.SelectedCell.cellId, CraftModel.Queue[cellId - CraftModel.Craft.Count].Id);
                    }
                }
                else
                {
                    CraftViewModel.RemoveSelectCell();
                }
            }
        }

        private void OnClickStudyHandler()
        {
            CraftModel.UnlockItem(CraftViewModel.SelectedCell.itemId);

            var item = ItemsDB.GetItem(CraftViewModel.SelectedCell.itemId);

            BluePrintsModel.Adjust(-item.UnlockablesItems[0].Amount);

            OnChangeSelectedHandler(CraftViewModel.SelectedCell);
        }

        private void InitializationQueueViews()
        {
            View.SetIsVisibleBoostButton(CraftModel.Queue.Count > 0 || CraftModel.Craft.Count > 0);
            var isLevelUpgradeLast = CraftModel.IsMaxLevelUpgrade;

            View.SetIsVisibleFirstQueueCellView(isLevelUpgradeLast);

            var cellsQueueData = GetQueueCellDatas();

            if (isLevelUpgradeLast)
            {
                View.QueueCells[0].SetBackgroundColor(View.QueueCells[0].BackgroundColorDefault);
                View.QueueCells[0].SetDefaultColor(View.QueueCells[0].BackgroundColorDefault);
                View.QueueCells[0].SetData(cellsQueueData[cellsQueueData.Length - 1]);
                if (cellsQueueData[cellsQueueData.Length - 1].ItemId >= 0)
                {
                    View.QueueCells[0].OnPointDown_ += OnPointDownQueueViewHandler;
                }

                for (int cellId = 0; cellId < cellsQueueData.Length - 1; cellId++)
                {
                    View.QueueCells[cellId + 1].SetBackgroundColor(View.QueueCells[cellId].BackgroundColorDefault);
                    View.QueueCells[cellId + 1].SetDefaultColor(View.QueueCells[cellId].BackgroundColorDefault);

                    View.QueueCells[cellId + 1].SetData(cellsQueueData[cellId]);

                    if (cellsQueueData[cellId].ItemId >= 0)
                    {
                        View.QueueCells[cellId + 1].OnPointDown_ += OnPointDownQueueViewHandler;
                    }
                }
            }
            else
            {
                for (int cellId = 0; cellId < cellsQueueData.Length; cellId++)
                {
                    if (cellId < CraftModel.CountCraftCellsAtLevel)
                    {
                        View.QueueCells[cellId + 1].SetBackgroundColor(View.QueueCells[cellId + 1].BackgroundColorDefault);
                        View.QueueCells[cellId + 1].SetDefaultColor(View.QueueCells[cellId + 1].BackgroundColorDefault);
                    }
                    else
                    {
                        View.QueueCells[cellId + 1].SetDefaultColor(View.QueueCells[cellId + 1].BackgroundColorNotBuyed);
                    }

                    View.QueueCells[cellId + 1].SetData(cellsQueueData[cellId]);
                    if (cellsQueueData[cellId].ItemId >= 0)
                    {
                        View.QueueCells[cellId + 1].OnPointDown_ += OnPointDownQueueViewHandler;
                    }
                }
            }
        }

        private void OnClickCancelHandler()
        {
            var item = ItemsDB.GetItem(CraftViewModel.SelectedCell.itemId);
            if(item.Recipe == null) return;

            InventoryOperationsModel.ItemConfig config = new InventoryOperationsModel.ItemConfig(null, AddedItemDestinationPriority.Inventory, true);
            InventoryOperationsModel.AddItemsToPlayer(item.Recipe.RequiredItems, null, config);

            var cellId = CraftViewModel.SelectedCell.cellId;
            if (cellId < CraftModel.Craft.Count)
            {
                CraftModel.RemoveItemFromCraft(cellId);
            }
            else
            {
                CraftModel.RemoveItemFromQueue(cellId - CraftModel.Craft.Count);
            }

            OnChangeSelectedHandler(CraftViewModel.SelectedCell);
            FinalizationQueueViews();
        }

        

        private void OnClickCraftHandler()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            var item = ItemsDB.GetItem(CraftViewModel.SelectedCell.itemId);
            if (CraftModel.TryAddItem(item))
            {
                if(item.Recipe != null)
                    InventoryOperationsModel.RemoveItemsFromPlayer(item.Recipe.RequiredItems);

                OnChangeSelectedHandler(CraftViewModel.SelectedCell);
            }
        }

        private void OnUpgradeCraftHandler()
        {
            UpdateAddQueueCellButton();
            UpdateBoostButtonGoldText();

            FinalizationQueueViews();
            InitializationQueueViews();
        }

        private void OnClickAddQueueCellHandler()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            if (IsAddQueueCellButtonWatch)
            {
                PurchasesModel.Purchase(PurchaseID.CraftSlotBuyWatch, OnPurchaseAddQueueCellWatch);
            }
            else
            {
                PurchasesModel.Purchase(PurchaseID.CraftSlotBuyGold, OnPurchaseAddQueueCellGold);
            }
        }

        private void OnPurchaseAddQueueCellWatch(PurchaseResult purchaseResult)
        {
            if (purchaseResult == PurchaseResult.Successfully)
            {
                CraftModel.Upgrade();
            }
        }

        private void OnPurchaseAddQueueCellGold(PurchaseResult purchaseResult)
        {
            if (purchaseResult == PurchaseResult.Successfully)
            {
                CraftModel.Upgrade();
            }
            else
            {
                ViewsSystem.Show<PurchasesView>(ViewConfigID.Purchases);
            }
        }

        private GameObject OnGetCellHandler(ContainerID containerID, int cellId)
        {
            return CraftCellViews[cellId].gameObject; // FIXME: out of range check
        }

        protected GameObject OnGetCraftButtonHandler() => View.CraftButton;
        protected GameObject OnGetBoostButtonHandler() => View.BoostButton;

        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        private void OnChangeSelectedHandler(CellInfo cellInfo)
        {
            RefreshCellView(CraftViewModel.SelectedCellLast);
            RefreshCellView(cellInfo);
            RefreshItemInfo(cellInfo);
            RefreshButtons(cellInfo);
            RefreshResourceCellViews(cellInfo);
        }
        private void RefreshResourceCellViews(CellInfo cellInfo)
        {
            switch (cellInfo.containerID)
            {
                case ContainerID.Craft:
                    RefreshResourceCellViewsCraft(cellInfo);
                    break;
                case ContainerID.Resource:
                    RefreshResourceCellViewsResource();
                    break;
                default:
                    RefreshResourceCellViewsDefault();
                    break;
            }
        }

        private void RefreshResourceCellViewsDefault()
        {
            for (var cellId = 0; cellId < View.ResourceCells.Length; cellId++)
            {
                View.ResourceCells[cellId].SetData(ResourceCellData.Empty);
            }
        }

        private void RefreshResourceCellViewsResource() => RefreshResourcecellView(ItemsDB.GetItem(CraftViewModel.SelectedCellCraftLast.itemId));

        private void RefreshResourceCellViewsCraft(CellInfo cellInfo) => RefreshResourcecellView(ItemsDB.GetItem(cellInfo.itemId));

        private void RefreshResourcecellView(ItemData itemData)
        {
            var isUnlocked = CraftModel.IsUnlocked(itemData.Id);
            var isLocked = itemData.IsUnlockable && !isUnlocked;
            var items = itemData.Recipe.RequiredItems;

            var cellId = 0;

            for (; cellId < items.Length; cellId++)
            {
                View.ResourceCells[cellId].SetData(GetResourceCelllData(cellId, ItemsDB.GetItem(items[cellId].Name), items[cellId].Amount, !isLocked));
            }
            for (; cellId < View.ResourceCells.Length; cellId++)
            {
                View.ResourceCells[cellId].SetData(ResourceCellData.Empty);
            }
        }

        private void RefreshButtons(CellInfo cellInfo)
        {
            switch (cellInfo.containerID)
            {
                case ContainerID.Craft:
                    RefreshButtonsCraft(cellInfo);
                    break;
                case ContainerID.Queue:
                    RefreshButtonsQueue();
                    break;
                default:
                    RefreshButtonsDefault();
                    break;
            }
        }

        private void RefreshButtonsDefault()
        {
            View.SetIsVisibleCancelButton(false);
            View.SetIsVisibleCraftButton(false);
            View.SetIsVisibleCraftDisableButton(false);
            View.SetIsVisibleStudyButton(false);
            View.SetIsVisibleStudyDisableButton(false);
        }

        private void RefreshButtonsQueue()
        {
            View.SetIsVisibleCancelButton(true);
            View.SetIsVisibleCraftButton(false);
            View.SetIsVisibleCraftDisableButton(false);
            View.SetIsVisibleStudyButton(false);
            View.SetIsVisibleStudyDisableButton(false);
        }

        private void RefreshButtonsCraft(CellInfo cellInfo)
        {
            var item = ItemsDB.GetItem(cellInfo.itemId);
            var isUnlocked = CraftModel.IsUnlocked(item.Id);
            var isLocked = item.IsUnlockable && !isUnlocked;

            if (item.IsHasProperty("RequiredShelterLevel"))
            {
                if (SheltersModel.ShelterModel != null || CraftViewModel.ShelterLevelMax > 0)
                {
                    shelterLevel = CraftViewModel.ShelterLevelMax;

                    var itemShelterLevel = item.GetProperty("RequiredShelterLevel");

                    isCanStudy = isLocked && item.UnlockablesItems[0].Amount <= BluePrintsModel.BluePrints && shelterLevel >= itemShelterLevel.Int.Current;

                    if (item.UnlockablesItems[0].Amount > BluePrintsModel.BluePrints)
                    {
                        isCanStudyDisable = true;
                    }
                    else
                    {
                        isCanStudyDisable = isLocked && shelterLevel < itemShelterLevel.Int.Current;
                    }
                }
                else
                {
                    isCanStudyDisable = true;
                }
            }
            else
            {
                isCanStudy = isLocked && item.UnlockablesItems[0].Amount <= BluePrintsModel.BluePrints;
                isCanStudyDisable = isLocked && item.UnlockablesItems[0].Amount > BluePrintsModel.BluePrints;
            }

            var isCanCraft = !isLocked && IsHasResources(item.Recipe.RequiredItems);

            View.SetIsVisibleCancelButton(false);
            View.SetIsVisibleCraftButton(isCanCraft);
            View.SetIsVisibleCraftDisableButton(!isCanCraft);
            View.SetIsVisibleStudyDisableButton(isCanStudyDisable);
            View.SetIsVisibleStudyButton(isCanStudy);

            if (isCanStudy)
            {
                View.SetStudyResourceIcon(ItemsDB.GetItem(item.UnlockablesItems[0].Name).Icon);
                View.SetStudyResourceCount(item.UnlockablesItems[0].Amount.ToString());
            }

            if (isCanStudyDisable)
            {
                View.SetStudyResourceDisableIcon(ItemsDB.GetItem(item.UnlockablesItems[0].Name).Icon);
                View.SetStudyResourceDisableCount(item.UnlockablesItems[0].Amount.ToString());
            }
        }

        private void SelectItem(ItemData itemData)
        {
            HideDescrition();
            HideDefaultDescription();

            if (itemData.TryGetProperty("ItemCategory", out var propertyCategory))
            {
                switch (propertyCategory.ItemCategoryID)
                {
                    case ItemCategoryID.Defence:
                        ShowItemInfoDefence(itemData);
                        break;
                    case ItemCategoryID.Medicine:
                        ShowItemInfoMedicine(itemData);
                        break;
                    case ItemCategoryID.Food:
                        ShowItemInfoFood(itemData);
                        break;
                    case ItemCategoryID.Weapon:
                        ShowItemInfoWeapon(itemData);
                        break;
                    case ItemCategoryID.Tool:
                        ShowItemInfoTool(itemData);
                        break;
                    case ItemCategoryID.Equipment:
                        ShowItemInfoEquipment(itemData);
                        break;
                }
            }
            else
            {
                ShowItemInfoOther(itemData);
            }
        }

        private void ShowItemInfoOther(ItemData item)
        {
            InfoPanelViewModel.SetTitle(item.DisplayNameKeyID);
            InfoPanelViewModel.SetDescription(item.DescriptionKeyID);

            UpdateDesctiption<InfoPanelView>(ViewConfigID.InfoPanel);
        }

        private void ShowItemInfoTool(ItemData item)
        {
            InfoPanelToolViewModel.SetTitle(item.DisplayNameKeyID);
            InfoPanelToolViewModel.SetDescription(item.DescriptionKeyID);
            InfoPanelToolViewModel.SetTool(item);

            UpdateDesctiption<InfoPanelToolView>(ViewConfigID.InfoPanelTool);
        }

        private void ShowItemInfoWeapon(ItemData item)
        {
            InfoPanelWeaponViewModel.SetTitle(item.DisplayNameKeyID);
            InfoPanelWeaponViewModel.SetDescription(item.DescriptionKeyID);
            InfoPanelWeaponViewModel.SetWeapon(item);

            UpdateDesctiption<InfoPanelWeaponView>(ViewConfigID.InfoPanelWeapon);
        }

        private void ShowItemInfoFood(ItemData item)
        {
            InfoPanelFoodViewModel.SetTitle(item.DisplayNameKeyID);
            InfoPanelFoodViewModel.SetDescription(item.DescriptionKeyID);
            InfoPanelFoodViewModel.SetFoodItem(item);

            UpdateDesctiption<InfoPanelFoodView>(ViewConfigID.InfoPanelFood);
        }

        private void ShowItemInfoMedicine(ItemData item)
        {
            InfoPanelMedicineViewModel.SetTitle(item.DisplayNameKeyID);
            InfoPanelMedicineViewModel.SetDescription(item.DescriptionKeyID);
            InfoPanelMedicineViewModel.SetHealth(item);

            UpdateDesctiption<InfoPanelMedicineView>(ViewConfigID.InfoPanelMedicine);
        }

        private void ShowItemInfoDefence(ItemData item)
        {
            InfoPanelDefenceViewModel.SetTitle(item.DisplayNameKeyID);
            InfoPanelDefenceViewModel.SetDescription(item.DescriptionKeyID);
            InfoPanelDefenceViewModel.SetHealthItem(item);

            UpdateDesctiption<InfoPanelDefenceView>(ViewConfigID.InfoPanelDefence);
        }

        private void ShowItemInfoEquipment(ItemData item)
        {
            InfoPanelEquipmentViewData data = new InfoPanelEquipmentViewData(item, false);
            UpdateDesctiption<InfoPanelEquipmentView>(ViewConfigID.InfoPanelEquipmentConfig, data);
        }

        private T ShowDescriptionView<T>(ViewConfigID viewConfigID) where T : Component, IView
        {
            var view = ViewsSystem.Show<T>(viewConfigID, View.Container);
            view.transform.localPosition = Vector3.zero;
            return view;
        }

        private void RefreshItemInfo(CellInfo cellInfo)
        {
            if (cellInfo.containerID == ContainerID.None)
            {
                HideDescrition();
            }
            else
            {
                SelectItem(ItemsDB.GetItem(cellInfo.itemId));
            }
        }

        private void RefreshCellView(CellInfo cellInfo)
        {
            switch (cellInfo.containerID)
            {
                case ContainerID.Craft:
                    RefreshCellViewCraft(cellInfo);
                    break;
                case ContainerID.Resource:
                    RefreshCellViewResource(cellInfo);
                    break;
            }
        }

        private void RefreshCellViewCraft(CellInfo cellInfo)
        {
            if (CraftCellViews.Count > cellInfo.cellId)
            {
                CraftCellViews[cellInfo.cellId].SetData(GetCraftCellData(ItemsDB.GetItem(cellInfo.itemId), cellInfo.cellId));
            }
        }

        private void RefreshCellViewResource(CellInfo cellInfo)
        {
            var cellCraft = CraftViewModel.SelectedCellCraftLast;
            if (cellCraft.containerID != ContainerID.None)
            {
                var itemCraft = ItemsDB.GetItem(cellCraft.itemId);
                var item = ItemsDB.GetItem(cellInfo.itemId);
                var isLocked = itemCraft.IsUnlockable && !CraftModel.IsUnlocked(itemCraft.Id);
                var itemsRecipe = itemCraft.Recipe.RequiredItems;
                var resourcesData = GetResourceCelllData(cellInfo.cellId, item, itemsRecipe[cellInfo.cellId].Amount, !isLocked);

                View.ResourceCells[cellInfo.cellId].SetData(resourcesData);
            }
        }

        private void OnPointDownCtaftViewHandler(CraftCellView cellView, PointerEventData eventData) => CraftViewModel.SelectCell(ContainerID.Craft, cellView.CellId, cellView.ItemId);

        private void OnPointDownQueueViewHandler(QueueCellView cellView, PointerEventData eventData) => CraftViewModel.SelectCell(ContainerID.Queue, cellView.CellId, cellView.ItemId);

        private void OnChangeCategoryHandler()
        {
            DeselectCategory(CraftViewModel.CategorySelectedLast);
            SelectCategory(CraftViewModel.CategorySelected);

            foreach (var craftCellView in CraftCellViews)
            {
                ViewsSystem.Hide(craftCellView);
            }
            CraftCellViews.Clear();

            var craftableItems = GetCurrentCategoryItems();

            var cellId = 0;
            foreach (var item in craftableItems)
            {
                var view = ViewsSystem.Show<CraftCellView>(ViewConfigID.CraftCell, View.ContainerCraftItems);

                view.SetData(GetCraftCellData(item, cellId));

                CraftCellViews.Add(view);

                ++cellId;
            }
        }

        private void OnSlectCategoryLeftHandler()
        {
            var newIndex = CraftViewModel.SelectedCategoryIndex - 1;
            if(newIndex >= 0 && newIndex <= CraftViewModel.MaxCategoryIndex)
                SetCategory(CraftViewModel.CategoryIdByIndex[newIndex]);
        }

        private void OnSlectCategoryRightHandler()
        {
            var newIndex = CraftViewModel.SelectedCategoryIndex + 1;
            if(newIndex >= 0 && newIndex <= CraftViewModel.MaxCategoryIndex)
                SetCategory(CraftViewModel.CategoryIdByIndex[newIndex]);
        }

        private void OnClickCategoryAllHandler() => SetCategory(CategoryID.None);

        private void OnClickCategoryDefenceHandler() => SetCategory(CategoryID.Defence);

        private void OnClickCategoryItemsHandler() => SetCategory(CategoryID.Items);

        private void OnClickCategoryMedicalHandler() => SetCategory(CategoryID.Medical);

        private void OnClickCategoryToolsHandler() => SetCategory(CategoryID.Tools);

        private void OnClickCategoryWeaponsHandler() => SetCategory(CategoryID.Weapons);

        private void SetCategory(CategoryID categoryID)
        {
            CraftViewModel.SelectCategory(categoryID);
        }


        private void SelectCategory(CategoryID categoryID) => SetCategoryColorView(categoryID)(View.CategoryColorSelected);

        private void DeselectCategory(CategoryID categoryID) => SetCategoryColorView(categoryID)(View.CategoryColorDefault);

        private Action<Color> SetCategoryColorView(CategoryID categoryID)
        {
            switch (categoryID)
            {
                case CategoryID.Tools:
                    return View.SetCategoryToolsColor;
                case CategoryID.Items:
                    return View.SetCategoryItemsColor;
                case CategoryID.Weapons:
                    return View.SetCategoryWeaponsColor;
                case CategoryID.Defence:
                    return View.SetCategoryDefenceColor;
                case CategoryID.Medical:
                    return View.SetCategoryMedicalColor;
            }

            return View.SetCategoryAllColor;
        }

        private void OnCloseHandler()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            ViewsSystem.Hide(View);
        }

        private CraftCellData GetCraftCellData(ItemData item, int cellId)
        {
            var isUnlocked = CraftModel.IsUnlocked(item.Id);
            var isLocked = item.IsUnlockable && !isUnlocked;
            var isEpic = item.PropertyValues.Any(x => x.Name == "IsEpic");
            var isActive = !isLocked && IsHasResources(item.Recipe.RequiredItems);
            var isCanUnlock = isLocked && InventoryModel.ItemsContainer.IsHasItems(ItemsDB.GetItem(item.UnlockablesItems[0].Name).Id, item.UnlockablesItems[0].Amount);
            var isSelected = CraftViewModel.SelectedCell.containerID == ContainerID.Craft && CraftViewModel.SelectedCell.cellId == cellId;

            return new CraftCellData()
            {
                CellId = cellId,

                ItemId = item.Id,
                Icon = item.Icon,
                Name = LocalizationModel.GetString(item.DisplayNameKeyID),

                IsSelected = isSelected,
                IsActive = isActive,
                IsEpic = isEpic,
                IsLocked = isLocked,
                IsCanUnlock = isCanUnlock
            };
        }

        private ResourceCellData GetResourceCelllData(int cellId, ItemData item, int count, bool isActive)
        {
            var countCurrent = InventoryModel.ItemsContainer.GetItemsCount(item.Id) + HotBarModel.ItemsContainer.GetItemsCount(item.Id);
            var isSelected = CraftViewModel.SelectedCell.containerID == ContainerID.Resource && CraftViewModel.SelectedCell.cellId == cellId;

            return new ResourceCellData()
            {
                ItemId = item.Id,
                CellId = cellId,
                Icon = item.Icon,
                Message = GetResourceMessage(count, isActive, countCurrent),
                IsActive = isActive,
                IsSelected = isSelected,
                ItemRarity = item.ItemRarity,
                IsComponent = item.Category == "Components",
            };
        }

        private string GetResourceMessage(int count, bool isActive, int countCurrent)
        {
            if (countCurrent < count)
            {
                if (isActive)
                {
                    return $"<color=#FF0000FF>{countCurrent}/{count}</color>";
                }
                else
                {
                    return $"<color=#FF00007F>{countCurrent}/{count}</color>";
                }
            }
            else
            {
                if (isActive)
                {
                    return $"<color=#FFECC4FF>{countCurrent}/{count}</color>";
                }
                else
                {
                    return $"<color=#FFECC47F>{countCurrent}/{count}</color>";
                }
            }
        }

        private bool IsHasResources(IEnumerable<RequiredItem> items) => items.All(item => InventoryModel.ItemsContainer.GetItemsCount(ItemsDB.GetItem(item.Name).Id) + HotBarModel.ItemsContainer.GetItemsCount(ItemsDB.GetItem(item.Name).Id) >= item.Amount);

        private bool IsHasResources(RequiredItem item) => InventoryModel.ItemsContainer.IsHasItems(ItemsDB.GetItem(item.Name).Id, item.Amount);

        private QueueCellData[] GetQueueCellDatas()
        {
            var result = new QueueCellData[CraftModel.CountAllQueueCells];
            var isCanSelect = CraftViewModel.SelectedCell.containerID == ContainerID.Queue;

            var cellId = 0;
            foreach (var craftItem in CraftModel.Craft)
            {
                var time = TimeSpan.FromSeconds(craftItem.RemainingTime);
                var timeLeft = string.Empty;

                if (time.Hours > 0)
                {
                    timeLeft = time.Hours + ":";
                }

                if (time.Minutes > 0)
                {
                    timeLeft += time.Minutes + ":";
                }

                timeLeft += time.Seconds;

                result[cellId] = new QueueCellData()
                {
                    CellId = cellId,
                    isBoost = CraftModel.IsBoostNow,
                    isBurn = true,
                    IsSelected = isCanSelect && CraftViewModel.SelectedCell.cellId == cellId,
                    ItemIcon = craftItem.ItemData.Icon,
                    ItemId = craftItem.ItemData.Id,
                    TimeLeft = timeLeft,
                    Progress = craftItem.RemainingTime / craftItem.AllTime
                };

                ++cellId;
            }

            foreach (var queueCell in CraftModel.Queue)
            {
                result[cellId] = new QueueCellData()
                {
                    CellId = cellId,
                    IsSelected = isCanSelect && CraftViewModel.SelectedCell.cellId == cellId,
                    ItemIcon = queueCell.Icon,
                    ItemId = queueCell.Id,
                    Progress = 0
                };

                ++cellId;
            }

            for (; cellId < CraftModel.CountAllQueueCells; cellId++)
            {
                result[cellId] = new QueueCellData()
                {
                    CellId = cellId,
                    IsSelected = isCanSelect && CraftViewModel.SelectedCell.cellId == cellId,
                    ItemId = -1,
                };
            }

            return result;
        }

        private void SetLocalization()
        {
            View.SetTextTitle(LocalizationModel.GetString(LocalizationKeyID.CraftMenu_Name));
            View.SetTextResourcesTitle(LocalizationModel.GetString(LocalizationKeyID.CraftMenu_ResourcesNeeded));
            View.SetTextResourcesQueueTitle(LocalizationModel.GetString(LocalizationKeyID.CraftMenu_CraftingQueue));
            View.SetTextCraftButton(LocalizationModel.GetString(LocalizationKeyID.CraftMenu_CraftBtn));
            View.SetTextStudyButton(LocalizationModel.GetString(LocalizationKeyID.CraftMenu_ExploreBtn));
            View.SetTextCancelButton(LocalizationModel.GetString(LocalizationKeyID.CraftMenu_CancelBtn));

            BoostButtonGoldView?.SetText(LocalizationModel.GetString(LocalizationKeyID.CraftMenu_BoostBtn));
            BoostButtonWatchView?.SetText(LocalizationModel.GetString(LocalizationKeyID.CraftMenu_BoostBtn));
        }
    }
}
