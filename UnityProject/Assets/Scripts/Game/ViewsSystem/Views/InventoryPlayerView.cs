using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class InventoryPlayerView : ViewBase, IDragAndDropInventoryView
    {
        #region Data
#pragma warning disable 0649
        [SerializeField] private InventoryMainView _inventoryMain;
        [SerializeField] private InventoryPlayerDataView _inventoryPlayerData;
        [SerializeField] private InventoryHotBarView _inventoryHotBar;
        [SerializeField] private InventoryEquipmentView _inventoryEquipmentView;

        [VisibleObject]
        [SerializeField] private GameObject _dragAndDrop;
        [SerializeField] private Image _dragAndDropIcon;

#pragma warning restore 0649
        #endregion

        public InventoryMainView InventoryMain => _inventoryMain;
        public InventoryPlayerDataView InventoryPlayerData => _inventoryPlayerData;
        public InventoryHotBarView InventoryHotBar => _inventoryHotBar;
        public InventoryEquipmentView InventoryEquipmentView => _inventoryEquipmentView;

        public void SetIsVisibleDragAndDrop(bool value) => _dragAndDrop.SetActive(value);       
        public void SetDragAndDropIcon(Sprite value) => _dragAndDropIcon.sprite = value;
        public void SetDragAndDropPosition(Vector2 value) => _dragAndDrop.transform.position = new Vector3(value.x, value.y, _dragAndDrop.transform.position.z);
       

        public event Action OnClose;
        public void ActionClose() => OnClose?.Invoke();

    }
}
