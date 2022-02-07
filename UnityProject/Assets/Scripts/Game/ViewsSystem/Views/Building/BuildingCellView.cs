using Core.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Views
{
    public class BuildingCellView : ViewBase, IPointerDownHandler, IPointerUpHandler
    {
        public event Action<BuildingCellView, PointerEventData> PointerDown;
        public event Action<BuildingCellView, PointerEventData> PointerUp;
        public event Action<BuildingCellView> Hold;

        #region Data
#pragma warning disable 0649
        [SerializeField] private Image _itemIcon;

        [VisibleObject]
        [SerializeField] private GameObject _selectionBorderObject;

        [SerializeField] private List<BuildingCellOptionView> _optionCells;
        [SerializeField] private Transform _cellsHolder;
        [SerializeField] private CanvasGroup _canvasGroup;
#pragma warning restore 0649
        #endregion

        public int CellID { get;  set; }

        public List<BuildingCellOptionView> OptionCells => _optionCells;

        public Transform CellsHolder => _cellsHolder;
        public Sprite ItemSprite => _itemIcon.sprite;

        public void SetItemIcon(Sprite value) => _itemIcon.sprite = value;
        public void SetIsVisibleSelectionBorder(bool isVisible) => _selectionBorderObject.SetActive(isVisible);
        public void SetIsVisible(bool isVisible) => this.gameObject.SetActive(isVisible);
        public void SetInteractable(bool isInteractable)
        {
            if(isInteractable)
            {
                _canvasGroup.alpha = 1f;
                _canvasGroup.interactable = true;
                _canvasGroup.blocksRaycasts = true;
            }
            else
            {
                _canvasGroup.alpha = 0.5f;
                _canvasGroup.interactable = false;
                _canvasGroup.blocksRaycasts = false;
            }
        }

        public void SetData(BuildingCellData data)
        {
            CellID = data.CellID;
            SetItemIcon(data.Icon);
            SetIsVisible(data.IsActive);
            SetIsVisibleSelectionBorder(data.IsSelected);
        }
       
        public void OnPointerDown(PointerEventData eventData)
        {
            PointerDown?.Invoke(this, eventData);
        }

        public void OnHold() {
            Hold?.Invoke(this);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            PointerUp.Invoke(this, eventData);
        }
    }
}
