using System;
using UnityEngine;

namespace Game.Models
{
    public class PlayerStatusViewModel : MonoBehaviour
    {
        public event Action OnEnableEnviroTime;
        public event Action OnDisableEnviroTime;

        public void EnableEnviroTime() => OnEnableEnviroTime?.Invoke();
        public void DisableEnviroTime() => OnDisableEnviroTime?.Invoke();

        public bool IsShowingAttackDelayView { get; set; }
    }
}
