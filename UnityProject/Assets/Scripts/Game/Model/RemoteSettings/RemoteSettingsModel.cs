using System.Collections.Generic;
using UnityEngine;

namespace Game.Models.RemoteSettings
{
    public class RemoteSettingsModel : MonoBehaviour
    {
        public InventoryRemoteSettings InventoryRemoteSettings { get; } = new InventoryRemoteSettings();
        public CraftRemoteSettings CraftRemoteSettings { get; } = new CraftRemoteSettings();
        public CampFireRemoteSettings CampFireRemoteSettings { get; } = new CampFireRemoteSettings();
        public FurnaceRemoteSettings FurnaceRemoteSettings { get; } = new FurnaceRemoteSettings();
        public HealthAddonRemoteSettings HealthAddonRemoteSettings { get; } = new HealthAddonRemoteSettings();
        public WaterAddonRemoteSettings WaterAddonRemoteSettings { get; } = new WaterAddonRemoteSettings();
        public FoodAddonRemoteSettings FoodAddonRemoteSettings { get; } = new FoodAddonRemoteSettings();
        public BasicChestRemoteSettings BasicChestRemoteSettings { get; } = new BasicChestRemoteSettings();
        public FoodChestRemoteSettings FoodChestRemoteSettings { get; } = new FoodChestRemoteSettings();
        public MilitaryChestRemoteSettings MilitaryChestRemoteSettings { get; } = new MilitaryChestRemoteSettings();
        public PlayerThirstRemoteSettings PlayerThirstRemoteSettings { get; } = new PlayerThirstRemoteSettings();
        public AnimalRemoteSettings AnimalRemoteSettings { get; } = new AnimalRemoteSettings();
        public LoomRemoteSettings LoomRemoteSettings { get; } = new LoomRemoteSettings();

        private IDictionary<RemoteSettingID, object> MapRemoteSettings { get; } = new Dictionary<RemoteSettingID, object>();

        public T Get<T>(RemoteSettingID remoteSettingID)
        {
            if (MapRemoteSettings.Count == 0)
            {
                Map();
            }

            return (T)MapRemoteSettings[remoteSettingID];
        }

        private void Map()
        {
            MapRemoteSettings.Add(RemoteSettingID.Inventory, InventoryRemoteSettings);
            MapRemoteSettings.Add(RemoteSettingID.Craft, CraftRemoteSettings);
            MapRemoteSettings.Add(RemoteSettingID.CampFire, CampFireRemoteSettings);
            MapRemoteSettings.Add(RemoteSettingID.Furnace, FurnaceRemoteSettings);
            MapRemoteSettings.Add(RemoteSettingID.HealthAddon, HealthAddonRemoteSettings);
            MapRemoteSettings.Add(RemoteSettingID.WaterAddon, WaterAddonRemoteSettings);
            MapRemoteSettings.Add(RemoteSettingID.FoodAddon, FoodAddonRemoteSettings);
            MapRemoteSettings.Add(RemoteSettingID.BasicChest, BasicChestRemoteSettings);
            MapRemoteSettings.Add(RemoteSettingID.FoodChest, FoodChestRemoteSettings);
            MapRemoteSettings.Add(RemoteSettingID.MilitaryChest, MilitaryChestRemoteSettings);
            MapRemoteSettings.Add(RemoteSettingID.PlayerThirst, PlayerThirstRemoteSettings);
            MapRemoteSettings.Add(RemoteSettingID.AnimalSettings, AnimalRemoteSettings);
            MapRemoteSettings.Add(RemoteSettingID.Loom, LoomRemoteSettings);
        }
    }
}
