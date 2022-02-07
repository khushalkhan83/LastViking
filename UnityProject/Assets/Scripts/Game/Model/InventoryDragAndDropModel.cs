using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Game.Views;
using UnityEngine.EventSystems;
using Core.Views;
using UltimateSurvival;

namespace Game.Models
{
    public class InventoryDragAndDropModel : MonoBehaviour
    {
        public event Action ShowInventory;
        public event Action<CellView, PointerEventData> PointerDown;
        public event Action<GameObject, ContainerID, int, PointerEventData, Action> DragBegin;
        public event Action<GameObject, ContainerID, int, PointerEventData> Drag;
        public event Action<GameObject, ContainerID, int, PointerEventData, Action> DragEnd;
        public event Action<CellView, PointerEventData> PointerUp;
        public event Action<CellView, PointerEventData> PointerExit;
        public event Action<CellView, PointerEventData> Click;
        public event Action<IView> HideView;


#pragma warning disable 0649
        [SerializeField] float _selectCellTime = 0.1f;
#pragma warning restore 0649


        public IDragAndDropInventoryView View {get;set;}
        public InventoryCellsViewModelBase InventoryCellsViewModel { get; set; }
        public float SelecCellTime 
        {
            get{ return _selectCellTime;}
            set{ _selectCellTime = value;}
        }

        public void OnPointerDown(CellView cellView, PointerEventData eventData)
        {
            PointerDown?.Invoke(cellView, eventData);
        }

        public void OnDragBegin(GameObject cellGO, ContainerID containerId, int cellId, PointerEventData eventData, Action setDefuatView)
        {
            DragBegin?.Invoke(cellGO, containerId, cellId, eventData, setDefuatView);
        }

        public void OnDrag(GameObject cellGO, ContainerID containerId, int cellId, PointerEventData eventData)
        {
            Drag?.Invoke(cellGO, containerId, cellId, eventData);
        }

        public void OnDragEnd(GameObject cellGO, ContainerID containerId, int cellId, PointerEventData eventData, Action updateFromView)
        {
            DragEnd?.Invoke(cellGO, containerId, cellId, eventData, updateFromView);
        }

        public void OnPointerUp(CellView cellView, PointerEventData eventData)
        {
            PointerUp?.Invoke(cellView, eventData);
        }

        public void OnPointerExit(CellView cellView, PointerEventData eventData)
        {
            PointerExit?.Invoke(cellView, eventData);
        }

        public void OnClick(CellView cellView, PointerEventData eventData)
        {
            Click?.Invoke(cellView, eventData);
        }

        public void OnHideView(IView view)
        {
            HideView?.Invoke(view);
        }

        public void OnShowInventory(IDragAndDropInventoryView view, InventoryCellsViewModelBase inventoryCellsViewModel)
        {
            View = view;
            InventoryCellsViewModel = inventoryCellsViewModel;
            ShowInventory?.Invoke();
        }

    }
}
