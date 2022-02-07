using System;
using UnityEngine;

namespace Game.Models
{
    public class InventoryRepairingModel : MonoBehaviour
    {
        public event Action OnShowRepairPopup;
        public event Action OnHideRepairPopup;

        public void ShowRepairPopup() => OnShowRepairPopup?.Invoke();
        public void HideRepairPopup() => OnHideRepairPopup?.Invoke();
    }
}
