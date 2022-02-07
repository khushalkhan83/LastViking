using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Game.Views
{
    public class TouchpadView : ViewBase,
        IPointerUpHandler,
        IPointerDownHandler,
        IDragHandler,
        IPointerEnterHandler
    {
        [SerializeField] private Image _attackImage;

        [SerializeField] private RectTransform _holdDownRect;

        public RectTransform HoldDownRect => _holdDownRect;

        public void SetVisibleAttackImage(bool on) => _attackImage.gameObject.SetActive(on);

        public event Action<PointerEventData> OnDownEvent;
        public event Action<PointerEventData> OnUpEvent;
        public event Action<PointerEventData> OnDragEvent;
        public event Action<PointerEventData> OnEnterEvent;

        public void OnPointerEnter(PointerEventData eventData) => OnEnterEvent?.Invoke(eventData);
        public void OnDrag(PointerEventData eventData) => OnDragEvent?.Invoke(eventData);
        public void OnPointerDown(PointerEventData eventData) => OnDownEvent?.Invoke(eventData);
        public void OnPointerUp(PointerEventData eventData) => OnUpEvent?.Invoke(eventData);
    }
}
