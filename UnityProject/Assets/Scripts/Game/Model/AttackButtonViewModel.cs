using System;
using UnityEngine;

namespace Game.Models
{
    public class AttackButtonViewModel : MonoBehaviour
    {
        public event Action OnPulseAnimationChanged;
        public bool PulseAnimation {get; private set;}

        public void SetPulseAnimation(bool pulseAnimation)
        {
            if(PulseAnimation != pulseAnimation)
            {
                PulseAnimation = pulseAnimation;
                OnPulseAnimationChanged?.Invoke();
            }
        }
    }
}
