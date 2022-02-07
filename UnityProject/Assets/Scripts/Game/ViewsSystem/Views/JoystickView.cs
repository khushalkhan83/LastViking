using Core.Views;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Views
{
    public class JoystickView : ViewBase,
        IPointerUpHandler,
        IPointerDownHandler,
        IPointerEnterHandler,
        IDragHandler
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private RectTransform _joystick;
        [SerializeField] private RectTransform _pivot;

#pragma warning restore 0649
        #endregion

        public RectTransform Joystick => _joystick;
        public RectTransform Pivot => _pivot;

        //
        public event Action<PointerEventData> OnDownEvent;
        public event Action<PointerEventData> OnUpEvent;
        public event Action<PointerEventData> OnEnterEvent;
        public event Action<PointerEventData> OnDragEvent;

        public void OnPointerDown(PointerEventData eventData) => OnDownEvent?.Invoke(eventData);
        public void OnPointerUp(PointerEventData eventData) => OnUpEvent?.Invoke(eventData);
        public void OnDrag(PointerEventData eventData) => OnDragEvent?.Invoke(eventData);
        public void OnPointerEnter(PointerEventData eventData) => OnEnterEvent?.Invoke(eventData);

    }
}
