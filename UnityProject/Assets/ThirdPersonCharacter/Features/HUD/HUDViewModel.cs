using System;
using UnityEngine;

namespace Game.ThirdPerson.HUD
{
    public class HUDViewModel : MonoBehaviour
    {
        public bool IsShow { get; private set; } = true;
        public event Action OnShowChanged;

        public void SetShow(bool value)
        {
            IsShow = value;
            OnShowChanged?.Invoke();
        }

        public bool IsShowAimButton { get; private set; }
        public event Action OnShowAimButtonChanged;

        public void SetShowAimButton(bool value)
        {
            IsShowAimButton = value;
            OnShowAimButtonChanged?.Invoke();
        }
    }
}