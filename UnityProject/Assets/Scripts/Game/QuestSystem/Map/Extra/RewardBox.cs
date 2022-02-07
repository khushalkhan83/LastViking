using System.Collections.Generic;
using Game.Models;
using Game.Providers;
using UltimateSurvival;
using UnityEngine;
using UnityEngine.Events;

namespace Game.QuestSystem.Map.Extra
{
    // TODO: move GetItemsByConfig logic to model
    public class RewardBox : MonoBehaviour
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private UnityEvent onRewardRecived;
        [SerializeField] private Transform dropPoint;
        [SerializeField] private LootGroupID lootGroupID;
        #pragma warning restore 0649
        #endregion

        private DropContainerModel DropContainerModel => ModelsSystem.Instance._dropContainerModel;
        private TreasureLootGroupProvider TreasureLootGroupProvider => ModelsSystem.Instance._treasureLootGroupProvider;
        private ItemsDB ItemsDB => ModelsSystem.Instance._itemsDB;
        
        
        public void ReceiveReward()
        {
            var itemsConfigs = TreasureLootGroupProvider[lootGroupID].items;
            var items = GetItemsByConfig(itemsConfigs);

            DropContainerModel.DropContainer(dropPoint.position,0,items);
            onRewardRecived?.Invoke();
        }

        //TODO: move to model. based on TreasureLootController logic
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