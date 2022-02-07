using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class FurnaceView : ViewBase, IDragAndDropInventoryView
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private InventoryMainView _inventoryMain;
        [SerializeField] private InventoryFurnaceSectionView _inventoryFurnaceSection;
        [SerializeField] private InventoryHotBarView _inventoryHotBar;

        [VisibleObject]
        [SerializeField] private GameObject _dragAndDrop;
        [SerializeField] private Image _dragAndDropIcon;

#pragma warning restore 0649
        #endregion

        public InventoryMainView InventoryMain => _inventoryMain;
        public InventoryFurnaceSectionView InventoryFurnaceSection => _inventoryFurnaceSection;
        public InventoryHotBarView InventoryHotBar => _inventoryHotBar;

        public void SetIsVisibleDragAndDrop(bool isVisible) => _dragAndDrop.SetActive(isVisible);
        public void SetDragAndDropIcon(Sprite sprite) => _dragAndDropIcon.sprite = sprite;
        public void SetDragAndDropPosition(Vector2 position) => _dragAndDrop.transform.position = new Vector3(position.x, position.y, _dragAndDrop.transform.position.z);

        //UI
        public event Action OnClose;
        public void ActionClose() => OnClose?.Invoke();

    }
}
