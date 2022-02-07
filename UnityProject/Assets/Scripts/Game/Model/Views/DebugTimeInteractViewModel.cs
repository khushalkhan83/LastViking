using System;
using UnityEngine;

namespace Game.Models
{
    public class DebugTimeInteractViewModel : MonoBehaviour
    {
        public event Action OnChangeIsCanVisibleDebug;

        public bool IsCanVisibleDebug { get; private set; }

        public void ShowDebug()
        {
            if (!IsCanVisibleDebug)
            {
                IsCanVisibleDebug = true;
                OnChangeIsCanVisibleDebug?.Invoke();
            }
        }

        public void HideDebug()
        {
            if (IsCanVisibleDebug)
            {
                IsCanVisibleDebug = false;
                OnChangeIsCanVisibleDebug?.Invoke();
            }
        }
    }
}
