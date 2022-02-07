using System;
using Core.Views;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Views
{
    public class CraftCellView : ViewBase
        , IPointerDownHandler
    {
        #region Data
#pragma warning disable 0649

        [VisibleObject]
        [SerializeField] private GameObject _itemIconObject;
        [SerializeField] private Image _itemIcon;

        [SerializeField] private Color _itemColorLocked;
        [SerializeField] private Color _itemColorLockedDisable;
        [SerializeField] private Color _itemColorUnlocked;
        [SerializeField] private Color _itemColorDisable;

        [VisibleObject]
        [SerializeField] private GameObject _featherIconObject;

        [VisibleObject]
        [SerializeField] private GameObject _epicBorderObject;
        [SerializeField] private Image _epicBorder;

        [SerializeField] private Color _epicBorderColorDefault;
        [SerializeField] private Color _epicBorderColorDisable;

        [VisibleObject]
        [SerializeField] private GameObject _nameTextObject;
        [SerializeField] private Text _nameText;

        [SerializeField] private Color _textColorDefault;
        [SerializeField] private Color _textColorDisable;

        [VisibleObject]
        [SerializeField] private GameObject _selectionBorderObject;

        [SerializeField] private Image _background;

        [SerializeField] private Color _backgroundColorDefault;
        [SerializeField] private Color _backgroundColorDisable;
        [SerializeField] private Color _backgroundColorSelected;
        [SerializeField] private Color _backgroundColorEpic;
        [SerializeField] private Color _backgroundColorEpicDisable;

#pragma warning restore 0649
        #endregion

        public Color ItemColorLocked => _itemColorLocked;
        public Color ItemColorLockedDisable => _itemColorLockedDisable;
        public Color ItemColorUnlocked => _itemColorUnlocked;
        public Color ItemColorDisable => _itemColorDisable;
        public Color NameColorDefault => _textColorDefault;
        public Color NameColorDisable => _textColorDisable;
        public Color BackgroundColorDefault => _backgroundColorDefault;
        public Color BackgroundColorDisable => _backgroundColorDisable;
        public Color BackgroundColorSelected => _backgroundColorSelected;
        public Color BackgroundColorEpicDefault => _backgroundColorEpic;
        public Color BackgroundColorEpicDisable => _backgroundColorEpicDisable;
        public Color EpicBorderColorDefault => _epicBorderColorDefault;
        public Color EpicBorderColorDisable => _epicBorderColorDisable;

        //

        public void SetIsVisibleItemIcon(bool value) => _itemIconObject.SetActive(value);
        public void SetIsVisibleLockedIcon(bool value) => _featherIconObject.SetActive(value);
        public void SetIsVisibleEpicBorder(bool value) => _epicBorderObject.SetActive(value);
        public void SetIsVisibleName(bool value) => _nameTextObject.SetActive(value);
        public void SetIsVisibleSelectionBorder(bool value) => _selectionBorderObject.SetActive(value);
        public void SetIcon(Sprite value) => _itemIcon.sprite = value;
        public void SetItemColor(Color value) => _itemIcon.color = value;
        public void SetName(string value) => _nameText.text = value;
        public void SetNameColor(Color value) => _nameText.color = value;
        public void SetEpicBorderColor(Color value) => _epicBorder.color = value;
        public void SetBackgroundColor(Color value) => _background.color = value;

        //

        public int ItemId { get; private set; }
        public int CellId { get; private set; }

        //

        public event Action<CraftCellView, PointerEventData> OnPointDown_;
        public void OnPointerDown(PointerEventData eventData) => OnPointDown_?.Invoke(this, eventData);

        //
        public void SetData(CraftCellData data)
        {
            ItemId = data.ItemId;
            CellId = data.CellId;

            SetIsVisibleItemIcon(data.Icon);
            if (data.Icon != null)
            {
                SetIcon(data.Icon);
            }

            var isvisibleName = !string.IsNullOrEmpty(data.Name);
            SetIsVisibleName(isvisibleName);

            if (isvisibleName)
            {
                SetName(data.Name);
            }

            SetIsVisibleLockedIcon(data.IsCanUnlock);
            SetItemColor(GetItemColor(data));
            SetNameColor(GetNameColor(data));
            SetBackgroundColor(GetBackGroundColor(data));
            SetIsVisibleEpicBorder(data.IsEpic);

            if (data.IsEpic)
            {
                SetEpicBorderColor(GetEpicColor(data));
            }

            SetIsVisibleSelectionBorder(data.IsSelected);
        }

        private Color GetEpicColor(CraftCellData data)
        {
            if (data.IsActive)
            {
                return EpicBorderColorDefault;
            }
            return EpicBorderColorDisable;
        }

        private Color GetBackGroundColor(CraftCellData data)
        {
            if (data.IsSelected)
            {
                return BackgroundColorSelected;
            }
            if (data.IsEpic)
            {
                if (data.IsActive)
                {
                    return BackgroundColorEpicDefault;
                }
                return BackgroundColorEpicDisable;
            }
            if (data.IsActive)
            {
                return BackgroundColorDefault;
            }
            return BackgroundColorDisable;
        }

        private Color GetNameColor(CraftCellData data)
        {
            if (data.IsSelected || data.IsActive)
            {
                return NameColorDefault;
            }

            return NameColorDisable;
        }

        private Color GetItemColor(CraftCellData data)
        {
            if (data.IsSelected)
            {
                if (data.IsLocked)
                {
                    return ItemColorLocked;
                }
                return ItemColorUnlocked;
            }
            if (data.IsLocked)
            {
                if (data.IsActive)
                {
                    return ItemColorLocked;
                }
                return ItemColorLockedDisable;
            }
            if (data.IsActive)
            {
                return ItemColorUnlocked;
            }
            return ItemColorDisable;
        }
    }
}
