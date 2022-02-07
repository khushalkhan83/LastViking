using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class InventoryMainView : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private DisablableButtonView _applyButton;
        [SerializeField] private DisablableButtonView _repairButton;
        [SerializeField] private DisablableButtonView _divideButton;
        [SerializeField] private DisablableButtonView _trashButton;

        [SerializeField] private CellView[] _inventoryCells;

        [SerializeField] private Text _coinsValue;
        [SerializeField] private Text _blueprintsValue;

        //Localization text targets
        [SerializeField] private Text _inventoryTitle;
        [SerializeField] private Transform[] _containersUnlockSlot;

        [SerializeField] private GameObject _topFade;
        [SerializeField] private GameObject _bottomFade;

        [SerializeField] private Scrollbar _scrollbar;
        [SerializeField] private ScrollRect _scrollRect;
 
#pragma warning restore 0649
        #endregion


        public CellView[] InventoryCells => _inventoryCells;
        public DisablableButtonView ApplyButtonView => _applyButton;
        public DisablableButtonView RepairButton => _repairButton;
        public DisablableButtonView DivideButtonView => _divideButton;
        public Transform[] ContainersUnlockSlots => _containersUnlockSlot;
        public float ScrollValue => _scrollbar.value;
        public Scrollbar Scrollbar => _scrollbar;
        public ScrollRect ScrollRect => _scrollRect;

        public void SetCoins(string value) => _coinsValue.text = value;
        public void SetBluePrints(string value) => _blueprintsValue.text = value;
        public void SetTextInventoryTitle(string text) => _inventoryTitle.text = text;

        public void SetIsActiveApplyButton(bool value)
        {
            _applyButton.SetIsVisibleActiveObject(value);
            _applyButton.SetIsVisiblePassiveObject(!value);
        }

        public void SetIsActiveRepairButton(bool value)
        {
            _repairButton.SetIsVisibleActiveObject(value);
            _repairButton.SetIsVisiblePassiveObject(!value);
        }

        public void SetIsActiveDivideButton(bool value)
        {
            _divideButton.SetIsVisibleActiveObject(value);
            _divideButton.SetIsVisiblePassiveObject(!value);
        }

        public void SetIsActiveTrashButton(bool value)
        {
            _trashButton.SetIsVisibleActiveObject(value);
            _trashButton.SetIsVisiblePassiveObject(!value);
        }

        public void SetIsActiveTopFade(bool value) => _topFade.SetActive(value);

        public void SetIsActiveBottomFade(bool value) => _bottomFade.SetActive(value);
        //UI
        public event Action OnApplyItem;
        public void ActionApplyItem() => OnApplyItem?.Invoke();

        //UI
        public event Action OnDivideItems;
        public void ActionDivideItems() => OnDivideItems?.Invoke();

        //UI
        public event Action OnAutoStackItems;
        public void ActionAutoStackItems() => OnAutoStackItems?.Invoke();

        //UI
        public event Action OnTrash;
        public void ActionTrash() => OnTrash?.Invoke();

        //UI
        public event Action OnAddCoins;
        public void ActionAddCoins() => OnAddCoins?.Invoke();

        //UI
        public event Action OnEditName;
        public void ActionEditName() => OnEditName?.Invoke();

        //UI
        public event Action OnAddCells;
        public void ActionAddCells() => OnAddCells?.Invoke();

        //UI
        public event Action OnRepairItem;
        public void ActionRepairItem() => OnRepairItem?.Invoke();
        //

        //UI
        public event Action<float> OnScrollChanged;
        public void ActionScrollChanged() => OnScrollChanged?.Invoke(_scrollbar.value);
        //
    }
}
