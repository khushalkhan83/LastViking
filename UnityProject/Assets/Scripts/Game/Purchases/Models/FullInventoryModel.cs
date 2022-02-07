using System;
using UnityEngine;

namespace Game.Models
{
    public class FullInventoryModel : MonoBehaviour
    {
        public event Action OnShowFullPopup;
        public event Action OnHideFullPopup;

        public void ShowFullPopup() => OnShowFullPopup?.Invoke();
        public void HideFullPopup() => OnHideFullPopup?.Invoke();
    }
}
