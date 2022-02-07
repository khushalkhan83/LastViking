using System;
using UnityEngine;

namespace Game.Models
{
    public class SettingsViewModel : MonoBehaviour
    {
        public event Action OnResetGame;

        public void ResetGame() => OnResetGame?.Invoke();
    }
}
