using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Game.Models
{
    public class MiniGameStateModel : MonoBehaviour
    {
        private bool _isMinigame = false;

        public bool IsMinigame
        {
            get { return _isMinigame; }
            set
            {
                if (_isMinigame!=value)
                {
                    _isMinigame = value;
                    OnStateChange?.Invoke(_isMinigame);
                }
            }
        }

        public event Action<bool> OnStateChange;
    }
}