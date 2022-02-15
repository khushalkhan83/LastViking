using Core;
using Core.Controllers;
using Game.Controllers.Controllers;
using Game.Controllers.Controllers.States;
using Game.Controllers.Controllers.States.Modificators;
using Game.Models;
using Game.Objectives;
using Game.Objectives.Controllers;
using Game.Providers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Controllers
{
    public partial class ControllersSystem : MonoBehaviour
    {   
        #region Data
#pragma warning disable 0649

        [SerializeField] private InjectionSystem _injectionSystem;
        [SerializeField] private ControllersModel _controllersModel;
        [SerializeField] private ControllerStatesProviderVariable _controllerStatesProvider;
        [SerializeField] private ControllerStatesProvider _controllerStatesProviderNew;

#pragma warning restore 0649
        #endregion

        public InjectionSystem InjectionSystem => _injectionSystem;
        public ControllersModel ControllersModel => _controllersModel;
        public ControllerStatesProvider ControllerStatesProviderNew => _controllerStatesProviderNew;

        private EditorGameSettings EditorGameSettings => EditorGameSettings.Instance;

        public Dictionary<ControllerID, IController> Controllers { get; } = new Dictionary<ControllerID, IController>();
        public Dictionary<ControllerID, IController> ControllersPool { get; } = new Dictionary<ControllerID, IController>();

        public Dictionary<ControllerID, Type> MapID2Type { get; } = new Dictionary<ControllerID, Type>
        {
            { ControllerID.PlaceItemController, typeof(IPlaceItemController) },
            { ControllerID.RotateButtonController, typeof(IRotateButtonController) },
            { ControllerID.DebugInteractController, typeof(IDebugInteractController) },
            { ControllerID.AimButtonController, typeof(IAimButtonController) },
            { ControllerID.PlayerThirstController, typeof(IPlayerThirstController) },
            { ControllerID.PlayerThirstDamagerController, typeof(IPlayerThirstDamagerController) },
            { ControllerID.PlayerHungerController, typeof(IPlayerHungerController) },
            { ControllerID.PlayerHungerDamagerController, typeof(IPlayerHungerDamagerController) },
            { ControllerID.PlayerRunController, typeof(IPlayerRunController) },
            { ControllerID.PlayerStaminaRegenerateController, typeof(IPlayerStaminaRegenerateController) },
            { ControllerID.PlayerAimController, typeof(IPlayerAimController) },
            { ControllerID.CursorController, typeof(ICursorController) },
            { ControllerID.LoseController, typeof(ILoseController) },
            { ControllerID.BloodEffectController, typeof(IBloodEffectController) },
            { ControllerID.AdvertisementsUAdsController, typeof(IAdvertisementsUAdsController) },
            { ControllerID.AdvertisementsAdmobController, typeof(IAdvertisementsAdmobController) },
            { ControllerID.VersionController, typeof(IVersionController) },
            { ControllerID.DiscordController, typeof(IDiscordController) },
            { ControllerID.StatisticsController, typeof(IStatisticsController) },
            { ControllerID.AnaliticsController, typeof(IAnaliticsController) },
            { ControllerID.CraftController, typeof(ICraftController) },
            { ControllerID.BackgroundPauseController, typeof(IBackgroundPauseController) },
            { ControllerID.RateMeController, typeof(IRateMeController) },
            { ControllerID.PlayerConsumeController, typeof(IPlayerConsumeController) },
            { ControllerID.GodModeViewController, typeof(IGodModeViewController) },
            { ControllerID.GameTimeController, typeof(IGameTimeController) },
            { ControllerID.PlayerDeathController, typeof(IPlayerDeathController) },
            { ControllerID.RepairController, typeof(IRepairController) },
            { ControllerID.PlayerSwicherMovementController, typeof(IPlayerSwicherMovementController) },
            { ControllerID.PlayerMovementController, typeof(IPlayerMovementController) },
            { ControllerID.PlayerSwitcherWeaponController, typeof(IPlayerSwitcherWeaponController) },
            { ControllerID.PlayerColdDamagerController, typeof(IPlayerColdDamagerController) },
            { ControllerID.PlayerColdController, typeof(IPlayerColdController) },
            { ControllerID.NetworkController, typeof(INetworkController) },
            { ControllerID.PlayerLandController, typeof(IPlayerLandController) },
            { ControllerID.TimingAdsController, typeof(ITimingAdsController) },
            { ControllerID.AndroidTimingAdsController, typeof(IAndroidTimingAdsController) },
            { ControllerID.AutosaveController, typeof(IAutosaveController) },
            { ControllerID.AutosaveInfoController, typeof(IAutosaveInfoController) },
            { ControllerID.PlayerBleedingDamagerController, typeof(IPlayerBleedingDamagerController) },
            { ControllerID.PlayerPoisonDamagerController, typeof (IPlayerPoisonDamagerController)},
            { ControllerID.WideScreenScaleController, typeof(IWideScreenScaleController) },
            { ControllerID.TutorialController, typeof(ITutorialController) },
            { ControllerID.JumpButtonController, typeof(IJumpButtonController) },
            { ControllerID.AttackButtonController, typeof(IAttackButtonController) },
            { ControllerID.TouchpadController, typeof(ITouchpadController) },
            { ControllerID.RunButtonController, typeof(IRunButtonController) },
            { ControllerID.CraftButtonController, typeof(ICraftButtonController) },
            { ControllerID.InventoryButtonController, typeof(IInventoryButtonController) },
            { ControllerID.HealthBarController, typeof(IHealthBarController) },
            { ControllerID.FoodBarController, typeof(IFoodBarController) },
            { ControllerID.WaterBarController, typeof(IWaterBarController) },
            { ControllerID.PlayerStatusController, typeof(IPlayerStatusController) },
            { ControllerID.HotBarController, typeof(IHotBarController) },
            { ControllerID.StaminaBarController, typeof(IStaminaBarController) },
            { ControllerID.SettingsButtonController, typeof(ISettingsButtonController) },
            { ControllerID.NotificationContainerController, typeof(INotificationContainerController) },
            { ControllerID.CoinsInfoController, typeof(ICoinsInfoController) },
            { ControllerID.ResourceMessagesController, typeof(IResourceMessagesController) },
            { ControllerID.DebugTimeButtonController, typeof(IDebugTimeButtonController) },
            { ControllerID.JoystickController, typeof(IJoystickController) },
            { ControllerID.StartUpController, typeof(IStartUpController) },
            { ControllerID.ObjectivesController, typeof(IObjectivesController) },
            { ControllerID.ObjectiveConditionsController, typeof(IObjectiveConditionsController) },
            { ControllerID.ObjectiveActionsController, typeof(IObjectiveActionsController) },
            { ControllerID.GameSparksController, typeof(IGameSparksController) },
            { ControllerID.TutorialDarkScreenController, typeof(ITutorialDarkScreenController) },
            { ControllerID.OpenObjectivesLogController, typeof(IEnableObjectivesButtonController) },
            // { ControllerID.NightInfoController, typeof(INightInfoController) },
            // { ControllerID.ObjectiveAttackProcessController, typeof(IObjectiveAttackProcessController) },
            { ControllerID.PlayerBoostStaminaController, typeof(IPlayerBoostStaminaController) },
            { ControllerID.EnableVerisonInfoView, typeof(IEnableVersionInfoViewController) },
            { ControllerID.EnableSaveInfoView, typeof(IEnableSaveInfoViewController) },
            { ControllerID.PlayerHealProcessController, typeof(IPlayerHealProcessController) },
            { ControllerID.PlayerEatProcessController, typeof(IPlayerEatProcessController) },
            { ControllerID.PlayerDrinkProcessController, typeof(IPlayerDrinkProcessController) },
            { ControllerID.PlayerObjectivesController, typeof(IPlayerObjectivesController) },
            { ControllerID.ObjectivesNotificationController, typeof(IObjectivesNotificationController) },
            { ControllerID.AddingInventoryItemsController, typeof(IAddingInventoryItemsController) },
            { ControllerID.OutLineMineableStoneController, typeof(ITutorialOutlineMineableStoneController) },
            { ControllerID.OutLineCoconutController, typeof(IOutLineCoconutController) },
            { ControllerID.OutLineBlueBarrelController, typeof(IOutLineBlueBarrelController) },
            { ControllerID.OutLineBagPickupController, typeof(IOutLineBagPickupController) },
            { ControllerID.AnaliticsDevtodevController, typeof(IAnaliticsDevtodevController) },
            { ControllerID.RealTimeController, typeof(IRealTimeController) },
            { ControllerID.HotBarRepairingController, typeof(IHotBarRepairingController) },
            { ControllerID.FishingInitController, typeof(IFishingInitController) },
            { ControllerID.FishingBiteController, typeof(IFishingBiteController) },
            { ControllerID.FishingPullController, typeof(IFishingPullController) },
            { ControllerID.MinigameStateController, typeof(IMinigameStateController) },
            { ControllerID.TokensController, typeof(ITokenController) },
            { ControllerID.PlayerEventObjectivesController, typeof(IPlayerEventObjectivesController) },
            { ControllerID.FindTombController, typeof(IFindTombController) },
            { ControllerID.FirebaseController, typeof(IFirebaseController) },
            { ControllerID.FirebaseRemoteSettingsController, typeof(IFirebaseRemoteSettingsController) },
            { ControllerID.AnaliticsFirebaseController, typeof(IAnaliticsFirebaseController) },
            { ControllerID.EventTokensController, typeof(IEventTokensController) },
            // { ControllerID.TutorialProgressController, typeof(ITutorialProgressController) },
            { ControllerID.OutLineBananaController, typeof(IOutLineBananaController)},
            { ControllerID.LocalNotificationController, typeof(ILocalNotificationsController) },
            { ControllerID.StarterPackController, typeof(IStarterPackController) },
            { ControllerID.StarterPackOpenController, typeof(IStarterPackOpenController) },
            { ControllerID.TeasureHuntTimerController, typeof(ITeasureHuntTimerController) },
            { ControllerID.TreasureHuntDigController, typeof(ITreasureHuntDigController) },
            { ControllerID.InternetErrorController, typeof(IInternetErrorController)},
            { ControllerID.PlayerScenesController, typeof(IPlayerScenesController)},
            { ControllerID.PlayerRespawnPointsController, typeof(IPlayerRespawnPointsController)},
            { ControllerID.DungeonProgressController, typeof(IDungeonProgressController)},
            { ControllerID.TreasureLootController, typeof(ITreasureLootController) },
            { ControllerID.Tmp__EpicLootChestController, typeof(IDeadManLootChestController) },
            { ControllerID.TreasureHuntRewardController, typeof(ITreasureHuntRewardController) },
            { ControllerID.EnvironmentTimeController, typeof(IEnvironmentTimeController) },
            { ControllerID.TreasureHuntLootController, typeof(ITreasureHuntLootController) },
            { ControllerID.UnloadUnusedAssetsController, typeof(IUnloadUnusedAssetsController) },
            { ControllerID.WeaponAssetsLoadingController, typeof(IWeaponAssetsLoadingController) },
            { ControllerID.FixPlayerUndergroundController, typeof(IFixPlayerUndergroundController) },
            { ControllerID.ChunksLoadersController, typeof(IChunksLoadersController) },
            { ControllerID.ItemsOwnershipController, typeof(IItemsOwnershipController) },
            { ControllerID.PurchaseComplitedController, typeof(IPurchaseComplitedController) },
            { ControllerID.ApplyItemController, typeof(IApplyItemController) },
            { ControllerID.DropItemController, typeof(IDropItemController) },
            { ControllerID.ExpandInventoryController, typeof(IExpandInventoryController) },
            { ControllerID.StatsEffectController, typeof(IStatsEffectController) },
            { ControllerID.InfoPanelController, typeof(IInfoPanelController) },
            { ControllerID.DivideItemsController, typeof(IDivideItemsController) },
            { ControllerID.FullInventoryController, typeof(IFullInventoryController) },
            { ControllerID.InventoryRepairingController, typeof(IInventoryRepairingController) },
            { ControllerID.CollectCellDataController, typeof(ICollectCellDataController) },
            { ControllerID.DebugPrefabCreationController, typeof(IDebugPrefabCreationController) },
            { ControllerID.InventoryDragAndDropController, typeof(IInventoryDragAndDropController) },
            { ControllerID.BuildingSwitchButtonController, typeof(IBuildingSwitchButtonController) },
            { ControllerID.BuildingHotBarController, typeof(IBuildingHotBarController) },
            { ControllerID.BuildingController, typeof(IBuildingController) },
            { ControllerID.PlacePartButtonController, typeof(IPlacePartButtonController) },
            { ControllerID.RotatePartButtonController, typeof(IRotatePartButtonController) },
            { ControllerID.BuildingProcessController, typeof(IBuildingProcessController) },
            { ControllerID.ConstructionInfoController, typeof(IConstructionInfoController) },
            { ControllerID.SceneControllerActivationController, typeof(ISceneControllerActivationController) },
            { ControllerID.TestWaterfallController, typeof(ITestWaterfallController) },
            { ControllerID.ObjectivesProgressController, typeof(IObjectivesProgressController) },
            { ControllerID.ObjectivesWindowController, typeof(IObjectivesWindowController) },
            { ControllerID.FixOldObjectivesController, typeof(IFixOldObjectivesController) },
            { ControllerID.EnoughSpaceController, typeof(IEnoughSpaceController) },
            { ControllerID.TargetsController, typeof(ITargetsController) },
            { ControllerID.ShelterUpgradeController, typeof(IShelterUpgradeController) },
            { ControllerID.ShelterAttackModeController, typeof(IShelterAttackModeController) },
            { ControllerID.EnemiesSpawnController, typeof(IEnemiesSpawnController) },
            { ControllerID.QuestsSaveController, typeof(IQuestsSaveController) },
            { ControllerID.FirstKrakenController, typeof(IFirstKrakenController) },
            { ControllerID.CinematicController, typeof(ICinematicController) },
            { ControllerID.ShelterCombatController, typeof(IShelterCombatController) },
            { ControllerID.QuestsLifecycleController, typeof(IQuestsLifecycleController) },
            { ControllerID.EnvironmentTransitionsController, typeof(IEnvironmentTransitionsController) },
            { ControllerID.InventoryOperationsController, typeof(IInventoryOperationsController) },
            { ControllerID.ViewsStateController, typeof(IViewsStateController) },
            { ControllerID.InputController, typeof(IInputController) },
            { ControllerID.EventSystemController, typeof(IEventSystemController) },
            { ControllerID.SideQuestsSaveController, typeof(ISideQuestsSaveController) },
            { ControllerID.HintsController, typeof(IHintsController) },
            { ControllerID.DisableLightController, typeof(IDisableLightController) },
            { ControllerID.DropContainerController, typeof(IDropContainerController) },
            { ControllerID.TutorialObjectivesDarkScreenController, typeof(ITutorialObjectivesDarkScreenController) },
            { ControllerID.PostTutorialController, typeof(IPostTutorialController) },
            { ControllerID.TutorialBuildingController, typeof(ITutorialBuildingController) },
            { ControllerID.TutorialResetToConstructionController, typeof(ITutorialResetToConstructionController) },
            { ControllerID.OpenConstructionTutorialController, typeof(IOpenConstructionTutorialController) },
            { ControllerID.FixMovePlayerFromDungeonController, typeof(IFixMovePlayerFromDungeonController) },
            { ControllerID.AudioEffectsController, typeof(IAudioEffectsController) },
            { ControllerID.SpesialMessagesController, typeof(ISpesialMessagesController) },
            { ControllerID.IgnoreSlopesController, typeof(IIgnoreSlopesController) },
            { ControllerID.DisableCoreWaterController, typeof(IDisableCoreWaterController) },
            { ControllerID.TeleporHomeController, typeof(ITeleporHomeController) },
            { ControllerID.TokenTargetDisablerController, typeof(ITokenTargetDisablerController) },
            { ControllerID.SpawnTombAtPlayerRespawnPoint, typeof(ISpawnTombAtPlayerRespawnPoint) },
            { ControllerID.RespawnPlayerOnGameStart, typeof(IRespawnPlayerOnGameStart) },
            { ControllerID.TombsController, typeof(ITombsController) },
            { ControllerID.WorldObjectsController, typeof(IWorldObjectsController) },
            { ControllerID.TutorialConstructionController, typeof(ITutorialConstructionController) },
            { ControllerID.CinematicCameraController, typeof(ICinematicCameraController) },
            { ControllerID.PopupsTimeScaleController, typeof(IPopupsTimeScaleController) },
            { ControllerID.OutLineMineableTreeController, typeof(ITutorialOutlineMineableTreeController) },
            { ControllerID.TokenMineableTreeController, typeof(ITokenMineableTreeController) },
            { ControllerID.TokenMineableStoneController, typeof(ITokenMineableStoneController) },
            { ControllerID.TutorialBananaTokenController, typeof(ITutorialBananaTokenController) },
            { ControllerID.TutorialCoconatTokenController, typeof(ITutorialCoconatTokenController) },
            { ControllerID.PerformanceStartCheckController, typeof(IPerformanceStartCheckController) },
            { ControllerID.UseCoconutTutorialController, typeof(IUseCoconutTutorialController) },
            { ControllerID.TutorialCoconutDarkController, typeof(ITutorialCoconutDarkController) },
            { ControllerID.UseBananaTutorialController, typeof(IUseBananaTutorialController) },
            { ControllerID.CraftPickaxeTutorialController, typeof(ICraftPickaxeTutorialController) },
            { ControllerID.TryEquipHookController, typeof(ITryEquipHookController) },
            { ControllerID.TutorialCraftBoostAnaliticsController, typeof(ITutorialCraftBoostAnaliticsController) },
            { ControllerID.AnaliticsUserIDController, typeof(IAnaliticsUserIDController) },
            { ControllerID.UIBlockPlayerMoveController, typeof(IUIBlockPlayerMoveController) },
            { ControllerID.AttackButtonPulseController, typeof(IAttackButtonPulseController) },
            { ControllerID.TutorialWoodExtractionController, typeof(ITutorialWoodExtractionController) },
            { ControllerID.TutorialRocksExtractionController, typeof(ITutorialRocksExtractionController) },
            { ControllerID.PlayerLocationController, typeof(IPlayerLocationController) },
            { ControllerID.AnalyticsPlayerLocationController, typeof(IAnalyticsPlayerLocationController) },
            { ControllerID.CheatController, typeof(ICheatController) },
            { ControllerID.FixMainQuestDoNotMatchShipLevelController, typeof(IFixMainQuestDoNotMatchShipLevelController) },
            { ControllerID.TutorialSkipController, typeof(ITutorialSkipController) },
            { ControllerID.ActivateObjectivesViewController, typeof(IActivateObjectivesViewController) },
            { ControllerID.FishingLinkController, typeof(IFishingLinkController) },
            { ControllerID.ReplaceBuildingController, typeof(IReplaceBuildingController) },
            { ControllerID.ActivateReplaceButtonViewController, typeof(IActivateReplaceButtonViewController) },
            { ControllerID.ActivatePlaceReplacedButtonViewController, typeof(IActivatePlaceReplacedButtonViewController) },
            { ControllerID.LocationEncountersController, typeof(ILocationEncountersController) },
            { ControllerID.DebugInputController, typeof(IDebugInputController) },
            { ControllerID.EncountersDebugController, typeof(IEncountersDebugController) },
            { ControllerID.TimeEncountersController, typeof(ITimeEncountersController) },
            { ControllerID.SpecialEncountersController, typeof(ISpecialEncountersController) },
            { ControllerID.ActivateActivitiesLogViewController, typeof(IActivateActivitiesLogViewController) },
            { ControllerID.ActivateActivitiesLogButtonViewController, typeof(IActivateActivitiesLogButtonViewController) },
            { ControllerID.EquipmentDurabilityController, typeof(IEquipmentDurabilityController) },
            { ControllerID.EquipmentBonusesController, typeof(IEquipmentBonusesController) },
            { ControllerID.PlayerHealthRegenerationController, typeof(IPlayerHealthRegenerationController) },
            { ControllerID.VillageBuildingController, typeof(IVillageBuildingController) },
            { ControllerID.ActivateWarmBarController, typeof(IActivateWarmBarController) },
            { ControllerID.VillageAttackController, typeof(IVillageAttackController) },
            { ControllerID.ActivateCitizensInfoViewController, typeof(IActivateCitizensInfoViewController) },
            { ControllerID.VillageCinematicsController, typeof(IVillageCinematicsController) },
            { ControllerID.ZoneMessageController, typeof(IZoneMessageController) },
            { ControllerID.VikingTutorialController, typeof(IVikingTutorialController) },
            { ControllerID.ActivateHouseBuildingViewController, typeof(IActivateHouseBuildingViewController) },
            { ControllerID.ActivateDragItemViewController, typeof(IActivateDragItemViewController) },
            { ControllerID.ActivateTaskViewController, typeof(IActivateTaskViewController) },
            { ControllerID.EnemiesModifierController, typeof(IEnemiesModifierController) },
            { ControllerID.ActivateLearnInputViewController, typeof(IActivateLearnInputViewController) },
            { ControllerID.WipeSaveController, typeof(IWipeSaveController) },
            { ControllerID.DeleteItemController, typeof(IDeleteItemController) },
            { ControllerID.TimeInGameController, typeof(ITimeInGameController) },
            { ControllerID.PlayerCamerasController, typeof(IPlayerCamerasController) },
            ///CODE_GENERATION_IDS
        };

        public Dictionary<Type, Type> Map { get; } = new Dictionary<Type, Type>
        {
            { typeof(IPlaceItemController), typeof(PlaceItemController) },
            { typeof(IRotateButtonController), typeof(RotateButtonController) },
            { typeof(IDebugInteractController), typeof(DebugInteractController) },
            { typeof(IAimButtonController), typeof(AimButtonController) },
            { typeof(IPlayerThirstController), typeof(PlayerThirstController) },
            { typeof(IPlayerThirstDamagerController), typeof(PlayerThirstDamagerController) },
            { typeof(IPlayerHungerController), typeof(PlayerHungerController) },
            { typeof(IPlayerHungerDamagerController), typeof(PlayerHungerDamagerController) },
            { typeof(IPlayerRunController), typeof(PlayerRunController) },
            { typeof(IPlayerStaminaRegenerateController), typeof(PlayerStaminaRegenerateController) },
            { typeof(IPlayerAimController), typeof(PlayerAimController) },
            { typeof(ICursorController), typeof(CursorController) },
            { typeof(ILoseController), typeof(LoseController) },
            { typeof(IBloodEffectController), typeof(BloodEffectController) },
            { typeof(IAdvertisementsUAdsController), typeof(AdvertisementsUAdsController) },
            { typeof(IAdvertisementsAdmobController), typeof(AdvertisementsAdmobController) },
            { typeof(IVersionController), typeof(VersionController) },
            { typeof(IDiscordController), typeof(DiscordController) },
            { typeof(IStatisticsController), typeof(StatisticsController) },
            { typeof(IAnaliticsController), typeof(AnaliticsController) },
            { typeof(ICraftController), typeof(CraftController) },
            { typeof(IBackgroundPauseController), typeof(BackgroundPauseController) },
            { typeof(IRateMeController), typeof(RateMeController) },
            { typeof(IPlayerConsumeController), typeof(PlayerConsumeController) },
            { typeof(IGodModeViewController), typeof(GodModeViewController) },
            { typeof(IGameTimeController), typeof(GameTimeController) },
            { typeof(IPlayerDeathController), typeof(PlayerDeathController) },
            { typeof(IRepairController), typeof(RepairController) },
            { typeof(IPlayerSwicherMovementController), typeof(PlayerSwicherMovementController) },
            { typeof(IPlayerMovementController), typeof(PlayerMovementController) },
            { typeof(IPlayerSwitcherWeaponController), typeof(PlayerSwitcherWeaponController) },
            { typeof(IPlayerColdDamagerController), typeof(PlayerColdDamagerController) },
            { typeof(IPlayerColdController), typeof(PlayerColdController) },
            { typeof(INetworkController), typeof(NetworkController) },
            { typeof(IPlayerLandController), typeof(PlayerLandController) },
            { typeof(ITimingAdsController), typeof(TimingAdsController) },
            { typeof(IAndroidTimingAdsController), typeof(AndroidTimingAdsController) },
            { typeof(IAutosaveController), typeof(AutosaveController) },
            { typeof(IAutosaveInfoController), typeof(AutosaveInfoController) },
            { typeof(IPlayerBleedingDamagerController), typeof(PlayerBleedingDamagerController) },
            { typeof(IPlayerPoisonDamagerController), typeof(PlayerPoisonDamagerController)},
            { typeof(IWideScreenScaleController), typeof(WideScreenScaleController) },
            { typeof(ITutorialController), typeof(TutorialController) },
            { typeof(IJumpButtonController), typeof(JumpButtonController) },
            { typeof(IAttackButtonController), typeof(AttackButtonController) },
            { typeof(ITouchpadController), typeof(TouchpadController) },
            { typeof(IRunButtonController), typeof(RunButtonController) },
            { typeof(ICraftButtonController), typeof(CraftButtonController) },
            { typeof(IInventoryButtonController), typeof(InventoryButtonController) },
            { typeof(IHealthBarController), typeof(HealthBarController) },
            { typeof(IFoodBarController), typeof(FoodBarController) },
            { typeof(IWaterBarController), typeof(WaterBarController) },
            { typeof(IPlayerStatusController), typeof(PlayerStatusController) },
            { typeof(IHotBarController), typeof(HotBarController) },
            { typeof(IStaminaBarController), typeof(StaminaBarController) },
            { typeof(ISettingsButtonController), typeof(SettingsButtonController) },
            { typeof(INotificationContainerController), typeof(NotificationContainerController) },
            { typeof(ICoinsInfoController), typeof(CoinsInfoController) },
            { typeof(IResourceMessagesController), typeof(ResourceMessagesController) },
            { typeof(IDebugTimeButtonController), typeof(DebugTimeButtonController) },
            { typeof(IJoystickController), typeof(JoystickController) },
            { typeof(IStartUpController), typeof(StartUpController) },
            { typeof(IObjectivesController), typeof(ObjectivesController) },
            { typeof(IObjectiveConditionsController), typeof(ObjectiveConditionsController) },
            { typeof(IObjectiveActionsController), typeof(ObjectiveActionsController) },
            { typeof(IGameSparksController), typeof(GameSparksController) },
            { typeof(ITutorialDarkScreenController), typeof(TutorialDarkScreenController) },
            { typeof(IEnableObjectivesButtonController), typeof(EnableObjectivesButtonController) },
            // { typeof(INightInfoController), typeof(NightInfoController) },
            // { typeof(IObjectiveAttackProcessController), typeof(ObjectiveAttackProcessController) },
            { typeof(IPlayerBoostStaminaController), typeof(PlayerBoostStaminaController) },
            { typeof(IEnableVersionInfoViewController), typeof(EnableVersionInfoViewController) },
            { typeof(IEnableSaveInfoViewController), typeof(EnableSaveInfoViewController) },
            { typeof(IPlayerHealProcessController), typeof(PlayerHealProcessController) },
            { typeof(IPlayerEatProcessController), typeof(PlayerEatProcessController) },
            { typeof(IPlayerDrinkProcessController), typeof(PlayerDrinkProcessController) },
            { typeof(IPlayerObjectivesController), typeof(PlayerObjectivesController) },
            { typeof(IObjectivesNotificationController), typeof(ObjectivesNotificationController) },
            { typeof(IAddingInventoryItemsController), typeof(AddingInventoryItemsController) },
            { typeof(ITutorialOutlineMineableStoneController), typeof(TutorialOutlineMineableStoneController) },
            { typeof(IOutLineCoconutController), typeof(OutLineCoconutController) },
            { typeof(IOutLineBlueBarrelController), typeof(OutLineBlueBarrelController) },
            { typeof(IOutLineBagPickupController), typeof(OutLineBagPickupController) },
            { typeof(IAnaliticsDevtodevController), typeof(AnaliticsDevtodevController) },
            { typeof(IRealTimeController), typeof(RealTimeController) },
            { typeof(IHotBarRepairingController), typeof(HotBarRepairingController) },
            { typeof(IFishingInitController), typeof(FishingInitController) },
            { typeof(IFishingBiteController), typeof(FishingBiteController) },
            { typeof(IFishingPullController), typeof(FishingPullController) },
            { typeof(IMinigameStateController), typeof(MinigameStateController) },
            { typeof(ITokenController), typeof(TokenController) },
            { typeof(IPlayerEventObjectivesController), typeof(PlayerEventObjectivesController) },
            { typeof(IFindTombController), typeof(FindTombController) },
            { typeof(IFirebaseController), typeof(FirebaseController) },
            { typeof(IFirebaseRemoteSettingsController), typeof(FirebaseRemoteSettingsController) },
            { typeof(IAnaliticsFirebaseController), typeof(AnaliticsFirebaseController) },
            { typeof(IEventTokensController), typeof(EventTokensController) },
            // { typeof(ITutorialProgressController), typeof(TutorialProgressController) },
            { typeof(IOutLineBananaController), typeof(OutLineBananaController) },
            { typeof(ILocalNotificationsController), typeof(LocalNotificationsController) },
            { typeof(IStarterPackController), typeof(StarterPackController) },
            { typeof(IStarterPackOpenController), typeof(StarterPackOpenController) },
            { typeof(ITeasureHuntTimerController),typeof(TeasureHuntTimerController) },
            { typeof(ITreasureHuntDigController), typeof(TreasureHuntDigController) },
            { typeof(IInternetErrorController), typeof(InternetErrorController) },
            { typeof(IPlayerScenesController), typeof(PlayerScenesController) },
            { typeof(IPlayerRespawnPointsController), typeof(PlayerRespawnPointsController) },
            { typeof(IDungeonProgressController), typeof(DungeonProgressController) },
            { typeof(ITreasureLootController), typeof(TreasureLootController) },
            { typeof(IDeadManLootChestController), typeof(DeadManLootChestController) },
            { typeof(ITreasureHuntRewardController), typeof(TreasureHuntRewardController) },
            { typeof(IEnvironmentTimeController), typeof(EnvironmentTimeController) },
            { typeof(ITreasureHuntLootController), typeof(TreasureHuntLootController) },
            { typeof(IUnloadUnusedAssetsController), typeof(UnloadUnusedAssetsController) },
            { typeof(IWeaponAssetsLoadingController), typeof(WeaponAssetsLoadingController) },
            { typeof(IFixPlayerUndergroundController), typeof(FixPlayerUndergroundController) },
            { typeof(IChunksLoadersController), typeof(ChunksLoadersController) },
            { typeof(IItemsOwnershipController), typeof(ItemsOwnershipController) },
            { typeof(IPurchaseComplitedController), typeof(PurchaseComplitedController) },
            { typeof(IApplyItemController), typeof(ApplyItemController) },
            { typeof(IDropItemController), typeof(DropItemController) },
            { typeof(IExpandInventoryController), typeof(ExpandInventoryController) },
            { typeof(IStatsEffectController), typeof(StatsEffectController) },
            { typeof(IInfoPanelController), typeof(InfoPanelController) },
            { typeof(IDivideItemsController), typeof(DivideItemsController) },
            { typeof(IFullInventoryController), typeof(FullInventoryController) },
            { typeof(IInventoryRepairingController), typeof(InventoryRepairingController) },
            { typeof(ICollectCellDataController), typeof(CollectCellDataController) },
            { typeof(IDebugPrefabCreationController), typeof(DebugPrefabCreationController) },
            { typeof(IInventoryDragAndDropController), typeof(InventoryDragAndDropController) },
            { typeof(IBuildingSwitchButtonController), typeof(BuildingSwitchButtonController) },
            { typeof(IBuildingHotBarController), typeof(BuildingHotBarController) },
            { typeof(IBuildingController), typeof(BuildingController) },
            { typeof(IPlacePartButtonController), typeof(PlacePartButtonController) },
            { typeof(IRotatePartButtonController), typeof(RotatePartButtonController) },
            { typeof(IBuildingProcessController), typeof(BuildingProcessController) },
            { typeof(IConstructionInfoController), typeof(ConstructionInfoController) },
            { typeof(ISceneControllerActivationController), typeof(SceneControllerActivationController) },
            { typeof(ITestWaterfallController), typeof(TestWaterfallController) },
            { typeof(IObjectivesProgressController), typeof(ObjectivesProgressController) },
            { typeof(IObjectivesWindowController), typeof(ObjectivesWindowController) },
            { typeof(IFixOldObjectivesController), typeof(FixOldObjectivesController) },
            { typeof(IEnoughSpaceController), typeof(EnoughSpaceController) },
            { typeof(ITargetsController), typeof(TargetsController) },
            { typeof(IShelterUpgradeController), typeof(ShelterUpgradeController) },
            { typeof(IShelterAttackModeController), typeof(ShelterAttackModeController) },
            { typeof(IEnemiesSpawnController), typeof(EnemiesSpawnController) },
            { typeof(IQuestsSaveController), typeof(QuestsSaveController) },
            { typeof(IFirstKrakenController), typeof(FirstKrakenController) },
            { typeof(ICinematicController), typeof(CinematicController) },
            { typeof(IShelterCombatController), typeof(ShelterCombatController) },
            { typeof(IQuestsLifecycleController), typeof(QuestsLifecycleController) },
            { typeof(IEnvironmentTransitionsController), typeof(EnvironmentTransitionsController) },
            { typeof(IInventoryOperationsController), typeof(InventoryOperationsController) },
            { typeof(IViewsStateController), typeof(ViewsStateController) },
            { typeof(IInputController), typeof(InputController) },
            { typeof(IEventSystemController), typeof(EventSystemController) },
            { typeof(ISideQuestsSaveController), typeof(SideQuestsSaveController) },
            { typeof(IHintsController), typeof(HintsController) },
            { typeof(IDisableLightController), typeof(DisableLightController) },
            { typeof(IDropContainerController), typeof(DropContainerController) },
            { typeof(ITutorialObjectivesDarkScreenController), typeof(TutorialObjectivesDarkScreenController) },
            { typeof(IPostTutorialController), typeof(PostTutorialController) },
            { typeof(ITutorialBuildingController), typeof(TutorialBuildingController) },
            { typeof(ITutorialResetToConstructionController), typeof(TutorialResetToConstructionController) },
            { typeof(IOpenConstructionTutorialController), typeof(OpenConstructionTutorialController) },
            { typeof(IFixMovePlayerFromDungeonController), typeof(FixMovePlayerFromDungeonController) },
            { typeof(IAudioEffectsController), typeof(AudioEffectsController) },
            { typeof(ISpesialMessagesController), typeof(SpesialMessagesController) },
            { typeof(IIgnoreSlopesController), typeof(IgnoreSlopesController) },
            { typeof(IDisableCoreWaterController), typeof(DisableCoreWaterController) },
            { typeof(ITeleporHomeController), typeof(TeleporHomeController) },
            { typeof(ITokenTargetDisablerController), typeof(TokenTargetDisablerController) },
            { typeof(ISpawnTombAtPlayerRespawnPoint), typeof(SpawnTombAtPlayerRespawnPoint) },
            { typeof(IRespawnPlayerOnGameStart), typeof(RespawnPlayerOnGameStart) },
            { typeof(IWorldObjectsController), typeof(WorldObjectsController) },
            { typeof(ITombsController), typeof(TombsController) },
            { typeof(ITutorialConstructionController), typeof(TutorialConstructionController) },
            { typeof(ICinematicCameraController), typeof(CinematicCameraController) },
            { typeof(IPopupsTimeScaleController), typeof(PopupsTimeScaleController) },
            { typeof(ITutorialOutlineMineableTreeController), typeof(TutorialOutlineMineableTreeController) },
            { typeof(ITokenMineableTreeController), typeof(TokenMineableTreeController) },
            { typeof(ITokenMineableStoneController), typeof(TokenMineableStoneController) },
            { typeof(ITutorialBananaTokenController), typeof(TutorialBananaTokenController) },
            { typeof(ITutorialCoconatTokenController), typeof(TutorialCoconatTokenController) },
            { typeof(IPerformanceStartCheckController), typeof(PerformanceStartCheckController) },
            { typeof(IUseCoconutTutorialController), typeof(UseCoconutTutorialController) },
            { typeof(ITutorialCoconutDarkController), typeof(TutorialSimpleDarkController) },
            { typeof(IUseBananaTutorialController), typeof(UseBananaTutorialController) },
            { typeof(ICraftPickaxeTutorialController), typeof(CraftPickaxeTutorialController) },
            { typeof(ITryEquipHookController), typeof(TryEquipHookController) },
            { typeof(ITutorialCraftBoostAnaliticsController), typeof(TutorialCraftBoostAnaliticsController) },
            { typeof(IAnaliticsUserIDController), typeof(AnaliticsUserIDController) },
            { typeof(IUIBlockPlayerMoveController), typeof(UIBlockPlayerMoveController) },
            { typeof(IAttackButtonPulseController), typeof(AttackButtonPulseController) },
            { typeof(ITutorialWoodExtractionController), typeof(TutorialWoodExtractionController) },
            { typeof(ITutorialRocksExtractionController), typeof(TutorialRocksExtractionController) },
            { typeof(IPlayerLocationController), typeof(PlayerLocationController) },
            { typeof(IAnalyticsPlayerLocationController), typeof(AnalyticsPlayerLocationController) },
            { typeof(ICheatController), typeof(CheatController) },
            { typeof(IFixMainQuestDoNotMatchShipLevelController), typeof(FixMainQuestDoNotMatchShipLevelController) },
            { typeof(ITutorialSkipController), typeof(TutorialSkipController) },
            { typeof(IActivateObjectivesViewController), typeof(ActivateObjectivesViewController) },
            { typeof(IFishingLinkController), typeof(FishingLinkController) },
            { typeof(IReplaceBuildingController), typeof(ReplaceBuildingController) },
            { typeof(IActivateReplaceButtonViewController), typeof(ActivateReplaceButtonViewController) },
            { typeof(IActivatePlaceReplacedButtonViewController), typeof(ActivatePlaceReplacedButtonViewController) },
            { typeof(ILocationEncountersController), typeof(LocationEncountersController) },
            { typeof(IDebugInputController), typeof(DebugInputController) },
            { typeof(IEncountersDebugController), typeof(EncountersDebugController) },
            { typeof(ITimeEncountersController), typeof(TimeEncountersController) },
            { typeof(ISpecialEncountersController), typeof(SpecialEncountersController) },
            { typeof(IActivateActivitiesLogViewController), typeof(ActivateActivitiesLogViewController) },
            { typeof(IActivateActivitiesLogButtonViewController), typeof(ActivateActivitiesLogButtonViewController) },
            { typeof(IEquipmentDurabilityController), typeof(EquipmentDurabilityController) },
            { typeof(IEquipmentBonusesController), typeof(EquipmentBonusesController) },
            { typeof(IPlayerHealthRegenerationController), typeof(PlayerHealthRegenerationController) },
            { typeof(IVillageBuildingController), typeof(VillageBuildingController) },
            { typeof(IActivateWarmBarController), typeof(ActivateWarmBarController) },
            { typeof(IVillageAttackController), typeof(VillageAttackController) },
            { typeof(IActivateCitizensInfoViewController), typeof(ActivateCitizensInfoViewController) },
            { typeof(IVillageCinematicsController), typeof(VillageCinematicsController) },
            { typeof(IZoneMessageController), typeof(ZoneMessageController) },
            { typeof(IVikingTutorialController), typeof(VikingTutorialController) },
            { typeof(IActivateHouseBuildingViewController), typeof(ActivateHouseBuildingViewController) },
            { typeof(IActivateDragItemViewController), typeof(ActivateDragItemViewController) },
            { typeof(IActivateTaskViewController), typeof(ActivateTaskViewController) },
            { typeof(IEnemiesModifierController), typeof(EnemiesModifierController) },
            { typeof(IActivateLearnInputViewController), typeof(ActivateLearnInputViewController) },
            { typeof(IWipeSaveController), typeof(WipeSaveController) },
            { typeof(IDeleteItemController), typeof(DeleteItemController) },
            { typeof(ITimeInGameController), typeof(TimeInGameController) },
            { typeof(IPlayerCamerasController), typeof(PlayerCamerasController) },
            ///CODE_GENERATION_INTERFACES
        };

        #region MonoBehaviour
        private void OnEnable()
        {
            InitStateMachines();
            
            // just to set current state right
            ControllersModel.ApplyState(ControllersModel.PreInitControllersStateID);

            ControllersModel.OnApplyState += OnApplyStateHandler;
            ControllersModel.OnApplyModificator += ApplyModificatorHandler;


            ControllersModel.ApplyState(ControllersModel.PreInitControllersStateID);
            
            // time for models and monobehaviours to call their callbacks (Awake, onEnable, Start ...)
            DoActionAfterFrame(() => {
                ControllersModel.ApplyState(ControllersModel.StartControllersStateID);
            });
        }

        private void OnDisable()
        {
            // TODO: add (return back) deactivation All ? mb in another form
            ControllersModel.OnApplyState -= OnApplyStateHandler;
            ControllersModel.OnApplyModificator -= ApplyModificatorHandler;
        }

        private void OnApplicationQuit() => DeactivationAll();
        
        #endregion


        private void DeactivationAll()
        {
            foreach (var controllerID in ControllersModel.ActiveControllerIDs.ToArray())
            {
                Deactivation(controllerID);
            }
        }

        private IEnumerator StartUpControllers()
        {
            ControllersModel.ApplyState(ControllersModel.ControllersStateID);
            yield return null;
            ControllersModel.ApplyState(ControllersModel.StartControllersStateID);
        }

        private IEnumerable<ControllerID> _allControllerIDs;
        private IEnumerable<ControllerID> AllControllerIDs => _allControllerIDs ?? (_allControllerIDs = ((ControllerID[])Enum.GetValues(typeof(ControllerID))).Skip(1).ToArray());


        private ControllersStateMachine ControllersStateMachine {get; set;}
        private ModificatorStateMachine<TutorialModificator> TutorialModificatorsStateMachine {get; set;}
        private ModificatorStateMachine<LocationModificator> LocationModificatorsStateMachine {get; set;}
        private ModificatorStateMachine<MinigameModificator> MinigameModificatorsStateMachine {get; set;}


        //FIXME: Reinit on deactivation. Test for Reset game
        private void InitStateMachines()
        {
            ControllersStateMachine = new ControllersStateMachine(ProcessModificator);
            
            TutorialModificatorsStateMachine = new ModificatorStateMachine<TutorialModificator>(Activation,Deactivation,Startup);
            LocationModificatorsStateMachine = new ModificatorStateMachine<LocationModificator>(ConditionalActivation,Deactivation,Startup);
            MinigameModificatorsStateMachine = new ModificatorStateMachine<MinigameModificator>(ConditionalActivation,ConditionalDeactivation,ConditionalStartup);
        }

        private void ConditionalActivation(ControllerID controllerID)
        {
            bool ignore = TutorialModificatorsStateMachine.DeActivatedController(controllerID);
            if(ignore)
            {
                Debug.Log($"<color=red>Tutorial modificator blocked {controllerID} activation </color>");
                return;
            }

            Activation(controllerID);

        }

        private void ConditionalDeactivation(ControllerID controllerID)
        {
            bool ignore = TutorialModificatorsStateMachine.ActivatedController(controllerID);
            if(ignore)
            {
                Debug.Log($"<color=red>Tutorial modificator blocked {controllerID} deactivation </color>");
                return;
            }

            Deactivation(controllerID);
        }

        private void ConditionalStartup(ControllerID controllerID)
        {
            bool ignore = TutorialModificatorsStateMachine.DeActivatedController(controllerID);
            if(ignore)
            {
                Debug.Log($"<color=red>Tutorial modificator blocked {controllerID} startup </color>");
                return;
            }

            Startup(controllerID);
        }

        public void ApplyModificatorHandler(ModificatorBase modificator)
        {
            // Debug.Log($"<color=red>Apply modificator {modificator} </color>");
            switch(modificator)
            {
                case LocationModificator locationModificator:
                    LocationModificatorsStateMachine.SetState(locationModificator);
                    return;
                case MinigameModificator minigameModificator:
                    MinigameModificatorsStateMachine.SetState(minigameModificator);
                    return;
                case TutorialModificator tutorialModificator:
                    TutorialModificatorsStateMachine.SetState(tutorialModificator);
                    return;
                default:
                    throw new Exception();
            }
        }

        private void OnApplyStateHandler(ControllersStateID controllerStateID)
        {   
            controllerStateID.Log("<color=orange>Apply: </color>");
            if (controllerStateID == ControllersStateID.None)
            {
                DeactivationAll();
            }
            else
            {
                IControllersState state = ControllerStatesProviderNew[controllerStateID];
                ControllersStateMachine.SetState(state);
            }
        }


        ///

        private void ProcessModificator(ControllerProcessingID controllerProcessingID, IEnumerable<ControllerID> controllerIDs)
        {
            var strategy = GetControllerProessing(controllerProcessingID);
            strategy.Invoke(controllerIDs);
        }

        ///

        private Action<IEnumerable<ControllerID>> GetControllerProessing(ControllerProcessingID controllerProcessingID)
        {
            switch (controllerProcessingID)
            {
                case ControllerProcessingID.EnableOnlyThese:
                    return ControllerProcessingEnableOnlyThese;
                case ControllerProcessingID.EnableNotThese:
                    return ControllerProcessingEnableNotThese;
                case ControllerProcessingID.AddThese:
                    return ControllerProcessingAddThese;
                case ControllerProcessingID.RemoveThese:
                    return ControllerProcessingRemoveThese;
            }

            throw new Exception();
        }

        private void ControllerProcessingEnableNotThese(IEnumerable<ControllerID> controllerIDs)
            {
            foreach (var controllerID in controllerIDs.Intersect(ControllersModel.ActiveControllerIDs).ToArray())
            {
                Deactivation(controllerID);
            }
            var activationIDs = AllControllerIDs.Except(controllerIDs).Except(ControllersModel.ActiveControllerIDs).ToArray();
            foreach (var controllerID in activationIDs)
            {
                Activation(controllerID);
            }
            foreach (var controllerID in activationIDs)
            {
                Startup(controllerID);
            }
        }

        private void ControllerProcessingEnableOnlyThese(IEnumerable<ControllerID> controllerIDs)
        {
            foreach (var controllerID in ControllersModel.ActiveControllerIDs.Except(controllerIDs).ToArray())
            {
                Deactivation(controllerID);
            }
            var activationIDs = controllerIDs.Except(ControllersModel.ActiveControllerIDs).ToArray();
            foreach (var controllerID in activationIDs)
            {
                Activation(controllerID);
            }
            foreach (var controllerID in activationIDs)
            {
                Startup(controllerID);
            }
        }

        private void ControllerProcessingAddThese(IEnumerable<ControllerID> controllerIDs)
        {
            foreach (var controllerID in controllerIDs)
            {
                Activation(controllerID);
            }
            foreach (var controllerID in controllerIDs)
            {
                Startup(controllerID);
            }
        }

        private void ControllerProcessingRemoveThese(IEnumerable<ControllerID> controllerIDs)
        {
            foreach (var controllerID in controllerIDs)
            {
                Deactivation(controllerID);
            }
        }

        private void Activation(ControllerID controllerID)
        {
            if(ControllersModel.ActiveControllerIDs.Contains(controllerID)) return;

            var controller = GetNew(controllerID);
            try
            {
                if(EditorGameSettings.ShowControllersSystemLogs)
                {
                    controllerID.Log("Enable");
                }
                controller.Enable();
            }
            catch (Exception e)
            {
                e.Error($"Exception on Enable controller {controllerID}: ");
            }
            Controllers[controllerID] = controller;
            ControllersModel.ActiveControllerIDs.Add(controllerID);
        }

        private void Deactivation(ControllerID controllerID)
        {
            if(!ControllersModel.ActiveControllerIDs.Contains(controllerID)) return;
            
            var controller = Controllers[controllerID];
            // controllerID.Log("Disable");
            controller.Disable();
            Controllers.Remove(controllerID);
            ControllersModel.ActiveControllerIDs.Remove(controllerID);
            ControllersModel.StartedControllersIDs.Remove(controllerID);

            if (controller is MonoBehaviour monoBehaviour)
            {
                Destroy(monoBehaviour);
            }
            else
            {
                ControllersPool[controllerID] = controller;
            }
        }

        private void Startup(ControllerID controllerID)
        {
            bool activated = ControllersModel.ActiveControllerIDs.Contains(controllerID);
            bool started = ControllersModel.StartedControllersIDs.Contains(controllerID);

            if(activated && started)
            {
                Debug.LogError($"Blocked {controllerID}. Startup in already started controller");
                return;
            }

            if(activated && !started)
            {
                try 
                { 
                    // controllerID.Log("Startup"); 
                    Controllers[controllerID].Start(); 
                    ControllersModel.StartedControllersIDs.Add(controllerID);
                }
                catch (Exception e) { e.Error($"Exception on Start controller {controllerID}: "); }
            }
        }

        private IController GetNew(ControllerID controllerID)
        {
            if (ControllersPool.TryGetValue(controllerID, out var controller))
            {
                ControllersPool.Remove(controllerID);
                return controller;
            }

            return Create(controllerID);
        }

        private static Type TypeMonoBehavior { get; } = typeof(MonoBehaviour);
        private IController Create(ControllerID controllerID)
        {
            System.Type type = null;
            try
            {
                type = MapID2Type[controllerID];
            }
            catch
            {
                Debug.LogError("Error here");
            }
            var typeInstance = Map[type];
            var controller = CreateController(typeInstance);

            InjectionSystem.Inject(controller);
            return controller;
        }

        private IController CreateController(Type type)
        {
            if (TypeMonoBehavior.IsAssignableFrom(type))
            {
                try
                {
                    return (IController)gameObject.AddComponent(type);
                }
                catch(Exception e)
                {
                    Debug.LogException(e);
                    Debug.Log(type.ToString());
                }
            }

            return (IController)Activator.CreateInstance(type);
        }

        private void DoActionAfterFrame(Action action) => StartCoroutine(CDoActionAfterFrame(action));

        // TODO: Code duplicate here and in other classes. Move to CoroutineModel
        private IEnumerator CDoActionAfterFrame( Action action)
        {
            yield return null;
            action?.Invoke();
        }
    }
}
