using System;
using UnityEngine;

namespace Game.Models
{
    public class ActivitiesLogButtonViewModel : MonoBehaviour
    {
        public bool IsShow { get; private set; } = true;
        public int Counter {get; private set;}
        public event Action OnShowChanged;
        public event Action OnClick;
        public event Action OnCounterChanged;

        public bool ShowCounter => Counter > 1;

        public event Action OnIsPulseAnimationChanged;
        private bool isPulseAnimation;
        public bool IsPulseAnimation
        {
            get => isPulseAnimation;
            private set {isPulseAnimation = value; OnIsPulseAnimationChanged?.Invoke();}
        }

        public void SetShow(bool value)
        {
            IsShow = value;
            OnShowChanged?.Invoke();
        }
        public void Click()
        {
            IsPulseAnimation = false;
            OnClick?.Invoke();
        }

        public void SetCounter(int value)
        {
            if(Counter == value) return;
            Counter = value;
            IsPulseAnimation = true;

            OnCounterChanged?.Invoke();
        }
    }
}