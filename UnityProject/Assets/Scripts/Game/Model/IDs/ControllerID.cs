namespace Game.Controllers
{
    public enum ControllerID
    {
        None = 0,
        PlaceItemController = 1,
        RotateButtonController = 2,
        DebugInteractController = 3,
        AimButtonController = 4,
        PlayerThirstController = 5,
        PlayerThirstDamagerController = 6,
        PlayerHungerController = 7,
        PlayerHungerDamagerController = 8,
        PlayerRunController = 9,
        PlayerStaminaRegenerateController = 10,
        PlayerAimController = 11,
        CursorController = 12,
        LoseController = 13,
        BloodEffectController = 14,
        AdvertisementsUAdsController = 15,
        VersionController = 16,
        DiscordController = 17,
        StatisticsController = 18,
        AnaliticsController = 19,
        CraftController = 20,
        BackgroundPauseController = 21,
        RateMeController = 22,
        PlayerConsumeController = 23,
        GodModeViewController = 24,
        GameTimeController = 25,
        PlayerDeathController = 26,
        RepairController = 27,
        PlayerSwicherMovementController = 28,
        PlayerMovementController = 29,
        PlayerSwitcherWeaponController = 30,
        PlayerColdDamagerController = 31,
        PlayerColdController = 32,
        NetworkController = 33,
        PlayerLandController = 34,
        TimingAdsController = 35,
        AndroidTimingAdsController = 36,
        AutosaveController = 37,
        AutosaveInfoController = 38,
        PlayerBleedingDamagerController = 39,
        WideScreenScaleController = 40,
        TutorialController = 41,
        JumpButtonController = 42,
        AttackButtonController = 43,
        TouchpadController = 44,
        RunButtonController = 45,
        CraftButtonController = 46,
        InventoryButtonController = 47,
        HealthBarController = 48,
        FoodBarController = 49,
        WaterBarController = 50,
        PlayerStatusController = 51,
        HotBarController = 52,
        StaminaBarController = 53,
        SettingsButtonController = 54,
        NotificationContainerController = 55,
        CoinsInfoController = 56,
        ResourceMessagesController = 57,
        DebugTimeButtonController = 58,
        JoystickController = 59,
        ObjectivesController = 60,
        ObjectiveConditionsController = 61,
        ObjectiveActionsController = 62,
        StartUpController = 63,
        GameSparksController = 64,
        TutorialDarkScreenController = 65,
        OpenObjectivesLogController = 66,
        AdvertisementsAdmobController = 67,
        PlayerPoisonDamagerController = 68,
        // NightInfoController = 69,
        // ObjectiveAttackProcessController = 70,
        PlayerBoostStaminaController = 71,
        EnableVerisonInfoView = 72,
        EnableSaveInfoView = 73,
        PlayerHealProcessController = 74,
        PlayerEatProcessController = 75,
        PlayerDrinkProcessController = 76,
        PlayerObjectivesController = 77,
        ObjectivesNotificationController = 78,
        AddingInventoryItemsController = 79,
        OutLineMineableStoneController = 80,
        OutLineCoconutController = 81,
        OutLineBlueBarrelController = 82,
        OutLineBagPickupController = 83,
        AnaliticsDevtodevController = 84,
        RealTimeController = 85,
        HotBarRepairingController = 86,
        FishingInitController = 87,
        FishingBiteController = 88,
        FishingPullController = 89,
        MinigameStateController = 90,
        TokensController = 91,
        PlayerEventObjectivesController = 92,
        FindTombController = 93,
        FirebaseController = 94,
        FirebaseRemoteSettingsController = 95,
        AnaliticsFirebaseController = 96,
        EventTokensController = 97,
        // TutorialProgressController = 98,
        OutLineBananaController = 99,
        LocalNotificationController = 100,
        StarterPackController = 101,
        StarterPackOpenController = 102,
        TeasureHuntTimerController = 103,
        TreasureHuntDigController = 104,
        InternetErrorController = 105,
        PlayerScenesController = 106,
        PlayerRespawnPointsController = 107,
        DungeonProgressController = 108,
        TreasureLootController = 109,
        Tmp__EpicLootChestController = 110,
        TreasureHuntRewardController = 111,
        EnvironmentTimeController = 112,
        //AnaliticsAppsFlyerController = 113,
        TreasureHuntLootController = 114,
        UnloadUnusedAssetsController = 115,
        WeaponAssetsLoadingController = 116,
        FixPlayerUndergroundController = 117,
        ChunksLoadersController = 118,
        ItemsOwnershipController = 119,
        ApplyItemController = 120,
        DropItemController = 121,
        ExpandInventoryController = 122,
        StatsEffectController = 123,
        InfoPanelController = 124,
        DivideItemsController = 125,
        FullInventoryController = 126,
        InventoryRepairingController = 127,
        CollectCellDataController = 128,
        PurchaseComplitedController = 129,
        DebugPrefabCreationController = 130,
        InventoryDragAndDropController = 131,
        BuildingSwitchButtonController = 132,
        BuildingHotBarController = 133,
        BuildingController = 134,
        PlacePartButtonController = 135,
        RotatePartButtonController = 136,
        BuildingProcessController = 137,
        ConstructionInfoController = 138,
        // EnablePlayerMovementController = 139, // removed because constructions located in core.
        SceneControllerActivationController = 140,
        TestWaterfallController = 141,
        ObjectivesProgressController = 142,
        ObjectivesWindowController = 143,
        FixOldObjectivesController = 144,
        EnoughSpaceController = 145,
        TargetsController = 146,
        ShelterUpgradeController = 147,
        ShelterAttackModeController = 148,
        EnemiesSpawnController = 149,
        QuestsSaveController = 150,
        CinematicController = 151,
        FirstKrakenController = 152,
        ShelterCombatController = 153,
        QuestsLifecycleController = 154,
        EnvironmentTransitionsController = 155,
        InventoryOperationsController = 156,
        ViewsStateController = 157,
        InputController = 158,
        EventSystemController = 159,
        SideQuestsSaveController = 160,
        HintsController = 161,
        DisableLightController = 162,
        DropContainerController = 163,
        TutorialObjectivesDarkScreenController = 164,
        PostTutorialController = 165,
        TutorialBuildingController = 166,
        TutorialResetToConstructionController = 167,
        OpenConstructionTutorialController = 168,
        FixMovePlayerFromDungeonController = 169,
        AudioEffectsController = 170,
        SpesialMessagesController = 171,
        //ConstructionTutorialController = 172,
        IgnoreSlopesController = 173,
        DisableCoreWaterController = 174,
        TeleporHomeController = 175,
        TokenTargetDisablerController = 176,
        SpawnTombAtPlayerRespawnPoint = 177,
        RespawnPlayerOnGameStart = 178,
        TombsController = 179,
        WorldObjectsController = 180,
        TutorialConstructionController = 181,
        CinematicCameraController = 182,
        PopupsTimeScaleController = 183,
        OutLineMineableTreeController = 184,
        TokenMineableTreeController = 185,
        TokenMineableStoneController = 186,
        TutorialBananaTokenController = 187,
        TutorialCoconatTokenController = 188,
        PerformanceStartCheckController = 189,
        UseCoconutTutorialController = 190,
        TutorialCoconutDarkController = 191,
        UseBananaTutorialController = 192,
        CraftPickaxeTutorialController = 193,
        TryEquipHookController = 194,
        TutorialCraftBoostAnaliticsController = 195,
        AnaliticsUserIDController = 196,
        UIBlockPlayerMoveController = 197,
        AttackButtonPulseController = 198,
        TutorialWoodExtractionController = 199,
        TutorialRocksExtractionController = 200,
        PlayerLocationController = 201,
        AnalyticsPlayerLocationController = 202,
        CheatController = 203,
        FixMainQuestDoNotMatchShipLevelController = 204,
        TutorialSkipController = 205,
        ActivateObjectivesViewController = 206,
        FishingLinkController = 207,
        ReplaceBuildingController = 208,
        ActivateReplaceButtonViewController = 209,
        ActivatePlaceReplacedButtonViewController = 210,
        LocationEncountersController = 211,
        DebugInputController = 212,
        EncountersDebugController = 213,
        TimeEncountersController = 214,
        SpecialEncountersController = 215,
        ActivateActivitiesLogViewController = 216,
        ActivateActivitiesLogButtonViewController = 217,
        EquipmentDurabilityController = 218,
        EquipmentBonusesController = 219,
        PlayerHealthRegenerationController = 220,
        VillageBuildingController = 221,
        ActivateWarmBarController = 222,
        VillageAttackController = 223,
        ActivateCitizensInfoViewController = 224,
        VillageCinematicsController = 225,
        ZoneMessageController = 226,
        VikingTutorialController = 227,
        ActivateHouseBuildingViewController = 228,
        ActivateDragItemViewController = 229,
        ActivateTaskViewController = 230,
        EnemiesModifierController = 231,
        ActivateLearnInputViewController = 232,
        WipeSaveController = 233,
        DeleteItemController = 234,
        ///CODE_GENERATION_IDS
    }
}
