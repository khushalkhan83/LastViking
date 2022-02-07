using System;
using UnityEngine;

namespace Game.Models
{
    public class DiscordButtonViewModel : MonoBehaviour
    {
        public event Action OnClick;
        
        public void Click()
        {
            OnClick?.Invoke();
        }
    }
}
