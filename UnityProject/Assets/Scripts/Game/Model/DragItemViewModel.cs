using System;
using UnityEngine;

namespace Game.Models
{
    public class DragItemViewModel : MonoBehaviour
    {
        public enum AnimationType {Drag, Click}

        public AnimationType Animation {get; private set;}

        public bool IsShow { get; private set; }
        public event Action OnShowChanged;

        public void SetShow(bool value)
        {
            IsShow = value;
            OnShowChanged?.Invoke();
        }

        public Transform From {get; private set;}
        public Transform To {get; private set;}
        public Transform ClickPosition {get; private set;}

        public event Action OnDataChanged;

        public void SetDragData(Transform from, Transform to)
        {
            From = from;
            To = to;
            Animation = AnimationType.Drag;
            OnDataChanged?.Invoke();
        }

        public void SetClickData(Transform position = null)
        {
            ClickPosition = position;
            Animation = AnimationType.Click;
            OnDataChanged?.Invoke();
        }
    }
}