using Core.Views;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Views.Cell
{
    public class HotBarCellView : ViewBase
        , IPointerDownHandler
    {
        private readonly float brokenFillAmount = 0.05f;

        #region Data
#pragma warning disable 0649

        [SerializeField] private Image _itemIcon;

        [VisibleObject]
        [SerializeField] private GameObject _itemIconObject;

        [VisibleObject]
        [SerializeField] private GameObject _durabilityObject;
        [SerializeField] private Image _durabilityAmount;
        [VisibleObject]
        [SerializeField] public GameObject _brokenIconObject;

        [VisibleObject]
        [SerializeField] private GameObject _sipsObject;
        [SerializeField] private Image _sipsAmount;

        [VisibleObject]
        [SerializeField] private GameObject _selectionBorderObject;

        [VisibleObject]
        [SerializeField] private GameObject _counterObject;
        [SerializeField] private Text _counter;

        [VisibleObject]
        [SerializeField] public GameObject _progressIconObject;
        [SerializeField] public Image _progressIcon;

        [VisibleObject]
        [SerializeField] public GameObject _repairingProgressIconObject;
        [SerializeField] public Image _repairingProgressIcon;

        [SerializeField] private Color _itemIconColorDefault;
        [SerializeField] private Color _ItemIconColorConsumeOther;

        [SerializeField] private Color _itemDurabilityWorkingColor;
        [SerializeField] private Color _itemDurabilityBrokenColor;

#pragma warning restore 0649
        #endregion

        //

        public Color ItemIconColorDefault => _itemIconColorDefault;
        public Color ItemIconColorConsumeOther => _ItemIconColorConsumeOther;

        public Color ItemDurabilityWorkingColor => _itemDurabilityWorkingColor;
        public Color ItemDurabilityBrokenColor => _itemDurabilityBrokenColor;

        public void SetCount(string value) => _counter.text = value;
        public void SetItemIcon(Sprite value) => _itemIcon.sprite = value;
        public void SetItemIconColor(Color color) => _itemIcon.color = color;
        public void SetIsVisibleItemIcon(bool isVisible) => _itemIconObject.SetActive(isVisible);
        public void SetIsVisibleCounter(bool isVisible) => _counterObject.SetActive(isVisible);
        public void SetIsVisibleDurabitity(bool isVisible) => _durabilityObject.SetActive(isVisible);
        public void SetIsVisibleBroken(bool isVisible) => _brokenIconObject.SetActive(isVisible);
        public void SetIsVisibleSips(bool isVisible) => _sipsObject.SetActive(isVisible);
        public void SetIsVisibleSelectionBorder(bool isVisible) => _selectionBorderObject.SetActive(isVisible);
        public void SetIsVisibleConsumeProgress(bool isVisible) => _progressIconObject.SetActive(isVisible);
        public void SetIsVisibleRepairingProgress(bool isVisible) => _repairingProgressIconObject.SetActive(isVisible);
        public void SetAmountDurability(float amount) => _durabilityAmount.fillAmount = amount;
        public void SetAmountSips(float amount) => _sipsAmount.fillAmount = amount;
        public void SetColorDurability(Color color) => _durabilityAmount.color = color;
        public void SetAmountConsumeProgress(float amount) => _progressIcon.fillAmount = amount;
        public void SetAmountRepairingProgress(float amount) => _repairingProgressIcon.fillAmount = amount;

        //

        public int Id { get; private set; }

        //

        public event Action<HotBarCellView, PointerEventData> OnPointDown_;
        public void OnPointerDown(PointerEventData eventData) => OnPointDown_?.Invoke(this, eventData);

        //

        public void Selection()
        {
            SetIsVisibleSelectionBorder(true);
        }

        public void Deselection()
        {
            SetIsVisibleSelectionBorder(false);
        }

        public void SetAsPassive()
        {
            Id = default;

            SetIsVisibleItemIcon(false);
            SetIsVisibleCounter(false);
            SetIsVisibleDurabitity(false);
            SetIsVisibleSelectionBorder(false);
            SetIsVisibleBroken(false);
        }

        public void SetData(HotBarCellData data)
        {
            Id = data.Id;

            SetIsVisibleSelectionBorder(data.IsVisibleBorder);

            SetIsVisibleItemIcon(data.Icon != null);
            if (data.Icon != null)
            {
                SetItemIcon(data.Icon);

                if (data.IsDisable)
                {
                    SetItemIconColor(ItemIconColorConsumeOther);
                }
                else
                {
                    SetItemIconColor(ItemIconColorDefault);
                }
            }

            SetIsVisibleCounter(data.Count.HasValue);
            if (data.Count.HasValue)
            {
                SetCount(data.Count.Value.ToString());
            }

            SetIsVisibleDurabitity(data.Durability.HasValue);
            SetIsVisibleBroken(false);
            if (data.Durability.HasValue)
            {
                if (data.Durability.Value > 0)
                {
                    SetColorDurability(ItemDurabilityWorkingColor);
                    SetAmountDurability(data.Durability.Value);
                }
                else
                {
                    SetAmountDurability(brokenFillAmount);
                    SetColorDurability(ItemDurabilityBrokenColor);
                    SetIsVisibleBroken(true);
                }
            }

            SetIsVisibleSips(data.Sips.HasValue);
            if (data.Sips.HasValue)
            {
                SetAmountSips(data.Sips.Value);
            }

            SetIsVisibleConsumeProgress(data.ProgressConsumable.HasValue);
            if (data.ProgressConsumable.HasValue)
            {
                SetAmountConsumeProgress(data.ProgressConsumable.Value);
            }

            SetIsVisibleRepairingProgress(data.ProgressRepairing.HasValue);
            if (data.ProgressRepairing.HasValue)
            {
                SetAmountRepairingProgress(data.ProgressRepairing.Value);
            }
        }
    }
}
