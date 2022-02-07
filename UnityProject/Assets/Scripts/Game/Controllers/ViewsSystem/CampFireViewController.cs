using Core;
using Core.Controllers;
using Extensions;
using Game.Audio;
using Game.Components;
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
using CellInfo = Game.Models.CampFireViewModel.CellInfo;

namespace Game.Controllers
{
    public class CampFireViewController : ViewControllerBase<CampFireView>
    {
        [Inject] public AudioSystem AudioSystem { get; private set; }
        [Inject] public CampFiresModel CampFiresModel { get; private set; }
        [Inject] public InventoryViewModel InventoryViewModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public CampFireViewModel CampFireViewModel { get; private set; }
        [Inject] public InventoryModel InventoryModel { get; private set; }
        [Inject] public HotBarModel HotBarModel { get; private set; }
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public ExpandsModel ExpandsModel { get; private set; }
        [Inject] public PlayerConsumeModel PlayerConsumeModel { get; private set; }
        [Inject] public CoinsModel CoinsModel { get; private set; }
        [Inject] public PurchasesModel PurchasesModel { get; private set; }
        [Inject] public LocalizationModel LocalizationModel { get; private set; }
        [Inject] public RemoteSettingsModel RemoteSettingsModel { get; private set; }
        [Inject] public NetworkModel NetworkModel { get; private set; }
        [Inject] public RepairingItemsModel RepairingItemsModel { get; private set; }
        [Inject] public RepairViewModel RepairViewModel { get; private set; }
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
        [Inject] public DragItemViewModel DragItemViewModel { get; private set; }  
        [Inject] public TutorialModel TutorialModel { get; private set; }  
        [Inject] public DeleteItemModel DeleteItemModel { get; private set; }

        protected CampFireModel CampFireModel { get; private set; }
        protected ExpandModel ExpandModel { get; private set; }

        protected AddCellGoldView AddCellGoldView { get; private set; }
        protected AddCellWatchView AddCellWatchView { get; private set; }
        protected BoostButtonGoldView BoostButtonGoldView { get; private set; }
        protected BoostButtonWatchView BoostButtonWatchView { get; private set; }

        protected bool IsBoostButtonWatch => !UnityRemoteSettingsCampFire.IsBoostGold && (WatchPurchaseCampFireBoost.IsCanPurchase || NetworkModel.IsHasConnection);
        protected bool IsAddQueueCellButtonWatch => !UnityRemoteSettingsCampFire.IsBuySlotGold && (WatchPurchaseCampFireSlotBuy.IsCanPurchase || NetworkModel.IsHasConnection);

        protected CampFireRemoteSettings UnityRemoteSettingsCampFire => RemoteSettingsModel.Get<CampFireRemoteSettings>(RemoteSettingID.CampFire);

        protected IWatchPurchase WatchPurchaseCampFireBoost => PurchasesModel.GetInfo<IWatchPurchase>(PurchaseID.CampFireBoostWatch);
        protected IWatchPurchase WatchPurchaseCampFireSlotBuy => PurchasesModel.GetInfo<IWatchPurchase>(PurchaseID.CampFireSlotBuyWatch);

        private InventoryMainView InventoryMainView => View.InventoryMain;
        private InventoryCampFireSectionView InventoryCampFireSectionView => View.InventoryCampFireSection;
        private InventoryHotBarView InventoryHotBarView => View.InventoryHotBar;

        protected override void Show()
        {
            AudioSystem.PlayOnce(AudioID.WindowOpen);

            InventoryDragAndDropModel.OnShowInventory(View, CampFireViewModel);

            LocalizationModel.OnChangeLanguage += SetLocalization;

            CampFiresModel.ActiveCampFire = PlayerEventHandler.RaycastData.Value.GameObject.GetComponent<CampFireModel>();
            CampFireModel = CampFiresModel.ActiveCampFire;
            ExpandModel = ExpandsModel.Get(CampFireModel.ExpandGroup);

            var countAdjustCells = CampFireModel.CountCellsStart + ExpandModel.ExpandLevel - CampFireModel.ItemsContainer.CountCells;
            if (countAdjustCells > 0)
            {
                CampFireModel.ItemsContainer.AddCells(countAdjustCells);
            }

            CampFireViewModel.OnChangeSelected += OnChangeSelectedHandler;
            CampFireViewModel.OnChangeHighlighted += OnChangeHighlightedHandler;
            CampFireViewModel.OnGetBoostsButton += OnGetBoostButtonHandler;
            CampFireViewModel.OnGetSwitchFireModeButton += OnGetSwitchFireModeButtonHandler;
            CampFireViewModel.OnGetApplyButton += OnGetApplyButtonHandler;

            InventoryViewModel.OnExpand += OnExpandInventoryChangeHandler;

            InventoryModel.ItemsContainer.OnChangeCell += OnChangeCellInventoryHandler;

            HotBarModel.ItemsContainer.OnChangeCell += OnChangeCellHotBarHandler;

            RepairingItemsModel.OnUpdateRepairing += OnUpdateRepairingHandler;
            RepairingItemsModel.OnChangeCount += OnChangeCountRepairingHandler;

            CampFireModel.ItemsContainer.OnChangeCell += OnChangeCellCampFireHandler;
            CampFireModel.OnChangeFireState += OnChangeFireStateHandler;
            CampFireModel.OnStartBoost += OnStartBoostHandler;
            CampFireModel.OnUpdateBoostRemainingTime += OnUpdateBoostRemainingTimeHandler;
            CampFireModel.OnEndBoost += OnEndBoostHandler;

            ExpandModel.OnExpand += OnExpandLootChangeHandler;

            PlayerConsumeModel.OnStartConsume.AddListener(InventoryModel.ItemsContainer, OnStartConsumePlayerHandler);
            PlayerConsumeModel.OnUpdateConsume.AddListener(InventoryModel.ItemsContainer, OnUpdateConsumePlayerHandler);
            PlayerConsumeModel.OnEndConsume.AddListener(InventoryModel.ItemsContainer, OnEndConsumePlayerHandler);

            PlayerConsumeModel.OnStartConsume.AddListener(HotBarModel.ItemsContainer, OnStartConsumePlayerHandler);
            PlayerConsumeModel.OnUpdateConsume.AddListener(HotBarModel.ItemsContainer, OnUpdateConsumePlayerHandler);
            PlayerConsumeModel.OnEndConsume.AddListener(HotBarModel.ItemsContainer, OnEndConsumePlayerHandler);

            PlayerConsumeModel.OnStartConsume.AddListener(CampFireModel.ItemsContainer, OnStartConsumePlayerHandler);
            PlayerConsumeModel.OnUpdateConsume.AddListener(CampFireModel.ItemsContainer, OnUpdateConsumePlayerHandler);
            PlayerConsumeModel.OnEndConsume.AddListener(CampFireModel.ItemsContainer, OnEndConsumePlayerHandler);

            NetworkModel.OnInternetConnectionStateChange += OnChangeConnectionHandler;

            CoinsModel.OnChange += OnChangeCoins;

            View.OnClose += OnCloseViewHandler;
            InventoryMainView.OnApplyItem += OnApplyItemHandler;
            InventoryMainView.OnDivideItems += OnDividetemHandler;
            InventoryMainView.OnTrash += OnTrashHandler;
            InventoryMainView.OnRepairItem += OnRepairItemHandler;
            InventoryCampFireSectionView.OnSetFire += OnSetFireHandler;
            InventoryCampFireSectionView.OnTurnOff += OnTurnOffHandler;
            InventoryMainView.OnAddCoins += OnAddCoinsHandler;
            InventoryMainView.OnAutoStackItems += OnAutoStackItems;
            InventoryMainView.OnScrollChanged += OnInventoryScrollChanged;

            NetworkModel.OnInternetConnectionStateChange += OnInternetConnectionStateChangeHandler;

            InventoryCampFireSectionView.SetIsVisibleBoostButton(true);

            if (!InventoryViewModel.IsMaxExpandLevel)
            {
                ExpandInventoryModel.SetButtonContainer(InventoryMainView.ContainersUnlockSlots[InventoryViewModel.TotalExpandedCountLevel]);
            }
            ExpandInventoryModel.ShowExpandInventoryButton();

            InitInventoryViews();
            InitHotBarViews();
            InitCookViews();

            ShowBoostButton();
            ShowAddQueueCellButton();

           
            UpdateScrollEnable();
            UpdateCoins();
            UpdateViewIsFire();

            SetLocalization();

            NetworkModel.UpdateInternetConnectionStatus();

            CampFireViewModel.RemoveSelectCell();
            CampFireViewModel.RemoveHighlightCell();

            StatsEffectModel.SetViewContainer(View.transform);
            InfoPanelModel.SetViewContainer(InventoryCampFireSectionView.Container);

            CampFireViewModel.OnTutorialHilightCell += OnTutorialHilightHandler;
            CampFireViewModel.OnRemoveAllTutorialCellsHilight += OnRemoveAllTutorialCellsHilightHandler;
            CampFireViewModel.OnTutorialDragAnimation += OnTutorialDragAnimationHandler;
            CampFireViewModel.OnStopTutorialDragAnimation += OnStopTutorialDragAnimatinonHandler;
            TutorialModel.OnStepChanged += UpdateScrollEnable;
            TutorialModel.OnComplete += UpdateScrollEnable;

            CampFireViewModel.Show(true);
        }

        protected override void Hide()
        {
            LocalizationModel.OnChangeLanguage -= SetLocalization;

            CampFireViewModel.OnChangeSelected -= OnChangeSelectedHandler;
            CampFireViewModel.OnChangeHighlighted -= OnChangeHighlightedHandler;
            CampFireViewModel.OnGetBoostsButton -= OnGetBoostButtonHandler;
            CampFireViewModel.OnGetSwitchFireModeButton -= OnGetSwitchFireModeButtonHandler;
            CampFireViewModel.OnGetApplyButton -= OnGetApplyButtonHandler;

            InventoryViewModel.OnExpand -= OnExpandInventoryChangeHandler;

            InventoryModel.ItemsContainer.OnChangeCell -= OnChangeCellInventoryHandler;

            HotBarModel.ItemsContainer.OnChangeCell -= OnChangeCellHotBarHandler;

            RepairingItemsModel.OnUpdateRepairing -= OnUpdateRepairingHandler;
            RepairingItemsModel.OnChangeCount -= OnChangeCountRepairingHandler;

            CampFireModel.ItemsContainer.OnChangeCell -= OnChangeCellCampFireHandler;
            CampFireModel.OnChangeFireState -= OnChangeFireStateHandler;
            CampFireModel.OnStartBoost -= OnStartBoostHandler;
            CampFireModel.OnUpdateBoostRemainingTime -= OnUpdateBoostRemainingTimeHandler;
            CampFireModel.OnEndBoost -= OnEndBoostHandler;

            PlayerConsumeModel.OnStartConsume.RemoveListener(InventoryModel.ItemsContainer, OnStartConsumePlayerHandler);
            PlayerConsumeModel.OnUpdateConsume.RemoveListener(InventoryModel.ItemsContainer, OnUpdateConsumePlayerHandler);
            PlayerConsumeModel.OnEndConsume.RemoveListener(InventoryModel.ItemsContainer, OnEndConsumePlayerHandler);

            PlayerConsumeModel.OnStartConsume.RemoveListener(HotBarModel.ItemsContainer, OnStartConsumePlayerHandler);
            PlayerConsumeModel.OnUpdateConsume.RemoveListener(HotBarModel.ItemsContainer, OnUpdateConsumePlayerHandler);
            PlayerConsumeModel.OnEndConsume.RemoveListener(HotBarModel.ItemsContainer, OnEndConsumePlayerHandler);

            PlayerConsumeModel.OnStartConsume.RemoveListener(CampFireModel.ItemsContainer, OnStartConsumePlayerHandler);
            PlayerConsumeModel.OnUpdateConsume.RemoveListener(CampFireModel.ItemsContainer, OnUpdateConsumePlayerHandler);
            PlayerConsumeModel.OnEndConsume.RemoveListener(CampFireModel.ItemsContainer, OnEndConsumePlayerHandler);

            NetworkModel.OnInternetConnectionStateChange -= OnChangeConnectionHandler;

            ExpandModel.OnExpand -= OnExpandLootChangeHandler;

            CoinsModel.OnChange -= OnChangeCoins;

            View.OnClose -= OnCloseViewHandler;
            InventoryMainView.OnApplyItem -= OnApplyItemHandler;
            InventoryMainView.OnDivideItems -= OnDividetemHandler;
            InventoryMainView.OnTrash -= OnTrashHandler;
            InventoryMainView.OnRepairItem -= OnRepairItemHandler;
            InventoryCampFireSectionView.OnSetFire -= OnSetFireHandler;
            InventoryCampFireSectionView.OnTurnOff -= OnTurnOffHandler;
            InventoryMainView.OnAddCoins -= OnAddCoinsHandler;
            InventoryMainView.OnAutoStackItems -= OnAutoStackItems;
            InventoryMainView.OnScrollChanged -= OnInventoryScrollChanged;
            DivideItemsModel.OnItemSplitted -= OnItemSplitted;
            DivideItemsModel.OnHideDividePopup -= OnHideDividePopup;

            NetworkModel.OnInternetConnectionStateChange -= OnInternetConnectionStateChangeHandler;

            DesubscribeCellViews(InventoryMainView.InventoryCells);
            DesubscribeCellViews(InventoryHotBarView.HotBarCells);
            DesubscribeCellViews(InventoryCampFireSectionView.CookCells);

            ExpandInventoryModel.HideExpandInventoryButton();

            HideAddQueueCellButton();
            HideBoostButton();

            CampFiresModel.ActiveCampFire = null;

            StatsEffectModel.HideStatsEffect();
            InfoPanelModel.HideItemInfo();

            InventoryDragAndDropModel.OnHideView(View);
            CampFireViewModel.OnTutorialHilightCell -= OnTutorialHilightHandler;
            CampFireViewModel.OnRemoveAllTutorialCellsHilight -= OnRemoveAllTutorialCellsHilightHandler;
            CampFireViewModel.OnTutorialDragAnimation -= OnTutorialDragAnimationHandler;
            CampFireViewModel.OnStopTutorialDragAnimation -= OnStopTutorialDragAnimatinonHandler;
            TutorialModel.OnStepChanged -= UpdateScrollEnable;
            TutorialModel.OnComplete -= UpdateScrollEnable;

            CampFireViewModel.Show(false);
            DragItemViewModel.SetShow(false);
        }

        private void OnTutorialDragAnimationHandler(ContainerID arg1, int arg2, ContainerID arg3, int arg4)
        {
             if(TryGetCellView(arg1,arg2,out GameObject cell_1))
            {
                if(TryGetCellView(arg3,arg4,out GameObject cell_2))
                {
                    DragItemViewModel.SetDragData(cell_1.transform,cell_2.transform);
                    DragItemViewModel.SetShow(true);
                }
            }
        }

        private void OnStopTutorialDragAnimatinonHandler()
        {
           DragItemViewModel.SetShow(false);
        }

        private void OnRemoveAllTutorialCellsHilightHandler()
        {
            foreach (var cell in hilightedCells)
            {
                if(cell == null) continue;

                cell.SafeDeactivateComponent<TutorialHilight>();
            }
        }

        private List<GameObject> hilightedCells = new List<GameObject>();

        private void OnTutorialHilightHandler(ContainerID containerID, int cellID)
        {
            if(TryGetCellView(containerID,cellID,out GameObject cellObject))
            {
                cellObject.SafeActivateComponent<TutorialHilight>();
                hilightedCells.Add(cellObject);
            }
        }

        private GameObject OnGetApplyButtonHandler()
        {
            return InventoryMainView.ApplyButtonView.gameObject;
        }
        private GameObject OnGetBoostButtonHandler()
        {
            return InventoryCampFireSectionView.ContainerBoostButton.gameObject;
        }

        private GameObject OnGetSwitchFireModeButtonHandler()
        {
            return InventoryCampFireSectionView.GetFireButton();
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

        private void UpdateBoostButon()
        {
            HideBoostButton();
            ShowBoostButton();
        }

        private void UpdateBoostButtonInteractable()
        {
            BoostButtonIsInteractable(!CampFireModel.IsBoost && CampFireModel.HasCookableBoostableItems);
            BoostButtonIsVisibleHandler(CampFireModel.IsBoost && CampFireModel.HasCookableBoostableItems);
        }

        private void ShowBoostButton()
        {
            if (IsBoostButtonWatch)
            {
                WatchPurchaseCampFireBoost.Prepere();
                ShowBoostButtonWatch();
            }
            else
            {
                ShowBoostButtonGold();
            }
        }

        private void ShowBoostButtonWatch()
        {
            var boostButtonWatchView = ViewsSystem.Show<BoostButtonWatchView>(ViewConfigID.BoostWatchButton, InventoryCampFireSectionView.ContainerBoostButton);
            boostButtonWatchView.transform.localPosition = Vector3.zero;
            boostButtonWatchView.OnClick += OnBoostHandler;
            boostButtonWatchView.SetIsInteractable(!CampFireModel.IsBoost && CampFireModel.HasCookableBoostableItems);
            boostButtonWatchView.SetIsVisibleHandler(CampFireModel.IsBoost && CampFireModel.HasCookableBoostableItems);
            boostButtonWatchView.SetFillAmount(GetBoostFill());
            boostButtonWatchView.SetText(LocalizationModel.GetString(LocalizationKeyID.CampFireMenu_BoostBtn));

            BoostButtonWatchView = boostButtonWatchView;
        }

        private void ShowBoostButtonGold()
        {
            BoostButtonGoldView = ViewsSystem.Show<BoostButtonGoldView>(ViewConfigID.BoostGoldButton, InventoryCampFireSectionView.ContainerBoostButton);
            BoostButtonGoldView.transform.localPosition = Vector3.zero;
            BoostButtonGoldView.OnClick += OnBoostHandler;
            BoostButtonGoldView.SetIsInteractable(!CampFireModel.IsBoost && CampFireModel.HasCookableBoostableItems);
            BoostButtonGoldView.SetIsVisibleHandler(CampFireModel.IsBoost && CampFireModel.HasCookableBoostableItems);
            BoostButtonGoldView.SetFillAmount(GetBoostFill());
            UpdateBoostButtonGoldText();
            BoostButtonGoldView.SetText(LocalizationModel.GetString(LocalizationKeyID.CampFireMenu_BoostBtn));
        }

        private void UpdateBoostButtonGoldText() => BoostButtonGoldView.SetCount(PurchasesModel.GetInfo<IPurchaseCoinInfo>(PurchaseID.CampFireBoostGold).CoinCost.ToString());

        private float GetBoostFill()
        {
            if (CampFireModel.IsBoost)
            {
                return CampFireModel.BoostRemainingTime / CampFireModel.BoostTime;
            }

            return 1;
        }

        private void OnChangeConnectionHandler() => UpdateRemoteButtons();

        private void UpdateRemoteButtons()
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
            if (!ExpandModel.IsMaxLevel)
            {
                if (IsAddQueueCellButtonWatch)
                {
                    WatchPurchaseCampFireSlotBuy.Prepere();
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
            AddCellGoldView = ViewsSystem.Show<AddCellGoldView>(ViewConfigID.AddCellGold, InventoryCampFireSectionView.ContainerExpandLootCell);
            AddCellGoldView.transform.localPosition = Vector3.zero;
            AddCellGoldView.OnClick += OnExpandCampFireHandler;
            AddCellGoldView.SetTextGoldValue(PurchasesModel.GetInfo<IPurchaseCoinInfo>(PurchaseID.CampFireSlotBuyGold).CoinCost.ToString());
        }

        private void ShowAddQueueCellButtonWatch()
        {
            AddCellWatchView = ViewsSystem.Show<AddCellWatchView>(ViewConfigID.AddCellWatch, InventoryCampFireSectionView.ContainerExpandLootCell);
            AddCellWatchView.transform.localPosition = Vector3.zero;
            AddCellWatchView.OnClick += OnExpandCampFireHandler;
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
                AddCellWatchView.OnClick -= OnExpandCampFireHandler;
                ViewsSystem.Hide(AddCellWatchView);
                AddCellWatchView = null;
            }
        }

        private void HideAddQueueCellButtonGold()
        {
            if (AddCellGoldView != null)
            {
                AddCellGoldView.OnClick -= OnExpandCampFireHandler;
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
                CreateAddStatsEffect(PlayerConsumeModel.Item, containerID, PlayerConsumeModel.CellId);
                UpdateCells();
            }
        }

        private void OnUpdateConsumePlayerHandler()
        {
            var containerID = ContainersModel.GetContainerID(PlayerConsumeModel.Container);
            if (containerID != ContainerID.None)
            {
                var cellId = PlayerConsumeModel.CellId;
                OnChangeCellHandler(PlayerConsumeModel.Container.GetCell(cellId), containerID);
                UpdateCells();
            }
        }

        private void OnEndConsumePlayerHandler()
        {
            var containerID = ContainersModel.GetContainerID(PlayerConsumeModel.Container);
            var cellId = PlayerConsumeModel.CellId;
            {
                OnChangeCellHandler(PlayerConsumeModel.Container.GetCell(cellId), containerID);
                var containerSelectedID = CampFireViewModel.SelectedCell.ContainerID;
                var cellSelectedId = CampFireViewModel.SelectedCell.CellId;
                if (containerID != ContainerID.None)
                {
                    CampFireViewModel.SelectCell(containerSelectedID, cellSelectedId);
                    UpdateCells();
                }
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

        private void OnTurnOffHandler() => CampFireModel.SetFire(false);

        private void OnSetFireHandler() => CampFireModel.SetFire(true);

        private void OnEndBoostHandler()
        {
            BoostButtonIsVisibleHandler(false);
            BoostButtonFillAmount(1);
            BoostButtonIsInteractable(true);
        }

        private void OnUpdateBoostRemainingTimeHandler() => BoostButtonFillAmount(CampFireModel.BoostRemainingTime / CampFireModel.BoostTime);

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
            if (IsBoostButtonWatch)
            {
                PurchasesModel.Purchase(PurchaseID.CampFireBoostWatch, OnPurchaseBoostWatch);
            }
            else
            {
                PurchasesModel.Purchase(PurchaseID.CampFireBoostGold, OnPurchaseBoostGold);
            }
        }

        private void OnPurchaseBoostWatch(PurchaseResult purchaseResult)
        {
            if (purchaseResult == PurchaseResult.Successfully)
            {
                BoostCampFire();
            }
        }

        private void OnPurchaseBoostGold(PurchaseResult purchaseResult)
        {
            if (purchaseResult == PurchaseResult.Successfully)
            {
                BoostCampFire();
            }
            else
            {
                ViewsSystem.Show<PurchasesView>(ViewConfigID.Purchases);
            }
        }

        private void BoostCampFire() => CampFireModel.Boost();

        private void OnChangeFireStateHandler() => UpdateViewIsFire();

        private void UpdateViewIsFire() => ViewSetIsFire(CampFireModel.IsFire);

        private void ViewSetIsFire(bool isFire)
        {
            InventoryCampFireSectionView.SetIsVisibleSetFireButton(!isFire);
            InventoryCampFireSectionView.SetIsVisibleTurnOffButton(isFire);
        }

        private void InitCookViews()
        {
            var isExpandedMax = ExpandModel.IsMaxLevel;

            InventoryCampFireSectionView.SetIsVisibleFirstLootCell(isExpandedMax);

            var cellsDataLoot = CollectCookCellsData(CampFireModel.ItemsContainer, (int)ContainerID.CampFire);
            var cellCountActive = CampFireModel.ItemsContainer.CountCells;

            if (isExpandedMax)
            {
                InventoryCampFireSectionView.CookCells[0].SetData(cellsDataLoot[cellsDataLoot.Length - 1]);
                CellViewSubscribe(InventoryCampFireSectionView.CookCells[0]);

                for (int cellId = 1; cellId < cellCountActive; cellId++)
                {
                    InventoryCampFireSectionView.CookCells[cellId].SetData(cellsDataLoot[cellId - 1]);
                    CellViewSubscribe(InventoryCampFireSectionView.CookCells[cellId]);
                }

                for (int cellId = cellCountActive; cellId < InventoryCampFireSectionView.CookCells.Length; cellId++)
                {
                    InventoryCampFireSectionView.CookCells[cellId].SetAsPassive();
                }
            }
            else
            {
                for (int cellId = 1; cellId < cellCountActive + 1; cellId++)
                {
                    InventoryCampFireSectionView.CookCells[cellId].SetData(cellsDataLoot[cellId - 1]);
                    CellViewSubscribe(InventoryCampFireSectionView.CookCells[cellId]);
                }

                for (int cellId = cellCountActive + 1; cellId < InventoryCampFireSectionView.CookCells.Length; cellId++)
                {
                    InventoryCampFireSectionView.CookCells[cellId].SetAsPassive();
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

        private void DesubscribeCellViews(CellView[] views)
        {
            foreach (var view in views)
            {
                CellViewDesubscribe(view);
            }
        }

        private void DesubscribeCellViews(CookCellView[] views)
        {
            foreach (var view in views)
            {
                CellViewDesubscribe(view);
            }
        }
        private void OnExpandLootChangeHandler(int id)
        {
            CampFireModel.ItemsContainer.AddCell();

            UpdateAddQueueCellButton();
            DesubscribeCellViews(InventoryCampFireSectionView.CookCells);
            InitCookViews();
        }

        private void OnExpandInventoryChangeHandler(int id)
        {
            DesubscribeCellViews(InventoryMainView.InventoryCells);
            InitInventoryViews();
            
            if (!InventoryViewModel.IsMaxExpandLevel)
            {
                ExpandInventoryModel.SetButtonContainer(InventoryMainView.ContainersUnlockSlots[InventoryViewModel.TotalExpandedCountLevel]);
            }
            ExpandInventoryModel.UpdateExpandInventoryButton();

            SetHighlight(CampFireViewModel.HighlightedCell, true);
        }

        private void InitInventoryViews()
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

        private void InitHotBarViews()
        {
            var cellsDataHotBar = CollectCellsData(HotBarModel.ItemsContainer, (int)ContainerID.HotBar);

            for (int cellId = 0; cellId < cellsDataHotBar.Length; cellId++)
            {
                InventoryHotBarView.HotBarCells[cellId].SetData(cellsDataHotBar[cellId]);
                CellViewSubscribe(InventoryHotBarView.HotBarCells[cellId], true);
            }
        }

        private void OnCloseViewHandler()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            ViewsSystem.Hide(View);
        }

        private void OnChangeSelectedHandler(CellInfo cellInfo)
        {
            if(cellInfo == null)
            {
                ViewRemoveSelected();
                return;
            }
            
            var container = GetContainer(cellInfo.containerID);

            if (container != null)
            {
                SetSelection(cellInfo, container);
            }
            else
            {
                ViewRemoveSelected();
            }
        }

        private ItemsContainer GetContainer(int type) => ContainersModel.GetContainer((ContainerID)type);

        private ItemsContainer GetContainer(ContainerID type) => ContainersModel.GetContainer(type); 

        private void OnChangeHighlightedHandler(CellInfo cellInfo)
        {
            SetHighlight(CampFireViewModel.HighlightedLast, false);
            SetHighlight(cellInfo, true);
        }

        private void SetHighlight(CellInfo cellInfo, bool isSelect) => SetHighlight(cellInfo.containerID, cellInfo.cellId, isSelect);

        private void SetHighlight(ContainerID containerID, int cellId, bool isSelect)
        {
            switch (containerID)
            {
                case ContainerID.Inventory:
                    SetHiglghtCell(InventoryMainView.InventoryCells, cellId, isSelect);
                    break;
                case ContainerID.HotBar:
                    SetHiglghtCell(InventoryHotBarView.HotBarCells, cellId, isSelect);
                    break;
                case ContainerID.CampFire:
                    SetHiglghtCell(GetCookCellViews(), cellId, isSelect);
                    break;
            }
        }

        private void SetHiglghtCell(CellView[] views, int cellId, bool isSelect)
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

        private void SetHiglghtCell(CookCellView[] views, int cellId, bool isSelect)
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
            var containerID = CampFireViewModel.SelectedCell.containerID;
            var cellId = CampFireViewModel.SelectedCell.cellId;

            var container = GetContainer(containerID);
            var cell = container.GetCell(cellId);

            ApplyItemModel.ApplyItem(container, cell);
        }

        private void OnDividetemHandler()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            var cellId = CampFireViewModel.SelectedCell.cellId;
            var containerID = CampFireViewModel.SelectedCell.containerID;
            var container = GetContainer(containerID);

            if (!TryGetEmptyCellForDivide(containerID, out var emptyCell) && !InventoryViewModel.IsMaxExpandLevel)
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
            var containerID = CampFireViewModel.SelectedCell.containerID;
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
                        result = GetContainer(ContainerID.CampFire).GetEmptyCell();
                    }
                }
                else if (containerID == ContainerID.Inventory)
                {
                    result = GetContainer(ContainerID.HotBar).GetEmptyCell();
                    if (result == null)
                    {
                        result = GetContainer(ContainerID.CampFire).GetEmptyCell();
                    }
                }
                else if (containerID == ContainerID.CampFire)
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
            var containerID = CampFireViewModel.SelectedCell.containerID;
            var container = GetContainer(containerID);
            var cellId = CampFireViewModel.SelectedCell.cellId;

            if (containerID != ContainerID.None)
            {
                AudioSystem.PlayOnce(AudioID.DropItem);
                DeleteItemModel.ShowDeletePopup(container, cellId);
            }
        }

        private void OnRepairItemHandler()
        {
            var container = GetContainer(CampFireViewModel.SelectedCell.containerID);
            var item = container.GetCell(CampFireViewModel.SelectedCell.cellId).Item;

            RepairViewModel.SetItem(item);
            InventoryRepairingModel.ShowRepairPopup();
        }

        private void SetSelection(CellInfo cellInfo) => SetSelection(cellInfo, GetContainer(cellInfo.containerID));

        private void SetSelection(CellInfo cellInfo, ItemsContainer containers) => ViewSetSelection(containers.GetCell(cellInfo.cellId));

        private void OnExpandCampFireHandler()
        {
            if (IsAddQueueCellButtonWatch)
            {
                PurchasesModel.Purchase(PurchaseID.CampFireSlotBuyWatch, OnPurchaseExpandCampFireWatch);
            }
            else
            {
                PurchasesModel.Purchase(PurchaseID.CampFireSlotBuyGold, OnPurchaseExpandCampFireGold);
            }
        }

        private void OnPurchaseExpandCampFireWatch(PurchaseResult purchaseResult)
        {
            if (purchaseResult == PurchaseResult.Successfully)
            {
                ExpandCampFire();
            }
        }

        private void OnPurchaseExpandCampFireGold(PurchaseResult purchaseResult)
        {
            if (purchaseResult == PurchaseResult.Successfully)
            {
                ExpandCampFire();
            }
            else
            {
                ViewsSystem.Show<PurchasesView>(ViewConfigID.Purchases);
            }
        }

        private void ExpandCampFire() => ExpandModel.ExpandCells();

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
            InfoPanelModel.UpdateItemInfo(item);
        }

        private void ViewSelectItem(SavableItem item)
        {
            var consuming = !(PlayerConsumeModel.IsCanConsume && item.IsCanConsume());
            var canApply = !consuming;
            var isCanDivide = item.Count > 1;
            var isCanRepair = false;
            var isCanTrash = true;

            if (item.TryGetProperty("Durability", out var durabilityProperty))
            {
                var cellInfo = CampFireViewModel.SelectedCell;
                var cellModel = GetContainer(cellInfo.containerID).GetCell(cellInfo.cellId);
                var data = CollectCellDataModel.CollectCellData(cellModel, (int)cellInfo.containerID);

                isCanRepair = durabilityProperty.Float.Current <= 0 && !data.ProgressRepairing.HasValue;
            }

            InventoryMainView.SetIsActiveApplyButton(canApply);
            InventoryMainView.SetIsActiveDivideButton(isCanDivide);
            InventoryMainView.SetIsActiveTrashButton(isCanTrash);
            InventoryMainView.SetIsActiveRepairButton(isCanRepair);
            InventoryCampFireSectionView.SetIsVisibleCookingProcess(false);

            SelectItemData(item.ItemData);
        }

        private void ViewRemoveSelected()
        {
            InventoryMainView.SetIsActiveApplyButton(false);
            InventoryMainView.SetIsActiveDivideButton(false);
            InventoryMainView.SetIsActiveTrashButton(false);
            InventoryMainView.SetIsActiveRepairButton(false);
            InventoryCampFireSectionView.SetIsVisibleCookingProcess(true);

            InventoryCampFireSectionView.SetIsVisibleItemName(true);
            InventoryCampFireSectionView.SetIsVisibleItemDescription(true);
            InfoPanelModel.HideItemInfo();
        }

        private void OnPointerDownViewHandler(CellView cell, PointerEventData eventData) =>  InventoryDragAndDropModel.OnPointerDown(cell, eventData); 

        private void OnClickCellViewHandler(CookCellView cell, PointerEventData eventData) => OnClickCellView((ContainerID)cell.ContainerId, cell.Id);

        private void OnClickCellView(ContainerID containerID, int cellId)
        {
            CampFireViewModel.SelectCell(containerID, cellId);
            CampFireViewModel.HighlightCell(containerID, cellId);
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
                var view = InventoryMainView.InventoryCells[cell.Id];
                SetDataView(view.gameObject, cell.Id, ContainerID.Inventory);
            }
            foreach (var cell in HotBarModel.ItemsContainer.Cells)
            {
                var view = InventoryHotBarView.HotBarCells[cell.Id];
                SetDataView(view.gameObject, cell.Id, ContainerID.HotBar);
            }
            var cookViews = GetCookCellViews();
            for (int i = 0; i < CampFireModel.ItemsContainer.Cells.Count(); i++)
            {
                var cell = CampFireModel.ItemsContainer.Cells.ElementAt(i);
                cookViews[i].SetData(GetCellCookData(cell, (int)ContainerID.CampFire));
                if (CampFireViewModel.HighlightedCell.containerID == ContainerID.CampFire &&
                    CampFireViewModel.HighlightedCell.cellId == cell.Id)
                {
                    ViewSetSelection(cell);
                    cookViews[i].Selection();
                }
            }
        }

        private void SetDataView(GameObject cellObject, int cellId, ContainerID containerId) // obj, cellMOdel, conId
        {
            var cellModel = GetContainer(containerId).Cells.ElementAt(cellId);
            var cellView = cellObject.GetComponent<CellView>();

            if (cellView)
            {
                cellView.SetData(CollectCellDataModel.CollectCellData(cellModel, (int)containerId));

                if (CampFireViewModel.HighlightedCell.containerID == containerId &&
                    CampFireViewModel.HighlightedCell.cellId == cellModel.Id)
                {
                    ViewSetSelection(cellModel);
                    cellView.Selection();
                }
                return;
            }

            var cookCellView = cellObject.GetComponent<CookCellView>();

            cellModel = GetContainer(containerId).Cells.ElementAt(cellId - 1);

            if (cookCellView && cookCellView.ContainerId == (int)containerId)
            {
                cookCellView.SetData(GetCellCookData(cellModel, (int)containerId));

                if (CampFireViewModel.HighlightedCell.containerID == containerId &&
                    CampFireViewModel.HighlightedCell.cellId == cellModel.Id)
                {
                    ViewSetSelection(cellModel);
                    cookCellView.Selection();
                }
                return;
            }
        }

        private void OnChangeCellInventoryHandler(CellModel cell) => OnChangeCellHandler(cell, ContainerID.Inventory);

        private void OnChangeCellHotBarHandler(CellModel cell) => OnChangeCellHandler(cell, ContainerID.HotBar);

        private void OnChangeCellCampFireHandler(CellModel cell) => OnChangeCellHandler(cell, ContainerID.CampFire);

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
                case ContainerID.CampFire:
                    ChangeCell(GetCookCellViews(), cell, containerID);
                    UpdateBoostButtonInteractable();
                    UpdateBoostButtonGoldText();
                    break;
            }

            if (containerID != ContainerID.None)
            {
                if (CampFireViewModel.SelectedCell.cellId == cell.Id)
                {
                    OnChangeHighlightedHandler(CampFireViewModel.HighlightedCell);
                    OnChangeSelectedHandler(CampFireViewModel.SelectedCell);
                }
            }
        }

        public bool TryGetCellView(ContainerID containerID, int cellId, out GameObject cellObject)
        {
            cellObject = null;
            switch (containerID)
            {
                case ContainerID.Inventory:
                    cellObject = InventoryMainView.InventoryCells[cellId].gameObject;
                    break;
                case ContainerID.HotBar:
                    cellObject = InventoryHotBarView.HotBarCells[cellId].gameObject;
                    break;
                case ContainerID.CampFire:
                    cellObject = GetCookCellViews()[cellId].gameObject;
                    break;
            }

            return cellObject != null;
        }

        private void ChangeCell(CellView[] views, CellModel cell, ContainerID containerID)
        {
            if (cell.Id < views.Length)
            {
                var cellData = CollectCellDataModel.CollectCellData(cell, (int)containerID);
                views[cell.Id].SetData(cellData);
            }
        }

        private void ChangeCell(CookCellView[] views, CellModel cell, ContainerID containerID)
        {
            if (cell.Id < views.Length)
            {
                var cellData = GetCellCookData(cell, (int)containerID);
                views[cell.Id].SetData(cellData);
            }
        }

        private CookCellView[] GetCookCellViews()
        {
            var result = new CookCellView[InventoryCampFireSectionView.CookCells.Length];
            Array.Copy(InventoryCampFireSectionView.CookCells, 1, result, 0, InventoryCampFireSectionView.CookCells.Length - 1);

            if (ExpandModel.IsMaxLevel)
            {
                result[CampFireModel.ItemsContainer.CountCells - 1] = InventoryCampFireSectionView.CookCells[0];
            }

            return result;
        }

        private CellData[] CollectCellsData(ItemsContainer container, int inventoryID) => container.Cells.Select(cell => CollectCellDataModel.CollectCellData(cell, inventoryID)).ToArray();

        //loot
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
            var isBoost = false;

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

                if (CampFireModel.IsFire)
                {
                    if (item.TryGetProperty("Cooking time", out var cookingTime))
                    {
                        ProcessAmount = cookingTime.Float.Ratio;
                    }
                }

                isBoost = CampFireModel.IsBoost && CampFireModel.CanBeBoosted(item);

                if (isConsumable)
                {
                    progressConsumable = PlayerConsumeModel.RemainingTimeNormalized;
                }

                if (RepairingItemsModel.TryFindRepairItemInfo(item, out var info))
                {
                    progressRepairing = info.RemainingTime / info.AllTime;
                }

                bool consuming = (item.IsCanConsume() && PlayerConsumeModel.IsInProgress);
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
                IsBoost = isBoost,
                ProgressConsumable = progressConsumable,
                ProgressRepairing = progressRepairing,
                Sips = sips,
                IsDisable = disable,
            };
        }

        private void SetLocalization()
        {
            InventoryMainView.ApplyButtonView.SetText(LocalizationModel.GetString(LocalizationKeyID.InventoryMenu_ApplyBtn));
            InventoryCampFireSectionView.SetTextSetFireButton(LocalizationModel.GetString(LocalizationKeyID.CampFireMenu_SetFireBtn));
            InventoryMainView.SetTextInventoryTitle(LocalizationModel.GetString(LocalizationKeyID.InventoryMenu_Name));
            InventoryCampFireSectionView.SetTextCampfireTitle(LocalizationModel.GetString(LocalizationKeyID.CampFireMenu_Name));
            InventoryCampFireSectionView.SetTextTurnOffButton(LocalizationModel.GetString(LocalizationKeyID.CampFireMenu_TurnOffBtn));
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
