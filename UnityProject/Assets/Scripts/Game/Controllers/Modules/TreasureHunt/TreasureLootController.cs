using Core;
using Core.Controllers;
using Game.Models;
using System.Collections.Generic;
using System.Linq;
using UltimateSurvival;

namespace Game.Controllers
{
    public interface ITreasureLootController { }
    public class TreasureLootController : IController, ITreasureLootController
    {
        [Inject] public TreasureLootModel TreasureLootModel { get; private set; }
        [Inject] public ItemsDB ItemsDB { get; private set; }
        [Inject] public EventLootModel EventLootModel { get; private set; }

        public void Enable()
        {
            TreasureLootModel.OnGetLootItems += GetLootItems;
            TreasureLootModel.OnGetCellRespinLootItem += GetCellRespinLootItem;
            TreasureLootModel.OnGetSpinItemsList += GetSpinItemsList;
            TreasureLootModel.OnIsCellSpecial += IsCellSpecial;
        }
        public void Disable()
        {
            TreasureLootModel.OnGetLootItems -= GetLootItems;
            TreasureLootModel.OnGetCellRespinLootItem -= GetCellRespinLootItem;
            TreasureLootModel.OnGetSpinItemsList -= GetSpinItemsList;
            TreasureLootModel.OnIsCellSpecial -= IsCellSpecial;
        }

        public void Start()
        {

        }

        private List<SavableItem> GetLootItems(TreasureID treasureID)
        {
            List<SavableItem> items = new List<SavableItem>();

            var cells = TreasureLootModel.TreasureConfigProvider[treasureID].cells;
            List<string> exceptItemNames = new List<string>();

            foreach (var cell in cells) {
                SavableItem item = FindCellItem(cell, exceptItemNames);
                if (item != null) {
                    exceptItemNames.Add(item.Name);
                    items.Add(item);
                }
            }

            return items;
        }

        private SavableItem GetCellRespinLootItem(TreasureID treasureID, int cellID, List<string> exceptItemNames)
        {
            var cells = TreasureLootModel.TreasureConfigProvider[treasureID].cells;
            if (cells == null || cells.Count <= cellID) {
                return null;
            }

            CellConfig cell = cells[cellID];
            return FindCellItem(cell, exceptItemNames);
        }

        private List<SavableItem> GetSpinItemsList(TreasureID treasureID, int cellID, SavableItem targetItem)
        {
            var cells = TreasureLootModel.TreasureConfigProvider[treasureID].cells;
            if (cells == null || cells.Count <= cellID)
            {
                return null;
            }

            CellConfig cellConfig = cells[cellID];

            List<ItemConfig> itemConfigs;
            if (cellConfig.spwanType == SpawnType.Groups)
            {
                itemConfigs = new List<ItemConfig>();
                foreach (var lootGroupPriority in cellConfig.lootGroupPriorities)
                {
                    itemConfigs.AddRange(GetGroupItems(lootGroupPriority.id));
                }
            }
            else
            {
                itemConfigs = new List<ItemConfig>(cellConfig.items);
            }

            if (cellConfig.canBeEvent)
            {
                var eventItems = GetSpingEventItems(cellConfig);
                itemConfigs.AddRange(eventItems);
            }

            List<SavableItem> items = new List<SavableItem>();

            for (int i = 0; i < TreasureLootModel.SpinItemsCount - 1 && i < itemConfigs.Count; i++)
            {
                var itemConfig = itemConfigs[UnityEngine.Random.Range(0, itemConfigs.Count)];
                itemConfigs.Remove(itemConfig);

                if (itemConfig.name == targetItem.Name)
                {
                    continue;
                }

                ItemData itemData;
                if (ItemsDB.ItemDatabase.FindItemByName(itemConfig.name, out itemData))
                {
                    int itemCount = UnityEngine.Random.Range(itemConfig.countMin, itemConfig.countMax + 1);
                    items.Add(new SavableItem(itemData, itemCount));
                }
            }

            int insertIndex = UnityEngine.Random.Range(0, items.Count);
            items.Insert(insertIndex, targetItem);
            return items;
        }

        private IEnumerable<ItemConfig> GetSpingEventItems(CellConfig cellConfig)
        {
            var lootItems = TreasureLootModel.EventLootProvider[cellConfig.eventLootID];
            var customItems = lootItems.items.Where(i => EventLootModel.HasCustomLoot(i.name));
            return customItems;
        }

        private bool IsCellSpecial(TreasureID treasureID, int cellID) {
            var cells = TreasureLootModel.TreasureConfigProvider[treasureID].cells;
            if (cells == null || cells.Count <= cellID)
            {
                return false;
            }

            return cells[cellID].isSpecial;
        }

        private SavableItem FindCellItem(CellConfig cellConfig, List<string> exceptItemNames) {
            ItemConfig itemConfig = null;

            if (cellConfig.canBeEvent)
            {
                bool gotLoot = TryGetEventItemConfig(cellConfig, out itemConfig);
                if (!gotLoot)
                {
                    itemConfig = GetSpawnedItemConfig(cellConfig, exceptItemNames);
                }
            }
            else
            {
                itemConfig = GetSpawnedItemConfig(cellConfig, exceptItemNames);
            }

            if (itemConfig == null)
                return null;

            ItemData itemData;
            if (ItemsDB.ItemDatabase.FindItemByName(itemConfig.name, out itemData))
            {
                int itemCount = UnityEngine.Random.Range(itemConfig.countMin, itemConfig.countMax + 1);
                return new SavableItem(itemData, itemCount);
            }
            return null;
        }

        private bool TryGetEventItemConfig(CellConfig cellConfig, out ItemConfig itemConfig)
        {
            var lootItems = TreasureLootModel.EventLootProvider[cellConfig.eventLootID];

            if (EventLootModel.GetLootFromCustom("item", 400, out string lootKey)) // ! string key
            {
                var lootItem = lootItems.items.FirstOrDefault(i => i.name == lootKey);
                if (lootItem != null)
                {
                    itemConfig = lootItem;
                    return true;
                }
            }

            itemConfig = default;
            return false;
        }

        private ItemConfig GetSpawnedItemConfig(CellConfig cellConfig, List<string> exceptItemNames)
        {
            if (cellConfig.spwanType == SpawnType.Groups)
            {
                return GetRandomItem(cellConfig.lootGroupPriorities, exceptItemNames);
            }
            else
            {
                var itemsForRandom = GetItemsExcept(cellConfig.items, exceptItemNames);
                return GetRandomItem(itemsForRandom);
            }
        }

        private List<ItemConfig> GetItemsExcept(List<ItemConfig> items, List<string> exceptItemNames) 
        {
            List<ItemConfig> itemsWithException = new List<ItemConfig>(items);

            if (exceptItemNames == null || exceptItemNames.Count == 0) {
                return itemsWithException;
            }

            foreach (string name in exceptItemNames)
            {
                foreach (var item in items)
                {
                    if (item.name == name)
                    {
                        itemsWithException.Remove(item);
                        break;
                    }
                }
            }
            return itemsWithException;
        }

        private ItemConfig GetRandomItem(List<LootGroupPriority> lootGroupPriorities, List<string> exceptItemNames)
        {
            if (lootGroupPriorities == null || lootGroupPriorities.Count == 0)
            {
                return null;
            }

            List<ItemConfig> items = null;

            float totalNumber = 0;
            foreach (var groupPriority in lootGroupPriorities)
            {
                totalNumber += groupPriority.dropPriority;
            }
            float number = UnityEngine.Random.Range(0, totalNumber);

            totalNumber = 0;
            foreach (var groupPriority in lootGroupPriorities)
            {
                if (number >= totalNumber && number <= totalNumber + groupPriority.dropPriority)
                {
                    items = GetGroupItems(groupPriority.id);
                    break;
                }
                totalNumber += groupPriority.dropPriority;
            }

            if (items != null)
            {
                var itemsForRandom = GetItemsExcept(items, exceptItemNames);
                return GetRandomItem(itemsForRandom);
            }
            else {
                return null;
            }
        }

        private ItemConfig GetRandomItem(List<ItemConfig> items) 
        {
            if (items == null || items.Count == 0)
            {
                return null;
            }

            float totalNumber = 0;
            foreach (var item in items)
            {
                totalNumber += item.dropPriority;
            }
            float number = UnityEngine.Random.Range(0, totalNumber);

            totalNumber = 0;
            foreach (var item in items)
            {
                if (number >= totalNumber && number <= totalNumber + item.dropPriority)
                {
                    return item;
                }
                totalNumber += item.dropPriority;
            }

            return null;
        }

        private List<ItemConfig> GetGroupItems(LootGroupID groupID) {
            return TreasureLootModel.TreasureLootGroupProvider[groupID].items;
        }

    }
}
