using Core.Views;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Views
{
    public class RotateButtonView : ViewBase
        , IPointerDownHandler
        , IPointerUpHandler
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Image _image;

#pragma warning restore 0649
        #endregion

        public void SetIcon(Sprite sprite) => _image.sprite = sprite;

        public event Action OnPointerDown_;
        public void OnPointerDown(PointerEventData eventData) => OnPointerDown_?.Invoke();

        public event Action OnPointerUp_;
        public void OnPointerUp(PointerEventData eventData) => OnPointerUp_?.Invoke();
    }
}