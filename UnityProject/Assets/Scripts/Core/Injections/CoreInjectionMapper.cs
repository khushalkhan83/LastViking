using ChunkLoaders;
using Game.Audio;
using Game.Controllers;
using Game.Models;
using Game.Models.AI;
using Game.Models.Notifications;
using Game.Models.RemoteSettings;
using Game.Models.RemoteSettings.Firebase;
using Game.Objectives;
using Game.Providers;
using Game.Purchases;
using Game.Views;
using System;
using System.Collections.Generic;
using System.Reflection;
using UltimateSurvival;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core
{
    public class CoreInjectionMapper : InjectionMapperBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private AbilityProvider _abilityProvider;
        [SerializeField] private ActionControllersMapper _actionControllersMapper;
        [SerializeField] private ActionsLogModel _actionsLogModel;
        [SerializeField] private AdvertisementsModel _advertisementsModel;
        [SerializeField] private AimModel _aimModel;
        [SerializeField] private AnaliticsModel _analiticsModel;
        [SerializeField] private AndroidTimingAdsModel _androidTimingAdsModel;
        [SerializeField] private AnimalsModel _animalsModel;
        [SerializeField] private AttackDelayStatusViewModel _attackDelayStatusViewModel;
        [SerializeField] private AudioSystem _audioSystem;
        [SerializeField] private AutosaveModel _autosaveModel;
        [SerializeField] private BloodEffectModel _bloodEffectModel;
        [SerializeField] private BluePrintsModel _bluePrintsModel;
        [SerializeField] private CampFiresModel _campFiresModel;
        [SerializeField] private CampFireViewModel _campFireViewModel;
        [SerializeField] private CoinsModel _coinsModel;
        [SerializeField] private ConditionControllersMapper _conditionControllersMapper;
        [SerializeField] private ConditionDataMapper _conditionDataMapper;
        [SerializeField] private ControllersModel _controllersModel;
        [SerializeField] private CraftInteractViewModel _craftInteractViewModel;
        [SerializeField] private CraftModel _craftModel;
        [SerializeField] private CraftViewModel _craftViewModel;
        [SerializeField] private DebugTimeInteractViewModel _debugTimeInteractViewModel;
        [SerializeField] private DebugTimeModel _debugTimeModel;
        [SerializeField] private DiscordModel _discordModel;
        [SerializeField] private EnableObjectivesButtonModel _enableObjectivesButtonModel;
        [SerializeField] private EnemiesModel _enemiesModel;
        [SerializeField] private ExpandRemoteSettings _expandRemoteSettings;
        [SerializeField] private ExpandsModel _expandsModel;
        [SerializeField] private FurnaceViewModel _furnaceViewModel;
        [SerializeField] private GameLateUpdateModel _gameLateUpdateModel;
        [SerializeField] private GameSparksModel _gameSparksModel;
        [SerializeField] private GameTimeModel _gameTimeModel;
        [SerializeField] private GameUpdateModel _gameUpdateModel;
        [SerializeField] private HotBarModel _hotBarModel;
        [SerializeField] private InfoPanelDefenceViewModel _infoPanelDefenceViewModel;
        [SerializeField] private InfoPanelFoodViewModel _infoPanelFoodViewModel;
        [SerializeField] private InfoPanelMedicineViewModel _infoPanelMedicineViewModel;
        [SerializeField] private InfoPanelToolViewModel _infoPanelToolViewModel;
        [SerializeField] private InfoPanelViewModel _infoPanelViewModel;
        [SerializeField] private InfoPanelWeaponViewModel _infoPanelWeaponViewModel;
        [SerializeField] private InjectionSystem _injectionSystem;
        [SerializeField] private InventoryLootViewModel _inventoryLootViewModel;
        [SerializeField] private InventoryModel _inventoryModel;
        [SerializeField] private InventoryPlayerViewModel _inventoryPlayerViewModel;
        [SerializeField] private InventoryViewModel _inventoryViewModel;
        [SerializeField] private ItemsDB _itemsDB;
        [SerializeField] private JoystickModel _joystickModel;
        [SerializeField] private LeaderboardViewModel _leaderboardViewModel;
        [SerializeField] private LoadingCircleViewModel _loadingCircleViewModel;
        [SerializeField] private LocalizationModel _localizationModel;
        [SerializeField] private LoomViewModel _loomViewModel;
        [SerializeField] private LoseViewModel _loseViewModel;
        [SerializeField] private MainTargetHealthModel _mainTargetHealthModel;
        [SerializeField] private MineableObjectsModel _mineableObjectsModel;
        [SerializeField] private NetworkModel _networkModel;
        [SerializeField] private NotificationContainerViewModel _notificationContainerViewModel;
        [SerializeField] private ObjectivesModel _objectivesModel;
        [SerializeField] private ObjectivesProvider _objectivesProvider;
        [SerializeField] private ObjectivesViewModel _objectivesViewModel;
        [SerializeField] private PickUpsModel _pickUpsModel;
        [SerializeField] private PlacementModel _placementModel;
        [SerializeField] private PlayerAudioModel playerAudioModel;
        [SerializeField] private PlayerBleedingDamagerModel _playerBleedingDamagerModel;
        [SerializeField] private PlayerCameras _PlayerCameras;
        [SerializeField] private PlayerColdDamagerModel _playerColdDamagerModel;
        [SerializeField] private PlayerWarmModel _playerWarmModel;
        [SerializeField] private PlayerConsumeModel _playerConsumeModel;
        [SerializeField] private PlayerDeathModel _playerDeathModel;
        [SerializeField] private PlayerDrinkProcessModel _playerDrinkProcessModel;
        [SerializeField] private PlayerEatProcessModel _playerEatProcessModel;
        [SerializeField] private PlayerEventHandler _playerEventHandler;
        [SerializeField] private PlayerFoodBarViewModel _playerFoodBarViewModel;
        [SerializeField] private PlayerFoodModel _playerFoodModel;
        [SerializeField] private PlayerHealProcessModel _playerHealProcessModel;
        [SerializeField] private PlayerHealthModel _playerHealthModel;
        [SerializeField] private PlayerHungerDamagerModel _playerHungerDamagerModel;
        [SerializeField] private PlayerHungerModel _playerHungerModel;
        [SerializeField] private PlayerLandModel _playerLandModel;
        [SerializeField] private PlayerMovementControllerGround _playerMovementControllerGround;
        [SerializeField] private PlayerMovementControllerWater _playerMovementControllerWater;
        [SerializeField] private PlayerMovementModel _playerMovementModel;
        [SerializeField] private PlayerPoisonDamagerModel _playerPoisonDamagerModel;
        [SerializeField] private PlayerProfileModel _playerProfileModel;
        //[SerializeField] private PlayerRespawnPoints _playerRespawnPoints;
        [SerializeField] private PlayerRunModel _playerRunModel;
        [SerializeField] private PlayerStaminaModel _playerStaminaModel;
        [SerializeField] private PlayerStaminaRegenerateModel _playerStaminaRegenerateModel;
        [SerializeField] private PlayerThirstDamagerModel _playerThirstDamagerModel;
        [SerializeField] private PlayerThirstModel _playerThirstModel;
        [SerializeField] private PlayerWaterBarViewModel _playerWaterBarViewModel;
        [SerializeField] private PlayerWaterModel _playerWaterModel;
        [SerializeField] private PrefabsProvider _prefabsProvider;
        [SerializeField] private PurchasesModel _purchasesModel;
        [SerializeField] private PurchasesViewModel _purchasesViewModel;
        [SerializeField] private QualityModel _qualityModel;
        [SerializeField] private RandomPlayerAvatarModel _randomPlayerAvatarModel;
        [SerializeField] private RateMeViewModel _rateMeViewModel;
        [SerializeField] private RemoteSettingsModel _remoteSettingsModel;
        [SerializeField] private RepairingItemsModel _repairingItemsModel;
        [SerializeField] private RepairViewModel _repairViewModel;
        [SerializeField] private RotateBuildablePreviewModel _rotateBuildablePreviewModel;
        [SerializeField] private SettingsViewModel _settingsViewModel;
        [SerializeField] private ShelterModelsProvider _shelterModelsProvider;
        [SerializeField] private SheltersModel _sheltersModel;
        [SerializeField] private StartNightInfoModel _startNightInfoModel;
        [SerializeField] private StatisticsModel _statisticsModel;
        [SerializeField] private StatisticWorldObjectsNodel _statisticWorldObjectsNodel;
        [SerializeField] private StorageModel _storageModel;
        [SerializeField] private TimingAdsModel _timingAdsModel;
        [SerializeField] private TombsModel _tombsModel;
        [SerializeField] private TouchpadModel _touchpadModel;
        [SerializeField] private TutorialDarkScreenModel _tutorialDarkScreen;
        [SerializeField] private TutorialModel _tutorialModel;
        [SerializeField] private VersionModel _versionModel;
        [SerializeField] private ViewsSystem _viewsSystem;
        [SerializeField] private WeaponCoolDownModel _weaponCoolDownModel;
        [SerializeField] private WorldCameraModel _worldCameraModel;
        [SerializeField] private WorldObjectCreator _worldObjectCreator;
        [SerializeField] private WorldObjectsModel _worldObjectsModel;
        [SerializeField] private SpritesProvider _spritesProvider;
        [SerializeField] private PlayerObjectivesModel _playerObjectivesModel;
        [SerializeField] private ObjectivesNotificationModel _objectivesNotificationModel;
        [SerializeField] private AddingInventoryItemsModel _addingInventoryItemsModel;
        [SerializeField] private ItemsOwnershipModel _itemsOwnershipModel;
        [SerializeField] private MinebleElementsModel _minebleElementsModel;
        [SerializeField] private MaterialsProvider _materialsProvider;
        [SerializeField] private OutLineCoconutModel _outLineCoconutModel;
        [SerializeField] private FurnaceGroupModel _furnaceGroupModel;
        [SerializeField] private LoomGroupModel _loomGroupModel;
        [SerializeField] private OutLineBlueBarrelModel _outLineBlueBarrelModel;
        [SerializeField] private OutLineBagPickupModel _outLineBagPickupModel;
        [SerializeField] private RealTimeModel _realTimeModel;
        [SerializeField] private FishingModel _fishingModel;
        [SerializeField] private MiniGameStateModel _miniGameStateModel;
        [SerializeField] private TokensModel _tokensModel;
        [SerializeField] private PlayerEventObjectivesModel _playerEventObjectivesModel;
        [SerializeField] private FirebaseModel _firebaseModel;
        [SerializeField] private FirebaseRemoteSettingsModel _firebaseRemoteSettingsModel;
        [SerializeField] private EventTokensModel _eventTokensModel;
        [SerializeField] private TutorialProgressModel _tutorialProgressModel;
        [SerializeField] private OutLineBanana _outLineBanana;
        [SerializeField] private LocalNotificationsModel _localNotificationsModel;
        [SerializeField] private StarterPackModel _starterPackModel;
        [SerializeField] private CoastLootChestModel _coastLootChestModel;
        [SerializeField] private TreasureHuntModel _treasureHuntModel;
        [SerializeField] private EventLootModel _eventLootModel;
        [SerializeField] private EnemiesProvider _enemiesProvider;
        [SerializeField] private PlayerScenesModel _playerScenesModel;
        [SerializeField] private SceneNamesProvider _sceneNamesProvider;
        [SerializeField] private WaterfallProgressModel _waterfallProgressModel;
        [SerializeField] private TreasureLootModel _treasureLootModel;
        [SerializeField] private DeadManLootChestModel _deadManLootChestModel;
        [SerializeField] private PlayerHealthBarViewModel _playerHealthBarViewModel;
        [SerializeField] private PlayerDeathHandler _playerDeathHandler;
        [SerializeField] private PurchasersStoreModel _purchasersStoreModel;
        [SerializeField] private PlayerStatusViewModel _playerStatusViewModel;
        [SerializeField] private FPManager _fPManager;
        [SerializeField] private FindTombModel _findTombModel;
        [SerializeField] private FirebaseRemoteKeyProvider _firebaseRemoteKeyProvider;
        [SerializeField] private PlayerWeaponProvider _playerWeaponProvider;
        [SerializeField] private PlayerRespawnPointsModel _playerRespawnPointsModel;
        [SerializeField] private FixPlayerUndergroundModel _fixPlayerUndergroundModel;
        [SerializeField] private ChunkLoadersProvider _chunkLoadersProvider;
        [SerializeField] private ChunkLoadersConfigProvider _chunkLoadersConfigProvider;
        [SerializeField] private UnloadUnusedAssetsModel _unloadUnusedAssetsModel;
        [SerializeField] private WeaponAssetsLoadingModel _weaponAssetsLoadingModel;
        [SerializeField] private ChunksLoadersModel _chunksLoadersModel;
        [SerializeField] private BuildingModeModel _buildingModeModel;
        [SerializeField] private BuildingHotBarModel _buildingHotBarModel;
        [SerializeField] private BuildingProcessModel _buildingProcessModel;
        [SerializeField] private LocationModificatorsProvider _locationModificatorsProvider;
        [SerializeField] private MinigameModificatorProvider _minigameModificatorProvider;
        [SerializeField] private ObjectivesProgressModel _objectivesProgressModel;
        [SerializeField] private ObjectivesWindowModel _objectivesWindowModel;
        [SerializeField] private FixOldObjectivesModel _fixOldObjectivesModel;
        [SerializeField] private ApplyItemModel _applyItemModel;
        [SerializeField] private StatsEffectModel _statsEffectModel;
        [SerializeField] private InfoPanelModel _infoPanelModel;
        [SerializeField] private DivideItemsModel _divideItemsModel;
        [SerializeField] private FullInventoryModel _fullInventoryModel;
        [SerializeField] private LootGroupModel _lootGroupModel;
        [SerializeField] private ContainersModel _containersModel;
        [SerializeField] private InventoryRepairingModel _inventoryRepairingModel;
        [SerializeField] private CollectCellDataModel _collectCellDataModel;
        [SerializeField] private ExpandInventoryModel _expandInventoryModel;
        [SerializeField] private DropItemModel _dropItemModel;
        [SerializeField] private CoroutineModel _coroutineModel;
        [SerializeField] private ApplicationCallbacksModel _applicationCallbacksModel;
        [SerializeField] private DebugPrefabCreationModel _debugPrefabCreationModel;
        [SerializeField] private InventoryDragAndDropModel _inventoryDragAndDropModel;
        [SerializeField] private PurchaseComplitedModel _purchaseComplitedModel;
        [SerializeField] private EnoughSpaceModel _enoughSpaceModel;
        [SerializeField] private InjectionModel _injectionModel;
        [SerializeField] private TargetsModel _targetsModel;
        [SerializeField] private ShelterUpgradeModel _shelterUpgradeModel;
        [SerializeField] private ShelterAttackModeModel _shelterAttackModeModel;
        [SerializeField] private EnemiesSpawnModel _enemiesSpawnModel;
        [SerializeField] private ConstructionDockModel _constructionDockkModel;
        [SerializeField] private QuestsModel _questsModel;
        [SerializeField] private QuestNotificationsModel _questNotificationsModel;
        [SerializeField] private QuestsSaveModel _questsSaveModel;
        [SerializeField] private MainQuestsProvider _mainQuestsProvider;
        [SerializeField] private CutsceneKrakenModel _cutsceneKrakenModel;
        [SerializeField] private FirstKrakenModel _firstKrakenModel;
        [SerializeField] private CinematicModel _cinematicModel;
        [SerializeField] private QuestsLifecycleModel _questsLifecycle;
        [SerializeField] private ConstructionInfoModel _constructionInfoModel;
        [SerializeField] private EnvironmentTransitionsModel _environmentTransitionsModel;
        [FormerlySerializedAs("_addItemToPlayerModel")]
        [SerializeField] private InventoryOperationsModel _inventoryOperationsModel;
        [SerializeField] private MedallionsModel _medallionsModel;
        [SerializeField] private DungeonsProgressModel _dungeonsProgressModel;
        [SerializeField] private ChurchProgressModel _churchProgressModel;
        [SerializeField] private InputModel _inputModel;
        [SerializeField] private ViewsStateModel _viewsStateModel;
        [SerializeField] private ViewsMapper _viewsMapper;
        [SerializeField] private EventSystemModel _eventSystemModel;
        [SerializeField] private MineProgressModel _mineProgressModel;
        [SerializeField] private SideQuestsSaveModel _sideQuestsSaveModel;
        [SerializeField] private SideQuestsModel _sideQuestsModel;
        [SerializeField] private HintsModel _hintsModel;
        [SerializeField] private LightModel _lightModel;
        [SerializeField] private DropContainerModel _dropContainerModel;
        [SerializeField] private QuestStageDesciptionModel _questStageDesciptionModel;
        [SerializeField] private TutorialHouseModel _tutorialHouseModel;
        [SerializeField] private TutorialObjectivesDarkScreenModel _tutorialObjectivesDarkScreenModel;
        [SerializeField] private TutorialBuildingModel _tutorialBuildingModel;
        [SerializeField] private FixMovePlayerFromDungeonModel _fixPlayerInWaterfallModel;
        [SerializeField] private SideQuestsProvider _sideQuestsProvider;
        [SerializeField] private OpenConstructionTutorialModel _openConstructionTutorialModel;
        [SerializeField] private AudioEffectsModel _audioEffectsModel;
        [SerializeField] private SpesialMessagesModel _spesialMessagesModel;
        [SerializeField] private CrewModel _crewModel;
        [SerializeField] private CrewMemberDataProvider _crewMemberDataProvider;
        [SerializeField] private ConstructionTutorialModel _constructionTutorialModel;
        [SerializeField] private EnvironmentTimeModel _environmentTimeModel;
        [SerializeField] private CoreEnvironmentModel _coreEnvironmentModel;
        [SerializeField] private TeleporHomeModel _teleporHomeModel;
        [SerializeField] private ConstructionModel _constructionModel;
        [SerializeField] private TreasureLootGroupProvider _treasureLootGroupProvider;
        [SerializeField] private QuetsEventDataProvider _quetsEventDataProvider;
        [SerializeField] private WorldObjectsProvider _worldObjectsProvider;
        [SerializeField] private EnvironmentTransitionProvider _environmentTransitionProvider;
        [SerializeField] private ViewsInputModel _viewsInputModel;
        [SerializeField] private CinematicCameraModel _cinematicCameraModel;
        [SerializeField] private ViewsSystemCameraModel _viewsSystemCameraModel;
        [SerializeField] private PopupsTimeScaleModel _popupTimeScaleModel;
        [SerializeField] private PerformanceStartCheckModel _performanceStartCheckModel;
        [SerializeField] private TutorialSimpleDarkViewModel _tutorialSimpleDarkViewModel;
        [SerializeField] private OverrideUISortingModel _overrideUISortingModel;
        [SerializeField] private InventoryButtonViewModel _inventoryButtonViewModel;
        [SerializeField] private CraftButtonViewModel _craftButtonViewModel;
        [SerializeField] private TutorialCraftBoostAnaliticsModel _tutorialCraftBoostAnaliticsModel;
        [SerializeField] private AnaliticsUserIDModel _analiticsUserIDModel;
        [SerializeField] private AttackButtonViewModel _attackButtonViewModel;
        [SerializeField] private ResourcesExtractionModel _resourcesExtractionModel;
        [SerializeField] private TutorialResourcesModel _tutorialResourcesModel;
        [SerializeField] private PlayerLocationModel _playerLocationModel;
        [SerializeField] private SECTR_Member _playerSectrMember;
        [SerializeField] private AnalyticsPlayerLocationModel _analyticsPlayerLocationModel;
        [SerializeField] private BonusItemsModel _bonusItemsModel;
        [SerializeField] private FishHealthModel _fishHealthModel;
        [SerializeField] private TooltipModel _tooltipModel;
        [SerializeField] private CoinObjectsModel _coinObjectsModel;
        [SerializeField] private CheatModel _сheatModel;
        [SerializeField] private MoreGamesModel _moreGamesModel;
        [SerializeField] private DiscordButtonViewModel _discordButtonViewModel;
        [SerializeField] private FishingLinkViewModel _fishingLinkViewModel;
        [SerializeField] private ReplaceBuildingModel _replaceBuildingModel;
        [SerializeField] private EncountersModel _encountersModel;
        [SerializeField] private EncountersViewModel _encountersViewModel;
        [SerializeField] private PushNotificationsModel _pushNotificationsModel;
        [SerializeField] private NotificationsModel _notificationsModel;
        [SerializeField] private ActivitiesLogViewModel _activitiesLogViewModel;
        [SerializeField] private ActivitiesLogButtonViewModel _activitiesLogButtonViewModel;
        [SerializeField] private SideQuetsEventDataProvider _sideQuetsEventDataProvider;
        [SerializeField] private ColorsViewModel _colorsViewModel;
        [SerializeField] private InventoryEquipmentModel _inventoryEquipmentModel;
        [SerializeField] private EquipmentModel _equipmentModel;
        [SerializeField] private PlayerHealthRegenerationModel _playerHealthRegenerationModel;
        [SerializeField] private VillageBuildingModel _villageBuildingModel;
        [SerializeField] private VillageAttackModel _villageAttackModel;
        [SerializeField] private VillageCinematicsModel _villageCinematicsModel;
        [SerializeField] private PuzzlesModel _puzzlesModel;
        [SerializeField] private DragItemViewModel _dragItemViewModel;
        [SerializeField] private TaskViewModel _taskViewModel;
        [SerializeField] private EnemiesModifierModel _enemiesModifyModel;
        [SerializeField] private LearnInputViewModel _learnInputViewModel;
        [SerializeField] private WipeSaveModel _wipeSaveModel;
        [SerializeField] private DeleteItemModel _deleteItemModel;
        [SerializeField] private VikingTutorialModel _vikingTutorialModel;
        [SerializeField] private ZoneModel _zoneModel;
        [SerializeField] private HouseBuildingViewModel _houseBuildingViewModel;
        ///CODE_GENERATION_FIELDS

#pragma warning restore 0649
        #endregion

        protected override void InitLinks()
        {
            Links = new Dictionary<Type, object>()
            {
                { typeof(AbilityProvider), _abilityProvider},
                { typeof(ActionControllersMapper), _actionControllersMapper},
                { typeof(ActionsLogModel), _actionsLogModel},
                { typeof(AdvertisementsModel), _advertisementsModel},
                { typeof(AimModel), _aimModel},
                { typeof(AnaliticsModel), _analiticsModel},
                { typeof(AndroidTimingAdsModel), _androidTimingAdsModel},
                { typeof(AnimalsModel), _animalsModel},
                { typeof(AttackDelayStatusViewModel), _attackDelayStatusViewModel},
                { typeof(AudioSystem), _audioSystem},
                { typeof(AutosaveModel), _autosaveModel},
                { typeof(BloodEffectModel), _bloodEffectModel},
                { typeof(BluePrintsModel), _bluePrintsModel},
                { typeof(CampFiresModel), _campFiresModel},
                { typeof(CampFireViewModel), _campFireViewModel},
                { typeof(CoinsModel), _coinsModel},
                { typeof(ConditionControllersMapper), _conditionControllersMapper},
                { typeof(ConditionDataMapper), _conditionDataMapper},
                { typeof(ControllersModel), _controllersModel},
                { typeof(CraftInteractViewModel), _craftInteractViewModel},
                { typeof(CraftModel), _craftModel},
                { typeof(CraftViewModel), _craftViewModel},
                { typeof(DebugTimeInteractViewModel), _debugTimeInteractViewModel},
                { typeof(DebugTimeModel), _debugTimeModel},
                { typeof(DiscordModel), _discordModel},
                { typeof(EnableObjectivesButtonModel), _enableObjectivesButtonModel},
                { typeof(EnemiesModel), _enemiesModel},
                { typeof(ExpandRemoteSettings), _expandRemoteSettings},
                { typeof(ExpandsModel), _expandsModel},
                { typeof(FurnaceViewModel), _furnaceViewModel},
                { typeof(GameLateUpdateModel), _gameLateUpdateModel},
                { typeof(GameSparksModel), _gameSparksModel},
                { typeof(GameTimeModel), _gameTimeModel},
                { typeof(GameUpdateModel), _gameUpdateModel},
                { typeof(HotBarModel), _hotBarModel},
                { typeof(InfoPanelDefenceViewModel), _infoPanelDefenceViewModel},
                { typeof(InfoPanelFoodViewModel), _infoPanelFoodViewModel},
                { typeof(InfoPanelMedicineViewModel), _infoPanelMedicineViewModel},
                { typeof(InfoPanelToolViewModel), _infoPanelToolViewModel},
                { typeof(InfoPanelViewModel), _infoPanelViewModel},
                { typeof(InfoPanelWeaponViewModel), _infoPanelWeaponViewModel},
                { typeof(InjectionSystem), _injectionSystem},
                { typeof(InventoryLootViewModel), _inventoryLootViewModel},
                { typeof(InventoryModel), _inventoryModel},
                { typeof(InventoryPlayerViewModel), _inventoryPlayerViewModel},
                { typeof(InventoryViewModel), _inventoryViewModel},
                { typeof(ItemsDB), _itemsDB},
                { typeof(JoystickModel), _joystickModel},
                { typeof(LeaderboardViewModel), _leaderboardViewModel},
                { typeof(LoadingCircleViewModel), _loadingCircleViewModel},
                { typeof(LocalizationModel), _localizationModel},
                { typeof(LoomViewModel), _loomViewModel},
                { typeof(LoseViewModel), _loseViewModel},
                { typeof(MainTargetHealthModel), _mainTargetHealthModel},
                { typeof(MineableObjectsModel), _mineableObjectsModel},
                { typeof(NetworkModel), _networkModel},
                { typeof(NotificationContainerViewModel), _notificationContainerViewModel},
                { typeof(ObjectivesModel), _objectivesModel},
                { typeof(ObjectivesProvider), _objectivesProvider},
                { typeof(ObjectivesViewModel), _objectivesViewModel},
                { typeof(PickUpsModel), _pickUpsModel},
                { typeof(PlacementModel), _placementModel},
                { typeof(PlayerAudioModel), playerAudioModel},
                { typeof(PlayerBleedingDamagerModel), _playerBleedingDamagerModel},
                { typeof(PlayerCameras), _PlayerCameras},
                { typeof(PlayerColdDamagerModel), _playerColdDamagerModel},
                { typeof(PlayerWarmModel), _playerWarmModel},
                { typeof(PlayerConsumeModel), _playerConsumeModel},
                { typeof(PlayerDeathModel), _playerDeathModel},
                { typeof(PlayerDrinkProcessModel), _playerDrinkProcessModel},
                { typeof(PlayerEatProcessModel), _playerEatProcessModel},
                { typeof(PlayerEventHandler), _playerEventHandler},
                { typeof(PlayerFoodBarViewModel), _playerFoodBarViewModel},
                { typeof(PlayerFoodModel), _playerFoodModel},
                { typeof(PlayerHealProcessModel), _playerHealProcessModel},
                { typeof(PlayerHealthModel), _playerHealthModel},
                { typeof(PlayerHungerDamagerModel), _playerHungerDamagerModel},
                { typeof(PlayerHungerModel), _playerHungerModel},
                { typeof(PlayerLandModel), _playerLandModel},
                { typeof(PlayerMovementControllerGround), _playerMovementControllerGround},
                { typeof(PlayerMovementControllerWater), _playerMovementControllerWater},
                { typeof(PlayerMovementModel), _playerMovementModel},
                { typeof(PlayerPoisonDamagerModel), _playerPoisonDamagerModel},
                { typeof(PlayerProfileModel), _playerProfileModel},
                //{ typeof(PlayerRespawnPoints), _playerRespawnPoints},
                { typeof(PlayerRunModel), _playerRunModel},
                { typeof(PlayerStaminaModel), _playerStaminaModel},
                { typeof(PlayerStaminaRegenerateModel), _playerStaminaRegenerateModel},
                { typeof(PlayerThirstDamagerModel), _playerThirstDamagerModel},
                { typeof(PlayerThirstModel), _playerThirstModel},
                { typeof(PlayerWaterBarViewModel), _playerWaterBarViewModel},
                { typeof(PlayerWaterModel), _playerWaterModel},
                { typeof(PrefabsProvider), _prefabsProvider},
                { typeof(PurchasesModel), _purchasesModel},
                { typeof(PurchasesViewModel), _purchasesViewModel},
                { typeof(QualityModel), _qualityModel},
                { typeof(RandomPlayerAvatarModel), _randomPlayerAvatarModel},
                { typeof(RateMeViewModel), _rateMeViewModel},
                { typeof(RemoteSettingsModel), _remoteSettingsModel},
                { typeof(RepairingItemsModel), _repairingItemsModel},
                { typeof(RepairViewModel), _repairViewModel},
                { typeof(RotateBuildablePreviewModel), _rotateBuildablePreviewModel},
                { typeof(SettingsViewModel), _settingsViewModel},
                { typeof(ShelterModelsProvider), _shelterModelsProvider},
                { typeof(SheltersModel), _sheltersModel},
                { typeof(StartNightInfoModel), _startNightInfoModel},
                { typeof(StatisticsModel), _statisticsModel},
                { typeof(StatisticWorldObjectsNodel), _statisticWorldObjectsNodel},
                { typeof(StorageModel), _storageModel},
                { typeof(TimingAdsModel), _timingAdsModel},
                { typeof(TombsModel), _tombsModel},
                { typeof(TouchpadModel), _touchpadModel},
                { typeof(TutorialDarkScreenModel), _tutorialDarkScreen},
                { typeof(TutorialModel), _tutorialModel},
                { typeof(VersionModel), _versionModel},
                { typeof(ViewsSystem), _viewsSystem},
                { typeof(WeaponCoolDownModel), _weaponCoolDownModel},
                { typeof(WorldCameraModel), _worldCameraModel},
                { typeof(WorldObjectCreator), _worldObjectCreator},
                { typeof(WorldObjectsModel), _worldObjectsModel},
                { typeof(SpritesProvider), _spritesProvider},
                { typeof(PlayerObjectivesModel), _playerObjectivesModel },
                { typeof(ObjectivesNotificationModel), _objectivesNotificationModel },
                { typeof(AddingInventoryItemsModel), _addingInventoryItemsModel },
                { typeof(ItemsOwnershipModel), _itemsOwnershipModel },
                { typeof(MinebleElementsModel), _minebleElementsModel },
                { typeof(MaterialsProvider), _materialsProvider },
                { typeof(OutLineCoconutModel), _outLineCoconutModel },
                { typeof(FurnaceGroupModel), _furnaceGroupModel },
                { typeof(LoomGroupModel), _loomGroupModel },
                { typeof(OutLineBlueBarrelModel), _outLineBlueBarrelModel },
                { typeof(OutLineBagPickupModel), _outLineBagPickupModel },
                { typeof(RealTimeModel), _realTimeModel },
                { typeof(FishingModel), _fishingModel },
                { typeof(MiniGameStateModel), _miniGameStateModel },
                { typeof(TokensModel), _tokensModel },
                { typeof(PlayerEventObjectivesModel), _playerEventObjectivesModel },
                { typeof(FirebaseModel), _firebaseModel },
                { typeof(FirebaseRemoteSettingsModel), _firebaseRemoteSettingsModel },
                { typeof(EventTokensModel), _eventTokensModel },
                { typeof(TutorialProgressModel), _tutorialProgressModel },
                { typeof(OutLineBanana), _outLineBanana },
                { typeof(LocalNotificationsModel), _localNotificationsModel },
                { typeof(StarterPackModel), _starterPackModel },
                { typeof(CoastLootChestModel), _coastLootChestModel },
                { typeof(TreasureHuntModel), _treasureHuntModel },
                { typeof(EventLootModel), _eventLootModel },
                { typeof(EnemiesProvider), _enemiesProvider},
                { typeof(PlayerScenesModel), _playerScenesModel},
                { typeof(SceneNamesProvider), _sceneNamesProvider},
                { typeof(WaterfallProgressModel), _waterfallProgressModel},
                { typeof(TreasureLootModel), _treasureLootModel },
                { typeof(DeadManLootChestModel), _deadManLootChestModel},
                { typeof(PlayerHealthBarViewModel), _playerHealthBarViewModel},
                { typeof(PlayerDeathHandler), _playerDeathHandler},
                { typeof(PurchasersStoreModel), _purchasersStoreModel},
                { typeof(PlayerStatusViewModel), _playerStatusViewModel},
                { typeof(FPManager), _fPManager},
                { typeof(FindTombModel), _findTombModel},
                { typeof(FirebaseRemoteKeyProvider), _firebaseRemoteKeyProvider},
                { typeof(PlayerWeaponProvider), _playerWeaponProvider},
                { typeof(PlayerRespawnPointsModel), _playerRespawnPointsModel},
                { typeof(FixPlayerUndergroundModel), _fixPlayerUndergroundModel},
                { typeof(ChunkLoadersProvider), _chunkLoadersProvider},
                { typeof(ChunkLoadersConfigProvider), _chunkLoadersConfigProvider},
                { typeof(UnloadUnusedAssetsModel), _unloadUnusedAssetsModel},
                { typeof(WeaponAssetsLoadingModel), _weaponAssetsLoadingModel},
                { typeof(ChunksLoadersModel), _chunksLoadersModel},
                { typeof(BuildingModeModel), _buildingModeModel},
                { typeof(BuildingHotBarModel), _buildingHotBarModel},
                { typeof(BuildingProcessModel), _buildingProcessModel},
                { typeof(LocationModificatorsProvider), _locationModificatorsProvider},
                { typeof(MinigameModificatorProvider), _minigameModificatorProvider},
                { typeof(ObjectivesProgressModel), _objectivesProgressModel},
                { typeof(ObjectivesWindowModel), _objectivesWindowModel},
                { typeof(FixOldObjectivesModel), _fixOldObjectivesModel},
                { typeof(ApplyItemModel), _applyItemModel},
                { typeof(StatsEffectModel), _statsEffectModel},
                { typeof(InfoPanelModel), _infoPanelModel},
                { typeof(DivideItemsModel), _divideItemsModel},
                { typeof(FullInventoryModel), _fullInventoryModel},
                { typeof(LootGroupModel), _lootGroupModel},
                { typeof(ContainersModel), _containersModel},
                { typeof(InventoryRepairingModel), _inventoryRepairingModel},
                { typeof(CollectCellDataModel), _collectCellDataModel},
                { typeof(ExpandInventoryModel), _expandInventoryModel},
                { typeof(DropItemModel), _dropItemModel},
                { typeof(CoroutineModel), _coroutineModel},
                { typeof(ApplicationCallbacksModel), _applicationCallbacksModel},
                { typeof(DebugPrefabCreationModel), _debugPrefabCreationModel},
                { typeof(InventoryDragAndDropModel), _inventoryDragAndDropModel},
                { typeof(PurchaseComplitedModel), _purchaseComplitedModel},
                { typeof(EnoughSpaceModel), _enoughSpaceModel},
                { typeof(InjectionModel), _injectionModel},
                { typeof(TargetsModel), _targetsModel},
                { typeof(ShelterUpgradeModel), _shelterUpgradeModel},
                { typeof(ShelterAttackModeModel), _shelterAttackModeModel},
                { typeof(EnemiesSpawnModel), _enemiesSpawnModel},
                { typeof(ConstructionDockModel), _constructionDockkModel},
                { typeof(QuestsModel), _questsModel},
                { typeof(QuestNotificationsModel), _questNotificationsModel},
                { typeof(QuestsSaveModel), _questsSaveModel},
                { typeof(MainQuestsProvider), _mainQuestsProvider},
                { typeof(CutsceneKrakenModel), _cutsceneKrakenModel},
                { typeof(FirstKrakenModel), _firstKrakenModel},
                { typeof(CinematicModel), _cinematicModel},
                { typeof(QuestsLifecycleModel), _questsLifecycle},
                { typeof(ConstructionInfoModel), _constructionInfoModel},
                { typeof(EnvironmentTransitionsModel), _environmentTransitionsModel},
                { typeof(InventoryOperationsModel), _inventoryOperationsModel},
                { typeof(MedallionsModel), _medallionsModel},
                { typeof(DungeonsProgressModel), _dungeonsProgressModel},
                { typeof(ChurchProgressModel), _churchProgressModel},
                { typeof(InputModel), _inputModel},
                { typeof(ViewsStateModel), _viewsStateModel},
                { typeof(ViewsMapper), _viewsMapper},
                { typeof(EventSystemModel), _eventSystemModel},
                { typeof(MineProgressModel), _mineProgressModel},
                { typeof(SideQuestsSaveModel), _sideQuestsSaveModel},
                { typeof(SideQuestsModel), _sideQuestsModel},
                { typeof(HintsModel), _hintsModel},
                { typeof(LightModel), _lightModel},
                { typeof(DropContainerModel), _dropContainerModel},
                { typeof(QuestStageDesciptionModel), _questStageDesciptionModel},
                { typeof(TutorialHouseModel), _tutorialHouseModel},
                { typeof(TutorialObjectivesDarkScreenModel), _tutorialObjectivesDarkScreenModel},
                { typeof(TutorialBuildingModel), _tutorialBuildingModel},
                { typeof(FixMovePlayerFromDungeonModel), _fixPlayerInWaterfallModel},
                { typeof(SideQuestsProvider), _sideQuestsProvider},
                { typeof(OpenConstructionTutorialModel), _openConstructionTutorialModel},
                { typeof(AudioEffectsModel), _audioEffectsModel},
                { typeof(SpesialMessagesModel), _spesialMessagesModel},
                { typeof(CrewModel), _crewModel},
                { typeof(CrewMemberDataProvider), _crewMemberDataProvider},
                { typeof(ConstructionTutorialModel), _constructionTutorialModel},
                { typeof(EnvironmentTimeModel), _environmentTimeModel},
                { typeof(CoreEnvironmentModel), _coreEnvironmentModel},
                { typeof(TeleporHomeModel), _teleporHomeModel},
                { typeof(ConstructionModel), _constructionModel},
                { typeof(TreasureLootGroupProvider), _treasureLootGroupProvider},
                { typeof(QuetsEventDataProvider), _quetsEventDataProvider},
                { typeof(WorldObjectsProvider), _worldObjectsProvider},
                { typeof(EnvironmentTransitionProvider), _environmentTransitionProvider},
                { typeof(ViewsInputModel), _viewsInputModel},
                { typeof(CinematicCameraModel), _cinematicCameraModel},
                { typeof(ViewsSystemCameraModel), _viewsSystemCameraModel},
                { typeof(PopupsTimeScaleModel), _popupTimeScaleModel},
                { typeof(PerformanceStartCheckModel), _performanceStartCheckModel},
                { typeof(TutorialSimpleDarkViewModel), _tutorialSimpleDarkViewModel},
                { typeof(OverrideUISortingModel), _overrideUISortingModel},
                { typeof(InventoryButtonViewModel), _inventoryButtonViewModel},
                { typeof(CraftButtonViewModel), _craftButtonViewModel},
                { typeof(TutorialCraftBoostAnaliticsModel), _tutorialCraftBoostAnaliticsModel},
                { typeof(AnaliticsUserIDModel), _analiticsUserIDModel},
                { typeof(AttackButtonViewModel), _attackButtonViewModel},
                { typeof(ResourcesExtractionModel), _resourcesExtractionModel},
                { typeof(TutorialResourcesModel), _tutorialResourcesModel},
                { typeof(PlayerLocationModel), _playerLocationModel},
                { typeof(SECTR_Member), _playerSectrMember},
                { typeof(AnalyticsPlayerLocationModel), _analyticsPlayerLocationModel},
                { typeof(BonusItemsModel), _bonusItemsModel},
                { typeof(FishHealthModel), _fishHealthModel},
                { typeof(TooltipModel), _tooltipModel},
                { typeof(CoinObjectsModel), _coinObjectsModel},
                { typeof(CheatModel), _сheatModel},
                { typeof(MoreGamesModel), _moreGamesModel},
                { typeof(DiscordButtonViewModel), _discordButtonViewModel},
                { typeof(FishingLinkViewModel), _fishingLinkViewModel},
                { typeof(ReplaceBuildingModel), _replaceBuildingModel},
                { typeof(EncountersModel), _encountersModel},
                { typeof(EncountersViewModel), _encountersViewModel},
                { typeof(PushNotificationsModel), _pushNotificationsModel},
                { typeof(NotificationsModel), _notificationsModel},
                { typeof(ActivitiesLogViewModel), _activitiesLogViewModel},
                { typeof(ActivitiesLogButtonViewModel), _activitiesLogButtonViewModel},
                { typeof(SideQuetsEventDataProvider), _sideQuetsEventDataProvider},
                { typeof(ColorsViewModel), _colorsViewModel},
                { typeof(InventoryEquipmentModel), _inventoryEquipmentModel},
                { typeof(EquipmentModel), _equipmentModel},
                { typeof(PlayerHealthRegenerationModel), _playerHealthRegenerationModel},
                { typeof(VillageBuildingModel), _villageBuildingModel},
                { typeof(VillageAttackModel), _villageAttackModel},
                { typeof(VillageCinematicsModel), _villageCinematicsModel},
                { typeof(PuzzlesModel), _puzzlesModel},
                { typeof(DragItemViewModel), _dragItemViewModel},
                { typeof(TaskViewModel), _taskViewModel},
                { typeof(EnemiesModifierModel), _enemiesModifyModel},
                { typeof(LearnInputViewModel), _learnInputViewModel},
                { typeof(WipeSaveModel), _wipeSaveModel},
                { typeof(DeleteItemModel), _deleteItemModel},
                { typeof(VikingTutorialModel), _vikingTutorialModel},
                { typeof(ZoneModel), _zoneModel},
                { typeof(HouseBuildingViewModel), _houseBuildingViewModel},
                ///CODE_GENERATION_LINKS
            };
        }

        #if UNITY_EDITOR
        public void SetLinkValue(Type type, object value)
        {
            FieldInfo[] fields = typeof(CoreInjectionMapper).GetFields(BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public);
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                if(field.FieldType == type)
                {
                    field.SetValue(this,value);
                    EditorUtility.SetDirty(this);
                    return;
                }   
            }
        }
        #endif
    }
}
