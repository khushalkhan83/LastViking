using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Game.Views
{
    public class TreasureLootPopupView : ViewBase
    {
        public event Action OnTakeAll;
        public event Action OnTakeVideo;
        public event Action OnTakeGold;
        public event Action OnClose;
        public event Action<int> OnRespin;

        #region Data
#pragma warning disable 0649
        [SerializeField] private List<RouletteSlotView> _slots;
        [SerializeField] private Transform _slotsHolder;
        [SerializeField] private GameObject _slotPref;
        [SerializeField] private Text _doubleGoldPrice;
        [SerializeField] private Text _takeButtonText;
        [SerializeField] private Text _takeX2VideoText;
        [SerializeField] private Text _takeX2GoldText;

        [VisibleObject] [SerializeField] private GameObject _videoButton;
        [VisibleObject] [SerializeField] private GameObject _goldButton;

        [VisibleObject] [SerializeField] private GameObject _closeButton;

#pragma warning restore 0649
        #endregion
        public List<RouletteSlotView> Slots => _slots;

        public void SetIsVisibleVideoButton(bool isVisible) => _videoButton.SetActive(isVisible);

        public void SetIsVisibleGoldButton(bool isVisible) => _goldButton.SetActive(isVisible);

        public void SetIsVisibleCloseButton(bool isVisible) => _closeButton.SetActive(isVisible);

        public void SetDoubleGoldPrice(int price) => _doubleGoldPrice.text = price.ToString();

        public void SetRespinGoldPrice(int price) 
        {
            foreach (var slot in _slots) 
            {
                slot.SetRespinGoldPrice(price);
            }
        }

        public void SetTakeButtonText(string text) => _takeButtonText.text = text;

        public void SetTakeX2ButtonText(string text)
        {
            _takeX2VideoText.text = text;
            _takeX2GoldText.text = text;
        }

        public RouletteSlotView AddSlot() {
            RouletteSlotView slot = Instantiate(_slotPref, _slotsHolder).GetComponent<RouletteSlotView>();
            Slots.Add(slot);
            slot.Click += OnSlotClick;
            return slot;
        }

        public void ActionTakeAll() => OnTakeAll?.Invoke();

        public void ActionTakeVideo() => OnTakeVideo?.Invoke();

        public void ActionTakeGold() => OnTakeGold?.Invoke();

        public void ActionClose() => OnClose?.Invoke();

        private void OnSlotClick(RouletteSlotView slot) 
        {
            int slotIndex = Slots.FindIndex(s => s == slot);
            if (OnRespin != null) {
                OnRespin(slotIndex);
            }
        }
    }

}