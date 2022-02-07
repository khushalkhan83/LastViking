using Core.Views;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Game.Views
{
    public class ConstructionInfoPopupView : ViewBase
    {
        public event Action OnOkButton;
        public event Action OnBuildingButton;

        #region Data
#pragma warning disable 0649

        [SerializeField] private Text _buildingText;
        [SerializeField] private Text _constructionText;
        [SerializeField] private Text _attensionText;
        [SerializeField] private Text _descriptionText;
        [SerializeField] private DisablableButtonView _okButton;
        [SerializeField] private Button _buildingButton;
        [SerializeField] private GameObject _popup;

#pragma warning restore 0649
        #endregion

        public void SetTextBuilding(string value) => _buildingText.text = value;
        public void SetTextConstruction(string value) => _constructionText.text = value;
        public void SetTextAttension(string value) => _attensionText.text = value;
        public void SetTextDescription(string value) => _descriptionText.text = value;
        public void SetTextOkButton(string value) => _okButton.SetText(value);
        public void SetIsVisiblePopup(bool value) => _popup.SetActive(value);
        public void SetOkButtonInteractable(bool interactable)
        {
            _okButton.SetIsVisibleActiveObject(interactable);
            _okButton.SetIsVisiblePassiveObject(!interactable);
        }
        public void SetBuildingButtonInteractable(bool interactable) => _buildingButton.interactable = interactable;

        public void ActionOK() => OnOkButton?.Invoke();

        public void ActionBuilding() => OnBuildingButton?.Invoke();

    }
}
