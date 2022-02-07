using System;
using UnityEngine;

namespace Game.Models
{
    public class AimModel : MonoBehaviour
    {
        public bool IsActive { get; private set; }
        public event Action OnChangeAim;

        public void SetActive(bool isActive)
        {
            IsActive = isActive;

            OnChangeAim?.Invoke();
        }
    }
}
