using System;
using UnityEngine;

namespace Game.Models
{
    public class InventoryButtonViewModel : MonoBehaviour
    {
        public event Func<GameObject> OnGetButton;
        public GameObject GetButton()
        {
            return OnGetButton?.Invoke();
        }

        public event Action OnPulseAnimationChanged;
        public bool PulseAnimation {get; private set;}

        public void SetPulseAnimation(bool pulseAnimation)
        {
            PulseAnimation = pulseAnimation;
            OnPulseAnimationChanged?.Invoke();
        }
    }
}