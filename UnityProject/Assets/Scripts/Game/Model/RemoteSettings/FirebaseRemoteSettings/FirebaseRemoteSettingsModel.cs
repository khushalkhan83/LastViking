using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Game.Models.RemoteSettings.Firebase
{
    public class FirebaseRemoteSettingsModel : MonoBehaviour
    {
        #region Data

        [Header("Control")]
        [Space(20f)] [SerializeField] private bool _applyChangesAfterFetch = true;

        [Space(20f)]
        [Header("Remote Settings")]
        [Space(20f)] [SerializeField] private CampfireRemoteSettings _campfireRemoteSettings;
        [Space(20f)] [SerializeField] private InventoryRemoteSettings _inventoryRemoteSettings;
        [Space(20f)] [SerializeField] private CraftRemoteSettings _craftRemoteSettings;
        [Space(20f)] [SerializeField] private FurnaceRemoteSettings _furnaceRemoteSettings;
        [Space(20f)] [SerializeField] private LoomRemoteSettings _loomRemoteSettings;
        [Space(20f)] [SerializeField] private HealthAddonRemoteSettings _healthAddonRemoteSettings;
        [Space(20f)] [SerializeField] private FoodAddonRemoteSettings _foodAddonRemoteSettings;
        [Space(20f)] [SerializeField] private WaterAddonRemoteSettings _waterAddonRemoteSettings;
        [Space(20f)] [SerializeField] private BasicChestRemoteSettings _basicChestRemoteSettings;
        [Space(20f)] [SerializeField] private FoodChestRemoteSettings _foodChestRemoteSettings;
        [Space(20f)] [SerializeField] private MilitaryChestRemoteSettings _militaryChestRemoteSettings;
        [Space(20f)] [SerializeField] private SkipObjectivesRemoteSettings _skipObjectivesRemoteSettings;
        [Space(20f)] [SerializeField] private ResurrectRemoteSettings _resurrectRemoteSettings;
        [Space(20f)] [SerializeField] private TombRemoteSettings _tombRemoteSettings;
        [Space(20f)] [SerializeField] private RepairRemoteSettings _repairRemoteSettings;
        
        #endregion

        public bool ApplyChangesAfterFetch => _applyChangesAfterFetch;

        public CampfireRemoteSettings CampfireRemoteSettings => _campfireRemoteSettings;
        public InventoryRemoteSettings InventoryRemoteSettings => _inventoryRemoteSettings;
        public CraftRemoteSettings CraftRemoteSettings => _craftRemoteSettings;
        public FurnaceRemoteSettings FurnaceRemoteSettings => _furnaceRemoteSettings;
        public LoomRemoteSettings LoomRemoteSettings => _loomRemoteSettings;
        public HealthAddonRemoteSettings HealthAddonRemoteSettings => _healthAddonRemoteSettings;
        public FoodAddonRemoteSettings FoodAddonRemoteSettings => _foodAddonRemoteSettings;
        public WaterAddonRemoteSettings WaterAddonRemoteSettings => _waterAddonRemoteSettings;
        public BasicChestRemoteSettings BasicChestRemoteSettings => _basicChestRemoteSettings;
        public FoodChestRemoteSettings FoodChestRemoteSettings => _foodChestRemoteSettings;
        public MilitaryChestRemoteSettings MilitaryChestRemoteSettings => _militaryChestRemoteSettings;
        public SkipObjectivesRemoteSettings SkipObjectivesRemoteSettings => _skipObjectivesRemoteSettings;
        public ResurrectRemoteSettings ResurrectRemoteSettings => _resurrectRemoteSettings;
        public TombRemoteSettings TombRemoteSettings => _tombRemoteSettings;
        public RepairRemoteSettings RepairRemoteSettings => _repairRemoteSettings;

        private Dictionary<FirebaseRemoteSettingID, FirebaseRemoteSettingsBase> RemoteParameters = new Dictionary<FirebaseRemoteSettingID, FirebaseRemoteSettingsBase>();

        public event Action OnSyncConfig;
        public event Action OnApplyConfig;

        public event Action OnFetchedRemote;

        public T GetParam<T>(FirebaseRemoteSettingID settingID) where T : FirebaseRemoteSettingsBase => (T)RemoteParameters[settingID];

        public void MapRemoteParameters()
        {
            RemoteParameters.Add(FirebaseRemoteSettingID.Campfire, _campfireRemoteSettings);
            RemoteParameters.Add(FirebaseRemoteSettingID.Inventory, _inventoryRemoteSettings);
            RemoteParameters.Add(FirebaseRemoteSettingID.Craft, _craftRemoteSettings);
            RemoteParameters.Add(FirebaseRemoteSettingID.Furnace, _furnaceRemoteSettings);
            RemoteParameters.Add(FirebaseRemoteSettingID.Loom, _loomRemoteSettings);
            RemoteParameters.Add(FirebaseRemoteSettingID.HealthAddon, _healthAddonRemoteSettings);
            RemoteParameters.Add(FirebaseRemoteSettingID.FoodAddon, _foodAddonRemoteSettings);
            RemoteParameters.Add(FirebaseRemoteSettingID.WaterAddon, _waterAddonRemoteSettings);
            RemoteParameters.Add(FirebaseRemoteSettingID.BasicChest, _basicChestRemoteSettings);
            RemoteParameters.Add(FirebaseRemoteSettingID.FoodChest, _foodChestRemoteSettings);
            RemoteParameters.Add(FirebaseRemoteSettingID.MilitaryChest, _militaryChestRemoteSettings);
            RemoteParameters.Add(FirebaseRemoteSettingID.SkipObjectives, _skipObjectivesRemoteSettings);
            RemoteParameters.Add(FirebaseRemoteSettingID.Resurrect, _resurrectRemoteSettings);
            RemoteParameters.Add(FirebaseRemoteSettingID.Tomb, _tombRemoteSettings);
            RemoteParameters.Add(FirebaseRemoteSettingID.Repair, _repairRemoteSettings);
        }

        public void SyncRemoteConfig() => OnSyncConfig?.Invoke();
        public void ApplySyncedConfig() => OnApplyConfig?.Invoke();

        public void SetRemoteFetched() => OnFetchedRemote?.Invoke();
    }
}
