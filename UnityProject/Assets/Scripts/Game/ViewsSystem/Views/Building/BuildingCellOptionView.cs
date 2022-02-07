using Core.Views;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Game.Views
{
    public class BuildingCellOptionView : ViewBase
    {
        public event Action<BuildingCellOptionView> OnClick;

        [SerializeField] private Image _itemIcon;
        [SerializeField] GameObject _selectionBorderObject;

        public Sprite ItemSprite => _itemIcon.sprite;

        public int CellID { get; set; }
        public int CategoryCellID { get; set; }

        public void SetItemIcon(Sprite value) => _itemIcon.sprite = value;
        public void SetIsVisibleSelectionBorder(bool isVisible) => _selectionBorderObject.SetActive(isVisible);
        public void SetIsVisible(bool isVisible) => this.gameObject.SetActive(isVisible);
        public void OnButtonClick() => OnClick?.Invoke(this);

        public void SetData(BuildingCellData data) {
            CellID = data.CellID;
            CategoryCellID = data.CategoryCellID;
            SetItemIcon(data.Icon);
            SetIsVisible(data.IsActive);
            SetIsVisibleSelectionBorder(data.IsSelected);
        }
    }
}
