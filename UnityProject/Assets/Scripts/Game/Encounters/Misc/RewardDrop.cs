using System.Collections.Generic;
using Game.Models;
using Game.Providers;
using UltimateSurvival;
using UnityEngine;

namespace Encounters
{
    public class RewardDrop : IRewardDrop
    {
        private readonly LootGroupID lootGroupID;

        public RewardDrop(LootGroupID lootGroupID)
        {
            this.lootGroupID = lootGroupID;
        }

        private DropContainerModel DropContainerModel {get => ModelsSystem.Instance._dropContainerModel;}
        private TreasureLootGroupProvider TreasureLootGroupProvider {get => ModelsSystem.Instance._treasureLootGroupProvider;}
        private ItemsDB ItemsDB {get => ModelsSystem.Instance._itemsDB;}
        private PlayerEventHandler PlayerEventHandler {get => ModelsSystem.Instance._playerEventHandler;}


        public void Drop(Vector3 dropPoint)
        {
            if(lootGroupID == LootGroupID.None) return;

            List<SavableItem> items = GetItems();

            DropContainerModel.DropContainer(dropPoint, 0, items);
        }

        public void TestDrop()
        {
            if(lootGroupID == LootGroupID.None) return;
            
            List<SavableItem> items = GetItems();
            DropContainerModel.DropContainer(PlayerEventHandler.transform.position,0,items);
        }

        private List<SavableItem> GetItems()
        {
            var itemsConfigs = TreasureLootGroupProvider[lootGroupID].items;
            var items = GetItemsByConfig(itemsConfigs);
            return items;
        }

        //TODO: move to model. based on TreasureLootController logic. Copy paste in 3 places
        private List<SavableItem> GetItemsByConfig(List<ItemConfig> itemConfigs)
        {
            List<SavableItem> items = new List<SavableItem>();

            for (int i = 0; i < itemConfigs.Count; i++)
            {
                var itemConfig = itemConfigs[UnityEngine.Random.Range(0, itemConfigs.Count)];
                itemConfigs.Remove(itemConfig);

                ItemData itemData;
                if (ItemsDB.ItemDatabase.FindItemByName(itemConfig.name, out itemData))
                {
                    int itemCount = UnityEngine.Random.Range(itemConfig.countMin, itemConfig.countMax + 1);
                    items.Add(new SavableItem(itemData, itemCount));
                }
            }
            return items;
        }
    }
}