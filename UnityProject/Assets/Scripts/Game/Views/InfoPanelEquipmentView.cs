using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class InfoPanelEquipmentView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Text _itemName;

        [SerializeField] private GameObject _bonusGO;
        [SerializeField] private Image _bonusIcon;
        [SerializeField] private Text _bonusValueText;

        [SerializeField] private GameObject _setBonusGO;
        [SerializeField] private Image _setBonusIcon;
        [SerializeField] private Text _setBonusValueText;

        [SerializeField] private GameObject _descriptionGO;
        [SerializeField] private GameObject _setDescriptionGO;
        [SerializeField] private Image _setIcon;
        [SerializeField] private Text _descriptioText;
        [SerializeField] private Text _setDescriptionText;

        [SerializeField] private Color _activeBonusColor;
        [SerializeField] private Color _disableBonusColor;

        [SerializeField] private GameObject _decor;

#pragma warning restore 0649
        #endregion

        public Text ItemName => _itemName;

        public GameObject BonusGO => _bonusGO;
        public Text BonusValueText => _bonusValueText;
        public Image BonusIcon => _bonusIcon;

        public GameObject SetBonusGO => _setBonusGO;
        public Text SetBonusValueText => _setBonusValueText;
        public Image SetBonusIcon => _setBonusIcon;

        public Image SetIcon => _setIcon;
        public GameObject DescriptionGO => _descriptionGO;
        public GameObject SetDescriptionGO => _setDescriptionGO;
        public Text DescriptioText => _descriptioText;
        public Text SetDescriptionText => _setDescriptionText;

        public GameObject Decor => _decor;

        public void SetEquipmentSetBonusActive(bool isActive)
        {
            _setBonusIcon.color = isActive ? _activeBonusColor : _disableBonusColor;
            _setBonusValueText.color = isActive ? _activeBonusColor : _disableBonusColor;
            _setIcon.color = isActive ? _activeBonusColor : _disableBonusColor;
            _setDescriptionText.color = isActive ? _activeBonusColor : _disableBonusColor;
        }
    }
}
