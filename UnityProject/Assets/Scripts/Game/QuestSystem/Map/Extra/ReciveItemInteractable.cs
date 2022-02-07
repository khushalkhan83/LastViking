using System.Collections;
using System.Collections.Generic;
using Game.Interactables;
using Game.Models;
using Game.QuestSystem.Map.Extra;
using UltimateSurvival;
using UnityEngine;
using UnityEngine.Events;

namespace Game.QuestSystem.Map.Extra
{
    public class ReciveItemInteractable : ItemsRelatedInteractableBase
    {
        [SerializeField] private UnityEvent _onItemsPlaced;
        private InventoryOperationsModel InventoryOperationsModel => ModelsSystem.Instance._inventoryOperationsModel;
        private InventoryViewModel InventoryViewModel => ModelsSystem.Instance._inventoryViewModel;
        private FullInventoryModel FullInventoryModel => ModelsSystem.Instance._fullInventoryModel;
        
        public override SavableItem[] RequiredItems => null;

        public override bool CanUse() => true;

        public override void Use()
        {
            if(InventoryOperationsModel.GetEmptyCellsCount() > 0)
            {
                _onItemsPlaced?.Invoke();
            }
            else if(!InventoryViewModel.IsMaxExpandLevel)
            {
                FullInventoryModel.ShowFullPopup();
            }
        }
    }
}
