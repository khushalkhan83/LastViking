using System;
using UnityEngine;

namespace Game.Models
{
    public class FishingLinkViewModel : MonoBehaviour
    {
        public event Action OnClick;
        
        public void Click()
        {
            OnClick?.Invoke();
        }
    }
}