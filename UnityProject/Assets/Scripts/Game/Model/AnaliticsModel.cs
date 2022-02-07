using Core.Storage;
using DTDEditor;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

namespace Game.Models
{
    public enum AnaliticEventID
    {
        None,
        GameSessionStart,
        ItemsCrafted,
        ItemsCraftedGlobalDays,
        BuyShelter,
        DeathShelter,
        ItemUnlocked,
        BuildItem,
        CoinsSpend,
        CoinsSink,
        WatchRevardedVideo,
        ShelterDeath,
        PlayerDeath,
        GameReset,
        GamePlayAgain,
        KillEnemy,
        KillAnimal,
        GetResourcesGlobalDays,
        ShelterLifeDays,
        GlobalDays,
        FirstProgression,
        FirstInteractionUI,
        UpdateShelterLevel01,
        UpdateShelterLevel02,
        PressButtonStart,
        Get_IAP_Tier01,
        Get_IAP_Tier02,
        Get_IAP_Tier03,
        Get_IAP_Tier04,
        Get_IAP_Tier05,
        Get_IAP_Tier06,
        GetBlueprint,
        Unlock_,
        Mission_Started, // depricated
        Mission_Completed, // depricated
        Mission_Failed, // depricated
        NotEnoughCoins,
        OpenStore,
        CoinsGet,
        GetEventsMission,
        SpawnObjectInZero,
        TutorialStep,
        NotEnoughStorageSpace,
        NotEnoughStorageSpaceCritical,
        _PromoInapPurchase,
        StraterPackOpenView00,
        StraterPackOpenView01,
        StraterPackOpenView02,
        StraterPackOpenViewNotif,
        GetOceanChest,
        GetTreasureHuntChest,
        GetWaterfallChest,
        TreasureHuntGetBottle,
        TreasureHuntStartDigging,
        FishingStart,
        FishingCatchFish,
        FishingGetFish,
        WaterfallChunk01,
        WaterfallChunk02,
        WaterfallChunk03,
        WaterfallChunk04,
        WaterfallExit,
        WaterfallUnlocked,
        PlaceConstructionPart,
        Quest_Main_QQ_Started,
        Quest_Main_QQ_Completed,
        Quest_Main_QQ_Stage_SS,
        Quest_Optional_QQ_Started,
        Quest_Optional_QQ_Completed,
        Quest_Optional_QQ_Stage_SS,
        ConstructionEnabled,
        OpenCraft,
        TutorialFirstCraftBoost,
        PlayerLocation,
        CompleteTutorialStep_XX,
        CollectCoin,
        Missions_Completed,
        PredefinedMissions_Completed,
        AllMedallionsCollected,
        StartedGameFromNotification,
        RedirectedFromNotificationToObjectives,
        RedirectToLastFishButtonClick,
        RedirectToDiscordButtonClick,
        Encounter_Init,
        Encounter_Unload,
        Encounter_Compleate,
        HouseBuildingStart,
        HouseBuildingComplete,
        PuzzleActivated,
        PickUp,
        TwonHallDeath,
    }

    enum FirstProgressionProgressID
    {
        FirstStrart,
        FirstShelter,
        FirstNightAttackCompleted,
        FirstShelterUpdate,
    }

    enum FirstInteractionUIProgressID
    {
        OpenInventory,
        OpenCraft,
        Attack,
        Jump,
        Run,
        SelectHotbar,
        OpenSettings,
        OpenStore,
        Joystick,
        Touchpad,
        Tomb,
    }

    public class AnaliticsModel : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase
        {
            public bool HasShown;

            public void SetHasShown(bool show)
            {
                HasShown = show;
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] private string[] _addedResourceNames;
        [SerializeField] private ulong _countGlobalDays;
        [SerializeField] private ulong _countShelterGlobalDays;

#pragma warning restore 0649
        #endregion

        public Data _Data => _data;
        public string[] AddedResourcesNames => _addedResourceNames;
        public ulong CountGlobalDays => _countGlobalDays;
        public ulong CountShelterGlobalDays => _countShelterGlobalDays;

        public string NoneGroup { get; } = "None";
        public string UnlockGroup { get; } = "Unlock";

        public string CoinCurrency { get; } = "Coins";
        public string BluepringCurrency { get; } = "Blueprints";

        public string HungerDeathReason { get; } = "Hunger";
        public string ThirstDeathReason { get; } = "Thirst";
        public string ColdDeathReason { get; } = "Cold";

        public string ParamItemID { get; } = "ItemID";
        public string ParamShelterID { get; } = "ShelterID";
        public string ParamGlobalDays { get; } = "GlobalDays";
        public string ParamShelterLevel { get; } = "ShelterLevel";
        public string ParamEnemyID { get; } = "EnemyID";
        public string ParamShelterLifeTime { get; } = "ShelterLifeTime";
        public string ParamBlueprintID { get; } = "BlueprintID";
        public string ParamBlueprintCost { get; } = "BlueprintCost";
        public string ParamPurchaseID { get; } = "PurchaseID";
        public string ParamCoinCost { get; } = "CoinCost";
        public string ParamDeathReason { get; } = "DeathReason";
        public string ParamPlayerLifeTime { get; } = "PlayerLifeTime";
        public string ParamKillAnimal { get; } = "KillAnimal";
        public string ParamResourcesID { get; } = "ResourcesID";
        public string ParamDay { get; } = "Day";
        public string ParamProgress { get; } = "Progress";
        public string ParamInteraction { get; } = "Interaction";
        public string ParamWorldTime { get; } = "WorldTime";
        public string ParamCount { get; } = "Count";
        public string ParamTasksCompleted { get; } = "TasksCompleted";
        public string ParamMissionID { get; } = "MissionID";
        public string ParamWorldObjectID { get; } = "WorldObjectID";
        public string ParamStepNumber { get; } = "Step";
        public string ParamDiggedCount { get; } = "DiggedCount";
        public string ParamConstructionPartID { get; } = "ConstructionPartID";
        public string ParamCraftBoostUsed { get; } = "ParamCraftBoostUsed";
        public string ParamLocation { get; } = "Location";
        public string ParamTutorialStepTime { get; } = "TutorialStepTime";
        public string ParamFrom { get; } = "From";
        public string ParamPredefinedMissionNum { get; } = "PredefinedMissionNum";
        public string ParamNotificationId { get; } = "NotificationKey";
        public string ParamEncounterID { get; } = "EncounterID";
        public string ParamEncounterType { get; } = "EncounterType";
        public string ParamEncounterDescription { get; } = "EncounterDescription";
        public string ParamEncounterUnloadReason { get; } = "EncounterUnloadReason";
        public string ParamType { get; } = "Type";
        public string ParamLevel { get; } = "Level";
        public string ParamName { get; } = "Name";


        public string QuestNumber => "QQ";
        public string QuestStageNumber => "SS";

        public string Property_IsCheater => "IsCheater";
        public string Property_IsCheater_Altenrative => "IsCheater_Altenrative"; // temp Property (test values)
        public string Property_NotCheater => "NotCheater"; // temp Property (test values)
        public string Property_NotFinishedTutorial => "NotFinishedTutorial";
        public string Property_ShipLevel => "ShipLevel";


        protected IDictionary<AnaliticEventID, string[]> EventsParametrs { get; } = new Dictionary<AnaliticEventID, string[]>()
        {
            { AnaliticEventID.GameSessionStart, null },
            { AnaliticEventID.ItemsCrafted, new []{ "ItemID" } },
            { AnaliticEventID.ItemsCraftedGlobalDays, new []{ "ItemID" } },
            { AnaliticEventID.BuyShelter, new [] { "ShelterID", "GlobalDays" } },
            { AnaliticEventID.DeathShelter, new [] { "ShelterID", "ShelterLevel", "EnemyID", "ShelterLifeTime" } },
            { AnaliticEventID.ItemUnlocked, new [] { "ItemID", "BlueprintID", "BlueprintCost" } },
            { AnaliticEventID.BuildItem, new [] { "ItemID" } },
            { AnaliticEventID.CoinsSpend, new [] { "PurchaseID", "CoinCost" } },
            { AnaliticEventID.CoinsSink, new [] { "PurchaseID" } },
            { AnaliticEventID.WatchRevardedVideo, new [] { "PurchaseID" } },
            { AnaliticEventID.ShelterDeath, new [] { "ShelterID", "ShelterLevel" } },
            { AnaliticEventID.PlayerDeath, new [] { "LastDamage", "PlayerLifeTime" } },
            { AnaliticEventID.GameReset, null },
            { AnaliticEventID.GamePlayAgain, null },
            { AnaliticEventID.KillEnemy, new [] { "EnemyID" } },
            { AnaliticEventID.KillAnimal, new [] { "KillAnimal" } },
            { AnaliticEventID.GetResourcesGlobalDays, new [] { "ResourcesID" } },
            { AnaliticEventID.ShelterLifeDays, new [] { "Day" } },
            { AnaliticEventID.GlobalDays, new [] { "Day" } },
            { AnaliticEventID.FirstProgression, new [] { "Progress" } },
            { AnaliticEventID.FirstInteractionUI, new [] { "Interaction" } },
            { AnaliticEventID.UpdateShelterLevel01, new [] { "ShelterLevel", "GlobalDays", "ShelterLifeTime" } },
            { AnaliticEventID.UpdateShelterLevel02, new [] { "ShelterLevel", "GlobalDays", "ShelterLifeTime" } },
            { AnaliticEventID.PressButtonStart, null},
            { AnaliticEventID.Get_IAP_Tier01, new [] { "GlobalDays", "WorldTime" } },
            { AnaliticEventID.Get_IAP_Tier02, new [] { "GlobalDays", "WorldTime" } },
            { AnaliticEventID.Get_IAP_Tier03, new [] { "GlobalDays", "WorldTime" } },
            { AnaliticEventID.Get_IAP_Tier04, new [] { "GlobalDays", "WorldTime" } },
            { AnaliticEventID.Get_IAP_Tier05, new [] { "GlobalDays", "WorldTime" } },
            { AnaliticEventID.Get_IAP_Tier06, new [] { "GlobalDays", "WorldTime" } },
            { AnaliticEventID.GetBlueprint, new [] { "Count" } },
            { AnaliticEventID.Unlock_, new [] { "GlobalDays", "WorldTime" } },
            { AnaliticEventID.Mission_Started, null },
            { AnaliticEventID.Mission_Failed, new [] { "TasksCompleted" } },
            { AnaliticEventID.Mission_Completed, new [] { "GlobalDays" } },
            { AnaliticEventID.NotEnoughCoins, new [] { "PurchaseID" } },
            { AnaliticEventID.GetEventsMission, new [] { "MissionID" } },
            { AnaliticEventID.SpawnObjectInZero, new [] { "WorldObjectID" } },
            { AnaliticEventID.TutorialStep, new [] { "Step" } },
            { AnaliticEventID.NotEnoughStorageSpace, null },
            { AnaliticEventID.NotEnoughStorageSpaceCritical, null },
            { AnaliticEventID._PromoInapPurchase, new [] { "GlobalDays", "WorldTime" } },
        };

        public event Action<WorldObjectID> OnObjectSpawn;

        public bool HasShownTomb
        {
            set => _Data.SetHasShown(value);
            get => _data.HasShown;
        }

        public delegate AnalyticsResult SendData(string eventName, IDictionary<string, object> eventData = null);

        public void SendWithPostfix(AnaliticEventID analiticEventID, object postfix, params object[] args) => Send(GetEventName(analiticEventID) + postfix.ToString(), AnalyticsEvent.Custom, EventsParametrs[analiticEventID], args);

        public void Send(AnaliticEventID analiticEventID, params object[] args) => Send(GetEventName(analiticEventID), AnalyticsEvent.Custom, EventsParametrs[analiticEventID], args);
        public void Send(string eventName, AnaliticEventID analiticEventID, params object[] args) => Send(eventName, AnalyticsEvent.Custom, EventsParametrs[analiticEventID], args);

        public void SendFirst(AnaliticEventID analiticEventID, params object[] args) => Send(GetEventName(analiticEventID), AnalyticsEvent.FirstInteraction, EventsParametrs[analiticEventID], args);

        private void Send(string name, SendData send, string[] parametrNames, object[] args)
        {
            if (parametrNames == null)
            {
                send(name);
            }
            else
            {
                var eventData = new Dictionary<string, object>();

                for (int i = 0; i < parametrNames.Length; i++)
                {
                    eventData.Add(parametrNames[i], args[i]);
                }

                send(name, eventData);
            }
        }

        public string GetEventName(AnaliticEventID analiticEventID) => analiticEventID.ToString();

        public string GetTierEventName(AnaliticEventID analiticEventID, byte tier)
        {
            switch (analiticEventID)
            {
                case AnaliticEventID.Mission_Started: return $"Tier{(tier + 1):00}_Started";
                case AnaliticEventID.Mission_Completed: return $"Tier{(tier + 1):00}_Completed";
                case AnaliticEventID.Mission_Failed: return $"Tier{(tier + 1):00}_Failed";
                default: return "UnknownTierId";
            }
        }

        public void SendSpawnObjectInZero(WorldObjectID worldObjectID) => OnObjectSpawn?.Invoke(worldObjectID);

        public bool EnableAnalitics
        {
            get
            {
                if(Debug.isDebugBuild)
                {
                    return EditorGameSettings.Instance.enableAnalitics;
                }
                return true;
            }
        }

        public bool DevToDevAnaliticsInited => DevToDevSDK.Initied;
    }
}
