using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class InfoPanelMedicineView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Text _medicineItemName;
        [SerializeField] private Text _healthAddon;

        [SerializeField] private Image _firstAbilityIcon;
        [SerializeField] private Image _secondAbilityIcon;
        [SerializeField] private GameObject _firstAbility;
        [SerializeField] private GameObject _secondAbility;
        [SerializeField] private Text _medicineDescription;
        [SerializeField] private Text _medicineDescriptionSecond;

        [SerializeField] private GameObject _decor;

#pragma warning restore 0649
        #endregion

        public Text MedicineItemName => _medicineItemName;
        public Text HealthAddon => _healthAddon;

        public Image FirstAbilityIcon => _firstAbilityIcon;
        public Image SecondAbilityIcon => _secondAbilityIcon;
        public GameObject FirstAbiltiy => _firstAbility;
        public GameObject SecondAbility => _secondAbility;
        public Text MedicineDescription => _medicineDescription;
        public Text MedicineDescriptionSecond => _medicineDescriptionSecond;

        public GameObject Decor => _decor;

        public void SetMedicineItemName(string value) => _medicineItemName.text = value;
        public void SetMedicineItemDescription(string value) => _medicineDescription.text = value;
        public void SetMedicineItemDescriptionSecond(string value) => _medicineDescriptionSecond.text = value;
        public void SetHealthAddon(string value) => _healthAddon.text = value;
        public void SetVisibleFisrtAbiltity(bool isVisible) => _firstAbility.SetActive(isVisible);
        public void SetVisibleSecondAbility(bool isVisible) => _secondAbility.SetActive(isVisible);
        public void SetFirstAbilityIcon(Sprite sprite) => _firstAbilityIcon.sprite = sprite;
        public void SetSecondAbilityIcon(Sprite sprite) => _secondAbilityIcon.sprite = sprite;
        public void SetDecorActive(bool isVisible) => _decor.SetActive(isVisible);
    }
}
