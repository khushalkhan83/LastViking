using System;
using UnityEngine;

namespace Game.Models
{
    public class EncountersViewModel : MonoBehaviour
    {
        public bool IsShow { get; private set; } = false;
        public event Action OnShowChanged;

        public void SetShow(bool value)
        {
            IsShow = value;
            OnShowChanged?.Invoke();
        }

        public void Switch() => SetShow(!IsShow);


        public event Action<string,bool> OnPresentEventMessage;
        public event Action OnRefresh;
        
        public void PresentEventMessage(string message, bool isPositive) => OnPresentEventMessage?.Invoke(message,isPositive);
        public void Refresh() => OnRefresh?.Invoke();
    }
}
