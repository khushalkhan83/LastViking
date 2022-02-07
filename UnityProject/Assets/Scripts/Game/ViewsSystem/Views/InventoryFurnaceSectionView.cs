using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class InventoryFurnaceSectionView : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [VisibleObject]
        [SerializeField] private GameObject _turnOffButton;

        [VisibleObject]
        [SerializeField] private GameObject _setFireButton;

        [VisibleObject]
        [SerializeField] private GameObject _boostButtonObject;
        [SerializeField] private Transform _containerBoostButton;

        [VisibleObject]
        [SerializeField] private GameObject _itemNameObject;

        [SerializeField] private RectTransform _container;

        [VisibleObject]
        [SerializeField] private GameObject _itemDescriptionObject;

        [VisibleObject]
        [SerializeField] private GameObject _cookingProcessObject;

        [SerializeField] private CookCellView[] _cookCells;

        [SerializeField] private Transform _containerExpandLootCell;

        [VisibleObject]
        [SerializeField] private GameObject _firstLootCellObject;

        [SerializeField] private Text _furnaceTitle;
        [SerializeField] private Text _setFireButtonTitle;
        [SerializeField] private Text _turnOffButtonTitle;

#pragma warning restore 0649
        #endregion

        public CookCellView[] CookCells => _cookCells;
        public Transform ContainerExpandLootCell => _containerExpandLootCell;
        public Transform ContainerBoostButton => _containerBoostButton;
        public RectTransform Container => _container;

        public void SetIsVisibleFirstLootCell(bool isVisible) => _firstLootCellObject.SetActive(isVisible);
        public void SetIsVisibleSetFireButton(bool isVisible) => _setFireButton.SetActive(isVisible);
        public void SetIsVisibleTurnOffButton(bool isVisible) => _turnOffButton.SetActive(isVisible);
        public void SetIsVisibleBoostButton(bool isVisible) => _boostButtonObject.SetActive(isVisible);
        public void SetIsVisibleItemName(bool isVisible) => _itemNameObject.SetActive(isVisible);
        public void SetIsVisibleItemDescription(bool isVisible) => _itemDescriptionObject.SetActive(isVisible);
        public void SetIsVisibleCookingProcess(bool isVisible) => _cookingProcessObject.SetActive(isVisible);
        public void SetTextFurnace(string text) => _furnaceTitle.text = text;
        public void SetTextSetFireButton(string text) => _setFireButtonTitle.text = text;
        public void SetTextTurnOffButton(string text) => _turnOffButtonTitle.text = text;

        //UI
        public event Action OnSetFire;
        public void ActionSetFire() => OnSetFire?.Invoke();

        //UI
        public event Action OnTurnOff;
        public void ActionTurnOff() => OnTurnOff?.Invoke();

    }
}
