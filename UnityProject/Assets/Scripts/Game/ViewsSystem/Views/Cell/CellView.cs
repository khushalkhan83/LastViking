using Core.Views;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UltimateSurvival;

namespace Game.Views
{
    public class CellView : ViewBase
        , IBeginDragHandler
        , IDragHandler
        , IEndDragHandler
        , IPointerDownHandler
        , IPointerUpHandler 
        , IPointerExitHandler 
        , IPointerClickHandler 
    {
        private readonly float brokenFillAmount = 0.05f;

        #region Data
#pragma warning disable 0649

        [ContextMenuItem("__Fill2", "__Fill2")]
        [SerializeField] public Image _background;
        [SerializeField] public Image _itemIcon;

        [VisibleObject]
        [SerializeField] public GameObject _itemIconObject;

        [VisibleObject]
        [SerializeField] public GameObject _durabilityObject;
        [SerializeField] public Image _durabilityAmount;
        [VisibleObject]
        [SerializeField] public GameObject _brokenIconObject;

        [ContextMenuItem("__Fill", "__Fill")]
        [VisibleObject]
        [SerializeField] private GameObject _sipsObject;
        [SerializeField] private Image _sipsAmount;

        [VisibleObject]
        [SerializeField] public GameObject _selectionBorderObject;

        [VisibleObject]
        [SerializeField] public GameObject _counterObject;
        [SerializeField] public Text _counter;

        [VisibleObject]
        [SerializeField] public GameObject _progressIconObject;
        [SerializeField] public Image _progressIcon;

        [VisibleObject]
        [SerializeField] public GameObject _repairingProgressIconObject;
        [SerializeField] public Image _repairingProgressIcon;

        [VisibleObject]
        [SerializeField] public GameObject _midRarityIconObject;
        [VisibleObject]
        [SerializeField] public GameObject _rareRarityIconObject;

        [SerializeField] public Color _colorBackgroundDefault;
        [SerializeField] public Color _colorBackgroundSelection;
        [SerializeField] public Color _colorBackgroundDisable;
        [SerializeField] public Color _colorBackgroundDefaultComponent;

        [SerializeField] private Color _itemIconColorDefault = Color.white;
        [SerializeField] private Color _ItemIconColorCantConsume = new Color (1f, 1f, 1f, 0.5f);

        [SerializeField] private Color _itemDurabilityWorkingColor;
        [SerializeField] private Color _itemDurabilityBrokenColor;

        public void __Fill()
        {
            var sips = transform.Find("Sips");
            _sipsObject = sips.gameObject;
            _sipsAmount = sips.GetChild(0).GetComponent<Image>();
        }

        public void __Fill2()
        {
            var sips = transform.Find("BrokenIcon");
            _brokenIconObject = sips.gameObject;

            var rep = transform.Find("ProgressRepairing");
            _repairingProgressIconObject = rep.gameObject;
            _repairingProgressIcon = rep.GetComponent<Image>();
        }

#pragma warning restore 0649
        #endregion

        public Color ColorBackgroundDefault => _colorBackgroundDefault;
        public Color ColorBackgroundSelection => _colorBackgroundSelection;
        public Color ColorBackgroundDisable => _colorBackgroundDisable;
        public Color ColorBackgroundDefaultComponent => _colorBackgroundDefaultComponent;

        public Color ItemIconColorDefault => _itemIconColorDefault;
        public Color ItemIconColorCantConsume => _ItemIconColorCantConsume;

        public Color ItemDurabilityWorkingColor => _itemDurabilityWorkingColor;
        public Color ItemDurabilityBrokenColor => _itemDurabilityBrokenColor;

        //

        public void SetCount(string value) => _counter.text = value;
        public void SetBackgroundColor(Color value) => _background.color = value;
        public void SetItemIcon(Sprite value) => _itemIcon.sprite = value;
        public void SetItemIconColor(Color color) => _itemIcon.color = color;
        public void SetIsVisibleItemIcon(bool isVisible) => _itemIconObject.SetActive(isVisible);
        public void SetIsVisibleCounter(bool isVisible) => _counterObject.SetActive(isVisible);
        public void SetIsVisibleConsumeProgress(bool isVisible) => _progressIconObject.SetActive(isVisible);
        public void SetIsVisibleRepairingProgress(bool isVisible) => _repairingProgressIconObject.SetActive(isVisible);
        public void SetIsVisibleMidRarityIcon(bool isVisible) => _midRarityIconObject.SetActive(isVisible);
        public void SetIsVisibleRareRarityIcon(bool isVisible) => _rareRarityIconObject.SetActive(isVisible);
        public void SetIsVisibleDurabitity(bool isVisible) => _durabilityObject.SetActive(isVisible);
        public void SetIsVisibleBroken(bool isVisible) => _brokenIconObject.SetActive(isVisible);
        public void SetIsVisibleSips(bool isVisible) => _sipsObject.SetActive(isVisible);
        public void SetIsVisibleSelectionBorder(bool isVisible) => _selectionBorderObject.SetActive(isVisible);
        public void SetAmountDurability(float amount) => _durabilityAmount.fillAmount = amount;
        public void SetAmountSips(float amount) => _sipsAmount.fillAmount = amount;
        public void SetColorDurability(Color color) => _durabilityAmount.color = color;
        public void SetAmountConsumeProgress(float amount) => _progressIcon.fillAmount = amount;
        public void SetAmountRepairingProgress(float amount) => _repairingProgressIcon.fillAmount = amount;

        //

        public int Id { get; private set; }
        public int ContainerId { get; private set; }
        public bool IsComponent { get; private set; }

        //

        public event Action<CellView, CellData> OnDataChanged;


        public event Action<CellView, PointerEventData> OnDragBegin_;
        public void OnBeginDrag(PointerEventData eventData) => OnDragBegin_?.Invoke(this, eventData);

        public event Action<CellView, PointerEventData> OnDrag_;
        public void OnDrag(PointerEventData eventData) => OnDrag_?.Invoke(this, eventData);

        public event Action<CellView, PointerEventData> OnDragEnd_;
        public void OnEndDrag(PointerEventData eventData) => OnDragEnd_?.Invoke(this, eventData);

        public event Action<CellView, PointerEventData> OnPointDown_;
        public void OnPointerDown(PointerEventData eventData) => OnPointDown_?.Invoke(this, eventData);

        public event Action<CellView, PointerEventData> OnPointerUp_;
        public void OnPointerUp(PointerEventData eventData) => OnPointerUp_?.Invoke(this, eventData);
        
        public event Action<CellView, PointerEventData> OnPointerExit_;
        public void OnPointerExit(PointerEventData eventData) => OnPointerExit_?.Invoke(this, eventData);

        public event Action<CellView, PointerEventData> OnPointerClick_;
        public void OnPointerClick(PointerEventData eventData) => OnPointerClick_?.Invoke(this, eventData);
        //

        public void Selection()
        {
            SetIsVisibleSelectionBorder(true);
            SetBackgroundColor(ColorBackgroundSelection);
        }

        public void Deselection(bool isActive)
        {
            SetIsVisibleSelectionBorder(false);

            if (isActive)
            {
                if (IsComponent)
                    SetBackgroundColor(ColorBackgroundDefaultComponent);
                else
                    SetBackgroundColor(ColorBackgroundDefault);
            }
            else
            {
                SetBackgroundColor(ColorBackgroundDisable);
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
            SetBackgroundColor(ColorBackgroundDisable);
            SetIsVisibleBroken(false);
            SetIsVisibleMidRarityIcon(false);
            SetIsVisibleRareRarityIcon(false);
        }

        public void SetData(CellData data)
        {
            Id = data.Id;
            ContainerId = data.ContainerId;
            IsComponent = data.IsComponent;

            SetIsVisibleSelectionBorder(false);

            if (IsComponent)
                SetBackgroundColor(ColorBackgroundDefaultComponent);
            else
                SetBackgroundColor(ColorBackgroundDefault);

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

            OnDataChanged?.Invoke(this, data);
        }
    }
}
