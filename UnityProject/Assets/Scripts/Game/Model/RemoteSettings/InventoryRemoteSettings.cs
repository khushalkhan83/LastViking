using Game.Purchases;

namespace Game.Models.RemoteSettings
{
    public class InventoryRemoteSettings : RemoteSettingsBase
    {
        public bool IsBuySlotsGold { get; set; } = false;

        public PurchaseID PurchaseIDBuySlot => IsBuySlotsGold
            ? PurchaseID.PlayerInventoryExpandGold
            : PurchaseID.InventoryExpandWatch;
    }
}
