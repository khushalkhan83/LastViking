using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Controllers;
using Game.Audio;
using Game.Models;
using UltimateSurvival;
using UnityEngine;
using static Game.Models.InventoryOperationsModel;
using ItemConfig = Game.Models.InventoryOperationsModel.ItemConfig;

namespace Game.Controllers
{
    public class InventoryOperationsController : IInventoryOperationsController, IController
    {
        [Inject] public InventoryOperationsModel InventoryOperationsModel { get; private set; }
        [Inject] public ItemsDB ItemsDB { get; private set; }
        [Inject] public InventoryModel InventoryModel { get; private set; }
        [Inject] public HotBarModel HotBarModel { get; private set; }
        [Inject] public DropItemModel DropItemModel { get; private set; }
        [Inject] public ActionsLogModel ActionsLogModel { get; private set; }
        [Inject] public EventLootModel EventLootModel { get; private set; }
        [Inject] public AudioSystem AudioSystem { get; private set; }

        [Inject] public CoinsModel CoinsModel { get; private set; }
        [Inject] public BluePrintsModel BluePrintsModel { get; private set; }

        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }


        void IController.Enable()
        {
            InventoryOperationsModel.OnAddItemToPlayer += AddItemToPlayer;
            InventoryOperationsModel.OnTryAddItemsToPlayer += TryAddItemsToPlayer;
            InventoryOperationsModel.OnRemoveItemsFromPlayer += RemoveItemsFromPlayer;
            InventoryOperationsModel.OnRemoveItemFromPlayer += RemoveItemFromPlayer;
            InventoryOperationsModel.OnPlayerHasItems += PlayerHasItems;
            InventoryOperationsModel.OnGetItemCount += GetIemsCount;
            InventoryOperationsModel.OnTryRemoveItemFromPlayer += TryRemoveItemFromPlayer;
            InventoryOperationsModel.OnGetEmptyCellsCount += GetEmptyCellsCount;
            InventoryOperationsModel.OnCanAddItemToPlayer += OnCanAddItemToPlayer;
            InventoryOperationsModel.OnTryEquipItem += OnTryEquipItem;
        }

        void IController.Start()
        {
        }

        void IController.Disable()
        {
            InventoryOperationsModel.OnAddItemToPlayer -= AddItemToPlayer;
            InventoryOperationsModel.OnTryAddItemsToPlayer -= TryAddItemsToPlayer;
            InventoryOperationsModel.OnRemoveItemsFromPlayer -= RemoveItemsFromPlayer;
            InventoryOperationsModel.OnRemoveItemFromPlayer -= RemoveItemFromPlayer;
            InventoryOperationsModel.OnPlayerHasItems -= PlayerHasItems;
            InventoryOperationsModel.OnGetItemCount -= GetIemsCount;
            InventoryOperationsModel.OnTryRemoveItemFromPlayer -= TryRemoveItemFromPlayer;
            InventoryOperationsModel.OnGetEmptyCellsCount -= GetEmptyCellsCount;
            InventoryOperationsModel.OnCanAddItemToPlayer -= OnCanAddItemToPlayer;
            InventoryOperationsModel.OnTryEquipItem -= OnTryEquipItem;
        }

        #region Event Handlers

        private void AddItemToPlayer(string name, int count, Transform dropPosition, ItemConfig config)
        {
            var itemData = ItemsDB.GetItem(name);
            var itemInfo = new ItemInfo(itemData.Name,count);
            TryAddItemToPlayer(itemInfo,config, out var savableItems);

            if(savableItems!= null && savableItems.Count > 0)
            {
                DropItemModel.DropItem(savableItems, dropPosition);
            }
        }

        private AddItemsResult TryAddItemsToPlayer(IEnumerable<ItemInfo> items, ItemConfig config)
        {
            bool notEnoughSpace = false;
            List<SavableItem> cache = new List<SavableItem>();
            foreach (var item in items)
            {
                if (!TryAddItemToPlayer(item, config, out SavableItem notAddedPart))
                {
                    cache.Add(notAddedPart);
                    notEnoughSpace = true;
                }
            }

            return new AddItemsResult(!notEnoughSpace, cache);
        }

        private void RemoveItemsFromPlayer(IEnumerable<ItemInfo> items)
        {
            foreach (var item in items)
            {
                var itemData = ItemsDB.GetItem(item.name);
                RemoveItemFromPlayer(itemData, item.count);
            }
        }

        // private bool PlayerHasItems(string name, int count)
        // {
        //     var itemData = ItemsDB.GetItem(name);
        //     return PlayerHasItems(itemData.Id,count,false);
        // }
        
        // TODO: optimize (remove dependency from id and use itemData instead)
        private bool PlayerHasItems(int id, int count, bool showNotEnoughMessage = false)
        {
            var itemsInInventory = InventoryModel.ItemsContainer.GetItemsCount(id);
            var hotbarItems = HotBarModel.ItemsContainer.GetItemsCount(id);

            bool playerHasItems = itemsInInventory + hotbarItems >= count;

            if(!playerHasItems && showNotEnoughMessage)
            {
                var itemData = ItemsDB.GetItem(id);
                ActionsLogModel.SendMessage(new MessageInventoryAttentionData(itemData));
                AudioSystem.PlayOnce(AudioID.Error);
            }
            
            return playerHasItems;
        }

        private int GetIemsCount(int id)
        {
            var itemsInInventory = InventoryModel.ItemsContainer.GetItemsCount(id);
            var hotbarItems = HotBarModel.ItemsContainer.GetItemsCount(id);

            return itemsInInventory + hotbarItems;
        }

        private bool TryRemoveItemFromPlayer(ItemData itemData)
        {
            bool hasItem = PlayerHasItems(itemData.Id,1);
            if(!hasItem) return false;

            RemoveItemFromPlayer(itemData,1);
            return true;
        }

        private int GetEmptyCellsCount()
        {
            return InventoryModel.ItemsContainer.GetEmptyCellsCount() + HotBarModel.ItemsContainer.GetEmptyCellsCount();
        }

        private bool OnCanAddItemToPlayer(ItemData itemData, int count)
        {
            if(InventoryModel.ItemsContainer.CanAddItem(itemData, count, out int left))
                return true;
            else 
                return HotBarModel.ItemsContainer.CanAddItem(itemData, left, out int left2);
        }

        //TODO: remove dublicate from ChoiseItemActionDataController
        private void OnTryEquipItem(string itemName)
        {
            var itemData = ItemsDB.ItemDatabase.GetItemByName(itemName);
            CellModel cell = null;

            var hotBarCell = HotBarModel.ItemsContainer.Cells.FirstOrDefault(x => x.IsHasItem && x.Item.ItemData.Id == itemData.Id);
            var inventoryCell = InventoryModel.ItemsContainer.Cells.FirstOrDefault(x => x.IsHasItem && x.Item.ItemData.Id == itemData.Id);

            if(hotBarCell != null)
            {
                cell = hotBarCell;
            }
            else if (inventoryCell != null)
            {
                var cellFrom = inventoryCell;
                var cellTo = GetEmptyOrFirstHotbarCell();

                var tmp = cellFrom.Item;

                cellFrom.Item = cellTo.Item;
                cellTo.Item = tmp;

                cell = cellTo;
            }

            HotBarModel.Equp(cell.Id);
            PlayerEventHandler.ChangeEquippedItem.Try(cell.Item, false);

            #region Methods
                
            CellModel GetEmptyOrFirstHotbarCell()
            {
                var emptyCell = HotBarModel.ItemsContainer.Cells.FirstOrDefault(x => x.IsEmpty);
                if(emptyCell != null) return emptyCell;

                return HotBarModel.ItemsContainer.Cells.FirstOrDefault(x => x.Id == 1);
            }
            #endregion
        }

        #endregion

        private bool TryAddItemToPlayer(ItemInfo item, ItemConfig config, out SavableItem notAdded)
        {
            #region Logic

            SavableItem notAddedPart = null;
            bool givenCompletely = false;
            ItemData itemData = ItemsDB.GetItem(item.name); 

            if (HandleCurrencyCategory())
            {
                notAdded = notAddedPart;
                return givenCompletely;
            }
            else if(HandleEventItemCategory())
            {
                notAdded = notAddedPart;
                return givenCompletely;
            }
            else
            {
                // TODO: pass itemData in method to reduce ItemsDB.GetItem calls
                givenCompletely = TryAddItemToPlayerInventoryOrHotbar(itemData, item.count, config, out int left);

                if(givenCompletely)
                    notAdded = null;
                else
                    notAdded = new SavableItem(itemData, left);
                return givenCompletely;
            }

            #endregion

            #region Methods

            // TODO: move category names to enums (do it in DropContainerController too)
            bool HandleCurrencyCategory()
            {
                bool isCurrencyItem = itemData.Category == "Currency";

                if (!isCurrencyItem) return false;

                givenCompletely = true;

                int addCount = item.count;
                switch (item.name)
                {
                    case "coins":
                        CoinsModel.Adjust(addCount);
                        ActionsLogModel.SendMessage(new MessageAppendCoinData(addCount));
                        break;
                    case "blueprints":
                        BluePrintsModel.Adjust(addCount);
                        ActionsLogModel.SendMessage(new MessageAppendBlueprintData(addCount));
                        break;
                    default:
                        givenCompletely = false;
                        "Error Here. Not handled exeption".Error();
                        break;
                }

                return true;
            }

            bool HandleEventItemCategory()
            {
                bool isCurrencyItem = itemData.Category == "Event";

                if(isCurrencyItem)
                {
                    EventLootModel.ReceiveLootFromCustom(item.name);
                    SendGatherMessage(1, itemData, config.displayAsExtraItem);

                    givenCompletely = true;

                    return true;
                }
                else return false;
            }
            #endregion
        }

        private bool TryAddItemToPlayerInventoryOrHotbar(ItemData itemData, int count, ItemConfig config, out int left)
        {
            bool givenAll = true;
            left = 0;

            var inventory = InventoryModel.ItemsContainer;
            var hotBar = HotBarModel.ItemsContainer;


            switch (config.priority)
            {
                case AddedItemDestinationPriority.HotBar:
                    AddToConainers(itemData,count,hotBar,inventory, config, out left, out givenAll);
                    break;
                case AddedItemDestinationPriority.Inventory:
                    AddToConainers(itemData,count,inventory,hotBar, config, out left, out givenAll);
                    break;
                default:
                    break;
            }

            return givenAll;
        }

        private void AddToConainers(ItemData itemData, int count, ItemsContainer main, ItemsContainer additional, ItemConfig config, out int left, out bool givenCompletely)
        {
            givenCompletely = true;

            List<ItemProperty.Value> customPropertyValues = null;
            if(config.modifiedProperties != null && config.modifiedProperties.Count > 0)
            {
                customPropertyValues = itemData.PropertyValues.Select(x => x.Clone()).ToList();
                foreach(var property in customPropertyValues)
                {
                    if(config.modifiedProperties.TryGetValue(property.Name, out var modifiedProperty))
                    {
                        if (property.Type == ItemProperty.Type.Int)
                            property.Int.Current = (int)modifiedProperty;
                        if (property.Type == ItemProperty.Type.Float)
                            property.Float.Current = (float)modifiedProperty;
                        if (property.Type == ItemProperty.Type.FloatRange)
                            property.FloatRange.Current = (float)modifiedProperty;
                        if (property.Type == ItemProperty.Type.IntRange)
                            property.IntRange.Current = (int)modifiedProperty;
                    }
                }
            }

            if(config.belongsToPlayer)
            {
                if(customPropertyValues == null) customPropertyValues = new List<ItemProperty.Value>();
                var prop = new ItemProperty.Value();
                prop.SetName("BelongsToPlayer");
                customPropertyValues.Add(prop);
            }
            
            SavableItem item = new SavableItem(itemData, count, customPropertyValues);

            left = main.AddItemsData(item, count);
            if (left > 0)
            {
                left = additional.AddItemsData(item, left);
            }
            if (left > 0)
            {
                givenCompletely = false;
            }
            else
            {
                // TODO: check
                SendGatherMessage(count - left, itemData, config.displayAsExtraItem);
            }
        }


        private void RemoveItemFromPlayer(ItemData itemData, int count)
        {
            var left = InventoryModel.ItemsContainer.RemoveItems(itemData.Id, count);
            if (left > 0)
            {
                HotBarModel.ItemsContainer.RemoveItems(itemData.Id, left);
            }

            ActionsLogModel.SendMessage(new MessageInventoryDroppedData(count, itemData));
        }

        private void SendGatherMessage(int count, ItemData itemData, bool isBonus)
        {
            if (isBonus)
            {
                ActionsLogModel.SendMessage(new MessageInventoryGatheredBonusData(count, itemData));
            }
            else
            {
                ActionsLogModel.SendMessage(new MessageInventoryGatheredData(count, itemData));
            }
        }
    }
}
