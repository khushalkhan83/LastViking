using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class BoostButtonWatchView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Text _textFront;
        [SerializeField] private Text _textBack;
        [SerializeField] private Image _maskImage;
        [SerializeField] private Slider _slider;
        [SerializeField] private Button _button;

        [VisibleObject]
        [SerializeField] private GameObject _handlerObject;

#pragma warning restore 0649
        #endregion

        public void SetIsVisibleHandler(bool value) => _handlerObject.SetActive(value);
        public void SetIsInteractable(bool value) => _button.interactable = value;

        public void SetText(string value)
        {
            _textFront.text = value;
            _textBack.text = value;
        }

        public void SetFillAmount(float value)
        {
            _maskImage.fillAmount = value;
            _slider.normalizedValue = value;
        }

        //UI
        public event Action OnClick;
        public void ActionClick() => OnClick?.Invoke();
    }
}
