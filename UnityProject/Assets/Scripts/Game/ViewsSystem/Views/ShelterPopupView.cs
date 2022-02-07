using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class ShelterPopupView : ViewBase
    {
        #region Data
#pragma warning disable 0649
        [SerializeField] private Image _mainIcon;

        [VisibleObject]
        [SerializeField] private GameObject _buyPanelObject;
        [VisibleObject]
        [SerializeField] private GameObject _skeletonsAttackObject;
        [VisibleObject]
        [SerializeField] private GameObject _buyButtonObject;

        [SerializeField] private Text _shipLevelText;
        [SerializeField] private Text _descriptionNameText;
        [SerializeField] private Text _descriptionText;
        [SerializeField] private Text _attackWarningText;
        [SerializeField] private Text _buildButtonText;
        [SerializeField] private ResourceCellView[] _resourceCells;
        [SerializeField] private ResourceCellView _questItemCells;
        [SerializeField] private Transform _container;
        [SerializeField] private Text _selectedResourceDescription;


#pragma warning restore 0649
        #endregion

        public ResourceCellView[] ResourceCells => _resourceCells;
        public ResourceCellView QuestItemCells => _questItemCells;

        public void SetIsVisibleBuyPanel(bool isVisible) => _buyPanelObject.SetActive(isVisible);
        public void SetIsVisibleSkeletonAttackPanel(bool isVisible) => _skeletonsAttackObject.SetActive(isVisible);
        public void SetIsVisibleBuyButton(bool isVisable) => _buyButtonObject.SetActive(isVisable);

        public void SetShipLevelText(string text) => _shipLevelText.text = text;
        public void SetTextBuildButton(string text) => _buildButtonText.text = text;
        public void SetTextDescriptionName(string text) => _descriptionNameText.text = text;
        public void SetTextDescription(string text) => _descriptionText.text = text;
        public void SetAttackWarningText(string text) => _attackWarningText.text = text;
        public void SetSelectedResourceDescription(string text) => _selectedResourceDescription.text = text;
        public void SetIsVisiableSelectedResourceDescription(bool isVisiable) => _selectedResourceDescription.gameObject.SetActive(isVisiable);
       
        //UI
        public event Action OnBuy;
        public void ActionBuy() => OnBuy?.Invoke();

        //UI
        public event Action OnClose;
        public void ActionClose() => OnClose?.Invoke();

        public Transform Container => _container;
    }
}
