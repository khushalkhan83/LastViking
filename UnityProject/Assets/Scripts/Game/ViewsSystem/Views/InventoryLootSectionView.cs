using Core.Views;
using Game.Models;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class InventoryLootSectionView : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649
        [VisibleObject]
        [SerializeField] private DisablableButtonView _takeAllButton;

        [VisibleObject]
        [SerializeField] private DisablableButtonView _takeButton;

        [SerializeField] private RectTransform _container;

        [SerializeField] private CellView[] _lootCells;

        [SerializeField] private Transform _containerExpandLootCell;

        [VisibleObject]
        [SerializeField] private GameObject _firstLootCellObject;

        [SerializeField] private Text _lootBoxTitle;

        [SerializeField] private GameObject _emptyDescription;

#pragma warning restore 0649
        #endregion
        public CellView[] LootCells => _lootCells;
        public Transform ContainerExpandLootCell => _containerExpandLootCell;

        public RectTransform Container => _container;
        public GameObject EmptyDescription => _emptyDescription;

        public DisablableButtonView TakeButtonView => _takeButton;
        public DisablableButtonView TakeAllButtonView => _takeAllButton;

        public void SetIsVisibleFirstLootCell(bool value) => _firstLootCellObject.SetActive(value);
        public void SetIsVisibleTakeButton(bool value) => _takeButton.SetIsVisible(value);
        public void SetIsVisibleTakeAllButton(bool value) => _takeAllButton.SetIsVisible(value);
        public void SetTextLootBoxTitle(string text) => _lootBoxTitle.text = text;
        public void SetEmptyDescriptionVisible(bool isVisible) => _emptyDescription.SetActive(isVisible);

        //UI
        public event Action OnTakeAll;
        public void ActionTakeAll() => OnTakeAll?.Invoke();

        //UI
        public event Action OnTake;
        public void ActionTake() => OnTake?.Invoke();
    }
}
