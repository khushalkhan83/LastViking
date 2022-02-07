using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class InfoPanelWeaponView : ViewBase
    {
        #region Data
#pragma warning disable 0649
        
        [SerializeField] private Text _weaponItemName;
        [SerializeField] private Image _weaponDamage;
        [SerializeField] private Image _weaponDurability;

        [SerializeField] private Image _firstAbilityIcon;
        [SerializeField] private Image _secondAbilityIcon;
        [SerializeField] private Text _weaponDescription;
        [SerializeField] private Text _weaponDescriptionSecond;
        [SerializeField] private GameObject _firstAbiltiy;
        [SerializeField] private GameObject _secondAbility;

        [SerializeField] private GameObject _decor;

        [SerializeField] int[] _damageGroupRanges = new int[4];
        [SerializeField] float[] _durabilityGroudRanges = new float[4];

#pragma warning restore 0649
        #endregion

        public float[] AmountFill { get; } = { 0, 0.25f, 0.5f, 0.75f, 1f };
        public int[] DamageGroupRanges => _damageGroupRanges;
        public float[] DurabilityGroupRanges => _durabilityGroudRanges;

        public Text WeaponItemName => _weaponItemName;
        public Image WeaponDamage => _weaponDamage;
        public Image WeaponDurability => _weaponDurability;

        public Image FirstAbilityIcon => _firstAbilityIcon;
        public Image SecondAbilityIcon => _secondAbilityIcon;
        public Text WeaponDescription => _weaponDescription;
        public Text WeaponDescriptionSecond => _weaponDescriptionSecond;
        public GameObject FirstAbiltiy => _firstAbiltiy;
        public GameObject SecondAbility => _secondAbility;

        public GameObject Decor => _decor;

        public void SetWeaponItemName(string value) => _weaponItemName.text = value;
        public void SetWeaponItemDescription(string value) => _weaponDescription.text = value;
        public void SetWeaponItemDescriptionSecond(string value) => _weaponDescriptionSecond.text = value;
        public void SetFillAmountWeaponDamage(float value) => _weaponDamage.fillAmount = value;
        public void SetFillAmountWeaponDurability(float value) => _weaponDurability.fillAmount = value;
        public void SetFirstAbilityIcon(Sprite sprite) => _firstAbilityIcon.sprite = sprite;
        public void SetSecondAbilityIcon(Sprite sprite) => _secondAbilityIcon.sprite = sprite;
        public void SetVisibleFisrtAbiltity(bool isVisible) => _firstAbiltiy.SetActive(isVisible);
        public void SetVisibleSecondAbility(bool isVisible) => _secondAbility.SetActive(isVisible);
        public void SetDecorActive(bool isVisible) => _decor.SetActive(isVisible);
    }
}
