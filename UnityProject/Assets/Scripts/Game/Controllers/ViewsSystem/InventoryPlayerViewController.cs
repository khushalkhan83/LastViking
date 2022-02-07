using Core;
using Core.Controllers;
using Extensions;
using Game.Audio;
using Game.Models;
using Game.Views;
using System.Linq;
using UltimateSurvival;
using UnityEngine;
using UnityEngine.EventSystems;
using CellInfo = Game.Models.InventoryPlayerViewModel.CellInfo;

namespace Game.Controllers
{
    public class InventoryPlayerViewController : WindowViewControllerBase<InventoryPlayerView>
    {
        [Inject] public InventoryPlayerViewModel InventoryPlayerViewModel { get; private set; }
        [Inject] public PlayerDrinkProcessModel PlayerDrinkProcessModel { get; private set; }
        [Inject] public PlayerHealProcessModel PlayerHealProcessModel { get; private set; }
        [Inject] public PlayerEatProcessModel PlayerEatProcessModel { get; private set; }
        [Inject] public RepairingItemsModel RepairingItemsModel { get; private set; }
        [Inject] public PlayerConsumeModel PlayerConsumeModel { get; private set; }
        [Inject] public InventoryViewModel InventoryViewModel { get; private set; }
        [Inject] public PlayerProfileModel PlayerProfileModel { get; private set; }
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public LocalizationModel LocalizationModel { get; private set; }
        [Inject] public PlayerWaterModel PlayerWaterModel { get; private set; }
        [Inject] public RepairViewModel RepairViewModel { get; private set; }
        [Inject] public PlayerFoodModel PlayerFoodModel { get; private set; }
        [Inject] public InventoryModel InventoryModel { get; private set; }
        [Inject] public NetworkModel NetworkModel { get; private set; }
        [Inject] public HotBarModel HotBarModel { get; private set; }
        [Inject] public CoinsModel CoinsModel { get; private set; }
        [Inject] public InventoryRepairingModel InventoryRepairingModel { get; private set; }
        [Inject] public ExpandInventoryModel ExpandInventoryModel { get; private set; }
        [Inject] public CollectCellDataModel CollectCellDataModel { get; private set; }
        [Inject] public FullInventoryModel FullInventoryModel { get; private set; }
        [Inject] public StatsEffectModel StatsEffectModel { get; private set; }
        [Inject] public DivideItemsModel DivideItemsModel { get; private set; }
        [Inject] public BluePrintsModel BluePrintsModel { get; private set; }
        [Inject] public ContainersModel ContainersModel { get; private set; }
        [Inject] public ApplyItemModel ApplyItemModel { get; private set; }
        [Inject] public InfoPanelModel InfoPanelModel { get; private set; }
        [Inject] public DropItemModel DropItemModel { get; private set; }
        [Inject] public InventoryDragAndDropModel InventoryDragAndDropModel { get; private set; }
        [Inject] public EventSystemModel EventSystemModel { get; private set; }
        [Inject] public InventoryEquipmentModel InventoryEquipmentModel { get; private set; }
        [Inject] public TutorialModel TutorialModel { get; private set; }
        [Inject] public DeleteItemModel DeleteItemModel { get; private set; }

        private InventoryMainView InventoryMainView => View.InventoryMain;
        private InventoryPlayerDataView InventoryPlayerDataView => View.InventoryPlayerData;
        private InventoryHotBarView InventoryHotBarView => View.InventoryHotBar;
        private InventoryEquipmentView InventoryEquipmentView => View.InventoryEquipmentView;

        private CellView celectedCell;
        private CellView celectedToMoveCell;
        private bool moveState = false;
        protected override void Show()
        {
            base.Show();

            celectedCell = null;
            celectedToMoveCell = null;
            moveState = false;

            AudioSystem.PlayOnce(AudioID.WindowOpen);

            InventoryDragAndDropModel.OnShowInventory(View, InventoryPlayerViewModel);
            InputModel.OnInput.AddListener(PlayerActions.UIMenu_Y,OnMoveButtonHandler);

            EventSystemModel.SetSelectedGameObject(View.InventoryMain.InventoryCells[0].gameObject);

            InventoryDragAndDropModel.InventoryCellsViewModel = InventoryPlayerViewModel;
            InventoryDragAndDropModel.View = this.View;

            LocalizationModel.OnChangeLanguage += SetLocalization;

            InventoryPlayerDataView.SetImagePlayer(PlayerProfileModel.PlayerAvatar);
            InventoryPlayerDataView.SetNamePlayer(PlayerProfileModel.PlayerName);

            InventoryPlayerViewModel.OnChangeSelected += OnChangeSelectedHandler;
            InventoryPlayerViewModel.OnChangeHighlighted += OnChangeHighlightedHandler;

            InventoryPlayerViewModel.OnGetCell += OnGetCellHandler;
            InventoryPlayerViewModel.OnGetApplyButton += OnGetApplyButtonHandler;
            
            InventoryPlayerViewModel.RemoveSelectCell();
            InventoryPlayerViewModel.RemoveHighlightCell();

            InventoryViewModel.OnExpand += OnExpandChangeHandler;

            InventoryModel.ItemsContainer.OnChangeCell += OnChangeCellInventoryHandler;
            HotBarModel.ItemsContainer.OnChangeCell += OnChangeCellHotBarHandler;
            InventoryEquipmentModel.ItemsContainer.OnChangeCell += OnChangeCellEquipmentHandler;

            RepairingItemsModel.OnUpdateRepairing += OnUpdateRepairingHandler;
            RepairingItemsModel.OnChangeCount += OnChangeCountRepairingHandler;

            CoinsModel.OnChange += OnChangeCoins;
            BluePrintsModel.OnChange += OnChangeBluePrints;

            PlayerConsumeModel.OnStartConsume.AddListener(InventoryModel.ItemsContainer, OnStartConsumePlayerHandler);
            PlayerConsumeModel.OnUpdateConsume.AddListener(InventoryModel.ItemsContainer, OnUpdateConsumePlayerHandler);
            PlayerConsumeModel.OnEndConsume.AddListener(InventoryModel.ItemsContainer, OnEndConsumePlayerHandler);

            PlayerConsumeModel.OnStartConsume.AddListener(HotBarModel.ItemsContainer, OnStartConsumePlayerHandler);
            PlayerConsumeModel.OnUpdateConsume.AddListener(HotBarModel.ItemsContainer, OnUpdateConsumePlayerHandler);
            PlayerConsumeModel.OnEndConsume.AddListener(HotBarModel.ItemsContainer, OnEndConsumePlayerHandler);

            PlayerConsumeModel.OnStartConsume.AddListener(InventoryEquipmentModel.ItemsContainer, OnStartConsumePlayerHandler);
            PlayerConsumeModel.OnUpdateConsume.AddListener(InventoryEquipmentModel.ItemsContainer, OnUpdateConsumePlayerHandler);
            PlayerConsumeModel.OnEndConsume.AddListener(InventoryEquipmentModel.ItemsContainer, OnEndConsumePlayerHandler);

            PlayerHealProcessModel.OnAddHeal += OnAddHeal;
            PlayerEatProcessModel.OnAddEat += OnAddEat;
            PlayerDrinkProcessModel.OnAddDrink += OnAddDrink;
            PlayerHealProcessModel.OnEndHeal += OnEndHeal;
            PlayerEatProcessModel.OnEndEat += OnEndEat;
            PlayerDrinkProcessModel.OnEndDrink += OnEndDrink;

            PlayerHealthModel.OnChangeHealth += UpdatePlayerCharacteristics;
            PlayerHealthModel.OnChangeEquipmentBonus += UpdatePlayerCharacteristics;
            PlayerFoodModel.OnChangeFood += UpdatePlayerCharacteristics;
            PlayerFoodModel.OnChangeEquipmentBonus += UpdatePlayerCharacteristics;
            PlayerWaterModel.OnChangeWater += UpdatePlayerCharacteristics;
            PlayerWaterModel.OnChangeEquipmentBonus += UpdatePlayerCharacteristics;
            TutorialModel.OnStepChanged += UpdateScrollEnable;
            TutorialModel.OnComplete += UpdateScrollEnable;

            NetworkModel.OnInternetConnectionStateChange += OnInternetConnectionStateChangeHandler;
            NetworkModel.UpdateInternetConnectionStatus();

            UpdatePlayerCharacteristics();

            View.OnClose += OnCloseViewHandler;
            // InputModel.OnInput.AddListener(PlayerActions.UIMenu_B,OnCloseViewHandler);
            InventoryMainView.OnAddCoins += OnAddCoinsHandler;
            InventoryMainView.OnAutoStackItems += OnAutoStackItems;
            InventoryMainView.OnScrollChanged += OnInventoryScrollChanged;

            EventSystemModel.OnSelectionChanged += OnSelectionChanged;

            InitInventoryViews();
            InitHotBarViews();
            InitEquipmentViews();

            if (!InventoryViewModel.IsMaxExpandLevel)
            {
                ExpandInventoryModel.SetButtonContainer(InventoryMainView.ContainersUnlockSlots[InventoryViewModel.TotalExpandedCountLevel]);
            }
            ExpandInventoryModel.ShowExpandInventoryButton();

            StatsEffectModel.SetViewContainer(View.transform);

            InfoPanelModel.SetViewContainer(InventoryPlayerDataView.Container);

            UpdateCoins();
            UpdateBluePrints();
            UpdateScrollEnable();

            SetLocalization();
        }

        protected override void Hide()
        {
            base.Hide();

            InputModel.OnInput.RemoveListener(PlayerActions.UIMenu_Y,OnMoveButtonHandler);

            LocalizationModel.OnChangeLanguage -= SetLocalization;

            InventoryPlayerViewModel.OnChangeSelected -= OnChangeSelectedHandler;
            InventoryPlayerViewModel.OnChangeHighlighted -= OnChangeHighlightedHandler;

            InventoryPlayerViewModel.OnGetCell -= OnGetCellHandler;
            InventoryPlayerViewModel.OnGetApplyButton -= OnGetApplyButtonHandler;

            InventoryViewModel.OnExpand -= OnExpandChangeHandler;

            PlayerConsumeModel.OnStartConsume.RemoveListener(InventoryModel.ItemsContainer, OnStartConsumePlayerHandler);
            PlayerConsumeModel.OnUpdateConsume.RemoveListener(InventoryModel.ItemsContainer, OnUpdateConsumePlayerHandler);
            PlayerConsumeModel.OnEndConsume.RemoveListener(InventoryModel.ItemsContainer, OnEndConsumePlayerHandler);

            PlayerConsumeModel.OnStartConsume.RemoveListener(HotBarModel.ItemsContainer, OnStartConsumePlayerHandler);
            PlayerConsumeModel.OnUpdateConsume.RemoveListener(HotBarModel.ItemsContainer, OnUpdateConsumePlayerHandler);
            PlayerConsumeModel.OnEndConsume.RemoveListener(HotBarModel.ItemsContainer, OnEndConsumePlayerHandler);

            PlayerConsumeModel.OnStartConsume.RemoveListener(InventoryEquipmentModel.ItemsContainer, OnStartConsumePlayerHandler);
            PlayerConsumeModel.OnUpdateConsume.RemoveListener(InventoryEquipmentModel.ItemsContainer, OnUpdateConsumePlayerHandler);
            PlayerConsumeModel.OnEndConsume.RemoveListener(InventoryEquipmentModel.ItemsContainer, OnEndConsumePlayerHandler);

            PlayerHealProcessModel.OnAddHeal -= OnAddHeal;
            PlayerEatProcessModel.OnAddEat -= OnAddEat;
            PlayerDrinkProcessModel.OnAddDrink -= OnAddDrink;
            PlayerHealProcessModel.OnEndHeal -= OnEndHeal;
            PlayerEatProcessModel.OnEndEat -= OnEndEat;
            PlayerDrinkProcessModel.OnEndDrink -= OnEndDrink;

            PlayerHealthModel.OnChangeHealth -= UpdatePlayerCharacteristics;
            PlayerHealthModel.OnChangeEquipmentBonus -= UpdatePlayerCharacteristics;
            PlayerFoodModel.OnChangeFood -= UpdatePlayerCharacteristics;
            PlayerFoodModel.OnChangeEquipmentBonus -= UpdatePlayerCharacteristics;
            PlayerWaterModel.OnChangeWater -= UpdatePlayerCharacteristics;
            PlayerWaterModel.OnChangeEquipmentBonus -= UpdatePlayerCharacteristics;

            NetworkModel.OnInternetConnectionStateChange -= OnInternetConnectionStateChangeHandler;

            InventoryModel.ItemsContainer.OnChangeCell -= OnChangeCellInventoryHandler;
            HotBarModel.ItemsContainer.OnChangeCell -= OnChangeCellHotBarHandler;
            InventoryEquipmentModel.ItemsContainer.OnChangeCell -= OnChangeCellEquipmentHandler;

            RepairingItemsModel.OnUpdateRepairing -= OnUpdateRepairingHandler;
            RepairingItemsModel.OnChangeCount -= OnChangeCountRepairingHandler;

            View.OnClose -= OnCloseViewHandler;
            // InputModel.OnInput.RemoveListener(PlayerActions.UIMenu_B,OnCloseViewHandler);
            InventoryMainView.OnApplyItem -= OnApplyItemHandler;
            InputModel.OnInput.RemoveListener(PlayerActions.UIMenu_A,OnApplyItemHandler);
            InventoryMainView.OnDivideItems -= OnDivideItemHandler;
            InventoryMainView.OnTrash -= OnTrashHandler;
            InputModel.OnInput.RemoveListener(PlayerActions.UIMenu_X,OnTrashHandler);
            InventoryMainView.OnRepairItem -= OnRepairItemHandler;
            InventoryMainView.OnAddCoins -= OnAddCoinsHandler;
            InventoryMainView.OnScrollChanged -= OnInventoryScrollChanged;
            DivideItemsModel.OnItemSplitted -= OnItemSplitted;
            DivideItemsModel.OnHideDividePopup -= OnHideDividePopup;

            EventSystemModel.OnSelectionChanged -= OnSelectionChanged;

            CoinsModel.OnChange -= OnChangeCoins;
            BluePrintsModel.OnChange -= OnChangeBluePrints;
            InventoryMainView.OnAutoStackItems -= OnAutoStackItems;
            TutorialModel.OnStepChanged -= UpdateScrollEnable;
            TutorialModel.OnComplete -= UpdateScrollEnable;

            CellViewsDesubscribe(InventoryMainView.InventoryCells);
            CellViewsDesubscribe(InventoryHotBarView.HotBarCells);
            CellViewsDesubscribe(InventoryEquipmentView.EquipmentCells);

            ExpandInventoryModel.HideExpandInventoryButton();
            StatsEffectModel.HideStatsEffect();
            InfoPanelModel.HideItemInfo();

            InventoryDragAndDropModel.OnHideView(View);
        }

        private void OnMoveButtonHandler()
        {
            if(celectedCell == null) return;

            AudioSystem.PlayOnce(AudioID.Button);

            if(moveState)
            {
                PointerEventData eventData = EventSystemModel.GetPointerData();
                var raycast = eventData.pointerCurrentRaycast;
                raycast.gameObject = celectedCell.gameObject;
                eventData.pointerCurrentRaycast = raycast;

                // eventData.pointerCurrentRaycast.gameObject = celectedCell;
                
                var containerID = celectedToMoveCell.ContainerId;
                var containerIDEnum = (ContainerID)containerID;
                var id = celectedToMoveCell.Id;
                // InventoryDragAndDropModel.OnDragEnd(cellView.gameObject, (ContainerID)cellView.ContainerId, cellView.Id, eventData, () => cellView.SetData(CollectCellDataModel.CollectCellData(GetContainer(cellView.ContainerId).GetCell(cellView.Id), cellView.ContainerId)));
                InventoryDragAndDropModel.OnDragEnd(celectedToMoveCell.gameObject,containerIDEnum, id, eventData,() => {
                    celectedToMoveCell.SetData(CollectCellDataModel.CollectCellData(GetContainer(containerID).GetCell(id), containerID));
                });
            }
            else
            {
                celectedToMoveCell = celectedCell;
                EventSystemModel.SetSelectedGameObject(InventoryHotBarView.HotBarCells[0].gameObject);
            }


            moveState = !moveState;
            // hilight destination
            // change view to show new input commands (a- apply, b - cancel)
            
        }

        private void OnSelectionChanged(GameObject newSelectedObject, GameObject oldSelectedObject)
        {
            CellView cell = newSelectedObject.CheckNull()?.GetComponent<CellView>();
            celectedCell = cell;
            if(cell == null) return;

            cell.OnPointerDown(EventSystemModel.GetPointerData());
        }

        private GameObject OnGetCellHandler(ContainerID containerID, int cellId)
        {
            if (TryGetContainerViews(containerID, out var cellViews))
            {
                var cellView = cellViews[cellId];
                return cellView.CheckNull()?.gameObject;
            }
            return null;
        }

        public GameObject OnGetApplyButtonHandler()
        {
            return InventoryMainView.CheckNull()?.ApplyButtonView.CheckNull()?.gameObject;
        }

        private void InitHotBarViews()
        {
            var cellsDataHotBar = CollectCellsData(HotBarModel.ItemsContainer, (int)ContainerID.HotBar);

            var cellId = 0;
            foreach (var view in InventoryHotBarView.HotBarCells)
            {
                view.SetData(cellsDataHotBar[cellId]);
                CellViewSubscribe(view, true);

                ++cellId;
            }
        }

        private void InitEquipmentViews()
        {
            var cellsDataEquipment = CollectCellsData(InventoryEquipmentModel.ItemsContainer, (int)ContainerID.Equipment);
            var cellId = 0;
            foreach (var view in InventoryEquipmentView.EquipmentCells)
            {
                view.SetData(cellsDataEquipment[cellId]);
                CellViewSubscribe(view, true);

                ++cellId;
            }
        }

        private void OnInternetConnectionStateChangeHandler() => ExpandInventoryModel.UpdateExpandInventoryButton();

        private void OnAddCoinsHandler() => ViewsSystem.Show<PurchasesView>(ViewConfigID.Purchases);

        private void OnChangeBluePrints() => UpdateBluePrints();

        private void OnChangeCoins() => UpdateCoins();

        private void UpdateCoins() => InventoryMainView.SetCoins(CoinsModel.Coins.ToString());

        private void UpdateBluePrints() => InventoryMainView.SetBluePrints(BluePrintsModel.BluePrints.ToString());

        private void OnStartConsumePlayerHandler()
        {
            SelectItem(PlayerConsumeModel.Item);
            var containerID = ContainersModel.GetContainerID(PlayerConsumeModel.Container);
            if (containerID != ContainerID.None)
            {
                UpdateConsumeCellView(containerID, PlayerConsumeModel.CellId);
                CreateAddStatsEffect(PlayerConsumeModel.Item, containerID, PlayerConsumeModel.CellId);
            }
        }

        private void OnUpdateConsumePlayerHandler()
        {
            var containerID = ContainersModel.GetContainerID(PlayerConsumeModel.Container);
            if (containerID != ContainerID.None)
            {
                UpdateConsumeCellView(containerID, PlayerConsumeModel.CellId);
            }

            UpdatePlayerCharacteristics();
        }

        private void OnEndConsumePlayerHandler()
        {
            var containerID = ContainersModel.GetContainerID(PlayerConsumeModel.Container);
            var cellId = PlayerConsumeModel.CellId;
            var containerSelectedID = InventoryPlayerViewModel.SelectedCell.ContainerID;
            var cellSelectedId = InventoryPlayerViewModel.SelectedCell.CellId;

            UpdateCellView(containerID, cellId);
            InventoryPlayerViewModel.SelectCell(containerSelectedID, cellSelectedId);

            UpdatePlayerCharacteristics();
        }

        private void UpdatePlayerCharacteristics()
        {
            InventoryPlayerDataView.SetHealth((int)PlayerHealthModel.HealthCurrent, (int)PlayerHealthModel.HealthCurrentMax);
            InventoryPlayerDataView.SetEat((int)PlayerFoodModel.FoodCurrent, (int)PlayerFoodModel.FoodCurrentMax);
            InventoryPlayerDataView.SetWater((int)PlayerWaterModel.WaterCurrent, (int)PlayerWaterModel.WaterCurrentMax);
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
            if (TryGetContainerViews(containerID, out var cellViews))
            {
                var cellView = cellViews[cellId];
                cellView.SetData(CollectCellDataModel.CollectConsumeCellData(PlayerConsumeModel.Item, (int)containerID, cellId));

                if (InventoryPlayerViewModel.HighlightedCell.containerID == containerID && InventoryPlayerViewModel.HighlightedCell.cellId == cellId)
                {
                    cellView.Selection();
                }
            }
        }

        private void UpdateCellView(ContainerID containerID, int cellId)
        {
            if (TryGetContainerViews(containerID, out var cellViews))
            {
                var cellView = cellViews[cellId];
                var cellModel = GetContainer(containerID).Cells.ElementAt(cellId);
                cellView.SetData(CollectCellDataModel.CollectCellData(GetContainer(containerID).GetCell(cellId), (int)containerID));

                if (InventoryPlayerViewModel.HighlightedCell.containerID == containerID && InventoryPlayerViewModel.HighlightedCell.cellId == cellId)
                {
                    SetSelection(cellModel);
                    cellView.Selection();
                }
            }
        }

        // TODO: refactoring 
        private void UpdateCells()
        {
            foreach (var cell in InventoryModel.ItemsContainer.Cells)
            {
                UpdateCellView(ContainerID.Inventory, cell.Id);
            }
            foreach (var cell in HotBarModel.ItemsContainer.Cells)
            {
                UpdateCellView(ContainerID.HotBar, cell.Id);
            }
            foreach (var cell in InventoryEquipmentModel.ItemsContainer.Cells)
            {
                UpdateCellView(ContainerID.Equipment, cell.Id);
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

        private void CellViewsDesubscribe(CellView[] views)
        {
            foreach (var view in views)
            {
                CellViewDesubscribe(view);
            }
        }

        private void OnExpandChangeHandler(int id)
        {
            CellViewsDesubscribe(InventoryMainView.InventoryCells);
            InitInventoryViews();
            if (!InventoryViewModel.IsMaxExpandLevel)
            {
                ExpandInventoryModel.SetButtonContainer(InventoryMainView.ContainersUnlockSlots[InventoryViewModel.TotalExpandedCountLevel]);
            }
            ExpandInventoryModel.UpdateExpandInventoryButton();
            SetItemSelectionsByType(InventoryPlayerViewModel.HighlightedCell, true);
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

        private void OnChangeHighlightedHandler(CellInfo cellInfo)
        {
            SetItemSelectionsByType(InventoryPlayerViewModel.HighlightedLast, false);
            SetItemSelectionsByType(cellInfo, true);
        }

        private void OnChangeSelectedHandler(CellInfo cellInfo)
        {
            if(cellInfo == null)
            {
                RemoveSelected();
                return;
            }

            var container = GetContainer(cellInfo.containerID);

            if (container != null)
            {
                SetSelection(cellInfo, container);
            }
            else
            {
                RemoveSelected();
            }
        }

        private void SetSelection(CellInfo cellInfo) => SetSelection(cellInfo, GetContainer(cellInfo.containerID));

        private void SetSelection(CellInfo cellInfo, ItemsContainer containers) => SetSelection(containers.GetCell(cellInfo.cellId));

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
            InventoryPlayerDataView.SetEmptyDescriptionVisible(false);
            InfoPanelModel.UpdateItemInfo(item);
        }

        private void SelectItem(SavableItem item)
        {
            InventoryMainView.OnApplyItem -= OnApplyItemHandler;
            InputModel.OnInput.RemoveListener(PlayerActions.UIMenu_A,OnApplyItemHandler);
            InventoryMainView.OnDivideItems -= OnDivideItemHandler;
            InventoryMainView.OnTrash -= OnTrashHandler;
            InputModel.OnInput.RemoveListener(PlayerActions.UIMenu_X,OnTrashHandler);
            InventoryMainView.OnRepairItem -= OnRepairItemHandler;

            SelectItemData(item.ItemData);

            bool consuming = !(PlayerConsumeModel.IsCanConsume && item.IsCanConsume());
            bool canApply = !consuming;

            InventoryMainView.SetIsActiveApplyButton(canApply);
            if (canApply)
            {
                InventoryMainView.OnApplyItem += OnApplyItemHandler;
                InputModel.OnInput.AddListener(PlayerActions.UIMenu_A,OnApplyItemHandler);
            }

            var isCanDivide = item.Count > 1;
            InventoryMainView.SetIsActiveDivideButton(isCanDivide);
            if (isCanDivide)
            {
                InventoryMainView.OnDivideItems += OnDivideItemHandler;
            }

            InventoryMainView.SetIsActiveTrashButton(true);
            InventoryMainView.OnTrash += OnTrashHandler;
            InputModel.OnInput.AddListener(PlayerActions.UIMenu_X,OnTrashHandler);

            if (item.TryGetProperty("Durability", out var durabilityProperty))
            {
                var cellInfo = InventoryPlayerViewModel.SelectedCell;
                var cellModel = GetContainer(cellInfo.containerID).GetCell(cellInfo.cellId);
                var data = CollectCellDataModel.CollectCellData(cellModel, (int)cellInfo.containerID);
                var isCanRepair = durabilityProperty.Float.Current <= 0 && !data.ProgressRepairing.HasValue;

                InventoryMainView.SetIsActiveRepairButton(isCanRepair);
                if (isCanRepair)
                {
                    InventoryMainView.OnRepairItem += OnRepairItemHandler;
                }
            }
            else
            {
                InventoryMainView.SetIsActiveRepairButton(false);
            }
        }

        private void RemoveSelected()
        {
            InventoryMainView.SetIsActiveApplyButton(false);
            InventoryMainView.SetIsActiveDivideButton(false);
            InventoryMainView.SetIsActiveTrashButton(false);
            InventoryMainView.SetIsActiveRepairButton(false);

            InventoryPlayerDataView.SetEmptyDescriptionVisible(true);
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

        private void SetItemSelectionsByType(CellInfo cellInfo, bool isSelect) => SetItemSelectionsByType(cellInfo.containerID, cellInfo.cellId, isSelect);

        private void SetItemSelectionsByType(ContainerID containerID, int cellId, bool isSelect)
        {
            if (TryGetContainerViews(containerID, out var cells))
            {
                if (isSelect)
                {
                    cells[cellId].Selection();
                }
                else
                {
                    cells[cellId].Deselection(true);
                }
            }
        }

        private void OnApplyItemHandler()
        {
            var containerID = InventoryPlayerViewModel.SelectedCell.containerID;
            var cellId = InventoryPlayerViewModel.SelectedCell.cellId;

            var container = GetContainer(containerID);
            var cell = container.GetCell(cellId);

            ApplyItemModel.ApplyItem(container, cell);
        }

        private void OnAddHeal() => InventoryPlayerDataView.SetHealthSize(41);
        private void OnAddEat() => InventoryPlayerDataView.SetEatSize(41);
        private void OnAddDrink() => InventoryPlayerDataView.SetWaterSize(41);
        private void OnEndHeal() => InventoryPlayerDataView.SetHealthSize(38);
        private void OnEndEat() => InventoryPlayerDataView.SetEatSize(38);
        private void OnEndDrink() => InventoryPlayerDataView.SetWaterSize(38);

        private ItemsContainer GetContainer(int type) => ContainersModel.GetContainer((ContainerID)type);

        private ItemsContainer GetContainer(ContainerID type) => ContainersModel.GetContainer(type);

        private void OnDivideItemHandler()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            var cellId = InventoryPlayerViewModel.SelectedCell.cellId;
            var containerID = InventoryPlayerViewModel.SelectedCell.containerID;
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
            AudioSystem.PlayOnce(AudioID.Button);
            var containerID = InventoryPlayerViewModel.SelectedCell.containerID;
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

                if (result == null)
                {
                    if (containerID == ContainerID.HotBar)
                    {
                        result = GetContainer(ContainerID.Inventory).GetEmptyCell();
                    }
                    else if (containerID == ContainerID.Inventory)
                    {
                        result = GetContainer(ContainerID.HotBar).GetEmptyCell();
                    }
                }
            }

            return result != null;
        }

        private void OnTrashHandler()
        {
            var containerID = InventoryPlayerViewModel.SelectedCell.containerID;
            var cellId = InventoryPlayerViewModel.SelectedCell.cellId;
            var container = GetContainer(containerID);

            if (containerID != ContainerID.None)
            {
                AudioSystem.PlayOnce(AudioID.DropItem);
                DeleteItemModel.ShowDeletePopup(container, cellId);
            }
        }

        private void OnRepairItemHandler()
        {
            var container = GetContainer(InventoryPlayerViewModel.SelectedCell.containerID);
            var item = container.GetCell(InventoryPlayerViewModel.SelectedCell.cellId).Item;

            RepairViewModel.SetItem(item);
            InventoryRepairingModel.ShowRepairPopup();
        }

        private void OnCloseViewHandler()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            ViewsSystem.Hide(View);
        }

        private CellData[] CollectCellsData(ItemsContainer container, int inventoryID) => container.Cells.Select(cell => CollectCellDataModel.CollectCellData(cell, inventoryID)).ToArray();

        private void OnChangeCellInventoryHandler(CellModel cell) => OnChangeCellHandler(cell, ContainerID.Inventory);

        private void OnChangeCellHotBarHandler(CellModel cell) => OnChangeCellHandler(cell, ContainerID.HotBar);
        private void OnChangeCellEquipmentHandler(CellModel cell) => OnChangeCellHandler(cell, ContainerID.Equipment);

        private void OnChangeCellHandler(CellModel cell, ContainerID containerID)
        {
            if (TryGetContainerViews(containerID, out var views))
            {
                if (cell.Id < views.Length)
                {
                    var cellData = CollectCellDataModel.CollectCellData(cell, (int)containerID);
                    views[cell.Id].SetData(cellData);
                }

                if (containerID != ContainerID.None)
                {
                    if (InventoryPlayerViewModel.SelectedCell.cellId == cell.Id)
                    {
                        OnChangeHighlightedHandler(InventoryPlayerViewModel.HighlightedCell);
                        OnChangeSelectedHandler(InventoryPlayerViewModel.SelectedCell);
                    }
                }
            }
        }

        private bool TryGetContainerViews(ContainerID containerID, out CellView[] cellsViews)
        {
            switch (containerID)
            {
                case ContainerID.Inventory:
                    cellsViews = InventoryMainView.InventoryCells;
                    return true;
                case ContainerID.HotBar:
                    cellsViews = InventoryHotBarView.HotBarCells;
                    return true;
                case ContainerID.Equipment:
                    cellsViews = InventoryEquipmentView.EquipmentCells;
                    return true;
            }

            cellsViews = null;
            return false;
        }

        private CellData GetDataDefault(int containerId, int cellId) => new CellData()
        {
            ContainerId = containerId,
            Id = cellId,
        };

        private void SetLocalization()
        {
            InventoryMainView.SetTextInventoryTitle(LocalizationModel.GetString(LocalizationKeyID.InventoryMenu_Name));
            InventoryMainView.ApplyButtonView.SetText(LocalizationModel.GetString(LocalizationKeyID.InventoryMenu_ApplyBtn));
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
    }
}
