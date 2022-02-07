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
using CellInfo = Game.Models.FurnaceViewModel.CellInfo;

namespace Game.Controllers
{
    public class FurnaceViewController : ViewControllerBase<FurnaceView>
    {
        [Inject] public AudioSystem AudioSystem { get; private set; }
        [Inject] public InventoryViewModel InventoryViewModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public FurnaceViewModel FurnaceViewModel { get; private set; }
        [Inject] public InventoryModel InventoryModel { get; private set; }
        [Inject] public HotBarModel HotBarModel { get; private set; }
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public ExpandsModel ExpandsModel { get; private set; }
        [Inject] public PlayerConsumeModel PlayerConsumeModel { get; private set; }
        [Inject] public CoinsModel CoinsModel { get; private set; }
        [Inject] public LocalizationModel LocalizationModel { get; private set; }
        [Inject] public PurchasesModel PurchasesModel { get; private set; }
        [Inject] public RemoteSettingsModel RemoteSettingsModel { get; private set; }
        [Inject] public NetworkModel NetworkModel { get; private set; }
        [Inject] public RepairingItemsModel RepairingItemsModel { get; private set; }
        [Inject] public RepairViewModel RepairViewModel { get; private set; }
        [Inject] public FurnaceGroupModel FurnaceGroupModel { get; private set; }
        [Inject] public StatsEffectModel StatsEffectModel { get; private set; }
        [Inject] public InfoPanelModel InfoPanelModel { get; private set; }
        [Inject] public DivideItemsModel DivideItemsModel { get; private set; }
        [Inject] public DropItemModel DropItemModel { get; private set; }
        [Inject] public ApplyItemModel ApplyItemModel { get; private set; }
        [Inject] public ExpandInventoryModel ExpandInventoryModel { get; private set; }
        [Inject] public FullInventoryModel FullInventoryModel { get; private set; }
        [Inject] public ContainersModel ContainersModel { get; private set; }
        [Inject] public InventoryRepairingModel InventoryRepairingModel { get; private set; }
        [Inject] public CollectCellDataModel CollectCellDataModel { get; private set; }
        [Inject] public InventoryDragAndDropModel InventoryDragAndDropModel { get; private set; } 
        [Inject] public TutorialModel TutorialModel { get; private set; }
        [Inject] public DeleteItemModel DeleteItemModel { get; private set; }

        protected FurnaceModel FurnaceModel { get; private set; }
        protected ExpandModel ExpandModel { get; private set; }
        protected AddCellGoldView AddCellGoldView { get; private set; }
        protected AddCellWatchView AddCellWatchView { get; private set; }
        protected BoostButtonGoldView BoostButtonGoldView { get; private set; }
        protected BoostButtonWatchView BoostButtonWatchView { get; private set; }

        protected bool IsWatchFurnaceSlotBuy => !RemoteSettingsFurnace.IsBuySlotGold && (WatchPurchaseFurnaceSlotBuy.IsCanPurchase || NetworkModel.IsHasConnection);
        protected bool IsWatchBoostButton => !RemoteSettingsFurnace.IsBoostGold && (WatchPurchaseBoost.IsCanPurchase || NetworkModel.IsHasConnection);

        protected bool IsCanShowInventoryExpandButton => InventoryViewModel.TotalExpandedCountLevel < InventoryViewModel.MaxExpandLevels;
        protected bool IsCanShowAddQueueCellButton => !ExpandModel.IsMaxLevel;

        protected FurnaceRemoteSettings RemoteSettingsFurnace => RemoteSettingsModel.Get<FurnaceRemoteSettings>(RemoteSettingID.Furnace);

        protected IWatchPurchase WatchPurchaseFurnaceSlotBuy => PurchasesModel.GetInfo<IWatchPurchase>(PurchaseID.FurnaceSlotBuyWatch);
        protected IWatchPurchase WatchPurchaseBoost => PurchasesModel.GetInfo<IWatchPurchase>(PurchaseID.FurnaceBoostWatch);

        protected IPurchaseCoinInfo PurchaseInfoFurnaceBoostGold => PurchasesModel.GetInfo<IPurchaseCoinInfo>(PurchaseID.FurnaceBoostGold);
        protected IPurchaseCoinInfo PurchaseInfoFurnaceSlotBuyGold => PurchasesModel.GetInfo<IPurchaseCoinInfo>(PurchaseID.FurnaceSlotBuyGold);

        protected float BoostFillAmount => FurnaceModel.IsBoost ? FurnaceModel.BoostRemainingTime / FurnaceModel.BoostTime : 1;

        private InventoryMainView InventoryMainView => View.InventoryMain;
        private InventoryFurnaceSectionView InventoryFurnaceSectionView => View.InventoryFurnaceSection;
        private InventoryHotBarView InventoryHotBarView => View.InventoryHotBar;

        protected override void Show()
        {
            AudioSystem.PlayOnce(AudioID.WindowOpen);

            InventoryDragAndDropModel.OnShowInventory(View, FurnaceViewModel);

            LocalizationModel.OnChangeLanguage += SetLocalization;

            FurnaceGroupModel.ActiveFurnace = PlayerEventHandler.RaycastData.Value.GameObject.GetComponent<FurnaceModel>();
            FurnaceModel = FurnaceGroupModel.ActiveFurnace;
            ExpandModel = ExpandsModel.Get(FurnaceModel.ExpandGroup);

            var countAdjustCells = FurnaceModel.CountCellsStart + ExpandModel.ExpandLevel - FurnaceModel.ItemsContainer.CountCells;
            if (countAdjustCells > 0)
            {
                FurnaceModel.ItemsContainer.AddCells(countAdjustCells);
            }

            FurnaceViewModel.OnChangeSelected += OnChangeSelectedHandler;
            FurnaceViewModel.OnChangeHighlighted += OnChangeHighlightedHandler;

            InventoryViewModel.OnExpand += OnExpandInventoryChangeHandler;

            InventoryModel.ItemsContainer.OnChangeCell += OnChangeCellInventoryHandler;

            HotBarModel.ItemsContainer.OnChangeCell += OnChangeCellHotBarHandler;

            RepairingItemsModel.OnUpdateRepairing += OnUpdateRepairingHandler;
            RepairingItemsModel.OnChangeCount += OnChangeCountRepairingHandler;

            PlayerConsumeModel.OnStartConsume.AddListener(InventoryModel.ItemsContainer, OnStartConsumePlayerHandler);
            PlayerConsumeModel.OnUpdateConsume.AddListener(InventoryModel.ItemsContainer, OnUpdateConsumePlayerHandler);
            PlayerConsumeModel.OnEndConsume.AddListener(InventoryModel.ItemsContainer, OnEndConsumePlayerHandler);

            PlayerConsumeModel.OnStartConsume.AddListener(HotBarModel.ItemsContainer, OnStartConsumePlayerHandler);
            PlayerConsumeModel.OnUpdateConsume.AddListener(HotBarModel.ItemsContainer, OnUpdateConsumePlayerHandler);
            PlayerConsumeModel.OnEndConsume.AddListener(HotBarModel.ItemsContainer, OnEndConsumePlayerHandler);

            PlayerConsumeModel.OnStartConsume.AddListener(FurnaceModel.ItemsContainer, OnStartConsumePlayerHandler);
            PlayerConsumeModel.OnUpdateConsume.AddListener(FurnaceModel.ItemsContainer, OnUpdateConsumePlayerHandler);
            PlayerConsumeModel.OnEndConsume.AddListener(FurnaceModel.ItemsContainer, OnEndConsumePlayerHandler);

            NetworkModel.OnInternetConnectionStateChange += OnInternetConnectioStateChangeHandler;
            NetworkModel.UpdateInternetConnectionStatus();

            FurnaceModel.ItemsContainer.OnChangeCell += OnChangeCellCampFireHandler;
            FurnaceModel.OnChangeFireState += OnChangeFireStateHandler;
            FurnaceModel.OnStartBoost += OnStartBoostHandler;
            FurnaceModel.OnUpdateBoostRemainingTime += OnUpdateBoostRemainingTimeHandler;
            FurnaceModel.OnEndBoost += OnEndBoostHandler;

            ExpandModel.OnExpand += OnExpandLootChangeHandler;

            CoinsModel.OnChange += OnChangeCoins;

            View.OnClose += OnCloseViewHandler;
            InventoryMainView.OnApplyItem += OnApplyItemHandler;
            InventoryMainView.OnDivideItems += OnDividetemHandler;
            InventoryMainView.OnTrash += OnTrashHandler;
            InventoryMainView.OnRepairItem += OnRepairItemHandler;
            InventoryFurnaceSectionView.OnSetFire += OnSetFireHandler;
            InventoryFurnaceSectionView.OnTurnOff += OnTurnOffHandler;
            InventoryMainView.OnAddCoins += OnAddCoinsHandler;
            InventoryMainView.OnAutoStackItems += OnAutoStackItems;
            InventoryMainView.OnScrollChanged += OnInventoryScrollChanged;
            TutorialModel.OnStepChanged += UpdateScrollEnable;
            TutorialModel.OnComplete += UpdateScrollEnable;

            NetworkModel.OnInternetConnectionStateChange += OnInternetConnectionStateChangeHandler;
            {
                var cellsDataInventory = CollectCellsData(InventoryModel.ItemsContainer, (int)ContainerID.Inventory);
                var cellId = 0;
                foreach (var cellView in InventoryMainView.InventoryCells)
                {
                    if (cellId < cellsDataInventory.Length)
                    {
                        cellView.SetData(cellsDataInventory[cellId]);
                        CellViewSubscribe(cellView, true);
                    }
                    else
                    {
                         CellViewSubscribe(cellView, false);
                        cellView.SetAsPassive();
                    }
                    ++cellId;
                }
            }

            {
                var cellsDataHotBar = CollectCellsData(HotBarModel.ItemsContainer, (int)ContainerID.HotBar);

                for (int cellId = 0; cellId < cellsDataHotBar.Length; cellId++)
                {
                    InventoryHotBarView.HotBarCells[cellId].SetData(cellsDataHotBar[cellId]);
                    CellViewSubscribe(InventoryHotBarView.HotBarCells[cellId], true);
                }
            }

            if (!InventoryViewModel.IsMaxExpandLevel)
            {
                ExpandInventoryModel.SetButtonContainer(InventoryMainView.ContainersUnlockSlots[InventoryViewModel.TotalExpandedCountLevel]);
            }
            ExpandInventoryModel.ShowExpandInventoryButton();

            ViewRemoveSelected();
            ViewSetIsFire(FurnaceModel.IsFire);

            UpdateScrollEnable();
            UpdateCookViews();
            UpdateCoins();
            UpdateAddQueueCellButton();
            UpdateBoostButton();

            SetLocalization();

            StatsEffectModel.SetViewContainer(View.transform);
            InfoPanelModel.SetViewContainer(InventoryFurnaceSectionView.Container);
        }

        protected override void Hide()
        {
            LocalizationModel.OnChangeLanguage -= SetLocalization;

            FurnaceViewModel.OnChangeSelected -= OnChangeSelectedHandler;
            FurnaceViewModel.OnChangeHighlighted -= OnChangeHighlightedHandler;

            InventoryViewModel.OnExpand -= OnExpandInventoryChangeHandler;

            InventoryModel.ItemsContainer.OnChangeCell -= OnChangeCellInventoryHandler;

            HotBarModel.ItemsContainer.OnChangeCell -= OnChangeCellHotBarHandler;

            RepairingItemsModel.OnUpdateRepairing -= OnUpdateRepairingHandler;
            RepairingItemsModel.OnChangeCount -= OnChangeCountRepairingHandler;

            PlayerConsumeModel.OnStartConsume.RemoveListener(InventoryModel.ItemsContainer, OnStartConsumePlayerHandler);
            PlayerConsumeModel.OnUpdateConsume.RemoveListener(InventoryModel.ItemsContainer, OnUpdateConsumePlayerHandler);
            PlayerConsumeModel.OnEndConsume.RemoveListener(InventoryModel.ItemsContainer, OnEndConsumePlayerHandler);

            PlayerConsumeModel.OnStartConsume.RemoveListener(HotBarModel.ItemsContainer, OnStartConsumePlayerHandler);
            PlayerConsumeModel.OnUpdateConsume.RemoveListener(HotBarModel.ItemsContainer, OnUpdateConsumePlayerHandler);
            PlayerConsumeModel.OnEndConsume.RemoveListener(HotBarModel.ItemsContainer, OnEndConsumePlayerHandler);

            PlayerConsumeModel.OnStartConsume.RemoveListener(FurnaceModel.ItemsContainer, OnStartConsumePlayerHandler);
            PlayerConsumeModel.OnUpdateConsume.RemoveListener(FurnaceModel.ItemsContainer, OnUpdateConsumePlayerHandler);
            PlayerConsumeModel.OnEndConsume.RemoveListener(FurnaceModel.ItemsContainer, OnEndConsumePlayerHandler);

            NetworkModel.OnInternetConnectionStateChange -= OnInternetConnectioStateChangeHandler;

            FurnaceModel.ItemsContainer.OnChangeCell -= OnChangeCellCampFireHandler;
            FurnaceModel.OnChangeFireState -= OnChangeFireStateHandler;
            FurnaceModel.OnStartBoost -= OnStartBoostHandler;
            FurnaceModel.OnUpdateBoostRemainingTime -= OnUpdateBoostRemainingTimeHandler;
            FurnaceModel.OnEndBoost -= OnEndBoostHandler;

            ExpandModel.OnExpand -= OnExpandLootChangeHandler;

            CoinsModel.OnChange -= OnChangeCoins;

            View.OnClose -= OnCloseViewHandler;
            InventoryMainView.OnApplyItem -= OnApplyItemHandler;
            InventoryMainView.OnDivideItems -= OnDividetemHandler;
            InventoryMainView.OnTrash -= OnTrashHandler;
            InventoryMainView.OnRepairItem -= OnRepairItemHandler;
            InventoryFurnaceSectionView.OnSetFire -= OnSetFireHandler;
            InventoryFurnaceSectionView.OnTurnOff -= OnTurnOffHandler;
            InventoryMainView.OnAddCoins -= OnAddCoinsHandler;
            InventoryMainView.OnAutoStackItems -= OnAutoStackItems;
            InventoryMainView.OnScrollChanged -= OnInventoryScrollChanged;
            DivideItemsModel.OnItemSplitted -= OnItemSplitted;
            DivideItemsModel.OnHideDividePopup -= OnHideDividePopup;
            TutorialModel.OnStepChanged -= UpdateScrollEnable;
            TutorialModel.OnComplete -= UpdateScrollEnable;

            NetworkModel.OnInternetConnectionStateChange -= OnInternetConnectionStateChangeHandler;

            DesubscribeCellView(InventoryFurnaceSectionView.CookCells);
            DesubscribeCellView(InventoryHotBarView.HotBarCells);
            DesubscribeCellView(InventoryMainView.InventoryCells);

            HideAddQueueCellButton();
            HideBoostButton();

            ExpandInventoryModel.HideExpandInventoryButton();

            FurnaceGroupModel.ActiveFurnace = null;

            StatsEffectModel.HideStatsEffect();
            InfoPanelModel.HideItemInfo();

            InventoryDragAndDropModel.OnHideView(View);
        }

        private void OnInternetConnectioStateChangeHandler() => UpdatePurchaseButton();

        private void UpdatePurchaseButton()
        {
            UpdateAddQueueCellButton();
            UpdateBoostButton();
        }

        public void UpdateBoostButton()
        {
            HideBoostButton();

            if (IsWatchBoostButton)
            {
                WatchPurchaseBoost.Prepere();
                ShowBoostButtonWatch();
            }
            else
            {
                ShowBoostButtonGold();
            }
        }

        private void HideBoostButton()
        {
            HideBoostButtonGold();
            HideBoostButtonWatch();
        }

        private void HideBoostButtonWatch()
        {
            if (BoostButtonWatchView != null)
            {
                BoostButtonWatchView.OnClick -= OnBoostHandler;
                ViewsSystem.Hide(BoostButtonWatchView);
                BoostButtonWatchView = null;
            }
        }

        private void HideBoostButtonGold()
        {
            if (BoostButtonGoldView != null)
            {
                BoostButtonGoldView.OnClick -= OnBoostHandler;
                ViewsSystem.Hide(BoostButtonGoldView);
                BoostButtonGoldView = null;
            }
        }

        private void ShowBoostButtonWatch()
        {
            BoostButtonWatchView = ViewsSystem.Show<BoostButtonWatchView>(ViewConfigID.BoostWatchButton, InventoryFurnaceSectionView.ContainerBoostButton);
            BoostButtonWatchView.transform.localPosition = Vector3.zero;
            BoostButtonWatchView.OnClick += OnBoostHandler;
            BoostButtonWatchView.SetIsInteractable(!FurnaceModel.IsBoost);
            BoostButtonWatchView.SetIsVisibleHandler(FurnaceModel.IsBoost);
            BoostButtonWatchView.SetFillAmount(BoostFillAmount);
            BoostButtonWatchView.SetText(LocalizationModel.GetString(LocalizationKeyID.FurnaceMenu_BoostBtn));
        }

        private void ShowBoostButtonGold()
        {
            BoostButtonGoldView = ViewsSystem.Show<BoostButtonGoldView>(ViewConfigID.BoostGoldButton, InventoryFurnaceSectionView.ContainerBoostButton);
            BoostButtonGoldView.transform.localPosition = Vector3.zero;
            BoostButtonGoldView.OnClick += OnBoostHandler;
            BoostButtonGoldView.SetIsInteractable(!FurnaceModel.IsBoost);
            BoostButtonGoldView.SetIsVisibleHandler(FurnaceModel.IsBoost);
            BoostButtonGoldView.SetFillAmount(BoostFillAmount);
            UpdateBoostButtonGoldText();
            BoostButtonGoldView.SetText(LocalizationModel.GetString(LocalizationKeyID.FurnaceMenu_BoostBtn));
        }

        private void UpdateBoostButtonGoldText() => BoostButtonGoldView.SetCount(PurchaseInfoFurnaceBoostGold.CoinCost.ToString());

        private void UpdateAddQueueCellButton()
        {
            HideAddQueueCellButton();

            if (IsCanShowAddQueueCellButton)
            {
                if (IsWatchFurnaceSlotBuy)
                {
                    WatchPurchaseFurnaceSlotBuy.Prepere();
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
            AddCellGoldView = ViewsSystem.Show<AddCellGoldView>(ViewConfigID.AddCellGold, InventoryFurnaceSectionView.ContainerExpandLootCell);
            AddCellGoldView.transform.localPosition = Vector3.zero;
            AddCellGoldView.OnClick += OnExpandLootHandler;
            AddCellGoldView.SetTextGoldValue(PurchaseInfoFurnaceSlotBuyGold.CoinCost.ToString());
        }


        private void ShowAddQueueCellButtonWatch()
        {
            AddCellWatchView = ViewsSystem.Show<AddCellWatchView>(ViewConfigID.AddCellWatch, InventoryFurnaceSectionView.ContainerExpandLootCell);
            AddCellWatchView.transform.localPosition = Vector3.zero;
            AddCellWatchView.OnClick += OnExpandLootHandler;
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
                AddCellWatchView.OnClick -= OnExpandLootHandler;
                ViewsSystem.Hide(AddCellWatchView);
                AddCellWatchView = null;
            }
        }

        private void HideAddQueueCellButtonGold()
        {
            if (AddCellGoldView != null)
            {
                AddCellGoldView.OnClick -= OnExpandLootHandler;
                ViewsSystem.Hide(AddCellGoldView);
                AddCellGoldView = null;
            }
        }

        private void OnInternetConnectionStateChangeHandler() => ExpandInventoryModel.UpdateExpandInventoryButton();

        private void OnAddCoinsHandler() => ViewsSystem.Show<PurchasesView>(ViewConfigID.Purchases);

        private void OnChangeCoins() => UpdateCoins();

        private void UpdateCoins() => InventoryMainView.SetCoins(CoinsModel.Coins.ToString());

        private void OnStartConsumePlayerHandler()
        {
            ViewSelectItem(PlayerConsumeModel.Item);

            var containerID = ContainersModel.GetContainerID(PlayerConsumeModel.Container);

            if (containerID != ContainerID.None)
            {
                var cellId = PlayerConsumeModel.CellId;
                OnChangeCellHandler(PlayerConsumeModel.Container.GetCell(cellId), containerID);
                CreateAddStatsEffect(PlayerConsumeModel.Item, containerID, cellId);
                UpdateCells();
            }
        }

        private void OnUpdateConsumePlayerHandler()
        {
            var containerID = ContainersModel.GetContainerID(PlayerConsumeModel.Container);

            if (containerID != ContainerID.None)
            {
                OnChangeCellHandler(PlayerConsumeModel.Container.GetCell(PlayerConsumeModel.CellId), containerID);
                UpdateCells();
            }
        }

        private void OnEndConsumePlayerHandler()
        {
            var containerID = ContainersModel.GetContainerID(PlayerConsumeModel.Container);
            var containerSelectedID = FurnaceViewModel.SelectedCell.ContainerID;
            var cellSelectedId = FurnaceViewModel.SelectedCell.CellId;

            OnChangeCellHandler(PlayerConsumeModel.Container.GetCell(PlayerConsumeModel.CellId), containerID);

            if (containerID != ContainerID.None)
            {
                FurnaceViewModel.SelectCell(containerSelectedID, cellSelectedId);
                UpdateCells();
            }
        }

        private void OnUpdateRepairingHandler() => UpdateCells();

        private void OnChangeCountRepairingHandler() => UpdateCells();

        private void CreateAddStatsEffect(SavableItem item, ContainerID containerID, int cellId)
        {
            if (TryGetCellView(containerID, cellId, out var cellObject))
            {
                StatsEffectModel.AddStatsEffect(item, cellObject.transform.position);
            }
        }

        private void OnTurnOffHandler() => FurnaceModel.SetFire(false);

        private void OnSetFireHandler() => FurnaceModel.SetFire(true);

        private void OnEndBoostHandler()
        {
            BoostButtonIsVisibleHandler(false);
            BoostButtonFillAmount(1);
            BoostButtonIsInteractable(true);
        }

        private void OnUpdateBoostRemainingTimeHandler() => BoostButtonFillAmount(BoostFillAmount);

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

        private void OnBoostHandler()
        {
            if (IsWatchBoostButton)
            {
                PurchasesModel.Purchase(PurchaseID.FurnaceBoostWatch, OnPurchaseBoostWatch);
            }
            else
            {
                PurchasesModel.Purchase(PurchaseID.FurnaceBoostGold, OnPurchaseBoostGold);
            }
        }

        private void OnPurchaseBoostWatch(PurchaseResult purchaseResult)
        {
            if (purchaseResult == PurchaseResult.Successfully)
            {
                FurnaceModel.Boost();
            }
        }

        private void OnPurchaseBoostGold(PurchaseResult purchaseResult)
        {
            if (purchaseResult == PurchaseResult.Successfully)
            {
                FurnaceModel.Boost();
            }
            else
            {
                ViewsSystem.Show<PurchasesView>(ViewConfigID.Purchases);
            }
        }

        private void OnChangeFireStateHandler() => ViewSetIsFire(FurnaceModel.IsFire);

        private void ViewSetIsFire(bool isFire)
        {
            InventoryFurnaceSectionView.SetIsVisibleSetFireButton(!isFire);
            InventoryFurnaceSectionView.SetIsVisibleTurnOffButton(isFire);
        }

        private void UpdateCookViews()
        {
            DesubscribeCellView(InventoryFurnaceSectionView.CookCells);

            var isExpandedMax = ExpandModel.IsMaxLevel;

            InventoryFurnaceSectionView.SetIsVisibleFirstLootCell(isExpandedMax);

            var cellsDataLoot = CollectCookCellsData(FurnaceModel.ItemsContainer, (int)ContainerID.Furnace);
            var cellCountActive = FurnaceModel.ItemsContainer.CountCells;

            if (isExpandedMax)
            {
                InventoryFurnaceSectionView.CookCells[0].SetData(cellsDataLoot[cellsDataLoot.Length - 1]);
                CellViewSubscribe(InventoryFurnaceSectionView.CookCells[0]);

                for (int cellId = 1; cellId < cellCountActive; cellId++)
                {
                    InventoryFurnaceSectionView.CookCells[cellId].SetData(cellsDataLoot[cellId - 1]);
                    CellViewSubscribe(InventoryFurnaceSectionView.CookCells[cellId]);
                }

                for (int cellId = cellCountActive; cellId < InventoryFurnaceSectionView.CookCells.Length; cellId++)
                {
                    InventoryFurnaceSectionView.CookCells[cellId].SetAsPassive();
                }
            }
            else
            {
                for (int cellId = 1; cellId < cellCountActive + 1; cellId++)
                {
                    InventoryFurnaceSectionView.CookCells[cellId].SetData(cellsDataLoot[cellId - 1]);
                    CellViewSubscribe(InventoryFurnaceSectionView.CookCells[cellId]);
                }

                for (int cellId = cellCountActive + 1; cellId < InventoryFurnaceSectionView.CookCells.Length; cellId++)
                {
                    InventoryFurnaceSectionView.CookCells[cellId].SetAsPassive();
                }
            }
        }

        private void CellViewSubscribe(CellView cellView, bool isCellActive)
        {
            if(isCellActive)  
            {  
                cellView.OnPointDown_ += OnPointerDownViewHandler;  
                cellView.OnPointerUp_ += OnPointerUpCellViewHandler;  
                cellView.OnPointerExit_ += OnPointerExitCellViewHandler;  
                cellView.OnPointerClick_ += OnClickCellViewHandler;  
            }  
  
            cellView.OnDragBegin_ += OnDragBeginCellViewHandler;  
            cellView.OnDrag_ += OnDragCellViewHandler;  
            cellView.OnDragEnd_ += OnDragEndCellViewHandler;  
        }

        private void CellViewSubscribe(CookCellView cellView)
        {
            cellView.OnPointDown_ += OnClickCellViewHandler;
            cellView.OnDragBegin_ += OnDragBeginCellViewHandler;
            cellView.OnDrag_ += OnDragCellViewHandler;
            cellView.OnDragEnd_ += OnDragEndCellViewHandler;
        }

        private void CellViewDesubscribe(CellView cellView)
        {
            cellView.OnPointDown_ -= OnPointerDownViewHandler;  
            cellView.OnDragBegin_ -= OnDragBeginCellViewHandler;  
            cellView.OnDrag_ -= OnDragCellViewHandler;  
            cellView.OnDragEnd_ -= OnDragEndCellViewHandler;  
            cellView.OnPointerUp_ -= OnPointerUpCellViewHandler;  
            cellView.OnPointerExit_ -= OnPointerExitCellViewHandler;  
            cellView.OnPointerClick_ -= OnClickCellViewHandler;  
        }

        private void CellViewDesubscribe(CookCellView cellView)
        {
            cellView.OnPointDown_ -= OnClickCellViewHandler;
            cellView.OnDragBegin_ -= OnDragBeginCellViewHandler;
            cellView.OnDrag_ -= OnDragCellViewHandler;
            cellView.OnDragEnd_ -= OnDragEndCellViewHandler;
        }

        private void DesubscribeCellView(CookCellView[] views)
        {
            foreach (var cellView in views)
            {
                CellViewDesubscribe(cellView);
            }
        }

        private void DesubscribeCellView(CellView[] views)
        {
            foreach (var cellView in views)
            {
                CellViewDesubscribe(cellView);
            }
        }

        private void OnExpandLootChangeHandler(int id)
        {
            UpdateAddQueueCellButton();

            FurnaceModel.ItemsContainer.AddCell();

            UpdateCookViews();
        }

        private void OnExpandInventoryChangeHandler(int id)
        {
            DesubscribeCellView(InventoryMainView.InventoryCells);

            {
                var cellsDataInventory = CollectCellsData(InventoryModel.ItemsContainer, (int)ContainerID.Inventory);
                var cellId = 0;
                foreach (var cellView in InventoryMainView.InventoryCells)
                {
                    if (cellId < cellsDataInventory.Length)
                    {
                        cellView.SetData(cellsDataInventory[cellId]);
                        CellViewSubscribe(cellView, true);
                    }
                    else
                    {
                        CellViewSubscribe(cellView, false);
                        cellView.SetAsPassive();
                    }
                    ++cellId;
                }
            }

            if (!InventoryViewModel.IsMaxExpandLevel)
            {
                ExpandInventoryModel.SetButtonContainer(InventoryMainView.ContainersUnlockSlots[InventoryViewModel.TotalExpandedCountLevel]);
            }
            ExpandInventoryModel.UpdateExpandInventoryButton();

            SetHighlight(FurnaceViewModel.HighlightedCell, true);
        }

        private void OnCloseViewHandler()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            ViewsSystem.Hide(View);
        }

        private void OnChangeSelectedHandler(CellInfo cellInfo)
        {
            if (cellInfo != null && TryGetContainer(cellInfo.containerID, out var container))
            {
                SetSelection(cellInfo, container);
            }
            else
            {
                ViewRemoveSelected();
            }
        }

        private bool TryGetContainer(ContainerID type, out ItemsContainer itemsContainer) => (itemsContainer = GetContainer(type)) != null;

        private ItemsContainer GetContainer(int type) => ContainersModel.GetContainer((ContainerID)type);

        private ItemsContainer GetContainer(ContainerID type) => ContainersModel.GetContainer(type);

        private void OnChangeHighlightedHandler(CellInfo cellInfo)
        {
            SetHighlight(FurnaceViewModel.HighlightedLast, false);
            SetHighlight(cellInfo, true);
        }

        private void SetHighlight(CellInfo cellInfo, bool isSelect) => SetHighlight(cellInfo.containerID, cellInfo.cellId, isSelect);

        private void SetHighlight(ContainerID containerID, int cellId, bool isSelect)
        {
            switch (containerID)
            {
                case ContainerID.Inventory:
                    SerHiglghtCell(InventoryMainView.InventoryCells, cellId, isSelect);
                    break;
                case ContainerID.HotBar:
                    SerHiglghtCell(InventoryHotBarView.HotBarCells, cellId, isSelect);
                    break;
                case ContainerID.Furnace:
                    SerHiglghtCell(GetCookCellViews(), cellId, isSelect);
                    break;
            }
        }

        private void SerHiglghtCell(CellView[] views, int cellId, bool isSelect)
        {
            if (isSelect)
            {
                views[cellId].Selection();
            }
            else
            {
                views[cellId].Deselection(true);
            }
        }

        private void SerHiglghtCell(CookCellView[] views, int cellId, bool isSelect)
        {
            if (isSelect)
            {
                views[cellId].Selection();
            }
            else
            {
                views[cellId].Deselection(true);
            }
        }

        private void OnApplyItemHandler()
        {
            var containerID = FurnaceViewModel.SelectedCell.containerID;
            var cellId = FurnaceViewModel.SelectedCell.cellId;

            var container = GetContainer(containerID);
            var cell = container.GetCell(cellId);

            ApplyItemModel.ApplyItem(container, cell);
        }

        private void OnDividetemHandler()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            var cellId = FurnaceViewModel.SelectedCell.cellId;
            var containerID = FurnaceViewModel.SelectedCell.containerID;
            var container = GetContainer(containerID);

            if (!TryGetEmptyCellForDivide(containerID, out _) && !InventoryViewModel.IsMaxExpandLevel)
            {
                FullInventoryModel.ShowFullPopup();
            }
            else
            {
                DivideItemsModel.OnItemSplitted += OnItemSplitted;
                DivideItemsModel.OnHideDividePopup += OnHideDividePopup;
                DivideItemsModel.ShowDividePopup(container, container.GetCell(cellId));
            }
        }

        private void OnItemSplitted(SavableItem itemSplit)
        {
            var containerID = FurnaceViewModel.SelectedCell.containerID;
            if (TryGetEmptyCellForDivide(containerID, out var emptyCell))
            {
                emptyCell.Item = itemSplit;
            }
            else
            {
                DropItemModel.DropItem(itemSplit);
            }

            OnHideDividePopup();
        }

        private void OnHideDividePopup()
        {
            DivideItemsModel.OnItemSplitted -= OnItemSplitted;
            DivideItemsModel.OnHideDividePopup -= OnHideDividePopup;
        }

        private bool TryGetEmptyCellForDivide(ContainerID containerID, out CellModel result)
        {
            result = GetContainer(containerID).GetEmptyCell();
            if (result == null)
            {
                if (containerID == ContainerID.HotBar)
                {
                    result = GetContainer(ContainerID.Inventory).GetEmptyCell();
                    if (result == null)
                    {
                        result = GetContainer(ContainerID.Furnace).GetEmptyCell();
                    }
                }
                else if (containerID == ContainerID.Inventory)
                {
                    result = GetContainer(ContainerID.HotBar).GetEmptyCell();
                    if (result == null)
                    {
                        result = GetContainer(ContainerID.Furnace).GetEmptyCell();
                    }
                }
                else if (containerID == ContainerID.Furnace)
                {
                    result = GetContainer(ContainerID.Inventory).GetEmptyCell();
                    if (result == null)
                    {
                        result = GetContainer(ContainerID.HotBar).GetEmptyCell();
                    }
                }
            }

            return result != null;
        }

        private void OnTrashHandler()
        {
            var containerID = FurnaceViewModel.SelectedCell.containerID;

            if (containerID != ContainerID.None)
            {
                var container = GetContainer(containerID);
                var cellId = FurnaceViewModel.SelectedCell.cellId;
                var item = container.GetCell(cellId).Item;

                AudioSystem.PlayOnce(AudioID.DropItem);
                DeleteItemModel.ShowDeletePopup(container, cellId);
            }
        }

        private void OnRepairItemHandler()
        {
            var container = GetContainer(FurnaceViewModel.SelectedCell.containerID);
            var item = container.GetCell(FurnaceViewModel.SelectedCell.cellId).Item;

            RepairViewModel.SetItem(item);
            InventoryRepairingModel.ShowRepairPopup();
        }

        private void SetSelection(CellInfo cellInfo) => SetSelection(cellInfo, GetContainer(cellInfo.containerID));

        private void SetSelection(CellInfo cellInfo, ItemsContainer containers) => ViewSetSelection(containers.GetCell(cellInfo.cellId));

        private void OnExpandLootHandler()
        {
            if (IsWatchFurnaceSlotBuy)
            {
                PurchasesModel.Purchase(PurchaseID.FurnaceSlotBuyWatch, OnPurchaseExpandLootWatch);
            }
            else
            {
                PurchasesModel.Purchase(PurchaseID.FurnaceSlotBuyGold, OnPurchaseExpandLootGold);
            }
        }

        private void OnPurchaseExpandLootWatch(PurchaseResult purchaseResult)
        {
            if (purchaseResult == PurchaseResult.Successfully)
            {
                ExpandModel.ExpandCells();
            }
        }

        private void OnPurchaseExpandLootGold(PurchaseResult purchaseResult)
        {
            if (purchaseResult == PurchaseResult.Successfully)
            {
                ExpandModel.ExpandCells();
            }
            else
            {
                ViewsSystem.Show<PurchasesView>(ViewConfigID.Purchases);
            }
        }

        private void ViewSetSelection(CellModel cell)
        {
            if (cell.IsHasItem)
            {
                ViewSelectItem(cell.Item);
            }
            else
            {
                ViewRemoveSelected();
            }
        }

        private void SelectItemData(ItemData item)
        {
            InventoryFurnaceSectionView.SetIsVisibleItemDescription(false);
            InfoPanelModel.UpdateItemInfo(item);
        }

        private void ViewSelectItem(SavableItem item)
        {
            var isConsuming = !(PlayerConsumeModel.IsCanConsume && item.IsCanConsume());
            var isCanApply = !isConsuming;
            var isCanDivide = item.Count > 1;
            var isCanRepair = false;
            var isCanTrash = true;

            if (item.TryGetProperty("Durability", out var durabilityProperty))
            {
                var cellInfo = FurnaceViewModel.SelectedCell;
                var cellModel = GetContainer(cellInfo.containerID).GetCell(cellInfo.cellId);
                var data = CollectCellDataModel.CollectCellData(cellModel, (int)cellInfo.containerID);

                isCanRepair = durabilityProperty.Float.Current <= 0 && !data.ProgressRepairing.HasValue;
            }

            InventoryMainView.SetIsActiveApplyButton(isCanApply);
            InventoryMainView.SetIsActiveDivideButton(isCanDivide);
            InventoryMainView.SetIsActiveTrashButton(isCanTrash);
            InventoryMainView.SetIsActiveRepairButton(isCanRepair);

            SelectItemData(item.ItemData);
        }

        private void ViewRemoveSelected()
        {
            InventoryMainView.SetIsActiveApplyButton(false);
            InventoryMainView.SetIsActiveDivideButton(false);
            InventoryMainView.SetIsActiveTrashButton(false);
            InventoryMainView.SetIsActiveRepairButton(false);
            InventoryFurnaceSectionView.SetIsVisibleCookingProcess(true);
            InventoryFurnaceSectionView.SetIsVisibleItemName(true);
            InventoryFurnaceSectionView.SetIsVisibleItemDescription(true);

            InfoPanelModel.HideItemInfo();
        }

        private void OnPointerDownViewHandler(CellView cell, PointerEventData eventData) => InventoryDragAndDropModel.OnPointerDown(cell, eventData);

        private void OnClickCellViewHandler(CookCellView cell, PointerEventData eventData) => OnClickCellView((ContainerID)cell.ContainerId, cell.Id);

        private void OnClickCellView(ContainerID containerID, int cellId)
        {
            FurnaceViewModel.SelectCell(containerID, cellId);
            FurnaceViewModel.HighlightCell(containerID, cellId);
        }

        private void OnDragBeginCellViewHandler(CellView cellView, PointerEventData eventData)
        {
            InventoryDragAndDropModel.OnDragBegin(cellView.gameObject, (ContainerID)cellView.ContainerId, cellView.Id, eventData, () => cellView.SetData(GetDataDefault(cellView.ContainerId, cellView.Id)));
        }

        private void OnDragBeginCellViewHandler(CookCellView cellView, PointerEventData eventData) 
        {
            InventoryDragAndDropModel.OnDragBegin(cellView.gameObject, (ContainerID)cellView.ContainerId, cellView.Id, eventData, () => cellView.SetData(GetDataCookDefault(cellView.ContainerId, cellView.Id)));
        }

        private void OnDragCellViewHandler(CellView cellView, PointerEventData eventData)
        {
             InventoryDragAndDropModel.OnDrag(cellView.gameObject, (ContainerID)cellView.ContainerId, cellView.Id, eventData);
        }

        private void OnDragCellViewHandler(CookCellView cellView, PointerEventData eventData)
        {
            InventoryDragAndDropModel.OnDrag(cellView.gameObject, (ContainerID)cellView.ContainerId, cellView.Id, eventData);
        }

        private void OnDragEndCellViewHandler(CellView cellView, PointerEventData eventData)
        {
            InventoryDragAndDropModel.OnDragEnd(cellView.gameObject, (ContainerID)cellView.ContainerId, cellView.Id, eventData, () => cellView.SetData(CollectCellDataModel.CollectCellData(GetContainer(cellView.ContainerId).GetCell(cellView.Id), cellView.ContainerId)));
        }

        private void OnDragEndCellViewHandler(CookCellView cellView, PointerEventData eventData)
        {
            InventoryDragAndDropModel.OnDragEnd(cellView.gameObject, (ContainerID)cellView.ContainerId, cellView.Id, eventData, () => cellView.SetData(GetCellCookData(GetContainer(cellView.ContainerId).GetCell(cellView.Id), cellView.ContainerId)));
        }

        private void OnPointerUpCellViewHandler(CellView cellView, PointerEventData eventData) 
        { 
            InventoryDragAndDropModel.OnPointerUp(cellView, eventData); 
        } 
 
        private void OnPointerExitCellViewHandler(CellView cellView, PointerEventData eventData) 
        { 
            InventoryDragAndDropModel.OnPointerExit(cellView, eventData); 
        } 
 
        private void OnClickCellViewHandler(CellView cellView, PointerEventData eventData) 
        { 
            InventoryDragAndDropModel.OnClick(cellView, eventData); 
        }

        private void UpdateCells()
        {
            foreach (var cell in InventoryModel.ItemsContainer.Cells)
            {
                SetDataView(InventoryMainView.InventoryCells[cell.Id].gameObject, cell.Id, ContainerID.Inventory);
            }
            foreach (var cell in HotBarModel.ItemsContainer.Cells)
            {
                SetDataView(InventoryHotBarView.HotBarCells[cell.Id].gameObject, cell.Id, ContainerID.HotBar);
            }

            var cookViews = GetCookCellViews();
            for (int i = 0; i < FurnaceModel.ItemsContainer.Cells.Count(); i++)
            {
                var cell = FurnaceModel.ItemsContainer.Cells.ElementAt(i);
                cookViews[i].SetData(GetCellCookData(cell, (int)ContainerID.Furnace));
                if (FurnaceViewModel.HighlightedCell.containerID == ContainerID.Furnace &&
                    FurnaceViewModel.HighlightedCell.cellId == cell.Id)
                {
                    ViewSetSelection(cell);
                    cookViews[i].Selection();
                }
            }
        }

        private void SetDataView(GameObject cellObject, int cellId, ContainerID containerId)
        {
            var cellView = cellObject.GetComponent<CellView>();
            if (cellView)
            {
                var cellModel = GetContainer(containerId).Cells.ElementAt(cellId);

                cellView.SetData(CollectCellDataModel.CollectCellData(cellModel, (int)containerId));

                if (FurnaceViewModel.HighlightedCell.containerID == containerId &&
                    FurnaceViewModel.HighlightedCell.cellId == cellModel.Id)
                {
                    ViewSetSelection(cellModel);
                    cellView.Selection();
                }
                return;
            }

            var cookCellView = cellObject.GetComponent<CookCellView>();
            if (cookCellView && cookCellView.ContainerId == (int)containerId)
            {
                var cellModel = GetContainer(containerId).Cells.ElementAt(cellId - 1);

                cookCellView.SetData(GetCellCookData(cellModel, (int)containerId));

                if (FurnaceViewModel.HighlightedCell.containerID == containerId &&
                    FurnaceViewModel.HighlightedCell.cellId == cellModel.Id)
                {
                    ViewSetSelection(cellModel);
                    cookCellView.Selection();
                }
                return;
            }
        }

        private void OnChangeCellInventoryHandler(CellModel cell) => OnChangeCellHandler(cell, ContainerID.Inventory);

        private void OnChangeCellHotBarHandler(CellModel cell) => OnChangeCellHandler(cell, ContainerID.HotBar);

        private void OnChangeCellCampFireHandler(CellModel cell) => OnChangeCellHandler(cell, ContainerID.Furnace);

        private void OnChangeCellHandler(CellModel cell, ContainerID containerID)
        {
            switch (containerID)
            {
                case ContainerID.Inventory:
                    ChangeCell(InventoryMainView.InventoryCells, cell, containerID);
                    break;
                case ContainerID.HotBar:
                    ChangeCell(InventoryHotBarView.HotBarCells, cell, containerID);
                    break;
                case ContainerID.Furnace:
                    ChangeCell(GetCookCellViews(), cell, containerID);
                    break;
            }

            if
            (
                containerID != ContainerID.None
                && FurnaceViewModel.SelectedCell.cellId == cell.Id
            )
            {
                OnChangeHighlightedHandler(FurnaceViewModel.HighlightedCell);
                OnChangeSelectedHandler(FurnaceViewModel.SelectedCell);
            }

            UpdateBoostButtonGoldText();
        }

        private bool TryGetCellView(ContainerID containerID, int cellId, out GameObject cellObject) => (cellObject = GetCellView(containerID, cellId)) != null;

        private GameObject GetCellView(ContainerID containerID, int cellId)
        {
            switch (containerID)
            {
                case ContainerID.Inventory:
                    return InventoryMainView.InventoryCells[cellId].gameObject;
                case ContainerID.HotBar:
                    return InventoryHotBarView.HotBarCells[cellId].gameObject;
                case ContainerID.Furnace:
                    return GetCookCellViews()[cellId].gameObject;
            }

            return default;
        }

        private void ChangeCell(CellView[] views, CellModel cell, ContainerID containerID)
        {
            if (cell.Id < views.Length)
            {
                views[cell.Id].SetData(CollectCellDataModel.CollectCellData(cell, (int)containerID));
            }
        }

        private void ChangeCell(CookCellView[] views, CellModel cell, ContainerID containerID)
        {
            if (cell.Id < views.Length)
            {
                views[cell.Id].SetData(GetCellCookData(cell, (int)containerID));
            }
        }

        private CookCellView[] GetCookCellViews()
        {
            var result = new CookCellView[InventoryFurnaceSectionView.CookCells.Length];
            Array.Copy(InventoryFurnaceSectionView.CookCells, 1, result, 0, InventoryFurnaceSectionView.CookCells.Length - 1);

            if (ExpandModel.IsMaxLevel)
            {
                result[FurnaceModel.ItemsContainer.CountCells - 1] = InventoryFurnaceSectionView.CookCells[0];
            }

            return result;
        }

        private CellData[] CollectCellsData(ItemsContainer container, int inventoryID) => container.Cells.Select(cell => CollectCellDataModel.CollectCellData(cell, inventoryID)).ToArray();

        private CookCellData[] CollectCookCellsData(ItemsContainer container, int inventoryID) => container.Cells.Select(cell => GetCellCookData(cell, inventoryID)).ToArray();

        private CellData GetDataDefault(int containerId, int cellId) => new CellData()
        {
            ContainerId = containerId,
            Id = cellId,
        };

        private CookCellData GetDataCookDefault(int containerId, int cellId) => new CookCellData()
        {
            ContainerId = containerId,
            Id = cellId,
        };

        private CookCellData GetCellCookData(CellModel cell, int containerId)
        {
            var icon = (Sprite)null;
            var count = (int?)null;
            var durability = (float?)null;
            var sips = (float?)null;
            var ProcessAmount = (float?)null;
            var progressConsumable = (float?)null;
            var progressRepairing = (float?)null;
            var disable = false;

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

                if (isConsumable)
                {
                    var isHasCount = item.ItemData.StackSize > 1 && item.Count > 0;
                    if (isHasCount)
                    {
                        count = item.Count + 1;
                    }
                }
                else
                {
                    if (item.ItemData.StackSize > 1)
                    {
                        count = item.Count;
                    }
                }

                if (item.TryGetProperty("Durability", out var durabilityProperty))
                {
                    durability = durabilityProperty.Float.Ratio;
                }

                if (item.TryGetProperty("Sips", out var sipsProperty))
                {
                    sips = sipsProperty.Int.Ratio;
                }

                if (FurnaceModel.IsFire)
                {
                    if (item.TryGetProperty("Melting time", out var cookingTime))
                    {
                        ProcessAmount = cookingTime.Float.Ratio;
                    }
                }

                if (isConsumable)
                {
                    progressConsumable = PlayerConsumeModel.RemainingTimeNormalized;
                }

                if (RepairingItemsModel.TryFindRepairItemInfo(item, out var info))
                {
                    progressRepairing = info.RemainingTime / info.AllTime;
                }

                var consuming = (item.IsCanConsume() && PlayerConsumeModel.IsInProgress);
                disable = consuming || item.IsBroken();
            }

            return new CookCellData
            {
                Id = cell.Id,
                ContainerId = containerId,
                Icon = icon,
                Count = count,
                Durability = durability,
                ProcessAmount = ProcessAmount,
                IsBoost = FurnaceModel.IsBoost,
                ProgressConsumable = progressConsumable,
                ProgressRepairing = progressRepairing,
                Sips = sips,
                IsDisable = disable,
            };
        }

        private void SetLocalization()
        {
            InventoryMainView.SetTextInventoryTitle(LocalizationModel.GetString(LocalizationKeyID.InventoryMenu_Name));
            InventoryFurnaceSectionView.SetTextFurnace(LocalizationModel.GetString(LocalizationKeyID.FurnaceMenu_Name));
            InventoryMainView.ApplyButtonView.SetText(LocalizationModel.GetString(LocalizationKeyID.InventoryMenu_ApplyBtn));
            InventoryFurnaceSectionView.SetTextSetFireButton(LocalizationModel.GetString(LocalizationKeyID.FurnaceMenu_SetFireBtn));
            InventoryFurnaceSectionView.SetTextTurnOffButton(LocalizationModel.GetString(LocalizationKeyID.FurnaceMenu_TurnOffBtn));
            InventoryMainView.RepairButton.SetText(LocalizationModel.GetString(LocalizationKeyID.InventoryMenu_RepairBtn));

            BoostButtonGoldView?.SetText(LocalizationModel.GetString(LocalizationKeyID.CampFireMenu_BoostBtn));
            BoostButtonWatchView?.SetText(LocalizationModel.GetString(LocalizationKeyID.CampFireMenu_BoostBtn));
        }


        private void OnAutoStackItems()
        {
            InventoryModel.ItemsContainer.AutoStackItems();
        }

        private void OnInventoryScrollChanged(float value)
        {
            if(value > 0.95f){
                InventoryMainView.SetIsActiveTopFade(false);
                InventoryMainView.SetIsActiveBottomFade(true);
            }
            else if(value < 0.05f)
            {
                InventoryMainView.SetIsActiveTopFade(true);
                InventoryMainView.SetIsActiveBottomFade(false);
            }
            else
            {
                InventoryMainView.SetIsActiveTopFade(true);
                InventoryMainView.SetIsActiveBottomFade(true);
            }
        }

        private void UpdateScrollEnable()
        {
            InventoryMainView.Scrollbar.enabled = TutorialModel.IsComplete;
            InventoryMainView.ScrollRect.enabled = TutorialModel.IsComplete;
        }
    }
}
