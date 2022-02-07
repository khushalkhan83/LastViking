using System;
using UnityEngine;

namespace Game.Models
{
    public class ExpandInventoryModel : MonoBehaviour
    {
        public Transform ButtonContainer { get; private set; }

        public event Action OnHideExpandInventoryButton;
        public event Action OnShowExpandInventoryButton;
        public event Action OnExpandInventory;
        public event Action<bool> OnInventoryExpanded;

        public void SetButtonContainer(Transform container) => ButtonContainer = container;

        public void HideExpandInventoryButton() => OnHideExpandInventoryButton?.Invoke();

        // add argument container ?
        public void ShowExpandInventoryButton() => OnShowExpandInventoryButton?.Invoke();

        public void UpdateExpandInventoryButton()
        {
            HideExpandInventoryButton();
            ShowExpandInventoryButton();
        }

        public void ExpandInventory() => OnExpandInventory?.Invoke();

        // triggered by controller
        public void TriggerExpandedCallback() => OnInventoryExpanded?.Invoke(true);
        public void TriggerNotExpandedCallback() => OnInventoryExpanded?.Invoke(false);
    }
}
