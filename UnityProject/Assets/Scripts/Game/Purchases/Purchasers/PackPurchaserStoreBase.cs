using System.Collections.Generic;
using Game.Models;
using UltimateSurvival;
using UnityEngine;
using static Game.Models.InventoryOperationsModel;
using static Game.Models.StarterPackModel;

namespace Game.Purchases.Purchasers
{
    public abstract class PackPurchaserStoreBase : PurchaserStoreBase
    {
        protected StarterPackModel StarterPackModel => ModelsSystem.Instance._starterPackModel;
        private ItemsDB ItemsDB => ModelsSystem.Instance._itemsDB;
        private DropContainerModel DropContainerModel => ModelsSystem.Instance._dropContainerModel;
        private PlayerEventHandler PlayerEventHandler => ModelsSystem.Instance._playerEventHandler;

        protected abstract ItemSettings[] Items {get;}
        
        protected override void AddReward()
        {
            var packItems = GetSavableItems(Items);
            DropItems(packItems);

            StarterPackModel.BuyPack();
        }

        private List<SavableItem> GetSavableItems(ItemSettings[] itemSettings)
        {
            List<SavableItem> items = new List<SavableItem>();
            foreach (var item in itemSettings)
            {
                var data = ItemsDB.GetItem(item.ItemName);
                var count = item.Count;

                while (count > data.StackSize)
                {
                    items.Add(new SavableItem(data, data.StackSize));
                    count -= data.StackSize;
                }
                items.Add(new SavableItem(data, count));
            }

            return items;
        }

        #region Give items to player
       
        private void DropItems(List<SavableItem> itmes)
        {
            Vector3 dropPoint = PlayerEventHandler.transform.position + PlayerEventHandler.transform.forward * 0.6f;
            DropContainerModel.DropContainer(dropPoint, 0.1f, itmes);
        }

        #endregion
    }
}
