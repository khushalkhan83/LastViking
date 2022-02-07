using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class LoomView : ViewBase, IDragAndDropInventoryView
    {
        #region Data
#pragma warning disable 0649
        [SerializeField] private InventoryMainView _inventoryMain;
        [SerializeField] private InventoryHotBarView _inventoryHotBarView;
        [SerializeField] private InventoryLoomSectionView _inventoryLoomSectionView;

        [VisibleObject]
        [SerializeField] private GameObject _dragAndDrop;
        [SerializeField] private Image _dragAndDropIcon;
#pragma warning restore 0649
        #endregion

        public InventoryMainView InventoryMain => _inventoryMain;
        public InventoryHotBarView InventoryHotBar => _inventoryHotBarView;
        public InventoryLoomSectionView InventoryLoomSection => _inventoryLoomSectionView;

        public void SetIsVisibleDragAndDrop(bool isVisible) => _dragAndDrop.SetActive(isVisible);
        public void SetDragAndDropIcon(Sprite sprite) => _dragAndDropIcon.sprite = sprite;
        public void SetDragAndDropPosition(Vector2 position) => _dragAndDrop.transform.position = new Vector3(position.x, position.y, _dragAndDrop.transform.position.z);

        public event Action OnClose;
        public void ActionClose() => OnClose?.Invoke();

    }
}
