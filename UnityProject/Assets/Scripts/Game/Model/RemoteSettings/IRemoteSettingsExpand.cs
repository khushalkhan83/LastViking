using Game.Purchases;

namespace Game.Models.RemoteSettings
{
    public interface IRemoteSettingsExpand
    {
        bool IsBuySlotGold { get; }
        PurchaseID PurchaseIDBuySlotWatch { get; }
        PurchaseID PurchaseIDBuySlotGold { get; }
    }
}
