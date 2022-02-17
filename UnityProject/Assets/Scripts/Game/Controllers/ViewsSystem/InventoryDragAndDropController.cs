using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using UnityEngine;
using UnityEngine.EventSystems;
using UltimateSurvival;
using Core.Views;
using Extensions;

namespace Game.Controllers
{
    public class InventoryDragAndDropController : IInventoryDragAndDropController, IController
    {
        [Inject] public InventoryDragAndDropModel InventoryDraAndDropModel {get;set;}
        [Inject] public ContainersModel ContainersModel { get; set; }
        [Inject] public PlayerConsumeModel PlayerConsumeModel { get; set; }
        [Inject] public CollectCellDataModel CollectCellDataModel { get; set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; set; }
        [Inject] public InventoryEquipmentModel InventoryEquipmentModel { get; set; }

        LootObject lootObject = null;
        private int _pointerId = -100;
        private CellView _selectCellView;
        private bool _isDraged = false;
        private bool _isHolding = false;
        private float _holdTime = 0;
      
        private InventoryCellsViewModelBase InventoryCellsViewModel => InventoryDraAndDropModel.InventoryCellsViewModel;
        private IDragAndDropInventoryView View => InventoryDraAndDropModel.View;
        private PlayerEventHandler PlayerEventHandler => ModelsSystem.Instance._playerEventHandler; 

        public void Enable()
        {
            InventoryDraAndDropModel.ShowInventory += OnShowInventory;
            InventoryDraAndDropModel.PointerDown += OnPointerDownCellViewHandler;
            InventoryDraAndDropModel.DragBegin += OnDragBeginCellViewHandler;
            InventoryDraAndDropModel.Drag += OnDragCellViewHandler;
            InventoryDraAndDropModel.DragEnd += OnDragEndCellViewHandler;
            InventoryDraAndDropModel.PointerUp += OnPointerUpCellViewHandler;
            InventoryDraAndDropModel.PointerExit += OnPointerExitCellViewHandler;
            InventoryDraAndDropModel.Click += OnClickCellViewHandler;
            InventoryDraAndDropModel.HideView += OnHideView;
            GameUpdateModel.OnUpdate += OnUpdate;
            ResetHold();
        }

        public void Disable()
        {
            InventoryDraAndDropModel.ShowInventory += OnShowInventory;
            InventoryDraAndDropModel.PointerDown -= OnPointerDownCellViewHandler;
            InventoryDraAndDropModel.DragBegin -= OnDragBeginCellViewHandler;
            InventoryDraAndDropModel.Drag -= OnDragCellViewHandler;
            InventoryDraAndDropModel.DragEnd -= OnDragEndCellViewHandler;
            InventoryDraAndDropModel.PointerUp -= OnPointerUpCellViewHandler;
            InventoryDraAndDropModel.PointerExit -= OnPointerExitCellViewHandler;
            InventoryDraAndDropModel.Click -= OnClickCellViewHandler;
            InventoryDraAndDropModel.HideView -= OnHideView;
            GameUpdateModel.OnUpdate -= OnUpdate;
        }

        public void Start()
        {
            
        }

        private void OnShowInventory()
        {
            lootObject = PlayerEventHandler.RaycastData.Value?.GameObject.CheckNull()?.GetComponent<LootObject>();
        }

        private void OnPointerDownCellViewHandler(CellView cellView, PointerEventData eventData)
        {
            _holdTime = 0;
            _selectCellView = cellView;
            _isHolding = true;
            _pointerId = eventData.pointerId;
        }

        private void OnDragBeginCellViewHandler(GameObject cellGO, ContainerID containerId, int cellId, PointerEventData eventData, Action setDefuatView)
        {
            if(IsCellSelected(containerId, cellId))
            {
                _isDraged = true;
                OnDragBegin(containerId, cellId, eventData, setDefuatView);
            }
            else
            {
                ExecuteEvents.ExecuteHierarchy(cellGO.transform.parent.gameObject, eventData, ExecuteEvents.beginDragHandler);
                ResetHold();
            }
        }

        private void OnDragCellViewHandler(GameObject cellGO, ContainerID containerId, int cellId, PointerEventData eventData)
        {
            if(_isDraged)
            {
                OnDrag(containerId, cellId, eventData);
            }
            else
            {
                ExecuteEvents.ExecuteHierarchy(cellGO.transform.parent.gameObject, eventData, ExecuteEvents.dragHandler);
            }
        }

        private void OnDragEndCellViewHandler(GameObject cellGO, ContainerID containerId, int cellId, PointerEventData eventData, Action updateFromView)
        {
            //_isDraged = true;
            if(_isDraged)
            {
                OnDragEnd(containerId, cellId, eventData, updateFromView);
            }
            else
            {
                ExecuteEvents.ExecuteHierarchy(cellGO.transform.parent.gameObject, eventData, ExecuteEvents.endDragHandler);
            }
            _isDraged = false;
        }

        private void OnPointerUpCellViewHandler(CellView cellView, PointerEventData eventData)
        {
            ResetHold();
        }

        private void OnPointerExitCellViewHandler(CellView cellView, PointerEventData eventData)
        {
            if(_pointerId == eventData.pointerId)
            {
                ResetHold();
            }
        }

        private void OnClickCellViewHandler(CellView cellView, PointerEventData eventData)
        {
            SelectCell(cellView);
            ResetHold();
        }

        private void OnHideView(IView view)
        {
            View.SetIsVisibleDragAndDrop(false);
            _isDraged = false;
            InventoryCellsViewModel.DragEnd();
            ResetHold();
        }

        private void OnUpdate()
        {
             if (_isHolding) { 
                _holdTime += Time.deltaTime; 
                if (_holdTime > InventoryDraAndDropModel.SelecCellTime)  
                { 
                    if(_selectCellView != null)
                    {
                        SelectCell(_selectCellView);
                    }
                    else
                    {
                        Debug.LogError("Select cell is null!");
                    }

                    ResetHold(); 
                } 
            } 
        }

        private void ResetHold()
        {
            _selectCellView = null;
            _isHolding = false;
            _pointerId = -100;
        }

        private void SelectCell(CellView cellView)
        {
            var containerID = (ContainerID)cellView.ContainerId;
            InventoryCellsViewModel.SelectCell(containerID, cellView.Id);
            InventoryCellsViewModel.HighlightCell(containerID, cellView.Id);
        }

        private bool IsCellSelected(ContainerID containerID, int cellId)
        {
            return InventoryCellsViewModel.SelectedCell.ContainerID == containerID
            && InventoryCellsViewModel.SelectedCell.CellId == cellId;
        }

        private void OnDragBegin(ContainerID containerID, int cellId, PointerEventData eventData, Action SetDefaultView)
        {
            if (InventoryCellsViewModel.IsHasDrag)
            {
                return;
            }

            if (containerID == ContainerID.None)
            {
                return;
            }

            var container = ContainersModel.GetContainer(containerID);
            var cellModel = container.GetCell(cellId);

            var isCanDragBegin = !IsFilteredDragBeginItem(containerID, cellModel);

            if (isCanDragBegin)
            {
                SetDefaultView?.Invoke();
                View.SetIsVisibleDragAndDrop(true);
                View.SetDragAndDropIcon(cellModel.Item.ItemData.Icon);
                View.SetDragAndDropPosition(eventData.enterEventCamera.ScreenToWorldPoint(eventData.position));
            }

            InventoryCellsViewModel.HighlightCell(containerID, cellId);
            InventoryCellsViewModel.DragBegin(eventData.pointerId);
        }

        private void OnDrag(ContainerID containerFromID, int cellFromId, PointerEventData eventData)
        {
            if (!eventData.enterEventCamera)
            {
                return;
            }

            if (InventoryCellsViewModel.DragId != eventData.pointerId)
            {
                return;
            }

            View.SetDragAndDropPosition(eventData.enterEventCamera.ScreenToWorldPoint(eventData.position));

            var cellViewTo = eventData.pointerCurrentRaycast.gameObject?.GetComponent<CellView>();
            if (!cellViewTo)
            {
                InventoryCellsViewModel.RemoveHighlightCell();
                return;
            }

            var containerFrom = ContainersModel.GetContainer(containerFromID);
            var cellFrom = containerFrom?.GetCell(cellFromId);

            var cellToId = cellViewTo.Id;
            var containerToID = (ContainerID)cellViewTo.ContainerId;
            if (containerToID == ContainerID.None)
            {
                InventoryCellsViewModel.RemoveHighlightCell();
                return;
            }

            var containerTo = ContainersModel.GetContainer(containerToID);
            var cellTo = containerTo.GetCell(cellToId);

            var isCanDrop = !CantDropItem(containerFromID, containerToID, cellFrom, cellTo);

            if (isCanDrop || cellFrom == cellTo)
            {
                InventoryCellsViewModel.HighlightCell(containerToID, cellToId);
            }
            else
            {
                InventoryCellsViewModel.RemoveHighlightCell();
            }
        }

         private void OnDragEnd(ContainerID containerFromID, int cellFromId, PointerEventData eventData, Action updateFromView)
        {
            if (InventoryCellsViewModel.DragId != eventData.pointerId)
            {
                return;
            }

            var containerFrom = ContainersModel.GetContainer(containerFromID);
            var cellFrom = containerFrom.GetCell(cellFromId);

            updateFromView?.Invoke();

            View.SetIsVisibleDragAndDrop(false);

            InventoryCellsViewModel.DragEnd();

            var cellToObject = eventData.pointerCurrentRaycast.gameObject;
            if (!cellToObject)
            {
                InventoryCellsViewModel.RemoveSelectCell();
                InventoryCellsViewModel.RemoveHighlightCell();
                return;
            }

            var cellToId = GetViewCellId(cellToObject);
            var containerToID = (ContainerID)GetViewCellContainerId(cellToObject);

            if (containerToID == ContainerID.None)
            {
                InventoryCellsViewModel.RemoveSelectCell();
                InventoryCellsViewModel.RemoveHighlightCell();
                return;
            }

            var containerTo = ContainersModel.GetContainer(containerToID);
            var cellTo = containerTo.GetCell(cellToId);
            var isCanDropItem = !CantDropItem(containerFromID, containerToID, cellFrom, cellTo);

            if (isCanDropItem)
            {
                if (cellFrom.Item.IsCanStacked(cellTo.Item))
                {
                    var left = containerTo.AdjustCountInCell(cellTo.Id, cellTo.Item.Id, cellFrom.Item.Count);
                    containerFrom.RemoveItemsFromCell(cellFrom.Id, cellFrom.Item.Count - left);
                }
                else if (cellFrom.IsHasItem)
                {
                    var tmp = cellFrom.Item;

                    cellFrom.Item = cellTo.Item;
                    cellTo.Item = tmp;
                }
            }

            if (isCanDropItem)
            {
                InventoryCellsViewModel.SelectCell(containerToID, cellToId);
                InventoryCellsViewModel.HighlightCell(containerToID, cellToId);
            }
            else
            {
                InventoryCellsViewModel.SelectCell(containerFromID, cellFromId);
                InventoryCellsViewModel.HighlightCell(containerFromID, cellFromId);
            }
        }

        private bool IsFilteredDragBeginItem(ContainerID containerID, CellModel cellModel)
        {
            var consumeContainerID = ContainersModel.GetContainerID(PlayerConsumeModel.Container);
            var cellData = CollectCellDataModel.CollectCellData(cellModel, (int)containerID);

            return cellModel.IsEmpty
                ||
                (
                    cellModel.Id == PlayerConsumeModel.CellId
                    && containerID == consumeContainerID
                )
                ||
                cellData.ProgressRepairing.HasValue;
        }

        private bool CantDropItem(ContainerID containerFromId, ContainerID containerToId, CellModel cellFrom, CellModel cellTo)
        {
            var consumeContainerID = ContainersModel.GetContainerID(PlayerConsumeModel.Container);
            var cellFromData = CollectCellDataModel.CollectCellData(cellFrom, (int)containerFromId);
            var cellToData = CollectCellDataModel.CollectCellData(cellTo, (int)containerToId);

            return 
                CantDropContainerNoneCheck(containerFromId, containerToId)
                || CantDropLootContainerCheck(containerToId)
                || CantDropSameCellCheck(containerFromId, containerToId, cellFrom, cellTo)
                || CantDropRepairingCheck(cellFromData, cellToData)
                || CantDropConsumeCheck(consumeContainerID, containerFromId, containerToId, cellFrom, cellTo)
                || CantDropWithoutProperties(cellFrom, containerToId)
                || CantDropToEquipmentSlot(containerToId, cellFrom, cellToData)
                || CantDropFromEquipmentSlot(containerFromId, cellFrom, cellTo)
                || CantDropFromArrowsSlot(containerFromId, cellFrom, cellTo)
                || CantDropToOrFromCampfireSlot(containerFromId, containerToId, cellFrom, cellTo);
        }

        private bool CantDropContainerNoneCheck(ContainerID containerFromId, ContainerID containerToId) => containerFromId == ContainerID.None || containerToId == ContainerID.None;
        private bool CantDropLootContainerCheck(ContainerID containerToId) => containerToId == ContainerID.Loot && (lootObject == null || !lootObject.IsStorage);
        private bool CantDropSameCellCheck(ContainerID containerFromId, ContainerID containerToId,  CellModel cellFrom, CellModel cellTo)
        {
            return containerFromId != ContainerID.None
                    && containerToId != ContainerID.None
                    && containerFromId == containerToId
                    && cellFrom.Id == cellTo.Id;
        }

        private bool CantDropRepairingCheck(CellData cellFromData, CellData cellToData) => cellFromData.ProgressRepairing.HasValue || cellToData.ProgressRepairing.HasValue;
        private bool CantDropConsumeCheck(ContainerID  consumeContainerID, ContainerID containerFromId, ContainerID containerToId,  CellModel cellFrom, CellModel cellTo)
        {
            return consumeContainerID != ContainerID.None
                    &&
                    (
                        (
                            cellFrom.Id == PlayerConsumeModel.CellId
                            && containerFromId == consumeContainerID
                        )
                        ||
                        (
                            cellTo.Id == PlayerConsumeModel.CellId
                            && containerToId == consumeContainerID
                        )
                    );
        }

        private bool CantDropWithoutProperties(CellModel cellFromData, ContainerID containerToId)
        {
            if(InventoryCellsViewModel.DisableItemsWithoutProperties 
                && InventoryCellsViewModel.ActiveItemsProperties.Length > 0
                && containerToId != ContainerID.HotBar 
                && containerToId != ContainerID.Inventory
                && containerToId != ContainerID.Equipment)
            {
                foreach(string prop in InventoryCellsViewModel.ActiveItemsProperties)
                {
                    if(cellFromData.Item.HasProperty(prop))
                        return false;
                }
                return true;
            }
            return false;
        }

        private bool CantDropToEquipmentSlot(ContainerID containerToId, CellModel cellFromModel, CellData cellToData)
        {
            if(containerToId != ContainerID.Equipment)
                return false;

            if(cellFromModel.Item.TryGetProperty("EquipmentCategory", out var property))
            {
                EquipmentCategory itemCategory = property.EquipmentCategory;
                EquipmentCategory slotCategory = InventoryEquipmentModel.GetSlotCategory(cellToData.Id);
                if(itemCategory == slotCategory)
                {
                    return false;
                }
            }

            return true;
        }

        private bool CantDropFromEquipmentSlot(ContainerID containerFromId, CellModel cellFromModel, CellModel cellToModel)
        {
            if (containerFromId != ContainerID.Equipment)
                return false;

            if (cellToModel.IsHasItem)
            {
                if (cellToModel.Item.TryGetProperty("EquipmentCategory", out var property))
                {
                    EquipmentCategory itemToCategory = property.EquipmentCategory;
                    EquipmentCategory slotCategory = InventoryEquipmentModel.GetSlotCategory(cellFromModel.Id);
                    if (itemToCategory == slotCategory)
                        return false;
                }
                return true;
            }

            return false;
        }

        private bool CantDropFromArrowsSlot(ContainerID containerFromId, CellModel cellFromModel, CellModel cellToModel)
        {
            if(containerFromId != ContainerID.Equipment)
                return false;

            if(cellToModel.IsHasItem)
            {
                if(cellToModel.Item.TryGetProperty("Is Arrow", out _))
                {
                    return false;
                }
                
                return true;
            }

            return false;
        }

        private bool CantDropToOrFromCampfireSlot(ContainerID containerFromId, ContainerID containerToId, CellModel cellFrom, CellModel cellTo)
        {
            if (containerToId == ContainerID.CampFire)
            {
                bool isFuel = cellFrom.Item.TryGetProperty("IsFuel", out var isFuelProperty);
                bool isFoodItem = cellFrom.Item.ItemData.Category == "Food";
                bool canDrag = isFuel || isFoodItem;
                return !canDrag;
            }
            else if (containerFromId == ContainerID.CampFire)
            {
                if (cellTo.IsHasItem)
                {
                    bool isFoodInTargetCell = cellTo.Item.ItemData.Category == "Food";
                    bool canDrag = isFoodInTargetCell;
                    bool cantDragCell = !canDrag;
                    return cantDragCell;
                }
            }

            return false;
        }
        private int GetViewCellId(GameObject cell) => cell.GetComponent<CellView>()?.Id
                ?? cell.GetComponent<CookCellView>()?.Id
                ?? cell.GetComponent<WeaveCellView>()?.Id
                ?? (int)ContainerID.None;

        private int GetViewCellContainerId(GameObject cell) => cell.GetComponent<CellView>()?.ContainerId
                ?? cell.GetComponent<CookCellView>()?.ContainerId
                ?? cell.GetComponent<WeaveCellView>()?.ContainerId
                ?? (int)ContainerID.None;
    }
}
