using Game.Purchases;

namespace Game.Models.RemoteSettings
{
    public class LoomRemoteSettings : RemoteSettingsBase
    {
        public bool IsBuySlotGold { get; set; } = false;
        public bool IsBoostGold { get; set; } = true;
    }
}
