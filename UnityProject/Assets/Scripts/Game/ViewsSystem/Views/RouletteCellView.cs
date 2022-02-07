using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UltimateSurvival;


namespace Game.Views
{
    public class RouletteCellView : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [VisibleObject]
        [SerializeField] private GameObject _countItemsObject;

        [SerializeField] private Text _countItemsText;

        [SerializeField] private Image _itemIcon;

        [SerializeField] private Image _background;

        [SerializeField] private Color _backgroundColorDefault;

        [SerializeField] private Color _backgroundColorComponent;

        [SerializeField] private Color _backgroundColorSpecial;

        [VisibleObject]
        [SerializeField] private GameObject _midRarityIconObject;
        [VisibleObject]
        [SerializeField] private GameObject _rareRarityIconObject;

#pragma warning restore 0649
        #endregion

        public void SetTextCountItems(string text) => _countItemsText.text = text;
        public void SetImageItem(Sprite sprite) => _itemIcon.sprite = sprite;
        public void SetBackgroundColor(Color color) => _background.color = color;
        public Color BackgroundColorDefault => _backgroundColorDefault;
        public Color BackgroundColorComponent => _backgroundColorComponent;
        public Color BackgroundColorSpecial => _backgroundColorSpecial;
        public void SetIsVisibleCount(bool isVisible) => _countItemsObject.SetActive(isVisible);
        public void SetIsVisible(bool isVisible) => gameObject.SetActive(isVisible);
        public void SetIsVisibleMidRarityIcon(bool isVisible) => _midRarityIconObject.SetActive(isVisible);
        public void SetIsVisibleRareRarityIcon(bool isVisible) => _rareRarityIconObject.SetActive(isVisible);

        public void SetData(RouletteCellData data) {
            SetImageItem(data.Icon);
            SetIsVisibleCount(data.Count > 1);
            SetTextCountItems(data.Count.ToString());

            switch (data.ItemRarity)
            {
                case ItemRarity.Mid:
                    SetIsVisibleMidRarityIcon(true);
                    SetIsVisibleRareRarityIcon(false);
                    break;
                case ItemRarity.Rare:
                    SetIsVisibleMidRarityIcon(false);
                    SetIsVisibleRareRarityIcon(true);
                    break;
                default:
                    SetIsVisibleMidRarityIcon(false);
                    SetIsVisibleRareRarityIcon(false);
                    break;
            }

            if (data.IsComponent)
            {
                SetBackgroundColor(BackgroundColorComponent);
            }
            else if (data.IsSpecial)
            {
                SetBackgroundColor(BackgroundColorSpecial);
            }
            else {
                SetBackgroundColor(BackgroundColorDefault);
            }
        }
    }
}
