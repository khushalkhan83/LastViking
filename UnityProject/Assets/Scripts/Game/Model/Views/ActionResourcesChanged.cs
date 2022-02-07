namespace Game.Models
{
    using Game.Purchases;
    using UltimateSurvival;

    public class MessageInventoryGatheredData
    {
        public int CountItems { get; }
        public ItemData ItemData { get; }

        public MessageInventoryGatheredData(int countItems, ItemData itemData)
        {
            CountItems = countItems;
            ItemData = itemData;
        }
    }

    public class MessageAppendCoinData
    {
        public int CountCoins { get; }
        
        public MessageAppendCoinData (int countCoins)
        {
            CountCoins = countCoins;
        }
    }

    public class MessageAppendBlueprintData
    {
        public int CountBlueprints { get; }

        public MessageAppendBlueprintData(int countBlueprints)
        {
            CountBlueprints = countBlueprints;
        }
    }

    public class MessageInventoryGatheredBonusData
    {
        public int CountItems { get; }
        public ItemData ItemData { get; }

        public MessageInventoryGatheredBonusData(int countItems, ItemData itemData)
        {
            CountItems = countItems;
            ItemData = itemData;
        }
    }

    public class MessageInventoryCraftedData
    {
        public int CountItems { get; }
        public ItemData ItemData { get; }

        public MessageInventoryCraftedData(int countItems, ItemData itemData)
        {
            CountItems = countItems;
            ItemData = itemData;
        }
    }

    public class MessageInventoryDroppedData
    {
        public int CountItems { get; private set; }
        public ItemData ItemData { get; private set; }

        public MessageInventoryDroppedData(int countItems, ItemData itemData)
        {
            CountItems = countItems;
            ItemData = itemData;
        }
    }

    public class MessageInventoryAttentionData
    {
        public ItemData ItemData { get; }

        public MessageInventoryAttentionData(ItemData itemData)
        {
            ItemData = itemData;
        }
    }
}