using System;
using UnityEngine;

namespace Game.Models
{
    public class EnvironmentTimeModel : MonoBehaviour
    {
        private bool isDayTime;
        public bool IsDayTime => isDayTime;

        public event Action OnDayTimeChanged;
        
        public void SetDay(bool value)
        {
            bool oldValue = isDayTime;
            isDayTime = value;
            if(oldValue != value)
                OnDayTimeChanged?.Invoke();
        }
    }
}
