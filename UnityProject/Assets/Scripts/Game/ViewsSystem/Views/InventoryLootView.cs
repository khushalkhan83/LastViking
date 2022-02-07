using Core.Views;
using Game.Models;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class InventoryLootView : ViewBase, IDragAndDropInventoryView
    {
        #region Data
#pragma warning disable 0649
        [SerializeField] private InventoryMainView _inventoryMain;
        [SerializeField] private InventoryLootSectionView _inventoryLootSection;
        [SerializeField] private InventoryHotBarView _inventoryHotBar;

        [VisibleObject]
        [SerializeField] private GameObject _dragAndDrop;
        [SerializeField] private Image _dragAndDropIcon;
#pragma warning restore 0649
        #endregion

        public InventoryMainView InventoryMain => _inventoryMain;
        public InventoryLootSectionView InventoryLootSection => _inventoryLootSection;
        public InventoryHotBarView InventoryHotBar => _inventoryHotBar;

        public void SetIsVisibleDragAndDrop(bool value) => _dragAndDrop.SetActive(value);
        public void SetDragAndDropIcon(Sprite value) => _dragAndDropIcon.sprite = value;
        public void SetDragAndDropPosition(Vector2 value) => _dragAndDrop.transform.position = new Vector3(value.x, value.y, _dragAndDrop.transform.position.z);
        

        public event Action OnClose;
        public void ActionClose() => OnClose?.Invoke();

    }
}
