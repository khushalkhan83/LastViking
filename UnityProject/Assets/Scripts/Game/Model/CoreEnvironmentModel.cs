using System;
using UnityEngine;

namespace Game.Models
{
    public class CoreEnvironmentModel : MonoBehaviour
    {
        public bool WaterActive {get; private set;} = true;

        public event Action OnWaterActiveChanged;
        
        public void SetWaterActive(bool active)
        {
            WaterActive = active;
            OnWaterActiveChanged?.Invoke();
        }
    }
}
