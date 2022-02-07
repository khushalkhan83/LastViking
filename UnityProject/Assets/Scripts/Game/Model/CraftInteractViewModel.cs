using System;
using UnityEngine;

namespace Game.Models
{
    public class CraftInteractViewModel : MonoBehaviour
    {
        public event Action OnStartCalculate;
        public event Action OnStopCalculate;
        public event Action OnStartPulse;
        public event Action OnStopPulse;
        public event Action<int> OnChangeGear;

        public bool IsHasChanges { get; private set; }
        public bool IsCalculateProcess { get; private set; }
        public bool IsPulse { get; private set; }

        public void ChangeResourceCount()
        {
            IsHasChanges = true;
            StopCalculateProcess();
            StartCalculateProcess();
        }

        public void StartPulse()
        {
            IsPulse = true;
            OnStartPulse?.Invoke();
            StopCalculateProcess();
        }

        public void Click()
        {
            if (IsPulse)
            {
                IsPulse = false;
                OnStopPulse?.Invoke();
            }
            StartCalculateProcess();
        }

        public void ApplyChanges(int count)
        {
            IsHasChanges = false;
            StopCalculateProcess();
            if (!IsPulse)
            {
                OnChangeGear?.Invoke(count);
            }
        }

        public void StopCalculateProcess()
        {
            if (IsCalculateProcess)
            {
                IsCalculateProcess = false;
                OnStopCalculate?.Invoke();
            }
        }

        private void StartCalculateProcess()
        {
            if (IsHasChanges && !IsCalculateProcess && !IsPulse)
            {
                IsCalculateProcess = true;
                OnStartCalculate?.Invoke();
            }
        }
    }
}
