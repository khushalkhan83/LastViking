using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Views
{
    public class CookCellView : MonoBehaviour
        , IBeginDragHandler
        , IDragHandler
        , IEndDragHandler
        , IPointerDownHandler
        , IPointerUpHandler
    {
        private readonly float brokenFillAmount = 0.05f;

        #region Data
#pragma warning disable 0649

        [SerializeField] private Image _background;
        [SerializeField] private Image _itemIcon;

        [VisibleObject]
        [SerializeField] private GameObject _itemIconObject;

        [VisibleObject]
        [SerializeField] private GameObject _durabilityObject;
        [SerializeField] private Image _durabilityAmount;
        [VisibleObject]
        [SerializeField] public GameObject _brokenIconObject;

        [ContextMenuItem("__Fill", "__Fill")]
        [VisibleObject]
        [SerializeField] private GameObject _sipsObject;
        [SerializeField] private Image _sipsAmount;

        [VisibleObject]
        [SerializeField] private GameObject _selectionBorderObject;

        [VisibleObject]
        [SerializeField] private GameObject _counterObject;
        [SerializeField] private Text _counter;

        [SerializeField] private Color _colorBackgroundDefault;
        [SerializeField] private Color _colorBackgroundSelection;
        [SerializeField] private Color _colorBackgroundDisable;

        [SerializeField] private Color _itemIconColorDefault = Color.white;
        [SerializeField] private Color _ItemIconColorCantConsume = new Color(1f, 1f, 1f, 0.5f);

        [VisibleObject]
        [SerializeField] private GameObject _processObject;
        [SerializeField] private Image _processAmount;

        [SerializeField] private Color _colorProcessDefault;
        [SerializeField] private Color _colorProcessBoost;
        
        [VisibleObject]
        [SerializeField] public GameObject _progressConsumeIconObject;
        [SerializeField] public Image _progressConsumeIcon;

        [VisibleObject]
        [SerializeField] public GameObject _repairingProgressIconObject;
        [SerializeField] public Image _repairingProgressIcon;

        [SerializeField] private Color _itemDurabilityWorkingColor;
        [SerializeField] private Color _itemDurabilityBrokenColor;

        public void __Fill()
        {
            var sips = transform.Find("Sips");
            _sipsObject = sips.gameObject;
            _sipsAmount = sips.GetChild(0).GetComponent<Image>();
        }
#pragma warning restore 0649
        #endregion

        public Color ColorBackgroundDefault => _colorBackgroundDefault;
        public Color ColorBackgroundSelection => _colorBackgroundSelection;
        public Color ColorBackgroundDisable => _colorBackgroundDisable;
        public Color ColorProcessDefault => _colorProcessDefault;
        public Color ColorProcessBoost => _colorProcessBoost;
        public Color ItemIconColorDefault => _itemIconColorDefault;
        public Color ItemIconColorCantConsume => _ItemIconColorCantConsume;

        public Color ItemDurabilityWorkingColor => _itemDurabilityWorkingColor;
        public Color ItemDurabilityBrokenColor => _itemDurabilityBrokenColor;

        //

        public void SetCount(string text) => _counter.text = text;
        public void SetColorBackground(Color color) => _background.color = color;
        public void SetItemIcon(Sprite sprite) => _itemIcon.sprite = sprite;
        public void SetItemIconColor(Color color) => _itemIcon.color = color;
        public void SetIsVisibleItemIcon(bool isVisible) => _itemIconObject.SetActive(isVisible);
        public void SetIsVisibleCounter(bool isVisible) => _counterObject.SetActive(isVisible);
        public void SetIsVisibleDurabitity(bool isVisible) => _durabilityObject.SetActive(isVisible);
        public void SetAmountDurability(float amount) => _durabilityAmount.fillAmount = amount;
        public void SetIsVisibleSelectionBorder(bool isVisible) => _selectionBorderObject.SetActive(isVisible);
        public void SetIsVisibleConsumeProgress(bool isVisible) => _progressConsumeIconObject.SetActive(isVisible);
        public void SetIsVisibleSips(bool isVisible) => _sipsObject.SetActive(isVisible);
        public void SetIsVisibleProcess(bool isVisible) => _processObject.SetActive(isVisible);
        public void SetAmountConsumeProgress(float amount) => _progressConsumeIcon.fillAmount = amount;
        public void SetAmountProcess(float amount) => _processAmount.fillAmount = 1 - amount;
        public void SetAmountSips(float amount) => _sipsAmount.fillAmount = amount;
        public void SetColorProcess(Color color) => _processAmount.color = color;
        public void SetIsVisibleBroken(bool isVisible) => _brokenIconObject.SetActive(isVisible);
        public void SetIsVisibleRepairingProgress(bool isVisible) => _repairingProgressIconObject.SetActive(isVisible);
        public void SetAmountRepairingProgress(float amount) => _repairingProgressIcon.fillAmount = amount;
        public void SetColorDurability(Color color) => _durabilityAmount.color = color;

        //

        public int Id { get; private set; }
        public int ContainerId { get; private set; }

        //

        public event Action<CookCellView, PointerEventData> OnDragBegin_;
        public void OnBeginDrag(PointerEventData eventData) => OnDragBegin_?.Invoke(this, eventData);

        public event Action<CookCellView, PointerEventData> OnDrag_;
        public void OnDrag(PointerEventData eventData) => OnDrag_?.Invoke(this, eventData);

        public event Action<CookCellView, PointerEventData> OnDragEnd_;
        public void OnEndDrag(PointerEventData eventData) => OnDragEnd_?.Invoke(this, eventData);

        public event Action<CookCellView, PointerEventData> OnPointDown_;
        public void OnPointerDown(PointerEventData eventData) => OnPointDown_?.Invoke(this, eventData);

        public event Action<CookCellView, PointerEventData> OnPointUp_;
        public void OnPointerUp(PointerEventData eventData) => OnPointUp_?.Invoke(this, eventData);

        //

        public void Selection()
        {
            SetIsVisibleSelectionBorder(true);
            SetColorBackground(ColorBackgroundSelection);
        }

        public void Deselection(bool isActive)
        {
            SetIsVisibleSelectionBorder(false);

            if (isActive)
            {
                SetColorBackground(ColorBackgroundDefault);
            }
            else
            {
                SetColorBackground(ColorBackgroundDisable);
            }
        }

        public void SetAsPassive()
        {
            Id = default;
            ContainerId = default;

            SetIsVisibleItemIcon(false);
            SetIsVisibleCounter(false);
            SetIsVisibleDurabitity(false);
            SetIsVisibleSelectionBorder(false);
            SetColorBackground(ColorBackgroundDisable);
            SetIsVisibleBroken(false);
        }

        public void SetData(CookCellData data)
        {
            Id = data.Id;
            ContainerId = data.ContainerId;

            SetIsVisibleSelectionBorder(false);

            SetColorBackground(ColorBackgroundDefault);

            SetIsVisibleItemIcon(data.Icon != null);
            if (data.Icon != null)
            {
                SetItemIcon(data.Icon);

                if (data.IsDisable)
                {
                    SetItemIconColor(ItemIconColorCantConsume);
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

            SetIsVisibleProcess(data.ProcessAmount.HasValue);
            if (data.ProcessAmount.HasValue)
            {
                if (data.IsBoost)
                {
                    SetColorProcess(ColorProcessBoost);
                }
                else
                {
                    SetColorProcess(ColorProcessDefault);
                }
                SetAmountProcess(data.ProcessAmount.Value);
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
