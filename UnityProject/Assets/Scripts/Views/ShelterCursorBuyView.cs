using Core.Views;
using System;
using UnityEngine.EventSystems;

namespace Game.Views
{
    public class ShelterCursorBuyView : ViewBase
        , IPointerDownHandler
    {

        public event Action OnDownPoint;
        public void OnPointerDown(PointerEventData eventData) => OnDownPoint?.Invoke();
    }
}
