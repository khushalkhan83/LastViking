using System;
using UnityEngine;

namespace Game.Models
{
    public class DebugTimeModel : MonoBehaviour
    {
        public bool IsGodMode { get; private set; }

        public event Action OnChangeIsGodMode;

        public void SetIsGodMode(bool isGodMode)
        {
            var last = IsGodMode;
            IsGodMode = isGodMode;

            if (IsGodMode != last)
            {
                OnChangeIsGodMode?.Invoke();
            }
        }
    }
}
