using Core;
using Core.Controllers;
using Core.Views;
using Game.Models;
using Game.Models.RemoteSettings.Firebase;
// using Firebase.Extensions;
using System.Collections.Generic;
// using Firebase.RemoteConfig;
using System.Globalization;
using System;
using System.Threading.Tasks;

namespace Game.Controllers
{
    public class FirebaseRemoteSettingsController : IFirebaseRemoteSettingsController, IController
    {
        [Inject] public FirebaseRemoteSettingsModel FirebaseRemoteSettingsModel { get; private set; }
        // [Inject] public FirebaseModel FirebaseModel { get; private set; }
        // [Inject] public FirebaseRemoteKeyProvider FirebaseRemoteKeyProvider { get; private set; }
        // [Inject] public ApplicationCallbacksModel ApplicationCallbacksModel { get; private set; }

        void IController.Enable()
        {
            // FirebaseModel.OnFirebaseReady += OnFirebaseReadyHandler;
            
            FirebaseRemoteSettingsModel.MapRemoteParameters();

            // ApplicationCallbacksModel.ApplicationPause += OnApplicationPause;
            // ApplicationCallbacksModel.ApplicationFocus += OnApplicationFocus;
        }

        void IController.Start()
        {
        }

        void IController.Disable()
        {
            // FirebaseModel.OnFirebaseReady -= OnFirebaseReadyHandler;
            // FirebaseRemoteSettingsModel.OnSyncConfig -= OnSyncConfigHandler;
            // FirebaseRemoteSettingsModel.OnApplyConfig -= OnApplySyncedConfigHandler;

            // ApplicationCallbacksModel.ApplicationPause -= OnApplicationPause;
            // ApplicationCallbacksModel.ApplicationFocus -= OnApplicationFocus;
        }

        // private void OnApplicationPause(bool pause)
        // {
        //     if (!FirebaseModel.IsFirebaseReady) return;

        //     if (!pause && (fetchTask == null || (fetchTask != null && fetchTask.IsCompleted)))
        //     {
        //         SyncRemoteConfig();
        //     }
        // }

        // private void OnApplicationFocus(bool focus)
        // {
        //     if (!FirebaseModel.IsFirebaseReady) return;

        //     if (focus && (fetchTask == null || (fetchTask != null && fetchTask.IsCompleted)))
        //     {
        //         SyncRemoteConfig();
        //     }
        // }

        // private void OnFirebaseReadyHandler()
        // {
        //     FirebaseRemoteSettingsModel.OnSyncConfig += OnSyncConfigHandler;
        //     FirebaseRemoteSettingsModel.OnApplyConfig += OnApplySyncedConfigHandler;

        //     SetDefaultConfig();
        //     SyncRemoteConfig();
        // }

        // private void OnSyncConfigHandler() => SyncRemoteConfig();
        // private void OnApplySyncedConfigHandler() => ApplySyncedConfig();

        // private Task fetchTask;
        // private void SyncRemoteConfig()
        // {
        //     if (!FirebaseModel.IsFirebaseReady) return;

        //     fetchTask = FirebaseRemoteConfig.FetchAsync(TimeSpan.Zero);
        //     fetchTask.ContinueWithOnMainThread(task =>
        //     {
        //         if (FirebaseRemoteSettingsModel.ApplyChangesAfterFetch)
        //         {
        //             ApplySyncedConfig();
        //         }

        //         FirebaseRemoteSettingsModel.CampfireRemoteSettings.BuySlotGold = GetRemoteParamInt(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.CampFireSettings_BuySlotGold]);
        //         FirebaseRemoteSettingsModel.CampfireRemoteSettings.BoostGoldPerSlot = GetRemoteParamInt(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.CampFireSettings_BoostGoldPerSlot]);
        //         FirebaseRemoteSettingsModel.InventoryRemoteSettings.ExpandSlotGold = GetRemoteParamInt(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.Inventory_ExpandSlotGold]);
        //         FirebaseRemoteSettingsModel.CraftRemoteSettings.BuySlotGold = GetRemoteParamInt(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.CraftSettings_BuySlotGold]);
        //         FirebaseRemoteSettingsModel.CraftRemoteSettings.BoostGoldPerSlot = GetRemoteParamInt(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.CraftSettings_BoostGoldPerSlot]);
        //         FirebaseRemoteSettingsModel.FurnaceRemoteSettings.BuySlotGold = GetRemoteParamInt(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.FurnaceSettings_BuySlotGold]);
        //         FirebaseRemoteSettingsModel.FurnaceRemoteSettings.BoostGoldPerSlot = GetRemoteParamInt(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.FurnaceSettings_BoostGoldPerSlot]);
        //         FirebaseRemoteSettingsModel.LoomRemoteSettings.BuySlotGold = GetRemoteParamInt(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.LoomSettings_BuySlotGold]);
        //         FirebaseRemoteSettingsModel.LoomRemoteSettings.BoostGoldPerSlot = GetRemoteParamInt(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.LoomSettings_BoostGoldPerSlot]);
        //         FirebaseRemoteSettingsModel.HealthAddonRemoteSettings.AddonGold = GetRemoteParamInt(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.HealthAddon_Gold]);
        //         FirebaseRemoteSettingsModel.FoodAddonRemoteSettings.AddonGold = GetRemoteParamInt(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.FoodAddon_Gold]);
        //         FirebaseRemoteSettingsModel.WaterAddonRemoteSettings.AddonGold = GetRemoteParamInt(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.WaterAddon_Gold]);
        //         FirebaseRemoteSettingsModel.BasicChestRemoteSettings.BuySlotGold = GetRemoteParamInt(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.BasicChest_BuySlotGold]);
        //         FirebaseRemoteSettingsModel.FoodChestRemoteSettings.BuySlotGold = GetRemoteParamInt(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.FoodAddon_Gold]);
        //         FirebaseRemoteSettingsModel.MilitaryChestRemoteSettings.BuySlotGold = GetRemoteParamInt(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.MilitaryChest_BuySlotGold]);
        //         FirebaseRemoteSettingsModel.SkipObjectivesRemoteSettings.SkipGold = GetRemoteParamInt(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.SkipObjectives_SkipGold]);
        //         FirebaseRemoteSettingsModel.ResurrectRemoteSettings.ResurrectGold = GetRemoteParamInt(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.Resurrect_Gold]);
        //         FirebaseRemoteSettingsModel.TombRemoteSettings.TakeItemsGold = GetRemoteParamInt(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.Tomb_TakeItemsGold]);
        //         FirebaseRemoteSettingsModel.RepairRemoteSettings.ItemHighGold01 = GetRemoteParamInt(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.Repair_ItemHighGold01]);
        //         FirebaseRemoteSettingsModel.RepairRemoteSettings.ItemHighGold02 = GetRemoteParamInt(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.Repair_ItemHighGold02]);
        //         FirebaseRemoteSettingsModel.RepairRemoteSettings.ItemHighGold03 = GetRemoteParamInt(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.Repair_ItemHighGold03]);
        //         FirebaseRemoteSettingsModel.RepairRemoteSettings.ItemMidGold01 = GetRemoteParamInt(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.Repair_ItemMidGold01]);
        //         FirebaseRemoteSettingsModel.RepairRemoteSettings.ItemMidGold02 = GetRemoteParamInt(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.Repair_ItemMidGold02]);
        //         FirebaseRemoteSettingsModel.RepairRemoteSettings.ItemMidGold03 = GetRemoteParamInt(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.Repair_ItemMidGold03]);
        //         FirebaseRemoteSettingsModel.RepairRemoteSettings.ItemLowGold01 = GetRemoteParamInt(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.Repair_ItemLowGold01]);
        //         FirebaseRemoteSettingsModel.RepairRemoteSettings.ItemLowGold02 = GetRemoteParamInt(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.Repair_ItemLowGold02]);
        //         FirebaseRemoteSettingsModel.RepairRemoteSettings.ItemLowGold03 = GetRemoteParamInt(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.Repair_ItemLowGold03]);

        //         FirebaseRemoteSettingsModel.SetRemoteFetched();
        //     });
        // }

        // private void ApplySyncedConfig()
        // {
        //     if (!FirebaseModel.IsFirebaseReady) return;

        //     FirebaseRemoteConfig.ActivateFetched();
        // }

        // private void SetDefaultConfig()
        // {
        //     if (!FirebaseModel.IsFirebaseReady) return;

        //     Dictionary<string, object> defaults = new Dictionary<string, object>();
        //     defaults.Add(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.CampFireSettings_BoostGoldPerSlot], FirebaseRemoteSettingsModel.CampfireRemoteSettings.BoostGoldPerSlot);
        //     defaults.Add(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.CampFireSettings_BuySlotGold], FirebaseRemoteSettingsModel.CampfireRemoteSettings.BuySlotGold);
        //     defaults.Add(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.Inventory_ExpandSlotGold], FirebaseRemoteSettingsModel.InventoryRemoteSettings);
        //     defaults.Add(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.CraftSettings_BoostGoldPerSlot], FirebaseRemoteSettingsModel.CraftRemoteSettings.BoostGoldPerSlot);
        //     defaults.Add(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.CraftSettings_BuySlotGold], FirebaseRemoteSettingsModel.CraftRemoteSettings.BuySlotGold);
        //     defaults.Add(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.FurnaceSettings_BoostGoldPerSlot], FirebaseRemoteSettingsModel.FurnaceRemoteSettings.BoostGoldPerSlot);
        //     defaults.Add(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.FurnaceSettings_BuySlotGold], FirebaseRemoteSettingsModel.FurnaceRemoteSettings.BuySlotGold);
        //     defaults.Add(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.LoomSettings_BoostGoldPerSlot], FirebaseRemoteSettingsModel.LoomRemoteSettings.BoostGoldPerSlot);
        //     defaults.Add(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.LoomSettings_BuySlotGold], FirebaseRemoteSettingsModel.LoomRemoteSettings.BuySlotGold);
        //     defaults.Add(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.HealthAddon_Gold], FirebaseRemoteSettingsModel.HealthAddonRemoteSettings.AddonGold);
        //     defaults.Add(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.FoodAddon_Gold], FirebaseRemoteSettingsModel.FoodAddonRemoteSettings.AddonGold);
        //     defaults.Add(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.WaterAddon_Gold], FirebaseRemoteSettingsModel.WaterAddonRemoteSettings.AddonGold);
        //     defaults.Add(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.BasicChest_BuySlotGold], FirebaseRemoteSettingsModel.BasicChestRemoteSettings.BuySlotGold);
        //     defaults.Add(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.FoodChest_BuySlotGold], FirebaseRemoteSettingsModel.FoodChestRemoteSettings.BuySlotGold);
        //     defaults.Add(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.MilitaryChest_BuySlotGold], FirebaseRemoteSettingsModel.MilitaryChestRemoteSettings.BuySlotGold);
        //     defaults.Add(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.SkipObjectives_SkipGold], FirebaseRemoteSettingsModel.SkipObjectivesRemoteSettings.SkipGold);
        //     defaults.Add(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.Resurrect_Gold], FirebaseRemoteSettingsModel.ResurrectRemoteSettings.ResurrectGold);
        //     defaults.Add(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.Tomb_TakeItemsGold], FirebaseRemoteSettingsModel.TombRemoteSettings.TakeItemsGold);
        //     defaults.Add(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.Repair_ItemHighGold01], FirebaseRemoteSettingsModel.RepairRemoteSettings.ItemHighGold01);
        //     defaults.Add(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.Repair_ItemHighGold02], FirebaseRemoteSettingsModel.RepairRemoteSettings.ItemHighGold02);
        //     defaults.Add(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.Repair_ItemHighGold03], FirebaseRemoteSettingsModel.RepairRemoteSettings.ItemHighGold03);
        //     defaults.Add(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.Repair_ItemMidGold01], FirebaseRemoteSettingsModel.RepairRemoteSettings.ItemMidGold01);
        //     defaults.Add(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.Repair_ItemMidGold02], FirebaseRemoteSettingsModel.RepairRemoteSettings.ItemMidGold02);
        //     defaults.Add(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.Repair_ItemMidGold03], FirebaseRemoteSettingsModel.RepairRemoteSettings.ItemMidGold03);
        //     defaults.Add(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.Repair_ItemLowGold01], FirebaseRemoteSettingsModel.RepairRemoteSettings.ItemLowGold01);
        //     defaults.Add(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.Repair_ItemLowGold02], FirebaseRemoteSettingsModel.RepairRemoteSettings.ItemLowGold02);
        //     defaults.Add(FirebaseRemoteKeyProvider[FirebaseRemoteSettingKeyID.Repair_ItemLowGold03], FirebaseRemoteSettingsModel.RepairRemoteSettings.ItemLowGold03);

        //     FirebaseRemoteConfig.SetDefaults(defaults);
        // }

        // private int GetRemoteParamInt(string key) => (int)FirebaseRemoteConfig.GetValue(key).LongValue;
        // private float GetRemoteParamFloat(string key) => float.Parse(FirebaseRemoteConfig.GetValue(key).StringValue, NumberStyles.Any, CultureInfo.InvariantCulture);
        // private string GetRemoteParamString(string key) => FirebaseRemoteConfig.GetValue(key).StringValue;
        // private bool GetRemoteParamBool(string key) => FirebaseRemoteConfig.GetValue(key).BooleanValue;
    }
}
