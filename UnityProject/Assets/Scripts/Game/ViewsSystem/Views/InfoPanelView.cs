using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class InfoPanelView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Text _itemName;
        [SerializeField] private Text _itemDescription;
        [SerializeField] private GameObject _decor;

#pragma warning restore 0649
        #endregion

        public Text ItemName => _itemName;
        public Text ItemDescription => _itemDescription;
        public GameObject Decor => _decor;

        public void SetItemName(string value) => _itemName.text = value;
        public void SetItemDescription(string value) => _itemDescription.text = value;
        public void SetDecorActive(bool isVisible) => _decor.SetActive(isVisible);
    }
}
