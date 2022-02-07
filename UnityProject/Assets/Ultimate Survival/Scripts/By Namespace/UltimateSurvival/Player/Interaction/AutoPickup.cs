using System.Collections;
using System.Collections.Generic;
using Coin;
using Game.Models;
using UnityEngine;

namespace UltimateSurvival
{
    public class AutoPickup : MonoBehaviour
    {
        [SerializeField] private CoinObject coinObject = default;
        [SerializeField] private ItemPickup itemPickup = default;
        [SerializeField] private Collider hitBoxCollider = default;

        private bool itemsCollected = false;

        private PlayerEventHandler PlayerEventHandler => ModelsSystem.Instance._playerEventHandler;
        private InventoryOperationsModel InventoryOperationsModel => ModelsSystem.Instance._inventoryOperationsModel;
        private PickUpsModel PickUpsModel => ModelsSystem.Instance._pickUpsModel;

        public void SetColliderActive(bool isActive)
        {
            if(hitBoxCollider != null) hitBoxCollider.enabled = isActive;
        }

        private void Update()
        {
            if(coinObject.canBeCollected && !itemsCollected)
            {
                if(itemPickup.IsHasItem && InventoryOperationsModel.CanAddItemToPlayer(itemPickup.ItemToAdd.ItemData, itemPickup.ItemToAdd.Count))
                {
                    InventoryOperationsModel.TryAddItemsToPlayer(itemPickup.ItemToAdd.ItemData, itemPickup.ItemToAdd.Count);
                    itemsCollected = true;
                    coinObject.StartCollect(PlayerEventHandler.transform);
                }
                else
                {
                    SetColliderActive(true);
                    this.enabled = false;
                }
                itemsCollected = true;
            }

            if(coinObject.isCollected)
            {
                PickUpsModel.PickUp(itemPickup.ItemToAdd.Name, itemPickup.ItemToAdd.Count);
                itemPickup.PickUp();
            }
        }
    }
}