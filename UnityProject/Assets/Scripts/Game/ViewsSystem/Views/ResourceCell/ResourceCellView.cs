using System;
using UltimateSurvival;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Views
{

    public class ResourceCellView : MonoBehaviour
        , IPointerDownHandler
    {
        #region Data
#pragma warning disable 0649

        [VisibleObject]
        [SerializeField] private GameObject _iconObject;
        [SerializeField] private Image _icon;

        [SerializeField] private Color _iconColorActive;
        [SerializeField] private Color _iconColorPassive;

        [VisibleObject]
        [SerializeField] private GameObject _textObject;
        [SerializeField] private Text _text;

        [SerializeField] private Color _textColorActive;
        [SerializeField] private Color _textColorPassive;

        [VisibleObject]
        [SerializeField] private GameObject _borderSelectionObject;

        [SerializeField] private Image _background;

        [SerializeField] private Color _backgroundColorDefault;
        [SerializeField] private Color _backgroundColorSelection;
        [SerializeField] private Color _backgroundColorDefaultComponent;

        [VisibleObject]
        [SerializeField] private GameObject _midRarityIconObject;
        [VisibleObject]
        [SerializeField] private GameObject _rareRarityIconObject;

#pragma warning restore 0649
        #endregion

        public Color BackgroundColorDefault => _backgroundColorDefault;
        public Color BackgroundColorSelection => _backgroundColorSelection;
        public Color BackgroundColorDefaultComponent => _backgroundColorDefaultComponent;
        public Color IconColorActive => _iconColorActive;
        public Color IconColorPassive => _iconColorPassive;
        public Color TextColorActive => _textColorActive;
        public Color TextColorPassive => _textColorPassive;

        //

        public void SetIsVisibleIcon(bool value) => _iconObject.SetActive(value);
        public void SetIsVisibleText(bool value) => _textObject.SetActive(value);
        public void SetIsVisibleBorderSelection(bool value) => _borderSelectionObject.SetActive(value);
        public void SetIsVisibleMidRarityIcon(bool isVisible) => _midRarityIconObject.SetActive(isVisible);
        public void SetIsVisibleRareRarityIcon(bool isVisible) => _rareRarityIconObject.SetActive(isVisible);
        public void SetIcon(Sprite value) => _icon.sprite = value;
        public void SetIconColor(Color value) => _icon.color = value;
        public void SetText(string value) => _text.text = value;
        public void SetBackgroundColor(Color value) => _background.color = value;
        public void SetTextColor(Color value) => _text.color = value;

        //

        public event Action<ResourceCellView, PointerEventData> OnPointDown_;
        public void OnPointerDown(PointerEventData eventData) => OnPointDown_?.Invoke(this, eventData);

        //

        public int ItemId { get; private set; }
        public int CellId { get; private set; }
        public bool IsComponent { get; private set; }

        public void SetData(ResourceCellData data)
        {
            ItemId = data.ItemId;
            CellId = data.CellId;
            IsComponent = data.IsComponent;

            SetIsVisibleIcon(data.Icon);
            SetIcon(data.Icon);
            if (data.Icon)
            {
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
            }
            else {
                SetIsVisibleMidRarityIcon(false);
                SetIsVisibleRareRarityIcon(false);
            }

            SetIsVisibleText(!string.IsNullOrEmpty(data.Message));
            SetText(data.Message);

            SetIsVisibleBorderSelection(data.IsSelected);

            if (data.IsSelected)
            {
                SetBackgroundColor(BackgroundColorSelection);
            }
            else
            {
                if (IsComponent)
                    SetBackgroundColor(BackgroundColorDefaultComponent);
                else
                    SetBackgroundColor(BackgroundColorDefault);
            }

            if (data.IsActive)
            {
                SetIconColor(IconColorActive);
                SetTextColor(TextColorActive);
            }
            else
            {
                SetIconColor(IconColorPassive);
                SetTextColor(TextColorPassive);
            }
        }
    }
}
