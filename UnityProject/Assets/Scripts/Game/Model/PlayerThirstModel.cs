using System;
using UnityEngine;

namespace Game.Models
{
    public class PlayerThirstModel : MonoBehaviour
    {
        public event Action OnChangeThirstProcess;

        public bool IsThirstProcess { get; private set; }

        public void BeginThirst()
        {
            if (!IsThirstProcess)
            {
                IsThirstProcess = true;
                OnChangeThirstProcess?.Invoke();
            }
        }

        public void EndThirst()
        {
            if (IsThirstProcess)
            {
                IsThirstProcess = false;
                OnChangeThirstProcess?.Invoke();
            }
        }
    }
}
