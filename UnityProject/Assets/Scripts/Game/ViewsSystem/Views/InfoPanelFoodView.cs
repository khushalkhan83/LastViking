using Core.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class InfoPanelFoodView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Text _foodItemName;
        [SerializeField] private Text _healthValue;
        [SerializeField] private Text _hungerValue;
        [SerializeField] private Text _thirstValue;

        [SerializeField] private Image _firstAbilityIcon;
        [SerializeField] private Image _secondAbilityIcon;
        [SerializeField] private GameObject _firstAbility;
        [SerializeField] private GameObject _secondAbility;
        [SerializeField] private Text _foodDescription;
        [SerializeField] private Text _foodDescriptionSecond;

        [SerializeField] private Image _healthIcon;
        [SerializeField] private Image _hungerIcon;
        [SerializeField] private Image _thirstIcon;
        [SerializeField] private Color _grey;
        [SerializeField] private Color _green;
        [SerializeField] private Color _red;
        [SerializeField] private GameObject _decor;

#pragma warning restore 0649
        #endregion

        public Text FoodItemName => _foodItemName;
        public Text HealthValue => _healthValue;
        public Text HungerValue => _hungerValue;
        public Text ThirstValue => _thirstValue;

        public Image FirstAbilityIcon => _firstAbilityIcon;
        public Image SecondAbilityIcon => _secondAbilityIcon;
        public GameObject FirstAbiltiy => _firstAbility;
        public GameObject SecondAbility => _secondAbility;
        public Text FoodDescription => _foodDescription;
        public Text FoodDescriptionSecond => _foodDescriptionSecond;

        public Image HealthIcon => _healthIcon;
        public Image HungerIcon => _hungerIcon;
        public Image ThirstIcon => _thirstIcon;
        public Color Grey => _grey;
        public Color Green => _green;
        public Color Red => _red;
        public GameObject Decor => _decor;

        public void SetFoodItemName(string value) => _foodItemName.text = value;
        public void SetFoodItemDescription(string value) => _foodDescription.text = value;
        public void SetFoodItemDescriptionSecond(string value) => _foodDescriptionSecond.text = value;
        public void SetHealthValue(string value) => _healthValue.text = value;
        public void SetHungerValue(string value) => _hungerValue.text = value;
        public void SetThirstValue(string value) => _thirstValue.text = value;
        public void SetVisibleFisrtAbiltity(bool isVisible) => _firstAbility.SetActive(isVisible);
        public void SetVisibleSecondAbility(bool isVisible) => _secondAbility.SetActive(isVisible);
        public void SetFirstAbilityIcon(Sprite sprite) => _firstAbilityIcon.sprite = sprite;
        public void SetSecondAbilityIcon(Sprite sprite) => _secondAbilityIcon.sprite = sprite;
        public void SetDecorActive(bool isVisible) => _decor.SetActive(isVisible);
        public void SetColorsRed(Text text, Image image)
        {
            text.color = Red;
            image.color = Red;
        }
        public void SetColorsGreen(Text text, Image image)
        {
            text.color = Green;
            image.color = Green;
        }
        public void SetColorsGrey(Text text, Image image)
        {
            text.color = Grey;
            image.color = Grey;
        }
    }
}
