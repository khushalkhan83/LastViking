using System.Collections;
using System.Collections.Generic;
using Game.Models;
using UnityEngine;
using UnityEngine.Events;

namespace Game.QuestSystem.Map.Extra
{
    public class PlayerReciveItemEvent : MonoBehaviour
    {
        [SerializeField] private int requiredItemId;
        [SerializeField] private UnityEvent onItemRecived;

        InventoryModel InventoryModel => ModelsSystem.Instance._inventoryModel;
        HotBarModel HotBarModel => ModelsSystem.Instance._hotBarModel;

        private void OnEnable() 
        {
            InventoryModel.ItemsContainer.OnAddItems += OnAddItemsToInventoryHandler;
            HotBarModel.ItemsContainer.OnAddItems += OnAddItemsToInventoryHandler;
        }

        private void OnDisable() 
        {
            InventoryModel.ItemsContainer.OnAddItems -= OnAddItemsToInventoryHandler;
            HotBarModel.ItemsContainer.OnAddItems -= OnAddItemsToInventoryHandler;
        }

        private void OnAddItemsToInventoryHandler(int itemId, int count)
        {
            if(this.requiredItemId == itemId)
            {
                onItemRecived?.Invoke();
            }
        }
    }
}
