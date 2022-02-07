using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Views
{
    public class HouseBuildingCellView : MonoBehaviour, IPointerDownHandler
    {
       #region Data
#pragma warning disable 0649

        [VisibleObject]
        [SerializeField] private GameObject _iconObject;
        [SerializeField] private Image _icon;

        [SerializeField] private Color _iconColorActive;
        [SerializeField] private Color _iconColorPassive;

        [VisibleObject]
        [SerializeField] private GameObject _countTextObject;
        [SerializeField] private Text _countText;

        [VisibleObject]
        [SerializeField] private GameObject _levelTextObject;
        [SerializeField] private Text _levelText;

        [SerializeField] private Color _textColorActive;
        [SerializeField] private Color _textColorPassive;

        [VisibleObject]
        [SerializeField] private GameObject _borderSelectionObject;

        [SerializeField] private Image _background;

        [SerializeField] private Color _backgroundColorDefault;
        [SerializeField] private Color _backgroundColorSelection;

#pragma warning restore 0649
        #endregion

        public Color BackgroundColorDefault => _backgroundColorDefault;
        public Color BackgroundColorSelection => _backgroundColorSelection;
        public Color IconColorActive => _iconColorActive;
        public Color IconColorPassive => _iconColorPassive;
        public Color TextColorActive => _textColorActive;
        public Color TextColorPassive => _textColorPassive;

        //

        public void SetIsVisibleIcon(bool value) => _iconObject.SetActive(value);
        public void SetIsVisibleCountText(bool value) => _countTextObject.SetActive(value);
        public void SetIsVisibleLevelText(bool value) => _levelTextObject.SetActive(value);
        public void SetIsVisibleBorderSelection(bool value) => _borderSelectionObject.SetActive(value);
        public void SetIcon(Sprite value) => _icon.sprite = value;
        public void SetIconColor(Color value) => _icon.color = value;
        public void SetCountText(string value) => _countText.text = value;
        public void SetLevelText(string value) => _levelText.text = value;
        public void SetBackgroundColor(Color value) => _background.color = value;
        public void SetCountTextColor(Color value) => _countText.color = value;
        public void SetLevelTextColor(Color value) => _countText.color = value;

        //

        public event Action<HouseBuildingCellView, PointerEventData> OnPointDown_;
        public void OnPointerDown(PointerEventData eventData) => OnPointDown_?.Invoke(this, eventData);

        //

        public void SetData(HouseBuildingCellData data)
        {

            SetIsVisibleIcon(data.Icon);
            SetIcon(data.Icon);

            SetIsVisibleCountText(!string.IsNullOrEmpty(data.countText));
            SetCountText(data.countText);

            SetLevelText(data.levelText);

            SetIsVisibleBorderSelection(data.IsSelected);

            if (data.IsSelected)
            {
                SetBackgroundColor(BackgroundColorSelection);
            }
            else
            {
                SetBackgroundColor(BackgroundColorDefault);
            }

            if (data.IsActive)
            {
                SetIconColor(IconColorActive);
                SetCountTextColor(TextColorActive);
            }
            else
            {
                SetIconColor(IconColorPassive);
                SetCountTextColor(TextColorPassive);
            }
        }
    }
}