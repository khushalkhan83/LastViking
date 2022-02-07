using System;
using System.Collections.Generic;
using UltimateSurvival;
using UnityEngine;
using static UltimateSurvival.FPToolRestore;

namespace Game.Models
{
    public class InventoryOperationsModel : MonoBehaviour
    {
        public class AddItemsResult
        {
            public readonly bool allAdded;
            public readonly IEnumerable<SavableItem> notAddedItems;

            public AddItemsResult(bool allAdded, IEnumerable<SavableItem> notAddedItems)
            {
                this.allAdded = allAdded;
                this.notAddedItems = notAddedItems;
            }
        }

        public class ItemInfo
        {
            public readonly string name;
            public readonly int count;
            public readonly AddedItemDestinationPriority priority;

            public ItemInfo(string name, int count)
            {
                this.name = name;
                this.count = count;
                this.priority = DefaultItemDestinationPriority;
            }
            public ItemInfo(string name, int count, AddedItemDestinationPriority priority)
            {
                this.name = name;
                this.count = count;
                this.priority = priority;
            }
        }

        public enum AddedItemDestinationPriority
        {
            Inventory,
            HotBar
        }

        #region Const

        private const AddedItemDestinationPriority DefaultItemDestinationPriority = AddedItemDestinationPriority.Inventory;

        #endregion

        #region Properties

        private ItemsDB ItemsDB => ModelsSystem.Instance._itemsDB;
            
        #endregion


        public event Action<string,int,Transform,ItemConfig> OnAddItemToPlayer;    
        public event Func<IEnumerable<ItemInfo>,ItemConfig,AddItemsResult> OnTryAddItemsToPlayer;

        public event Action<ItemData,int> OnRemoveItemFromPlayer;
        public event Action<IEnumerable<ItemInfo>> OnRemoveItemsFromPlayer;

        public event Func<int,int,bool,bool> OnPlayerHasItems;
        public event Func<int,int> OnGetItemCount;

        public event Func<ItemData,bool> OnTryRemoveItemFromPlayer;
        public event Func<int> OnGetEmptyCellsCount;
        public event Func<ItemData, int, bool> OnCanAddItemToPlayer;
        public event Action<string> OnTryEquipItem;


        public class ItemConfig
        {
            public readonly Dictionary<string,object> modifiedProperties;
            public readonly AddedItemDestinationPriority priority;
            public readonly bool belongsToPlayer;
            public readonly bool displayAsExtraItem;

            public ItemConfig(Dictionary<string,object> modifiedProperties, AddedItemDestinationPriority priority = DefaultItemDestinationPriority, bool belongsToPlayer = false, bool displayAsExtraItem = false)
            {
                this.modifiedProperties = modifiedProperties;
                this.priority = priority;
                this.belongsToPlayer = belongsToPlayer;
                this.displayAsExtraItem = displayAsExtraItem;
            }
            public ItemConfig()
            {
                this.modifiedProperties = null;
                this.priority = DefaultItemDestinationPriority;
                this.belongsToPlayer = false;
                this.displayAsExtraItem = false;
            }

            public ItemConfig(bool displayAsExtraItem)
            {
                this.modifiedProperties = null;
                this.priority = DefaultItemDestinationPriority;
                this.belongsToPlayer = false;
                this.displayAsExtraItem = displayAsExtraItem;
            }
        }

        public void AddItemToPlayer(string name, int count = 1, Transform dropPosition = null, ItemConfig config = null)
        {
            if(config == null) config = new ItemConfig();
            OnAddItemToPlayer?.Invoke(name,count,dropPosition,config);
        }

        public void AddItemsToPlayer(IEnumerable<SavableItem> items,Transform dropPosition = null,  ItemConfig config = null)
        {
            if(config == null) config = new ItemConfig();
            foreach (var item in items)
            {
                OnAddItemToPlayer?.Invoke(item.Name,item.Count,dropPosition,config);
            }
        }
        public void AddItemsToPlayer(IEnumerable<RequiredItem> items,Transform dropPosition = null,  ItemConfig config = null)
        {
            if(config == null) config = new ItemConfig();
            foreach (var item in items)
            {
                OnAddItemToPlayer?.Invoke(item.Name,item.Amount,dropPosition,config);
            }
        }
        public void AddItemsToPlayer(IEnumerable<ItemInfo> items,Transform dropPosition = null,  ItemConfig config = null)
        {
            if(config == null) config = new ItemConfig();
            foreach (var item in items)
            {
                OnAddItemToPlayer?.Invoke(item.name,item.count,dropPosition,config);
            }
        }

        public AddItemsResult TryAddItemsToPlayer(ItemData itemData, int count, ItemConfig config = null)
        {
            if(config == null) config = new ItemConfig();
            List<ItemInfo> itemsInfo = new List<ItemInfo>();
            itemsInfo.Add(new ItemInfo(itemData.Name,count));

            var answer = OnTryAddItemsToPlayer.Invoke(itemsInfo,config);
            return answer;
        }
        public AddItemsResult TryAddItemsToPlayer(IEnumerable<ItemInfo> items, ItemConfig config = null)
        {
            if(config == null) config = new ItemConfig();
            var answer = OnTryAddItemsToPlayer.Invoke(items,config);
            return answer;
        }
        public AddItemsResult TryAddItemsToPlayer(IEnumerable<SavableItem> items, ItemConfig config = null)
        {
            if(config == null) config = new ItemConfig();
            List<ItemInfo> itemsInfo = new List<ItemInfo>();

            foreach (var item in items)
            {
                itemsInfo.Add(new ItemInfo(item.Name,item.Count,config.priority));
            }

            var answer = OnTryAddItemsToPlayer.Invoke(itemsInfo,config);
            return answer;
        }

        public void RemoveItemsFromPlayer(IEnumerable<RequiredItem> requiredItems)
        {
            List<ItemInfo> items = new List<ItemInfo>();

            foreach (var item in requiredItems)
            {
                items.Add(new ItemInfo(item.Name,item.Amount));
            }
            
            OnRemoveItemsFromPlayer.Invoke(items);
        }

        public void RemoveItemsFromPlayer(IEnumerable<ItemCost> cost)
        {
            List<ItemInfo> items = new List<ItemInfo>();

            foreach (var item in cost)
            {
                items.Add(new ItemInfo(item.Name, item.Count));
            }

            OnRemoveItemsFromPlayer.Invoke(items);
        }

        public void RemoveItemsFromPlayer(IEnumerable<ItemInfo> items)
        {
            OnRemoveItemsFromPlayer.Invoke(items);
        }
        public void RemoveItemFromPlayer(string name, int count)
        {
            var itemData = ItemsDB.GetItem(name);
            OnRemoveItemFromPlayer?.Invoke(itemData,count);
        }
        public void RemoveItemFromPlayer(ItemData itemData, int count)
        {
            OnRemoveItemFromPlayer?.Invoke(itemData,count);
        }


        public bool PlayerHasItem(int id)
        {
            return OnPlayerHasItems.Invoke(id,1,false);
        }
        public bool PlayerHasItem(ItemData itemData)
        {
            return OnPlayerHasItems.Invoke(itemData.Id,1,false);
        }
        public bool PlayerHasItem(string name)
        {
            var itemData = ItemsDB.GetItem(name);
            return OnPlayerHasItems.Invoke(itemData.Id,1,false);
        }

        public bool PlayerHasItems(int id, int count)
        {
            return OnPlayerHasItems.Invoke(id,count,false);
        }
        public bool PlayerHasItems(ItemData itemData, int count)
        {
            return OnPlayerHasItems.Invoke(itemData.Id,count,false);
        }
        public bool PlayerHasItems(RequiredItem[] items, bool showNotEnoughMessage = false)
        {
            foreach (var item in items)
            {
                var itemData = ItemsDB.GetItem(item.Name);
                bool hasItem = OnPlayerHasItems.Invoke(itemData.Id,item.Amount,showNotEnoughMessage);
                if(hasItem == false) return false;
            }
            return true;
        }
        public bool PlayerHasItems(IEnumerable<ItemInfo> items, bool showNotEnoughMessage = false)
        {
            foreach (var item in items)
            {
                var itemData = ItemsDB.GetItem(item.name);
                bool hasItem = OnPlayerHasItems.Invoke(itemData.Id,item.count,showNotEnoughMessage);
                if(hasItem == false) return false;
            }
            return true;
        }

        public int GetItemCount(int id)
        {
            return OnGetItemCount.Invoke(id);
        }

        public int GetItemCount(string name)
        {
            var itemData = ItemsDB.GetItem(name);
            return OnGetItemCount.Invoke(itemData.Id);
        }

        public int GetItemCount(ItemData itemData)
        {
            return OnGetItemCount.Invoke(itemData.Id);
        }
        
        public bool TryRemoveItem(string name)
        {
            var itemData = ItemsDB.GetItem(name);
            return OnTryRemoveItemFromPlayer.Invoke(itemData);
        }
        public bool TryRemoveItem(ItemData itemData)
        {
            return OnTryRemoveItemFromPlayer.Invoke(itemData);
        }

        public int GetEmptyCellsCount()
        {
            return OnGetEmptyCellsCount.Invoke();
        }

        public bool CanAddItemToPlayer(ItemData itemData, int count)
        {
            return OnCanAddItemToPlayer.Invoke(itemData, count);
        }

        public void TryEquipItem(string itemName)
        {
            OnTryEquipItem?.Invoke(itemName);
        }
    }
}
