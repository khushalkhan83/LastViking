using Game.Purchases;

namespace Game.Models.RemoteSettings
{
    public class BasicChestRemoteSettings : RemoteSettingsBase, IRemoteSettingsExpand
    {
        public bool IsBuySlotGold { get; set; } = false;
        
        public PurchaseID PurchaseIDBuySlotWatch => PurchaseID.BasicChaestSlotBuyWatch;

        public PurchaseID PurchaseIDBuySlotGold => PurchaseID.BasicChaestSlotBuyGold;
    }
}
