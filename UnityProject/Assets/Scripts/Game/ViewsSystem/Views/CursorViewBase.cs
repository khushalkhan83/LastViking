using Core.Views;
using System;
using UnityEngine.EventSystems;

namespace Game.Views
{
    public class CursorViewBase : ViewBase
    {
        public event Action OnInteract;
        public event Action OnInteractAlternative;
        public void Interact() => OnInteract?.Invoke();
        public void InteractAlternative() => OnInteractAlternative?.Invoke();
    }
}
