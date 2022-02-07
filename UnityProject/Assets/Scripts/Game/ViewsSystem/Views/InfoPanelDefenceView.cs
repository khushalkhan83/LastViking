using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class InfoPanelDefenceView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Text _defenceItemName;
        [SerializeField] private Text _defenceHealth;
        [SerializeField] private Text _defenceDescription;
        [SerializeField] private GameObject _decor;

#pragma warning restore 0649
        #endregion

        public Text DefenceItemName => _defenceItemName;
        public Text DefenceHealth => _defenceHealth;
        public Text DefenceDescription => _defenceDescription;
        public GameObject Decor => _decor;

        public void SetDefenceItemName(string value) => _defenceItemName.text = value;
        public void SetDefenceItemDescription(string value) => _defenceDescription.text = value;
        public void SetDefenceHealth(string value) => _defenceHealth.text = value;
        public void SetDecorActive(bool isVisible) => _decor.SetActive(isVisible);
    }
}
