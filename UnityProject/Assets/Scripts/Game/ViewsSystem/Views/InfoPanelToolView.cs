using Core.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class InfoPanelToolView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Text _toolItemName;
        [SerializeField] private Image _toolGathering;
        [SerializeField] private Image _toolDurability;
        [SerializeField] private GameObject _toolIconWood;
        [SerializeField] private GameObject _toolIconStone;
        [SerializeField] private GameObject _toolIconGather;

        [SerializeField] private Image _firstAbilityIcon;
        [SerializeField] private Image _secondAbilityIcon;
        [SerializeField] private GameObject _firstAbility;
        [SerializeField] private GameObject _secondAbility;
        [SerializeField] private Text _toolDescription;
        [SerializeField] private Text _toolDescriptionSecond;

        [SerializeField] private GameObject _decor;

        [SerializeField] int[] _damage = new int[4];
        [SerializeField] float[] _durability = new float[4];

#pragma warning restore 0649
        #endregion

        public float[] AmountFill { get; } = { 0, 0.25f, 0.5f, 0.75f, 1f };
        public int[] DamageGroupRanges => _damage;
        public float[] DurabilityGroupRanges => _durability;

        public Text ToolItemName => _toolItemName;
        public Image ToolGathering => _toolGathering;
        public Image ToolDurability => _toolDurability;
        public GameObject ToolIconWood => _toolIconWood;
        public GameObject ToolIconStone => _toolIconStone;
        public GameObject ToolIconGather => _toolIconGather;

        public Image FirstAbilityIcon => _firstAbilityIcon;
        public Image SecondAbilityIcon => _secondAbilityIcon;
        public GameObject FirstAbiltiy => _firstAbility;
        public GameObject SecondAbility => _secondAbility;
        public Text ToolDescription => _toolDescription;
        public Text ToolDescriptionSecond => _toolDescriptionSecond;

        public GameObject Decor => _decor;

        public void SetToolItemName(string value) => _toolItemName.text = value;
        public void SetToolItemDescription(string value) => _toolDescription.text = value;
        public void SetToolItemDescriptionSecond(string value) => _toolDescriptionSecond.text = value;
        public void SetFillAmountToolGather(float value) => _toolGathering.fillAmount = value;
        public void SetFillAmountToolDurability(float value) => _toolDurability.fillAmount = value;
        public void SetItemIconWood()
        {
            _toolIconWood.SetActive(true);
            _toolIconStone.SetActive(false);
            _toolIconGather.SetActive(false);
        }
        public void SetItemIconStone()
        {
            _toolIconWood.SetActive(false);
            _toolIconStone.SetActive(true);
            _toolIconGather.SetActive(false);
        }
        public void SetItemIcon()
        {
            _toolIconWood.SetActive(false);
            _toolIconStone.SetActive(false);
            _toolIconGather.SetActive(true);
        }
        public void SetVisibleFisrtAbiltity(bool isVisible) => _firstAbility.SetActive(isVisible);
        public void SetVisibleSecondAbility(bool isVisible) => _secondAbility.SetActive(isVisible);
        public void SetFirstAbilityIcon(Sprite sprite) => _firstAbilityIcon.sprite = sprite;
        public void SetSecondAbilityIcon(Sprite sprite) => _secondAbilityIcon.sprite = sprite;
        public void SetDecorActive(bool isVisible) => _decor.SetActive(isVisible);

    }
}
