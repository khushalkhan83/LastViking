using Core.Views;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Views
{
    public class QueueCellView : ViewBase
        , IPointerDownHandler
    {
        #region Data
#pragma warning disable 0649

        [VisibleObject]
        [SerializeField] private GameObject _itemIconObject;
        [SerializeField] private Image _itemIcon;

        [VisibleObject]
        [SerializeField] private GameObject _timeTextObject;
        [SerializeField] private Text _timeText;

        [VisibleObject]
        [SerializeField] private GameObject _progressIconObject;
        [SerializeField] private Image _progressIcon;

        [SerializeField] private Color _progressIconColorDefault;
        [SerializeField] private Color _progressIconColorBoost;

        [VisibleObject]
        [SerializeField] private GameObject _borderBurnObject;

        [VisibleObject]
        [SerializeField] private GameObject _borderSelectionObject;

        [SerializeField] private Image _background;

        [SerializeField] private Color _backgroundColorDefault;
        [SerializeField] private Color _backgroundColorSelection;
        [SerializeField] private Color _backgroundColorNotBuyed;
        private Color _defaultColor;

#pragma warning restore 0649
        #endregion

        public Color BackgroundColorDefault => _backgroundColorDefault;
        public Color BackgroundColorSelection => _backgroundColorSelection;
        public Color ProgressIconColorDefault => _progressIconColorDefault;
        public Color ProgressIconColorBoost => _progressIconColorBoost;
        public Color BackgroundColorNotBuyed => _backgroundColorNotBuyed;
        public Color DefaultColor => _defaultColor;
        //

        public void SetIsVisibleItemIcon(bool value) => _itemIconObject.SetActive(value);
        public void SetIsVisibleTimeText(bool value) => _timeTextObject.SetActive(value);
        public void SetIsVisibleProgressIcon(bool value) => _progressIconObject.SetActive(value);
        public void SetIsVisibleBorderBurn(bool value) => _borderBurnObject.SetActive(value);
        public void SetIsVisibleBorderSelection(bool value) => _borderSelectionObject.SetActive(value);
        public void SetItemIcon(Sprite value) => _itemIcon.sprite = value;
        public void SetTimeText(string value) => _timeText.text = value;
        public void SetProgressAmount(float value) => _progressIcon.fillAmount = value;
        public void SetBackgroundColor(Color value) => _background.color = value;
        public void SetProgressIconColor(Color value) => _progressIcon.color = value;
        public void SetDefaultColor(Color value) => _defaultColor = value;

        //

        public event Action<QueueCellView, PointerEventData> OnPointDown_;
        public void OnPointerDown(PointerEventData eventData) => OnPointDown_?.Invoke(this, eventData);
        
        //

        public int CellId { get; private set; }
        public int ItemId { get; private set; }

        //

        public void SetData(QueueCellData cellData)
        {
            CellId = cellData.CellId;
            ItemId = cellData.ItemId;

            SetIsVisibleBorderSelection(cellData.IsSelected);

            if (cellData.IsSelected)
            {
                SetBackgroundColor(BackgroundColorSelection);
            }
            else
            {
                SetBackgroundColor(DefaultColor);
            }

            SetIsVisibleBorderBurn(cellData.isBurn);
            SetIsVisibleProgressIcon(cellData.isBurn);
            if (cellData.isBurn)
            {
                if (cellData.isBoost)
                {
                    SetProgressIconColor(ProgressIconColorBoost);
                }
                else
                {
                    SetProgressIconColor(ProgressIconColorDefault);
                }
            }

            if (cellData.isBurn)
            {
                SetProgressAmount(cellData.Progress);
            }

            SetIsVisibleItemIcon(cellData.ItemIcon != null);
            SetItemIcon(cellData.ItemIcon);

            SetIsVisibleTimeText(!string.IsNullOrEmpty(cellData.TimeLeft));
            SetTimeText(cellData.TimeLeft);
        }
    }
}
