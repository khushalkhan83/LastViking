using Game.Purchases;

namespace Game.Models.RemoteSettings
{
    public class FoodChestRemoteSettings : RemoteSettingsBase, IRemoteSettingsExpand
    {
        public bool IsBuySlotGold { get; set; } = false;
        
        public PurchaseID PurchaseIDBuySlotWatch => PurchaseID.FoodChaestSlotBuyWatch;

        public PurchaseID PurchaseIDBuySlotGold => PurchaseID.FoodChaestSlotBuyGold;
    }
}
