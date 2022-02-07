using Core;
using Core.Controllers;
using Game.Models;
using System.Collections.Generic;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers
{
    public class DropContainerController : IDropContainerController, IController
    {
        [Inject] public DropContainerModel DropContainerModel {get;private set;}
        [Inject] public WorldObjectCreator WorldObjectCreator { get; private set; }
        [Inject] public InventoryOperationsModel InventoryOperationsModel { get; private set; }

        void IController.Enable() 
        {
            DropContainerModel.OnDropContainer += OnDropContainer;
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            DropContainerModel.OnDropContainer -= OnDropContainer;
        }

        private List<LootObject> OnDropContainer(Vector3 point, float randomizePosition, List<SavableItem> items, bool bigChest)
        {
            List<LootObject> lootObjects= new List<LootObject>();
            List<SavableItem> extraItems = new List<SavableItem>();
            foreach(var item in items)
            {
                // TODO: move category names to enums (do it in InventoryOperationController too)
                if(item.ItemData.Category == "Currency" || item.ItemData.Category == "Event")
                {
                    extraItems.Add(item);
                }
            }

            foreach(var item in extraItems)
            {
                items.Remove(item);
            }

            InventoryOperationsModel.TryAddItemsToPlayer(extraItems);

            int i = 0;
            do
            {
                int j = 0;
                var worldObjectID = bigChest ? WorldObjectID.drop_container_big : WorldObjectID.drop_container;
                var dropContainer = WorldObjectCreator.Create(worldObjectID, point, Quaternion.identity);
                var lootObject = dropContainer.GetComponentInChildren<LootObject>();
                lootObjects.Add(lootObject);
                var itemsContainer = lootObject.ItemsContainer;

                lootObject.LoadData();

                if(lootObject.CountOpenStart == 0)
                {
                    "lootObject.CountOpenStart is 0".Error();
                    break;
                }

                for(; i < items.Count && j < lootObject.CountOpenStart;)
                {

                    //Place last item in last cell. Last cell showed in InventoryLootView as first
                    if(i == items.Count - 1 && j != lootObject.CountOpenStart - 1)
                    {
                        itemsContainer.AddCells(lootObject.CountOpenStart - j - 1);
                    }

                    itemsContainer.AddCell(items[i]);
                    i++;
                    j++;
                }

                dropContainer.GetComponentInChildren<DropContainerObject>().Place(point, randomizePosition);

            }while(i < items.Count);

            return lootObjects;
        }

    }
}
