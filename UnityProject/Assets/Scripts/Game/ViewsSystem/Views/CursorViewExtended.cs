using Core.Views;
using System;
using UnityEngine.EventSystems;

namespace Game.Views
{
    public class CursorViewExtended : CursorViewBase
        , IPointerDownHandler
    {
        public void OnPointerDown(PointerEventData eventData) => Interact();
    }
}
