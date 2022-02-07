using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class InventoryLoomSectionView : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [VisibleObject]
        [SerializeField] private GameObject _turnOffButton;

        [VisibleObject]
        [SerializeField] private GameObject _setWeaveButton;

        [VisibleObject]
        [SerializeField] private GameObject _boostButtonObject;
        [SerializeField] private Transform _containerBoostButton;

        [VisibleObject]
        [SerializeField] private GameObject _itemNameObject;

        [VisibleObject]
        [SerializeField] private GameObject _itemDescriptionObject;

        [SerializeField] private RectTransform _container;

        [VisibleObject]
        [SerializeField] private GameObject _weavingProcessObject;

        [SerializeField] private WeaveCellView[] _weaveCells;

        [SerializeField] private Transform _containerExpandLootCell;

        [VisibleObject]
        [SerializeField] private GameObject _firstLootCellObject;

        [SerializeField] private Text _loomTitle;
        [SerializeField] private Text _setWeaveButtonTitle;
        [SerializeField] private Text _turnOffButtonTitle;

#pragma warning restore 0649
        #endregion
        public WeaveCellView[] WeaveCells => _weaveCells;
        public Transform ContainerExpandLootCell => _containerExpandLootCell;
        public Transform ContainerBoostButton => _containerBoostButton;
        public RectTransform Container => _container;

        public void SetIsVisibleFirstLootCell(bool isVisible) => _firstLootCellObject.SetActive(isVisible);
        public void SetIsVisibleSetWeaveButton(bool isVisible) => _setWeaveButton.SetActive(isVisible);
        public void SetIsVisibleTurnOffButton(bool isVisible) => _turnOffButton.SetActive(isVisible);
        public void SetIsVisibleBoostButton(bool isVisible) => _boostButtonObject.SetActive(isVisible);
        public void SetIsVisibleItemName(bool isVisible) => _itemNameObject.SetActive(isVisible);
        public void SetIsVisibleItemDescription(bool isVisible) => _itemDescriptionObject.SetActive(isVisible);
        public void SetIsVisibleWeavingProcess(bool isVisible) => _weavingProcessObject.SetActive(isVisible);
        public void SetTextLoom(string text) => _loomTitle.text = text;
        public void SetTextSetWeaveButton(string text) => _setWeaveButtonTitle.text = text;
        public void SetTextTurnOffButton(string text) => _turnOffButtonTitle.text = text;

        //UI
        public event Action OnSetWeave;
        public void ActionSetWeave() => OnSetWeave?.Invoke();

        //UI
        public event Action OnTurnOff;
        public void ActionTurnOff() => OnTurnOff?.Invoke();

    }
}
