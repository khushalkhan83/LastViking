using Core.Views;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace Game.Views
{
    public class FishingPullView : ViewBase
        , IPointerDownHandler,IPointerUpHandler
    {
#pragma warning disable 0649
        [SerializeField] Image _colorIndicator;
        [SerializeField] Image _buttonImage;
        [SerializeField] UI_FishingSlider _slider;
        [SerializeField] Text _hintText;
        [SerializeField] Color _normalButtonColor;
        [SerializeField] Color _pressedButtonColor;
#pragma warning restore 0649

        public void SetTension(float tension)
        {
            _colorIndicator.color = Color.Lerp(Color.green, Color.red, tension);
            _slider.SetTension( tension);
        }

        public void SetHint(string hint)
        {
            _hintText.text = hint;
        }

        public event Action<bool> OnPointer;
        public void OnPointerDown(PointerEventData eventData) => OnPointer?.Invoke(true);
        public void OnPointerUp(PointerEventData eventData) => OnPointer?.Invoke(false);
        public void SetButtonPressed(bool pressed) => _buttonImage.color = pressed ? _pressedButtonColor : _normalButtonColor;
    }
}