using Coin;
using Core;
using Core.Controllers;
using Game.Models;
using Game.Providers;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers
{
    public class DropItemController : IDropItemController, IController
    {
        [Inject] public DropItemModel DropItemModel { get; private set; }
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public ItemsDB ItemsDB { get; private set; }
        [Inject] public ActionsLogModel ActionsLogModel { get; private set; }
        [Inject] public WorldObjectCreator WorldObjectCreator { get; private set; }
        [Inject] public WorldObjectsProvider WorldObjectsProvider {get; private set;}

        void IController.Enable() 
        {
            DropItemModel.OnItemDrop += OnItemDropped;
            DropItemModel.OnItemDropFloating += OnItemDroppedFloating;
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            DropItemModel.OnItemDrop -= OnItemDropped;
            DropItemModel.OnItemDropFloating -= OnItemDroppedFloating;
        }

        private GameObject OnItemDropped(SavableItem item, Transform dropPosition = null, bool removeAutoDestroyComponent = false)
        {
            var itemData = ItemsDB.GetItem(item.Name);
            ActionsLogModel.SendMessage(new MessageInventoryDroppedData(item.Count, itemData));

            Transform targetT = dropPosition != null ? dropPosition : PlayerEventHandler.transform;

            var itemRootObject = WorldObjectCreator.Create(item.ItemData.WorldObjectID,
                                                       targetT.position + targetT.forward + targetT.up,
                                                       Quaternion.identity);

            ItemPickup itemPickup = itemRootObject.GetComponentInChildren<ItemPickup>();
            itemPickup.SetItemToAdd(item);

            if(removeAutoDestroyComponent)
            {
                RemoveAutoDestroyComponent(itemRootObject.gameObject);
            }

            DropItemModel.ItemDropped(itemRootObject.gameObject);
            return itemRootObject.gameObject;
        }

        private void RemoveAutoDestroyComponent(GameObject go)
        {
            var autoDestroyer = go.GetComponent<DestroyItemTimeDelay>();
            if (autoDestroyer == null)
                "Not handled exeption".Error();
            else
                autoDestroyer.RemoveComponentAndSaveIt();
        }

        private GameObject OnItemDroppedFloating(SavableItem item, Vector3 position, bool autopickup, bool closeToPlayer)
        {
            Vector3 dropPosition = GetDropPosition(position, closeToPlayer);
            var itemRootObject = WorldObjectCreator.Create(WorldObjectID.bag_pickup_floating,
                                                       dropPosition,
                                                       Quaternion.identity);

            ItemPickup itemPickup = itemRootObject.GetComponentInChildren<ItemPickup>();
            itemPickup.SetItemToAdd(item);

            var coinObject = itemRootObject.GetComponent<CoinObject>();
            coinObject.Place(dropPosition, 1f);

            AutoPickup autoPickup = itemRootObject.GetComponentInChildren<AutoPickup>();
            autoPickup.enabled = autopickup;
            autoPickup.SetColliderActive(!autopickup);

            if(item.ItemData.WorldObjectID != WorldObjectID.Bag_Pickup)
            {
                ReplaceView(item.ItemData.WorldObjectID, itemPickup);
            }

            DropItemModel.ItemDroppedFloating(itemRootObject.gameObject);
            return itemRootObject.gameObject;
        }

        private Vector3 GetDropPosition(Vector3 position, bool closeToPlayer)
        {
            if(closeToPlayer)
            {
                position = position + (PlayerEventHandler.transform.position - position).normalized;
                var positionRandom = new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f)) * 0.15f;
                return position + positionRandom;
            }
            else
            {
                var positionRandom = new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f)) * 0.5f;
                return position + positionRandom;
            }
        }

        private void ReplaceView(WorldObjectID worldObjectID, ItemPickup itemPickup)
        {
            itemPickup.IconSprite.gameObject.SetActive(false);
            itemPickup.ViewObject.SetActive(false);

            Transform viewObjectRoot = itemPickup.ViewObject.transform.parent;
            var newObjectView = GameObject.Instantiate(WorldObjectsProvider[worldObjectID], viewObjectRoot);

            itemPickup.SetOutlineRenderers(newObjectView.GetComponentInChildren<ItemPickup>()?.GetRenderers());

            DisableWorldObjectComponents(newObjectView);

            void DisableWorldObjectComponents(WorldObjectModel worldObject)
            {
                var worldObjectController = worldObject.GetComponent<WorldObjectController>();
                if(worldObjectController != null) worldObjectController.enabled = false;

                var rigidbody = worldObject.GetComponent<Rigidbody>();
                if(rigidbody != null) rigidbody.isKinematic  = true;

                var destroyItemTimeDelay = worldObject.GetComponent<DestroyItemTimeDelay>();
                if(destroyItemTimeDelay != null) destroyItemTimeDelay.enabled = false;

                foreach(var collider in worldObject.GetComponentsInChildren<Collider>())
                {
                    collider.enabled = false;
                }
            }
        }
    }
}
