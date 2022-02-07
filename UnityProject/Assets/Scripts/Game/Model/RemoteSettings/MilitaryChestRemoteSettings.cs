using Game.Purchases;

namespace Game.Models.RemoteSettings
{
    public class MilitaryChestRemoteSettings : RemoteSettingsBase, IRemoteSettingsExpand
    {
        public bool IsBuySlotGold { get; set; } = false;

        public PurchaseID PurchaseIDBuySlotWatch => PurchaseID.MilitaryChaestSlotBuyWatch;

        public PurchaseID PurchaseIDBuySlotGold => PurchaseID.MilitaryChaestSlotBuyGold;
    }
}
