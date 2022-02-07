using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class DividePopupView : ViewBase
    {

        #region Data
#pragma warning disable 0649

        [SerializeField] private Text _countLeft;
        [SerializeField] private Text _countrigth;
        [SerializeField] private Image _iconLeft;
        [SerializeField] private Image _iconRigth;

        [SerializeField] private Slider _slider;

        //Localization text targets
        [SerializeField] private Text _title;
        [SerializeField] private Text _applyButtonText;


#pragma warning restore 0649
        #endregion

        public Slider Slider => _slider;
        public void SetCountLeft(string value) => _countLeft.text = value;
        public void SetCountRigth(string value) => _countrigth.text = value;
        public void SetIconLeft(Sprite value) => _iconLeft.sprite = value;
        public void SetIconRigth(Sprite value) => _iconRigth.sprite = value;
        public void SetValue(int value) => _slider.value = value;

        //

        public void SetCountItems(int value) => _slider.maxValue = value - 1;

        //

        public int ValueLeft => (int)_slider.value;

        public int ValueRigth => (int)(_slider.maxValue + 1 - _slider.value);

        public void SetIconItem(Sprite value)
        {
            SetIconLeft(value);
            SetIconRigth(value);
        }

        //UI
        public event Action OnApply;
        public void Apply() => OnApply?.Invoke();

        public event Action OnClose;
        public void Close() => OnClose?.Invoke();

        //UI
        public void ValueChange()
        {
            SetCountLeft(ValueLeft.ToString());
            SetCountRigth(ValueRigth.ToString());
        }

        public void SetTextTitle(string text) => _title.text = text;

        public void SetTextApplyButton(string text) => _applyButtonText.text = text;
    }
}
