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
using System.Linq;
using UltimateSurvival;
using UnityEngine;
using UnityEngine.EventSystems;
using CellInfo = Game.Models.InventoryLootViewModel.CellInfo;

namespace Game.Controllers
{
    public class InventoryLootViewControllerData : IDataViewController
    {
        public LootObject LootObject{ get; private set;}
        public InventoryLootViewControllerData(LootObject lootObject)
        {
            LootObject = lootObject;
        }
    }

    public class InventoryLootViewController : WindowViewControllerBase<InventoryLootView, InventoryLootViewControllerData>
    {
        [Inject] public InventoryLootViewModel InventoryLootViewModel { get; private set; }
        [Inject] public ExpandRemoteSettings ExpandRemoteSettings { get; private set; }
        [Inject] public RemoteSettingsModel RemoteSettingsModel { get; private set; }
        [Inject] public RepairingItemsModel RepairingItemsModel { get; private set; }
        [Inject] public PlayerConsumeModel PlayerConsumeModel { get; private set; }
        [Inject] public InventoryViewModel InventoryViewModel { get; private set; }
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public LocalizationModel LocalizationModel { get; private set; }
        [Inject] public RepairViewModel RepairViewModel { get; private set; }
        [Inject] public PurchasesModel PurchasesModel { get; private set; }
        [Inject] public InventoryModel InventoryModel { get; private set; }
        [Inject] public NetworkModel NetworkModel { get; private set; }
        [Inject] public ExpandsModel ExpandsModel { get; private set; }
        [Inject] public HotBarModel HotBarModel { get; private set; }
        [Inject] public CoinsModel CoinsModel { get; private set; }
        [Inject] public StatsEffectModel StatsEffectModel { get; private set; }
        [Inject] public InfoPanelModel InfoPanelModel { get; private set; }
        [Inject] public DivideItemsModel DivideItemsModel { get; private set; }
        [Inject] public DropItemModel DropItemModel { get; private set; }
        [Inject] public ApplyItemModel ApplyItemModel { get; private set; }
        [Inject] public ExpandInventoryModel ExpandInventoryModel { get; private set; }
        [Inject] public FullInventoryModel FullInventoryModel { get; private set; }
        [Inject] public ContainersModel ContainersModel { get; private set; }
        [Inject] public LootGroupModel LootGroupModel { get; private set; }
        [Inject] public InventoryRepairingModel InventoryRepairingModel { get; private set; }
        [Inject] public CollectCellDataModel CollectCellDataModel { get; private set; }
        [Inject] public InventoryDragAndDropModel InventoryDragAndDropModel { get; private set; } 
        [Inject] public EventSystemModel EventSystemModel { get; private set; } 
        [Inject] public TutorialModel TutorialModel { get; private set; }
        [Inject] public DeleteItemModel DeleteItemModel { get; private set; }

        protected LootObject LootObject { get; private set; }
        protected ExpandModel ExpandModel { get; private set; }

        protected AddCellGoldView AddCellGoldView { get; private set; }
        protected AddCellWatchView AddCellWatchView { get; private set; }

        protected bool GetIsWatchAddQueueCellButton(IRemoteSettingsExpand expand) => GetIsWatchAddQueueCellButton(expand, GetWatchPurchaseLootSlotBuy(expand));
        protected bool GetIsWatchAddQueueCellButton(IRemoteSettingsExpand expand, IWatchPurchase watchPurchase) => !expand.IsBuySlotGold && (watchPurchase.IsCanPurchase || NetworkModel.IsHasConnection);

        protected bool IsCanShowAddQueueButton => !ExpandModel.IsMaxLevel;
       

        protected IRemoteSettingsExpand GetRemoteSettingsLootObject(RemoteSettingID remoteSettingsID) => RemoteSettingsModel.Get<IRemoteSettingsExpand>(remoteSettingsID);

        protected IWatchPurchase GetWatchPurchaseLootSlotBuy(IRemoteSettingsExpand expand) => GetWatchPurchaseLootSlotBuy(expand.PurchaseIDBuySlotWatch);
        protected IWatchPurchase GetWatchPurchaseLootSlotBuy(PurchaseID purchaseID) => PurchasesModel.GetInfo<IWatchPurchase>(purchaseID);

        private InventoryMainView InventoryMainView => View.InventoryMain;
        private InventoryLootSectionView InventoryLootSectionView => View.InventoryLootSection;
        private InventoryHotBarView InventoryHotBarView => View.InventoryHotBar;

        protected override void Show()
        {
            base.Show();
            AudioSystem.PlayOnce(AudioID.WindowOpen);

            EventSystemModel.SetSelectedGameObject(View.InventoryMain.InventoryCells[0].gameObject);
            EventSystemModel.OnSelectionChanged += OnSelectionChanged;
            InventoryDragAndDropModel.OnShowInventory(View, InventoryLootViewModel);

            LocalizationModel.OnChangeLanguage += SetLocalization;

            if(Data != null && Data.LootObject != null)
            {
                LootGroupModel.ActiveLoot = Data.LootObject;
            }
            else
            {
                LootGroupModel.ActiveLoot = PlayerEventHandler.RaycastData.Value.GameObject.GetComponent<LootObject>();
            }

            LootObject = LootGroupModel.ActiveLoot;
            LootObject.ItemsContainer.OnChangeCell += OnChangeCellLootHandler;

            ExpandModel = ExpandsModel.Get(LootObject.ExpandGroup);
            ExpandModel.OnExpand += OnExpandLootChangeHandler;

            PlayerConsumeModel.OnStartConsume.AddListener(InventoryModel.ItemsContainer, OnStartConsumePlayerHandler);
            PlayerConsumeModel.OnUpdateConsume.AddListener(InventoryModel.ItemsContainer, OnUpdateConsumePlayerHandler);
            PlayerConsumeModel.OnEndConsume.AddListener(InventoryModel.ItemsContainer, OnEndConsumePlayerHandler);

            PlayerConsumeModel.OnStartConsume.AddListener(HotBarModel.ItemsContainer, OnStartConsumePlayerHandler);
            PlayerConsumeModel.OnUpdateConsume.AddListener(HotBarModel.ItemsContainer, OnUpdateConsumePlayerHandler);
            PlayerConsumeModel.OnEndConsume.AddListener(HotBarModel.ItemsContainer, OnEndConsumePlayerHandler);

            PlayerConsumeModel.OnStartConsume.AddListener(LootObject.ItemsContainer, OnStartConsumePlayerHandler);
            PlayerConsumeModel.OnUpdateConsume.AddListener(LootObject.ItemsContainer, OnUpdateConsumePlayerHandler);
            PlayerConsumeModel.OnEndConsume.AddListener(LootObject.ItemsContainer, OnEndConsumePlayerHandler);

            NetworkModel.OnInternetConnectionStateChange += OnChangeConnectionHandler;

            InventoryLootViewModel.OnChangeSelected += OnChangeSelectedHandler;
            InventoryLootViewModel.OnChangeHighlighted += OnChangeHighlightedHandler;

            InventoryViewModel.OnExpand += OnExpandInventoryChangeHandler;

            InventoryModel.ItemsContainer.OnChangeCell += OnChangeCellInventoryHandler;

            HotBarModel.ItemsContainer.OnChangeCell += OnChangeCellHotBarHandler;

            RepairingItemsModel.OnUpdateRepairing += OnUpdateRepairingHandler;
            RepairingItemsModel.OnChangeCount += OnChangeCountRepairingHandler;

            CoinsModel.OnChange += OnChangeCoins;

            if (LootObject.ItemsContainer.CountCells < LootObject.CountOpenStart)
            {
                LootObject.ItemsContainer.AddCells(LootObject.CountOpenStart - LootObject.ItemsContainer.CountCells);
            }

            View.OnClose += OnCloseViewHandler;
            InputModel.OnInput.AddListener(PlayerActions.UIMenu_B,OnCloseViewHandler);
            InventoryMainView.OnApplyItem += OnApplyItemHandler;
            InventoryMainView.OnDivideItems += OnDividetemHandler;
            InventoryMainView.OnTrash += OnTrashHandler;
            InventoryMainView.OnRepairItem += OnRepairItemHandler;
            InventoryLootSectionView.OnTake += OnTakeHandler;
            InventoryLootSectionView.OnTakeAll += OnTakeAllHandler;
            InputModel.OnInput.AddListener(PlayerActions.UIMenu_Y,OnTakeAllHandler);
            InventoryMainView.OnAddCoins += OnAddCoinsHandler;
            InventoryMainView.OnAutoStackItems += OnAutoStackItems;
            InventoryMainView.OnScrollChanged += OnInventoryScrollChanged;
            TutorialModel.OnStepChanged += UpdateScrollEnable;
            TutorialModel.OnComplete += UpdateScrollEnable;

            NetworkModel.OnInternetConnectionStateChange += OnInternetConnectionStateChangeHandler;

            if (!InventoryViewModel.IsMaxExpandLevel)
            {
                ExpandInventoryModel.SetButtonContainer(InventoryMainView.ContainersUnlockSlots[InventoryViewModel.TotalExpandedCountLevel]);
            }
            ExpandInventoryModel.ShowExpandInventoryButton();

            UpdateScrollEnable();
            InitInventoryViews();
            InitHotBarViews();
            InitLootViews();

            ShowAddQueueCellButton();
            UpdateCoins();

            SetLocalization();

            NetworkModel.UpdateInternetConnectionStatus();

            InventoryLootViewModel.RemoveSelectCell();
            InventoryLootViewModel.RemoveHighlightCell();
            InventoryLootViewModel.SelectedCell.Reset();
            InventoryLootViewModel.HighlightedCell.Reset();

            StatsEffectModel.SetViewContainer(View.transform);
            InfoPanelModel.SetViewContainer(InventoryLootSectionView.Container);

            InventoryLootViewModel.OnTakeAllButtonClickedChanged += UpdateTutorialHilight;
            UpdateTutorialHilight();
            
            InventoryLootViewModel.SetShow(true);
        }

        protected override void Hide()
        {
            base.Hide();
            EventSystemModel.OnSelectionChanged -= OnSelectionChanged;

            LocalizationModel.OnChangeLanguage -= SetLocalization;

            InventoryLootViewModel.OnChangeSelected -= OnChangeSelectedHandler;
            InventoryLootViewModel.OnChangeHighlighted -= OnChangeHighlightedHandler;

            InventoryViewModel.OnExpand -= OnExpandInventoryChangeHandler;

            PlayerConsumeModel.OnStartConsume.RemoveListener(InventoryModel.ItemsContainer, OnStartConsumePlayerHandler);
            PlayerConsumeModel.OnUpdateConsume.RemoveListener(InventoryModel.ItemsContainer, OnUpdateConsumePlayerHandler);
            PlayerConsumeModel.OnEndConsume.RemoveListener(InventoryModel.ItemsContainer, OnEndConsumePlayerHandler);

            PlayerConsumeModel.OnStartConsume.RemoveListener(HotBarModel.ItemsContainer, OnStartConsumePlayerHandler);
            PlayerConsumeModel.OnUpdateConsume.RemoveListener(HotBarModel.ItemsContainer, OnUpdateConsumePlayerHandler);
            PlayerConsumeModel.OnEndConsume.RemoveListener(HotBarModel.ItemsContainer, OnEndConsumePlayerHandler);

            PlayerConsumeModel.OnStartConsume.RemoveListener(LootObject.ItemsContainer, OnStartConsumePlayerHandler);
            PlayerConsumeModel.OnUpdateConsume.RemoveListener(LootObject.ItemsContainer, OnUpdateConsumePlayerHandler);
            PlayerConsumeModel.OnEndConsume.RemoveListener(LootObject.ItemsContainer, OnEndConsumePlayerHandler);

            NetworkModel.OnInternetConnectionStateChange -= OnChangeConnectionHandler;

            InventoryModel.ItemsContainer.OnChangeCell -= OnChangeCellInventoryHandler;

            HotBarModel.ItemsContainer.OnChangeCell -= OnChangeCellHotBarHandler;

            RepairingItemsModel.OnUpdateRepairing -= OnUpdateRepairingHandler;
            RepairingItemsModel.OnChangeCount -= OnChangeCountRepairingHandler;

            LootObject.ItemsContainer.OnChangeCell -= OnChangeCellLootHandler;

            ExpandModel.OnExpand -= OnExpandLootChangeHandler;

            CoinsModel.OnChange -= OnChangeCoins;

            View.OnClose -= OnCloseViewHandler;
            InputModel.OnInput.RemoveListener(PlayerActions.UIMenu_B,OnCloseViewHandler);
            InventoryMainView.OnApplyItem -= OnApplyItemHandler;
            InventoryMainView.OnDivideItems -= OnDividetemHandler;
            InventoryMainView.OnTrash -= OnTrashHandler;
            InventoryMainView.OnRepairItem -= OnRepairItemHandler;
            InventoryLootSectionView.OnTake -= OnTakeHandler;
            InventoryLootSectionView.OnTakeAll -= OnTakeAllHandler;
            InputModel.OnInput.RemoveListener(PlayerActions.UIMenu_Y,OnTakeAllHandler);
            InventoryMainView.OnAddCoins -= OnAddCoinsHandler;
            InventoryMainView.OnAutoStackItems -= OnAutoStackItems;
            InventoryMainView.OnScrollChanged -= OnInventoryScrollChanged;
            DivideItemsModel.OnItemSplitted -= OnItemSplitted;
            DivideItemsModel.OnHideDividePopup -= OnHideDividePopup;
            TutorialModel.OnStepChanged -= UpdateScrollEnable;
            TutorialModel.OnComplete -= UpdateScrollEnable;

            NetworkModel.OnInternetConnectionStateChange -= OnInternetConnectionStateChangeHandler;

            DesubscribeCellView(InventoryMainView.InventoryCells);
            DesubscribeCellView(InventoryHotBarView.HotBarCells);
            DesubscribeCellView(InventoryLootSectionView.LootCells);

            ExpandInventoryModel.HideExpandInventoryButton();
            HideAddQueueCellButton();

            StatsEffectModel.HideStatsEffect();
            InfoPanelModel.HideItemInfo();

            InventoryDragAndDropModel.OnHideView(View);

            InventoryLootViewModel.OnTakeAllButtonClickedChanged -= UpdateTutorialHilight;

            InventoryLootViewModel.SetShow(false);
        }

        private void OnSelectionChanged(GameObject newSelectedObject, GameObject oldSelectedObject)
        {
            CellView cell = newSelectedObject.CheckNull()?.GetComponent<CellView>();
            if(cell == null) return;

            cell.OnPointerDown(EventSystemModel.GetPointerData());
        }

        private void UpdateAddQueueCellButton()
        {
            HideAddQueueCellButton();
            ShowAddQueueCellButton();
        }

        private void ShowAddQueueCellButton()
        {
            var remoteSettingsID = ExpandRemoteSettings.Get(LootObject.ExpandGroup);

            if (remoteSettingsID != RemoteSettingID.None)
            {
                if (IsCanShowAddQueueButton)
                {
                    var expand = GetRemoteSettingsLootObject(remoteSettingsID);
                    var watchPurchase = GetWatchPurchaseLootSlotBuy(expand);

                    if (GetIsWatchAddQueueCellButton(expand, watchPurchase))
                    {
                        watchPurchase.Prepere();
                        ShowAddQueueCellButtonWatch();
                    }
                    else
                    {
                        ShowAddQueueCellButtonGold(expand);
                    }
                }
            }
        }

        private void ShowAddQueueCellButtonGold(IRemoteSettingsExpand expand)
        {
            var addCellGoldView = ViewsSystem.Show<AddCellGoldView>(ViewConfigID.AddCellGold, InventoryLootSectionView.ContainerExpandLootCell);
            addCellGoldView.transform.localPosition = Vector3.zero;
            addCellGoldView.OnClick += OnExpandLootHandler;
            addCellGoldView.SetTextGoldValue(PurchasesModel.GetInfo<IPurchaseCoinInfo>(expand.PurchaseIDBuySlotGold).CoinCost.ToString());

            AddCellGoldView = addCellGoldView;
        }

        private void ShowAddQueueCellButtonWatch()
        {
            var addcellWatchView = ViewsSystem.Show<AddCellWatchView>(ViewConfigID.AddCellWatch, InventoryLootSectionView.ContainerExpandLootCell);
            addcellWatchView.transform.localPosition = Vector3.zero;
            addcellWatchView.OnClick += OnExpandLootHandler;

            AddCellWatchView = addcellWatchView;
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

        private void OnChangeConnectionHandler()
        {
            UpdateAddQueueCellButton();
        }

        private void OnInternetConnectionStateChangeHandler() => ExpandInventoryModel.UpdateExpandInventoryButton();

        private void OnAddCoinsHandler() => ViewsSystem.Show<PurchasesView>(ViewConfigID.Purchases);

        private void OnChangeCoins() => UpdateCoins();

        private void UpdateCoins() => InventoryMainView.SetCoins(CoinsModel.Coins.ToString());

        private void OnStartConsumePlayerHandler()
        {
            var containerID = ContainersModel.GetContainerID(PlayerConsumeModel.Container);

            SelectItem(PlayerConsumeModel.Item);

            if (containerID != ContainerID.None)
            {
                UpdateCells();
                UpdateConsumeCellView(containerID, PlayerConsumeModel.CellId);
                CreateAddStatsEffect(PlayerConsumeModel.Item, containerID, PlayerConsumeModel.CellId);
            }
        }

        private void OnUpdateConsumePlayerHandler()
        {
            var containerID = ContainersModel.GetContainerID(PlayerConsumeModel.Container);

            UpdateCells();

            if (containerID != ContainerID.None)
            {
                UpdateConsumeCellView(containerID, PlayerConsumeModel.CellId);
            }
        }

        private void OnEndConsumePlayerHandler()
        {
            UpdateCells();
            UpdateCellView(ContainersModel.GetContainerID(PlayerConsumeModel.Container), PlayerConsumeModel.CellId);

            InventoryLootViewModel.SelectCell(InventoryLootViewModel.SelectedCell.ContainerID, InventoryLootViewModel.SelectedCell.CellId);
        }

        private void OnUpdateRepairingHandler() => UpdateCells();

        private void OnChangeCountRepairingHandler() => UpdateCells();

        private void CreateAddStatsEffect(SavableItem item, ContainerID containerID, int cellId)
        {
            if (TryGetContainerViews(containerID, out var cellViews))
            {
                var cellView = cellViews[cellId];
                StatsEffectModel.AddStatsEffect(item, cellView.transform.position);
            }
        }

        private void UpdateConsumeCellView(ContainerID containerID, int cellId)
        {
            if (TryGetCellView(containerID, cellId, out var view))
            {
                view.SetData(CollectCellDataModel.CollectConsumeCellData(PlayerConsumeModel.Item, (int)containerID, cellId));

                if (InventoryLootViewModel.HighlightedCell.containerID == containerID && InventoryLootViewModel.HighlightedCell.cellId == cellId)
                {
                    view.Selection();
                }
            }
        }

        private void UpdateCellView(ContainerID containerID, int cellId)
        {
            if (TryGetCellView(containerID, cellId, out var view))
            {
                view.SetData(CollectCellDataModel.CollectCellData(GetContainer(containerID).GetCell(cellId), (int)containerID));

                if (InventoryLootViewModel.HighlightedCell.containerID == containerID && InventoryLootViewModel.HighlightedCell.cellId == cellId)
                {
                    view.Selection();
                }
            }
        }

        private void UpdateLootViews()
        {
            DesubscribeCellView(InventoryLootSectionView.LootCells);
            InitLootViews();
        }

        private void InitLootViews()
        {
            var isExpandedMax = ExpandModel.IsMaxLevel;
            var cellsDataLoot = CollectCellsData(LootObject.ItemsContainer, (int)ContainerID.Loot);
            var cellCountActive = LootObject.CountOpenStart + ExpandModel.ExpandLevel;

            if (isExpandedMax)
            {
                InventoryLootSectionView.LootCells[0].SetData(cellsDataLoot[cellsDataLoot.Length - 1]);
                CellViewSubscribe(InventoryLootSectionView.LootCells[0], true);

                for (int cellId = 1; cellId < cellCountActive; cellId++)
                {
                    InventoryLootSectionView.LootCells[cellId].SetData(cellsDataLoot[cellId - 1]);
                    CellViewSubscribe(InventoryLootSectionView.LootCells[cellId], true);
                }

                for (int cellId = cellCountActive; cellId < InventoryLootSectionView.LootCells.Length; cellId++)
                {
                    InventoryLootSectionView.LootCells[cellId].SetAsPassive();
                }
            }
            else
            {
                for (int cellId = 1; cellId < cellCountActive + 1; cellId++)
                {
                    InventoryLootSectionView.LootCells[cellId].SetData(cellsDataLoot[cellId - 1]);
                    CellViewSubscribe(InventoryLootSectionView.LootCells[cellId], true);
                }

                for (int cellId = cellCountActive + 1; cellId < InventoryLootSectionView.LootCells.Length; cellId++)
                {
                    InventoryLootSectionView.LootCells[cellId].SetAsPassive();
                }
            }

            InventoryLootSectionView.SetIsVisibleFirstLootCell(isExpandedMax);
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
            UpdateLootViews();
        }

        private void OnExpandInventoryChangeHandler(int id)
        {
            UpdateInventoryViews();
            if (!InventoryViewModel.IsMaxExpandLevel)
            {
                ExpandInventoryModel.SetButtonContainer(InventoryMainView.ContainersUnlockSlots[InventoryViewModel.TotalExpandedCountLevel]);
            }
            ExpandInventoryModel.UpdateExpandInventoryButton();
            TrySetHighlight(InventoryLootViewModel.HighlightedCell, true);
        }

        private void UpdateInventoryViews()
        {
            DesubscribeCellView(InventoryMainView.InventoryCells);
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
            if (cellInfo != null && TryGetContainer(cellInfo.containerID, out var container))
            {
                SetSelection(cellInfo, container);
            }
            else
            {
                RemoveSelected();
            }
        }

        private ItemsContainer GetContainer(int type) => ContainersModel.GetContainer((ContainerID)type);

        private bool TryGetContainer(ContainerID type, out ItemsContainer container) => (container = GetContainer(type)) != null;

        private ItemsContainer GetContainer(ContainerID type) => ContainersModel.GetContainer(type); 

        private void OnChangeHighlightedHandler(CellInfo cellInfo)
        {
            TrySetHighlight(InventoryLootViewModel.HighlightedLast, false);
            TrySetHighlight(cellInfo, true);
        }

        private void TrySetHighlight(CellInfo cellInfo, bool isSelect)
        {
            if(cellInfo == null) return;

            SetHighlight(cellInfo.containerID, cellInfo.cellId, isSelect);
        }

        private void SetHighlight(ContainerID containerID, int cellId, bool isSelect)
        {
            if (TryGetCellView(containerID, cellId, out var view))
            {
                if (isSelect)
                {
                    view.Selection();
                }
                else
                {
                    if(containerID == ContainerID.Loot)
                    {
                        int cellCountActive = LootObject.CountOpenStart + ExpandModel.ExpandLevel;
                        bool isActive = cellId < cellCountActive;
                        view.Deselection(isActive);
                    }
                    else
                    {
                        view.Deselection(true);
                    }
                }
            }
        }

        private void OnApplyItemHandler()
        {
            var containerID = InventoryLootViewModel.SelectedCell.containerID;
            var cellId = InventoryLootViewModel.SelectedCell.cellId;

            var container = GetContainer(containerID);
            var cell = container.GetCell(cellId);

            ApplyItemModel.ApplyItem(container, cell);
        }

        private void OnDividetemHandler()
        {
            AudioSystem.PlayOnce(AudioID.Button);

            var containerID = InventoryLootViewModel.SelectedCell.containerID;
            var cellId = InventoryLootViewModel.SelectedCell.cellId;
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
            var containerID = InventoryLootViewModel.SelectedCell.containerID;
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
            if (containerID == ContainerID.Loot && !LootObject.IsStorage) //has divide only in Loot storagable container
            {
                result = GetContainer(ContainerID.Inventory).GetEmptyCell();
                if (result == null)
                {
                    result = GetContainer(ContainerID.HotBar).GetEmptyCell();
                }

                return result != null;
            }

            result = GetContainer(containerID).GetEmptyCell();
            if (result == null)
            {
                if (containerID == ContainerID.HotBar)
                {
                    result = GetContainer(ContainerID.Inventory).GetEmptyCell();
                    if (result == null && LootObject.IsStorage)
                    {
                        result = GetContainer(ContainerID.Loot).GetEmptyCell();
                    }
                }
                else if (containerID == ContainerID.Inventory)
                {
                    result = GetContainer(ContainerID.HotBar).GetEmptyCell();
                    if (result == null && LootObject.IsStorage)
                    {
                        result = GetContainer(ContainerID.Loot).GetEmptyCell();
                    }
                }
                else if (containerID == ContainerID.Loot)
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

        private void OnTakeAllHandler()
        {
            foreach (var cellFrom in LootObject.ItemsContainer.Cells.Take(LootObject.CountOpenStart + ExpandModel.ExpandLevel))
            {
                if (!TryAddAllItems(cellFrom, InventoryModel.ItemsContainer))
                    if (!TryAddAllItems(cellFrom, HotBarModel.ItemsContainer))
                    {
                        FullInventoryModel.ShowFullPopup();
                        break;
                    }
            }

            InventoryLootViewModel.RemoveSelectCell();

            AudioSystem.PlayOnce(AudioID.PickUp);
            

            InventoryLootViewModel.TakeAllButtonClicked();
        }

        private void OnTakeHandler()
        {
            var containerFrom = GetContainer(InventoryLootViewModel.SelectedCell.containerID);
            var cellFrom = containerFrom.GetCell(InventoryLootViewModel.SelectedCell.cellId);
            var containerTo = InventoryModel.ItemsContainer;

            TryAddAllItems(cellFrom, containerTo);

            AudioSystem.PlayOnce(AudioID.PickUp);
        }

        private bool TryAddAllItems(CellModel cellFrom, ItemsContainer containerTo)
        {
            if (!cellFrom.IsHasItem)
            {
                return true;
            }

            if (cellFrom.Item.IsCanStackable)
            {
                var left = containerTo.AddItemsData(cellFrom.Item, cellFrom.Item.Count);
                if (left == 0)
                {
                    cellFrom.Item = null;
                }
                else
                {
                    cellFrom.Item.Count = left;
                    return false;
                }
            }
            else
            {
                var cellTo = containerTo.Cells.FirstOrDefault(x => !x.IsHasItem);
                if (cellTo != null)
                {
                    cellTo.Item = cellFrom.Item;
                    cellFrom.Item = null;
                }
                else return false;
            }

            return true;
        }

        private void OnTrashHandler()
        {
            var containerID = InventoryLootViewModel.SelectedCell.containerID;
            var container = GetContainer(containerID);
            var cellId = InventoryLootViewModel.SelectedCell.cellId;

            if (containerID != ContainerID.None)
            {
                AudioSystem.PlayOnce(AudioID.DropItem);
                DeleteItemModel.ShowDeletePopup(container, cellId);
            }
        }

        private void OnRepairItemHandler()
        {
            var container = GetContainer(InventoryLootViewModel.SelectedCell.containerID);
            var item = container.GetCell(InventoryLootViewModel.SelectedCell.cellId).Item;

            RepairViewModel.SetItem(item);
            InventoryRepairingModel.ShowRepairPopup();
        }

        private void SetSelection(CellInfo cellInfo) => SetSelection(cellInfo, GetContainer(cellInfo.containerID));

        private void SetSelection(CellInfo cellInfo, ItemsContainer containers) => SetSelection(containers.GetCell(cellInfo.cellId));


        private void OnExpandLootHandler()
        {
            var remoteSettingsID = ExpandRemoteSettings.Get(LootObject.ExpandGroup);

            if (remoteSettingsID != RemoteSettingID.None)
            {
                var expand = GetRemoteSettingsLootObject(remoteSettingsID);

                if (GetIsWatchAddQueueCellButton(expand))
                {
                    PurchasesModel.Purchase(expand.PurchaseIDBuySlotWatch, OnPurchaseExpandLootWatch);
                }
                else
                {
                    PurchasesModel.Purchase(expand.PurchaseIDBuySlotGold, OnPurchaseExpandLootGold);
                }
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

        private void SetSelection(CellModel cell)
        {
            if (cell.IsHasItem)
            {
                SelectItem(cell.Item);
            }
            else
            {
                RemoveSelected();
            }
        }

        private void SelectItemData(ItemData item)
        {
            InventoryLootSectionView.SetEmptyDescriptionVisible(false);
            InfoPanelModel.UpdateItemInfo(item);
        }

        private void SelectItem(SavableItem item)
        {
            var isConsuming = !(PlayerConsumeModel.IsCanConsume && item.IsCanConsume());
            var isCanApply = !isConsuming;
            var isCanDivide = item.Count > 1;
            var isCanRepair = false;
            var isCanTake = InventoryLootViewModel.SelectedCell.containerID == ContainerID.Loot && !LootObject.ItemsContainer.IsEmptyCell(InventoryLootViewModel.SelectedCell.cellId);
            var cellCountActive = LootObject.CountOpenStart + ExpandModel.ExpandLevel;
            var isCanTakeAll = false;

            foreach (var cell in LootObject.ItemsContainer.Cells.Take(cellCountActive))
            {
                if (cell.IsHasItem)
                {
                    isCanTakeAll = true;
                    break;
                }
            }

            var isCanTrash = true;

            if (item.TryGetProperty("Durability", out var durabilityProperty))
            {
                var cellInfo = InventoryLootViewModel.SelectedCell;
                var cellModel = GetContainer(cellInfo.containerID).GetCell(cellInfo.cellId);
                var data = CollectCellDataModel.CollectCellData(cellModel, (int)cellInfo.containerID);

                isCanRepair = durabilityProperty.Float.Current <= 0 && !data.ProgressRepairing.HasValue;
            }

            InventoryMainView.SetIsActiveApplyButton(isCanApply);
            InventoryMainView.SetIsActiveDivideButton(isCanDivide);
            InventoryMainView.SetIsActiveTrashButton(isCanTrash);
            InventoryMainView.SetIsActiveRepairButton(isCanRepair);
            InventoryLootSectionView.SetIsVisibleTakeButton(isCanTake);
            InventoryLootSectionView.SetIsVisibleTakeAllButton(isCanTakeAll);

            SelectItemData(item.ItemData);
        }

        private void RemoveSelected()
        {
            InventoryMainView.SetIsActiveApplyButton(false);
            InventoryMainView.SetIsActiveDivideButton(false);
            InventoryMainView.SetIsActiveTrashButton(false);
            InventoryMainView.SetIsActiveRepairButton(false);
            InventoryLootSectionView.SetIsVisibleTakeButton(false);

            var isCanTake = InventoryLootViewModel.SelectedCell.containerID == ContainerID.Loot && !LootObject.ItemsContainer.IsEmptyCell(InventoryLootViewModel.SelectedCell.cellId);
            var isCanTakeAll = false;
            foreach (var cell in LootObject.ItemsContainer.Cells)
            {
                if (cell.IsHasItem)
                {
                    isCanTakeAll = true;
                    break;
                }
            }

            InventoryLootSectionView.SetEmptyDescriptionVisible(true);
            InventoryLootSectionView.SetIsVisibleTakeButton(isCanTake);
            InventoryLootSectionView.SetIsVisibleTakeAllButton(isCanTakeAll);

            InfoPanelModel.HideItemInfo();
        }

        private void OnPointerDownViewHandler(CellView cellView, PointerEventData eventData)
        {
            InventoryDragAndDropModel.OnPointerDown(cellView, eventData); 
        }

        private void OnDragBeginCellViewHandler(CellView cellView, PointerEventData eventData)
        {
            InventoryDragAndDropModel.OnDragBegin(cellView.gameObject, (ContainerID)cellView.ContainerId, cellView.Id, eventData, () => cellView.SetData(GetDataDefault(cellView.ContainerId, cellView.Id))); 
        }

        private void OnDragCellViewHandler(CellView cellView, PointerEventData eventData) 
        {
            InventoryDragAndDropModel.OnDrag(cellView.gameObject, (ContainerID)cellView.ContainerId, cellView.Id, eventData);
        }

        private void OnDragEndCellViewHandler(CellView cellView, PointerEventData eventData)
        {
            InventoryDragAndDropModel.OnDragEnd(cellView.gameObject, (ContainerID)cellView.ContainerId, cellView.Id, eventData, () => cellView.SetData(CollectCellDataModel.CollectCellData(GetContainer(cellView.ContainerId).GetCell(cellView.Id), cellView.ContainerId))); 
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

        private void UpdateCells() //REFACTOR
        {
            foreach (var cell in InventoryModel.ItemsContainer.Cells)
            {
                UpdateCellView(ContainerID.Inventory, cell.Id);
            }
            foreach (var cell in HotBarModel.ItemsContainer.Cells)
            {
                UpdateCellView(ContainerID.HotBar, cell.Id);
            }
            foreach (var cell in LootObject.ItemsContainer.Cells)
            {
                UpdateCellView(ContainerID.Loot, cell.Id);
            }
        }

        private CellData[] CollectCellsData(ItemsContainer container, int inventoryID) => container.Cells.Select(cell => CollectCellDataModel.CollectCellData(cell, inventoryID)).ToArray();

        private void OnChangeCellInventoryHandler(CellModel cell) => OnChangeCellHandler(cell, ContainerID.Inventory);

        private void OnChangeCellHotBarHandler(CellModel cell) => OnChangeCellHandler(cell, ContainerID.HotBar);

        private void OnChangeCellLootHandler(CellModel cell)
        {
            OnChangeCellHandler(cell, ContainerID.Loot);

            var cellCountActive = LootObject.CountOpenStart + ExpandModel.ExpandLevel;
            var isCanTakeAll = LootObject.ItemsContainer.Cells.Take(cellCountActive).Any(x => x.IsHasItem);

            InventoryLootSectionView.SetIsVisibleTakeAllButton(isCanTakeAll);
        }

        private void OnChangeCellHandler(CellModel cell, ContainerID containerID)
        {
            if (TryGetContainerViews(containerID, out var views))
            {
                if (cell.Id < views.Length)
                {
                    views[cell.Id].SetData(CollectCellDataModel.CollectCellData(cell, (int)containerID));
                }

                if (containerID != ContainerID.None)
                {
                    if (InventoryLootViewModel.SelectedCell.cellId == cell.Id)
                    {
                        OnChangeHighlightedHandler(InventoryLootViewModel.HighlightedCell);
                        OnChangeSelectedHandler(InventoryLootViewModel.SelectedCell);
                    }
                }
            }
        }

        private bool TryGetCellView(ContainerID containerID, int cellId, out CellView cellView)
        {
            if (TryGetContainerViews(containerID, out var cellViews))
            {
                cellView = cellViews[cellId];
                if(cellView != null)
                    return true;
            }

            cellView = default;
            return false;
        }

        private bool TryGetContainerViews(ContainerID containerID, out CellView[] cellsViews)
        {
            switch (containerID)
            {
                case ContainerID.Inventory:
                    cellsViews = InventoryMainView.InventoryCells;
                    break;
                case ContainerID.HotBar:
                    cellsViews = InventoryHotBarView.HotBarCells;
                    break;
                case ContainerID.Loot:
                    cellsViews = GetLootCellViews();
                    break;
                default:
                    cellsViews = null;
                    break;
            }

            return cellsViews != null;
        }

        private CellView[] GetLootCellViews()
        {
            var result = new CellView[InventoryLootSectionView.LootCells.Length];
            Array.Copy(InventoryLootSectionView.LootCells, 1, result, 0, InventoryLootSectionView.LootCells.Length - 1);

            if (ExpandModel.IsMaxLevel)
            {
                result[LootObject.CountOpenStart + ExpandModel.ExpandLevel - 1] = InventoryLootSectionView.LootCells[0];
            }

            return result;
        }

        private CellData GetDataDefault(int containerId, int cellId) => new CellData()
        {
            ContainerId = containerId,
            Id = cellId,
        };

        private void SetLocalization()
        {
            InventoryMainView.SetTextInventoryTitle(LocalizationModel.GetString(LocalizationKeyID.InventoryMenu_Name));
            InventoryLootSectionView.SetTextLootBoxTitle(LocalizationModel.GetString(LootObject.NameKeyID));
            InventoryMainView.ApplyButtonView.SetText(LocalizationModel.GetString(LocalizationKeyID.InventoryMenu_ApplyBtn));
            InventoryLootSectionView.TakeButtonView.SetText(LocalizationModel.GetString(LocalizationKeyID.LootBoxMenu_TakeBtn));
            InventoryLootSectionView.TakeAllButtonView.SetText(LocalizationModel.GetString(LocalizationKeyID.LootBoxMenu_TakeAllBtn));
            InventoryMainView.RepairButton.SetText(LocalizationModel.GetString(LocalizationKeyID.InventoryMenu_RepairBtn));

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

        private void UpdateTutorialHilight()
        {
            if (InventoryLootViewModel.IsTakeAllButtonHilight)
            {
                EnableTutorialHiligth();
            }
            else
            {
                DisableTutorialHilight();
            }
        }

        private void EnableTutorialHiligth()
        {
            View.InventoryLootSection.TakeAllButtonView.SafeActivateComponent<TutorialHilightAndAnimation>();
        }

        private void DisableTutorialHilight()
        {
            View.InventoryLootSection.TakeAllButtonView.SafeDeactivateComponent<TutorialHilightAndAnimation>();
        }
    }
}
