using Core.Views;
using System;
using UnityEngine.EventSystems;

namespace Game.Views
{
    public class PickUpStuffCursorView : ViewBase
        , IPointerDownHandler
    {
        public event Action OnDown;
        public void OnPointerDown(PointerEventData eventData) => OnDown?.Invoke();
    }
}
