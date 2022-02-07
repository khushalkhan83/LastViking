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
using CellInfo = Game.Models.LoomViewModel.CellInfo;

namespace Game.Controllers
{
    public class LoomViewController : ViewControllerBase<LoomView>
    {
        [Inject] public AudioSystem AudioSystem { get; private set; }
        [Inject] public InventoryViewModel InventoryViewModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public LoomViewModel LoomViewModel { get; private set; }
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
        [Inject] public LoomGroupModel LoomGroupModel { get; private set; }

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



        protected LoomModel LoomModel { get; private set; }
        protected ExpandModel ExpandModel { get; private set; }

        protected AddCellGoldView AddCellGoldView { get; private set; }
        protected AddCellWatchView AddCellWatchView { get; private set; }
        protected BoostButtonGoldView BoostButtonGoldView { get; private set; }
        protected BoostButtonWatchView BoostButtonWatchView { get; private set; }

        protected LoomRemoteSettings LoomRemoteSettings => RemoteSettingsModel.Get<LoomRemoteSettings>(RemoteSettingID.Loom);

        protected bool IsBoostButtonWatch => !LoomRemoteSettings.IsBoostGold && (WatchPurchaseLoomBoost.IsCanPurchase || NetworkModel.IsHasConnection);
        protected bool IsAddQueueCellButtonWatch => !LoomRemoteSettings.IsBuySlotGold && (WatchPurchaseLoomSlotBuy.IsCanPurchase || NetworkModel.IsHasConnection);

        protected IWatchPurchase WatchPurchaseLoomSlotBuy => PurchasesModel.GetInfo<IWatchPurchase>(PurchaseID.LoomSlotBuyWatch);
        protected IWatchPurchase WatchPurchaseLoomBoost => PurchasesModel.GetInfo<IWatchPurchase>(PurchaseID.LoomBoostWatch);

       

        private InventoryMainView InventoryMainView => View.InventoryMain;

        private InventoryHotBarView InventoryHotBarView => View.InventoryHotBar;

        private InventoryLoomSectionView InventoryLoomSectionView => View.InventoryLoomSection;

        protected override void Show()
        {
            AudioSystem.PlayOnce(AudioID.WindowOpen);

            InventoryDragAndDropModel.OnShowInventory(View, LoomViewModel);

            LocalizationModel.OnChangeLanguage += SetLocalization;

            LoomGroupModel.ActiveLoom = PlayerEventHandler.RaycastData.Value.GameObject.GetComponent<LoomModel>();
            LoomModel = LoomGroupModel.ActiveLoom;
            ExpandModel = ExpandsModel.Get(LoomModel.ExpandGroup);

            var countAdjustCells = LoomModel.CountCellsStart + ExpandModel.ExpandLevel - LoomModel.ItemsContainer.CountCells;
            if (countAdjustCells > 0)
            {
                LoomModel.ItemsContainer.AddCells(countAdjustCells);
            }

            LoomViewModel.OnChangeSelected += OnChangeSelectedHandler;
            LoomViewModel.OnChangeHighlighted += OnChangeHighlightedHandler;

            LoomViewModel.RemoveSelectCell();
            LoomViewModel.RemoveHighlightCell();

            InventoryViewModel.OnExpand += OnExpandInventoryChangeHandler;

            InventoryModel.ItemsContainer.OnChangeCell += OnChangeCellInventoryHandler;

            HotBarModel.ItemsContainer.OnChangeCell += OnChangeCellHotBarHandler;

            RepairingItemsModel.OnUpdateRepairing += OnUpdateRepairingHandler;
            RepairingItemsModel.OnChangeCount += OnChangeCountRepairingHandler;

            LoomModel.ItemsContainer.OnChangeCell += OnChangeCellLoomHandler;
            LoomModel.OnChangeWeaveState += OnChangeWeaveStateHandler;
            LoomModel.OnStartBoost += OnStartBoostHandler;
            LoomModel.OnUpdateBoostRemainingTime += OnUpdateBoostRemainingTimeHandler;
            LoomModel.OnEndBoost += OnEndBoostHandler;

            ExpandModel.OnExpand += OnExpandLootChangeHandler;



            NetworkModel.OnInternetConnectionStateChange += OnInternetConnectionStateChangeHandler;



            PlayerConsumeModel.OnStartConsume.AddListener(InventoryModel.ItemsContainer, OnStartConsumePlayerHandler);
            PlayerConsumeModel.OnUpdateConsume.AddListener(InventoryModel.ItemsContainer, OnUpdateConsumePlayerHandler);
            PlayerConsumeModel.OnEndConsume.AddListener(InventoryModel.ItemsContainer, OnEndConsumePlayerHandler);

            PlayerConsumeModel.OnStartConsume.AddListener(HotBarModel.ItemsContainer, OnStartConsumePlayerHandler);
            PlayerConsumeModel.OnUpdateConsume.AddListener(HotBarModel.ItemsContainer, OnUpdateConsumePlayerHandler);
            PlayerConsumeModel.OnEndConsume.AddListener(HotBarModel.ItemsContainer, OnEndConsumePlayerHandler);

            PlayerConsumeModel.OnStartConsume.AddListener(LoomModel.ItemsContainer, OnStartConsumePlayerHandler);
            PlayerConsumeModel.OnUpdateConsume.AddListener(LoomModel.ItemsContainer, OnUpdateConsumePlayerHandler);
            PlayerConsumeModel.OnEndConsume.AddListener(LoomModel.ItemsContainer, OnEndConsumePlayerHandler);

            NetworkModel.OnInternetConnectionStateChange += OnChangeConnectionHandler;
            NetworkModel.UpdateInternetConnectionStatus();

            CoinsModel.OnChange += OnChangeCoins;

            View.OnClose += OnCloseViewHandler;
            InventoryMainView.OnApplyItem += OnApplyItemHandler;
            InventoryMainView.OnDivideItems += OnDividetemHandler;
            InventoryMainView.OnTrash += OnTrashHandler;
            InventoryMainView.OnRepairItem += OnRepairItemHandler;
            InventoryLoomSectionView.OnSetWeave += OnSetWeaveHandler;
            InventoryLoomSectionView.OnTurnOff += OnTurnOffHandler;
            InventoryMainView.OnAddCoins += OnAddCoinsHandler;
            InventoryMainView.OnScrollChanged += OnInventoryScrollChanged;

            InventoryMainView.OnAutoStackItems += OnAutoStackItems;
            TutorialModel.OnStepChanged += UpdateScrollEnable;
            TutorialModel.OnComplete += UpdateScrollEnable;



            InventoryLoomSectionView.SetIsVisibleBoostButton(true);



            if (!InventoryViewModel.IsMaxExpandLevel)

            {

                ExpandInventoryModel.SetButtonContainer(InventoryMainView.ContainersUnlockSlots[InventoryViewModel.TotalExpandedCountLevel]);

            }

            ExpandInventoryModel.ShowExpandInventoryButton();



            InitInventoryViews();

            {
                var cellsDataHotBar = CollectCellsData(HotBarModel.ItemsContainer, (int)ContainerID.HotBar);

                for (int cellId = 0; cellId < cellsDataHotBar.Length; cellId++)
                {
                    InventoryHotBarView.HotBarCells[cellId].SetData(cellsDataHotBar[cellId]);
                    CellViewSubscribe(InventoryHotBarView.HotBarCells[cellId], true);
                }
            }

            InitCookViews();

            ViewSetIsWeave(LoomModel.IsWeave);
            UpdateScrollEnable();
            UpdateCoins();
            SetLocalization();

            ShowBoostButon();
            ShowAddQueueCellButton();



            StatsEffectModel.SetViewContainer(View.transform);
            InfoPanelModel.SetViewContainer(InventoryLoomSectionView.Container);

        }

        protected override void Hide()
        {
            LocalizationModel.OnChangeLanguage -= SetLocalization;

            LoomViewModel.OnChangeSelected -= OnChangeSelectedHandler;
            LoomViewModel.OnChangeHighlighted -= OnChangeHighlightedHandler;

            InventoryViewModel.OnExpand -= OnExpandInventoryChangeHandler;

            InventoryModel.ItemsContainer.OnChangeCell -= OnChangeCellInventoryHandler;

            HotBarModel.ItemsContainer.OnChangeCell -= OnChangeCellHotBarHandler;

            RepairingItemsModel.OnUpdateRepairing -= OnUpdateRepairingHandler;
            RepairingItemsModel.OnChangeCount -= OnChangeCountRepairingHandler;

            LoomModel.ItemsContainer.OnChangeCell -= OnChangeCellLoomHandler;
            LoomModel.OnChangeWeaveState -= OnChangeWeaveStateHandler;
            LoomModel.OnStartBoost -= OnStartBoostHandler;
            LoomModel.OnUpdateBoostRemainingTime -= OnUpdateBoostRemainingTimeHandler;
            LoomModel.OnEndBoost -= OnEndBoostHandler;

            PlayerConsumeModel.OnStartConsume.RemoveListener(InventoryModel.ItemsContainer, OnStartConsumePlayerHandler);
            PlayerConsumeModel.OnUpdateConsume.RemoveListener(InventoryModel.ItemsContainer, OnUpdateConsumePlayerHandler);
            PlayerConsumeModel.OnEndConsume.RemoveListener(InventoryModel.ItemsContainer, OnEndConsumePlayerHandler);

            PlayerConsumeModel.OnStartConsume.RemoveListener(HotBarModel.ItemsContainer, OnStartConsumePlayerHandler);
            PlayerConsumeModel.OnUpdateConsume.RemoveListener(HotBarModel.ItemsContainer, OnUpdateConsumePlayerHandler);
            PlayerConsumeModel.OnEndConsume.RemoveListener(HotBarModel.ItemsContainer, OnEndConsumePlayerHandler);

            PlayerConsumeModel.OnStartConsume.RemoveListener(LoomModel.ItemsContainer, OnStartConsumePlayerHandler);
            PlayerConsumeModel.OnUpdateConsume.RemoveListener(LoomModel.ItemsContainer, OnUpdateConsumePlayerHandler);
            PlayerConsumeModel.OnEndConsume.RemoveListener(LoomModel.ItemsContainer, OnEndConsumePlayerHandler);

            NetworkModel.OnInternetConnectionStateChange -= OnChangeConnectionHandler;

            ExpandModel.OnExpand -= OnExpandLootChangeHandler;

            CoinsModel.OnChange -= OnChangeCoins;

            View.OnClose -= OnCloseViewHandler;
            InventoryMainView.OnApplyItem -= OnApplyItemHandler;
            InventoryMainView.OnDivideItems -= OnDividetemHandler;
            InventoryMainView.OnTrash -= OnTrashHandler;
            InventoryMainView.OnRepairItem -= OnRepairItemHandler;
            InventoryLoomSectionView.OnSetWeave -= OnSetWeaveHandler;
            InventoryLoomSectionView.OnTurnOff -= OnTurnOffHandler;
            InventoryMainView.OnAddCoins -= OnAddCoinsHandler;
            InventoryMainView.OnScrollChanged -= OnInventoryScrollChanged;

            InventoryMainView.OnAutoStackItems -= OnAutoStackItems;
            TutorialModel.OnStepChanged -= UpdateScrollEnable;
            TutorialModel.OnComplete -= UpdateScrollEnable;

            DivideItemsModel.OnItemSplitted -= OnItemSplitted;

            DivideItemsModel.OnHideDividePopup -= OnHideDividePopup;



            NetworkModel.OnInternetConnectionStateChange -= OnInternetConnectionStateChangeHandler;



            CellViewsDesubscribe(InventoryMainView.InventoryCells);
            CellViewsDesubscribe(InventoryHotBarView.HotBarCells);
            CellViewsDesubscribe(InventoryLoomSectionView.WeaveCells);



            ExpandInventoryModel.HideExpandInventoryButton();

            HideAddQueueCellButton();
            HideBoostButton();

            LoomGroupModel.ActiveLoom = null;



            StatsEffectModel.HideStatsEffect();

            InfoPanelModel.HideItemInfo();

            InventoryDragAndDropModel.OnHideView(View);
        }

        private void UpdateBoostButton()
        {
            HideBoostButton();
            ShowBoostButon();
        }

        private void HideBoostButton()
        {
            HideBoostButtonGold();
            HideBoostButtonWatch();
        }

        private void HideBoostButtonGold()
        {
            if (BoostButtonWatchView != null)
            {
                BoostButtonWatchView.OnClick -= OnBoostHandler;
                ViewsSystem.Hide(BoostButtonWatchView);
                BoostButtonWatchView = null;
            }
        }

        private void HideBoostButtonWatch()
        {
            if (BoostButtonGoldView != null)
            {
                BoostButtonGoldView.OnClick -= OnBoostHandler;
                ViewsSystem.Hide(BoostButtonGoldView);
                BoostButtonGoldView = null;
            }
        }

        private void ShowBoostButon()
        {
            if (IsBoostButtonWatch)
            {
                WatchPurchaseLoomBoost.Prepere();
                ShowBoostButtonWatch();
            }
            else
            {
                ShowBoostButtonGold();
            }
        }

        private void ShowBoostButtonWatch()
        {
            BoostButtonWatchView = ViewsSystem.Show<BoostButtonWatchView>(ViewConfigID.BoostWatchButton, InventoryLoomSectionView.ContainerBoostButton);
            BoostButtonWatchView.transform.localPosition = Vector3.zero;
            BoostButtonWatchView.OnClick += OnBoostHandler;
            BoostButtonWatchView.SetIsInteractable(!LoomModel.IsBoost);
            BoostButtonWatchView.SetIsVisibleHandler(LoomModel.IsBoost);
            BoostButtonWatchView.SetFillAmount(GetBoostFillAmount());
            BoostButtonWatchView.SetText(LocalizationModel.GetString(LocalizationKeyID.CampFireMenu_BoostBtn));
        }

        private void ShowBoostButtonGold()
        {
            BoostButtonGoldView = ViewsSystem.Show<BoostButtonGoldView>(ViewConfigID.BoostGoldButton, InventoryLoomSectionView.ContainerBoostButton);
            BoostButtonGoldView.transform.localPosition = Vector3.zero;
            BoostButtonGoldView.OnClick += OnBoostHandler;
            BoostButtonGoldView.SetIsInteractable(!LoomModel.IsBoost);
            BoostButtonGoldView.SetIsVisibleHandler(LoomModel.IsBoost);
            BoostButtonGoldView.SetFillAmount(GetBoostFillAmount());
            UpdateBoostButtonGoldText();
            BoostButtonGoldView.SetText(LocalizationModel.GetString(LocalizationKeyID.CampFireMenu_BoostBtn));
        }

        private void UpdateBoostButtonGoldText() => BoostButtonGoldView.SetCount(PurchasesModel.GetInfo<IPurchaseCoinInfo>(PurchaseID.LoomBoostGold).CoinCost.ToString());

        private float GetBoostFillAmount() => LoomModel.IsBoost
                ? LoomModel.BoostRemainingTime / LoomModel.BoostTime
                : 1;

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
                    WatchPurchaseLoomSlotBuy.Prepere();
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
            AddCellGoldView = ViewsSystem.Show<AddCellGoldView>(ViewConfigID.AddCellGold, InventoryLoomSectionView.ContainerExpandLootCell);
            AddCellGoldView.transform.localPosition = Vector3.zero;
            AddCellGoldView.OnClick += OnExpandLoomHandler;
            AddCellGoldView.SetTextGoldValue(PurchasesModel.GetInfo<IPurchaseCoinInfo>(PurchaseID.LoomSlotBuyGold).CoinCost.ToString());
        }

        private void ShowAddQueueCellButtonWatch()
        {
            AddCellWatchView = ViewsSystem.Show<AddCellWatchView>(ViewConfigID.AddCellWatch, InventoryLoomSectionView.ContainerExpandLootCell);
            AddCellWatchView.transform.localPosition = Vector3.zero;
            AddCellWatchView.OnClick += OnExpandLoomHandler;
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
                AddCellWatchView.OnClick -= OnExpandLoomHandler;
                ViewsSystem.Hide(AddCellWatchView);
                AddCellWatchView = null;
            }
        }

        private void HideAddQueueCellButtonGold()
        {
            if (AddCellGoldView != null)
            {
                AddCellGoldView.OnClick -= OnExpandLoomHandler;
                ViewsSystem.Hide(AddCellGoldView);
                AddCellGoldView = null;
            }
        }

        private void OnChangeConnectionHandler()
        {
            UpdateAddQueueCellButton();
            UpdateBoostButton();
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
                var containerSelectedID = LoomViewModel.SelectedCell.ContainerID;
                var cellSelectedId = LoomViewModel.SelectedCell.CellId;
                if (containerID != ContainerID.None)
                {
                    LoomViewModel.SelectCell(containerSelectedID, cellSelectedId);
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

        private void OnTurnOffHandler() => LoomModel.SetWeave(false);

        private void OnSetWeaveHandler() => LoomModel.SetWeave(true);

        private void OnEndBoostHandler()
        {
            BoostButtonIsVisibleHandler(false);
            BoostButtonFillAmount(1);
            BoostButtonIsInteractable(true);
        }

        private void OnUpdateBoostRemainingTimeHandler() => BoostButtonFillAmount(LoomModel.BoostRemainingTime / LoomModel.BoostTime);

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
                PurchasesModel.Purchase(PurchaseID.LoomBoostWatch, OnPurchaseBoostWatch);
            }
            else
            {
                PurchasesModel.Purchase(PurchaseID.LoomBoostGold, OnPurchaseBoostGold);
            }
        }

        private void OnPurchaseBoostWatch(PurchaseResult purchaseResult)
        {
            if (purchaseResult == PurchaseResult.Successfully)
            {
                LoomModel.Boost();
            }
        }

        private void OnPurchaseBoostGold(PurchaseResult purchaseResult)
        {
            if (purchaseResult == PurchaseResult.Successfully)
            {
                LoomModel.Boost();
            }
            else
            {
                ViewsSystem.Show<PurchasesView>(ViewConfigID.Purchases);
            }
        }

        private void OnChangeWeaveStateHandler() => ViewSetIsWeave(LoomModel.IsWeave);

        private void ViewSetIsWeave(bool isWeave)
        {
            InventoryLoomSectionView.SetIsVisibleSetWeaveButton(!isWeave);
            InventoryLoomSectionView.SetIsVisibleTurnOffButton(isWeave);
        }

        private void InitCookViews()
        {
            var isExpandedMax = ExpandModel.IsMaxLevel;

            InventoryLoomSectionView.SetIsVisibleFirstLootCell(isExpandedMax);

            var cellsDataLoot = CollectCookCellsData(LoomModel.ItemsContainer, (int)ContainerID.Loom);
            var cellCountActive = LoomModel.ItemsContainer.CountCells;

            if (isExpandedMax)
            {
                InventoryLoomSectionView.WeaveCells[0].SetData(cellsDataLoot[cellsDataLoot.Length - 1]);
                CellViewSubscribe(InventoryLoomSectionView.WeaveCells[0]);

                for (int cellId = 1; cellId < cellCountActive; cellId++)
                {
                    InventoryLoomSectionView.WeaveCells[cellId].SetData(cellsDataLoot[cellId - 1]);
                    CellViewSubscribe(InventoryLoomSectionView.WeaveCells[cellId]);
                }

                for (int cellId = cellCountActive; cellId < InventoryLoomSectionView.WeaveCells.Length; cellId++)
                {
                    InventoryLoomSectionView.WeaveCells[cellId].SetAsPassive();
                }
            }
            else
            {
                for (int cellId = 1; cellId < cellCountActive + 1; cellId++)
                {
                    InventoryLoomSectionView.WeaveCells[cellId].SetData(cellsDataLoot[cellId - 1]);
                    CellViewSubscribe(InventoryLoomSectionView.WeaveCells[cellId]);
                }

                for (int cellId = cellCountActive + 1; cellId < InventoryLoomSectionView.WeaveCells.Length; cellId++)
                {
                    InventoryLoomSectionView.WeaveCells[cellId].SetAsPassive();
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

        private void CellViewSubscribe(WeaveCellView cellView)
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

        private void CellViewDesubscribe(WeaveCellView cellView)
        {
            cellView.OnPointDown_ -= OnClickCellViewHandler;
            cellView.OnDragBegin_ -= OnDragBeginCellViewHandler;
            cellView.OnDrag_ -= OnDragCellViewHandler;
            cellView.OnDragEnd_ -= OnDragEndCellViewHandler;
        }

        private void CellViewsDesubscribe(CellView[] views)
        {
            foreach (var view in views)
            {
                CellViewDesubscribe(view);
            }
        }

        private void CellViewsDesubscribe(WeaveCellView[] views)
        {
            foreach (var view in views)
            {
                CellViewDesubscribe(view);
            }
        }

        private void OnExpandLootChangeHandler(int id)
        {
            LoomModel.ItemsContainer.AddCell();

            UpdateAddQueueCellButton();
            CellViewsDesubscribe(InventoryLoomSectionView.WeaveCells);
            InitCookViews();
        }

        private void OnExpandInventoryChangeHandler(int id)
        {
            UpdateInventoryViews();



            if (!InventoryViewModel.IsMaxExpandLevel)

            {

                ExpandInventoryModel.SetButtonContainer(InventoryMainView.ContainersUnlockSlots[InventoryViewModel.TotalExpandedCountLevel]);

            }

            ExpandInventoryModel.UpdateExpandInventoryButton();



            SetHighlight(LoomViewModel.HighlightedCell, true);
        }

        private void UpdateInventoryViews()
        {
            CellViewsDesubscribe(InventoryMainView.InventoryCells);
            InitInventoryViews();
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
            SetHighlight(LoomViewModel.HighlightedLast, false);
            SetHighlight(cellInfo, true);
        }

        private void SetHighlight(CellInfo cellInfo, bool isSelect)
        {
            SetHighlight(cellInfo.containerID, cellInfo.cellId, isSelect);
        }

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
                case ContainerID.Loom:
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

        private void SerHiglghtCell(WeaveCellView[] views, int cellId, bool isSelect)
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

            var containerID = LoomViewModel.SelectedCell.containerID;

            var cellId = LoomViewModel.SelectedCell.cellId;



            var container = GetContainer(containerID);

            var cell = container.GetCell(cellId);



            ApplyItemModel.ApplyItem(container, cell);
        }

        private void OnDividetemHandler()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            var cellId = LoomViewModel.SelectedCell.cellId;
            var containerID = LoomViewModel.SelectedCell.containerID;
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

            var containerID = LoomViewModel.SelectedCell.containerID;

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
                        result = GetContainer(ContainerID.Loom).GetEmptyCell();
                    }
                }
                else if (containerID == ContainerID.Inventory)
                {
                    result = GetContainer(ContainerID.HotBar).GetEmptyCell();
                    if (result == null)
                    {
                        result = GetContainer(ContainerID.Loom).GetEmptyCell();
                    }
                }
                else if (containerID == ContainerID.Loom)
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
            var containerID = LoomViewModel.SelectedCell.containerID;
            var container = GetContainer(containerID);
            var cellId = LoomViewModel.SelectedCell.cellId;

            if (containerID != ContainerID.None)
            {
                AudioSystem.PlayOnce(AudioID.DropItem);
                DeleteItemModel.ShowDeletePopup(container, cellId);
            }
        }

        private void OnRepairItemHandler()
        {
            var container = GetContainer(LoomViewModel.SelectedCell.containerID);
            var item = container.GetCell(LoomViewModel.SelectedCell.cellId).Item;

            RepairViewModel.SetItem(item);
            InventoryRepairingModel.ShowRepairPopup();

        }

        private void SetSelection(CellInfo cellInfo) => SetSelection(cellInfo, GetContainer(cellInfo.containerID));

        private void SetSelection(CellInfo cellInfo, ItemsContainer containers) => ViewSetSelection(containers.GetCell(cellInfo.cellId));

        private void OnExpandLoomHandler()
        {
            if (IsAddQueueCellButtonWatch)
            {
                PurchasesModel.Purchase(PurchaseID.LoomSlotBuyWatch, OnPurchaseExpandWeaveWatch);
            }
            else
            {
                PurchasesModel.Purchase(PurchaseID.LoomSlotBuyGold, OnPurchaseExpandWeaveGold);
            }
        }

        private void OnPurchaseExpandWeaveWatch(PurchaseResult purchaseResult)
        {
            if (purchaseResult == PurchaseResult.Successfully)
            {
                ExpandModel.ExpandCells();
            }
        }

        private void OnPurchaseExpandWeaveGold(PurchaseResult purchaseResult)
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
                var cellInfo = LoomViewModel.SelectedCell;
                var cellModel = GetContainer(cellInfo.containerID).GetCell(cellInfo.cellId);
                var data = CollectCellDataModel.CollectCellData(cellModel, (int)cellInfo.containerID);

                isCanRepair = durabilityProperty.Float.Current <= 0 && !data.ProgressRepairing.HasValue;
            }

            InventoryMainView.SetIsActiveApplyButton(isCanApply);
            InventoryMainView.SetIsActiveDivideButton(isCanDivide);
            InventoryMainView.SetIsActiveTrashButton(isCanTrash);
            InventoryMainView.SetIsActiveRepairButton(isCanRepair);

            InventoryLoomSectionView.SetIsVisibleWeavingProcess(false);

            SelectItemData(item.ItemData);
        }

        private void ViewRemoveSelected()
        {
            InventoryMainView.SetIsActiveApplyButton(false);
            InventoryMainView.SetIsActiveDivideButton(false);
            InventoryMainView.SetIsActiveTrashButton(false);
            InventoryMainView.SetIsActiveRepairButton(false);
            InventoryLoomSectionView.SetIsVisibleWeavingProcess(true);
            InventoryLoomSectionView.SetIsVisibleItemName(true);
            InventoryLoomSectionView.SetIsVisibleItemDescription(true);
            InfoPanelModel.HideItemInfo();

        }

        private void OnPointerDownViewHandler(CellView cell, PointerEventData eventData) => InventoryDragAndDropModel.OnPointerDown(cell, eventData); 

        private void OnClickCellViewHandler(WeaveCellView cell, PointerEventData eventData) => OnClickCellView((ContainerID)cell.ContainerId, cell.Id);

        private void OnClickCellView(ContainerID containerID, int cellId)
        {
            LoomViewModel.SelectCell(containerID, cellId);
            LoomViewModel.HighlightCell(containerID, cellId);
        }

        private void OnDragBeginCellViewHandler(CellView cellView, PointerEventData eventData)
        {
             InventoryDragAndDropModel.OnDragBegin(cellView.gameObject, (ContainerID)cellView.ContainerId, cellView.Id, eventData, () => cellView.SetData(GetDataDefault(cellView.ContainerId, cellView.Id)));
        }

        private void OnDragBeginCellViewHandler(WeaveCellView cellView, PointerEventData eventData)
        {
            InventoryDragAndDropModel.OnDragBegin(cellView.gameObject, (ContainerID)cellView.ContainerId, cellView.Id, eventData, () => cellView.SetData(GetDataCookDefault(cellView.ContainerId, cellView.Id)));
        }

        private void OnDragCellViewHandler(CellView cellView, PointerEventData eventData) 
        {
            InventoryDragAndDropModel.OnDrag(cellView.gameObject, (ContainerID)cellView.ContainerId, cellView.Id, eventData);
        }

        private void OnDragCellViewHandler(WeaveCellView cellView, PointerEventData eventData)
        {
            InventoryDragAndDropModel.OnDrag(cellView.gameObject, (ContainerID)cellView.ContainerId, cellView.Id, eventData);
        }

        private void OnDragEndCellViewHandler(CellView cellView, PointerEventData eventData)
        {
            InventoryDragAndDropModel.OnDragEnd(cellView.gameObject, (ContainerID)cellView.ContainerId, cellView.Id, eventData, () => cellView.SetData(CollectCellDataModel.CollectCellData(GetContainer(cellView.ContainerId).GetCell(cellView.Id), cellView.ContainerId))); 
        }

        private void OnDragEndCellViewHandler(WeaveCellView cellView, PointerEventData eventData) 
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
            var weaveViews = GetCookCellViews();
            for (int i = 0; i < LoomModel.ItemsContainer.Cells.Count(); i++)
            {
                var cell = LoomModel.ItemsContainer.Cells.ElementAt(i);
                weaveViews[i].SetData(GetCellCookData(cell, (int)ContainerID.Loom));
                if (LoomViewModel.HighlightedCell.containerID == ContainerID.Loom &&
                    LoomViewModel.HighlightedCell.cellId == cell.Id)
                {
                    ViewSetSelection(cell);
                    weaveViews[i].Selection();
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

                if (LoomViewModel.HighlightedCell.containerID == containerId &&
                    LoomViewModel.HighlightedCell.cellId == cellModel.Id)
                {
                    ViewSetSelection(cellModel);
                    cellView.Selection();
                }
                return;
            }

            var cookCellView = cellObject.GetComponent<WeaveCellView>();
            if (cookCellView && cookCellView.ContainerId == (int)containerId)
            {
                var cellModel = GetContainer(containerId).Cells.ElementAt(cellId - 1);
                cookCellView.SetData(GetCellCookData(cellModel, (int)containerId));

                if (LoomViewModel.HighlightedCell.containerID == containerId &&
                    LoomViewModel.HighlightedCell.cellId == cellModel.Id)
                {
                    ViewSetSelection(cellModel);
                    cookCellView.Selection();
                }
                return;
            }
        }

        private void OnChangeCellInventoryHandler(CellModel cell) => OnChangeCellHandler(cell, ContainerID.Inventory);

        private void OnChangeCellHotBarHandler(CellModel cell) => OnChangeCellHandler(cell, ContainerID.HotBar);

        private void OnChangeCellLoomHandler(CellModel cell) => OnChangeCellHandler(cell, ContainerID.Loom);

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
                case ContainerID.Loom:
                    ChangeCell(GetCookCellViews(), cell, containerID);
                    break;
            }

            if (containerID != ContainerID.None)
            {
                if (LoomViewModel.SelectedCell.cellId == cell.Id)
                {
                    OnChangeHighlightedHandler(LoomViewModel.HighlightedCell);
                    OnChangeSelectedHandler(LoomViewModel.SelectedCell);
                }
            }

            UpdateBoostButtonGoldText();
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
                case ContainerID.Loom:
                    cellObject = GetCookCellViews()[cellId].gameObject;
                    break;
            }

            return cellObject != null;
        }

        private void ChangeCell(CellView[] views, CellModel cell, ContainerID containerID)
        {
            if (cell.Id < views.Length)
            {
                views[cell.Id].SetData(CollectCellDataModel.CollectCellData(cell, (int)containerID));
            }
        }

        private void ChangeCell(WeaveCellView[] views, CellModel cell, ContainerID containerID)
        {
            if (cell.Id < views.Length)
            {
                views[cell.Id].SetData(GetCellCookData(cell, (int)containerID));
            }
        }

        private WeaveCellView[] GetCookCellViews()
        {
            var result = new WeaveCellView[InventoryLoomSectionView.WeaveCells.Length];
            Array.Copy(InventoryLoomSectionView.WeaveCells, 1, result, 0, InventoryLoomSectionView.WeaveCells.Length - 1);

            if (ExpandModel.IsMaxLevel)
            {
                result[LoomModel.ItemsContainer.CountCells - 1] = InventoryLoomSectionView.WeaveCells[0];
            }

            return result;
        }

        private CellData[] CollectCellsData(ItemsContainer container, int inventoryID) => container.Cells.Select(cell => CollectCellDataModel.CollectCellData(cell, inventoryID)).ToArray();

        private WeaveCellData[] CollectCookCellsData(ItemsContainer container, int inventoryID) => container.Cells.Select(cell => GetCellCookData(cell, inventoryID)).ToArray();

        private CellData GetDataDefault(int containerId, int cellId) => new CellData()
        {
            ContainerId = containerId,
            Id = cellId,
        };

        private WeaveCellData GetDataCookDefault(int containerId, int cellId) => new WeaveCellData()
        {
            ContainerId = containerId,
            Id = cellId,
        };

        private WeaveCellData GetCellCookData(CellModel cell, int containerId)
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

                if (LoomModel.IsWeave)
                {
                    if (item.TryGetProperty("Weaving time", out var cookingTime))
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

                var isConsuming = (item.IsCanConsume() && PlayerConsumeModel.IsInProgress);
                disable = isConsuming || item.IsBroken();
            }

            return new WeaveCellData
            {
                Id = cell.Id,
                ContainerId = containerId,
                Icon = icon,
                Count = count,
                Durability = durability,
                ProcessAmount = ProcessAmount,
                IsBoost = LoomModel.IsBoost,
                ProgressConsumable = progressConsumable,
                ProgressRepairing = progressRepairing,
                Sips = sips,
                IsDisable = disable,
            };
        }

        private void SetLocalization()
        {
            InventoryMainView.ApplyButtonView.SetText(LocalizationModel.GetString(LocalizationKeyID.InventoryMenu_ApplyBtn));
            InventoryLoomSectionView.SetTextSetWeaveButton(LocalizationModel.GetString(LocalizationKeyID.LoomMenu_ProcessingBtn));
            InventoryMainView.SetTextInventoryTitle(LocalizationModel.GetString(LocalizationKeyID.InventoryMenu_Name));
            InventoryLoomSectionView.SetTextLoom(LocalizationModel.GetString(LocalizationKeyID.LoomMenu_Name));
            InventoryLoomSectionView.SetTextTurnOffButton(LocalizationModel.GetString(LocalizationKeyID.LoomMenu_StopBtn));
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
