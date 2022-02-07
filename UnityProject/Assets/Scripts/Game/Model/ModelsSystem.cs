using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using System;
using System.Reflection;
using CustomeEditorTools;
using Game.AI;
using Game.Models.AI;
using Game.Models.RemoteSettings;
using Game.Models.RemoteSettings.Firebase;
using Game.Objectives;
using Game.Objectives.Stacks;
using Game.Providers;
using Game.Purchases;
using UltimateSurvival;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Game.Models.Notifications;

namespace Game.Models
{

    public class ModelsSystem : MonoBehaviour // TODO: move dependencies from ModelsSystem to Injection
    {
        private static ModelsSystem instance;
        public static ModelsSystem Instance
        {
            get
            {
                if(instance == null)
                {
                    var modelVariable = Resources.Load<ModelsSystemVariable>("SO_System_Models");
                    instance = modelVariable.Value;
                }
                return instance;
            }
        }

        [SerializeField] private ModelsSystemVariable variable;
        
        [Space]
        #region Not included in injection system
        public SurfaceDatabaseModel surfaceDatabaseModel;
        public PurchasersStoreModel purchasersStoreModel;
        public Target _playerAsTarget;

        public EnemiesProvider enemiesProvider;
        public AnimalsProvider animalsProvider;
        public PlayerWeaponProvider playerWeaponProvider;
        public Target shelterTarget;
            
        #endregion

        public ActionsLogModel _actionsLogModel;
        public AimModel _aimModel;
        public AnaliticsModel _analiticsModel;
        public AnimalsModel _animalsModel;
        public AttackDelayStatusViewModel _attackDelayStatusViewModel;
        public AutosaveModel _autosaveModel;
        public BluePrintsModel _bluePrintsModel;
        public CampFiresModel _campFiresModel;
        public CoinsModel _coinsModel;
        public CoinObjectsModel _coinObjectsModel;
        public CraftViewModel _craftViewModel;
        public EnemiesModel _enemiesModel;
        public GameTimeModel _gameTimeModel;
        public GameUpdateModel _gameUpdateModel;
        public HotBarModel _hotBarModel;
        public InventoryModel _inventoryModel;
        public InventoryViewModel _inventoryViewModel;
        public ItemsDB _itemsDB;
        public ItemsGroup _debugitemsGroup;
        public JoystickModel _joystickModel;
        public LocalizationModel _localizationModel;
        public MineableObjectsModel _mineableObjectsModel;
        public NetworkModel _networkModel;
        public NotificationContainerViewModel _notificationContainerViewModel;
        public PlacementModel _placementModel;
        public PlayerBleedingDamagerModel _playerBleedingDamagerModel;
        public PlayerCameras _playerCameras;
        public PlayerDeathModel _playerDeathModel;
        public PlayerEventHandler _playerEventHandler;
        public PlayerFoodModel _playerFoodModel;
        public PlayerHealthModel _playerHealthModel;
        public PlayerLandModel _playerLandModel;
        public PlayerMovementModel _playerMovementModel;
        public PlayerProfileModel _playerProfileModel;
        public PlayerRunModel _playerRunModel;
        public PlayerStaminaModel _playerStaminaModel;
        public PlayerWaterModel _playerWaterModel;
        public PrefabsProvider _prefabsProvider;
        public PurchasesModel _purchasesModel;
        public QualityModel _qualityModel;
        public RandomPlayerAvatarModel _randomPlayerAvatarModel;
        public RemoteSettingsModel _remoteSettingsModel;
        public ShelterModelsProvider _shelterModelsProvider;
        public SheltersModel _sheltersModel;
        public StartNightInfoModel _startNightInfoModel;
        public StatisticsModel _statisticsModel;
        public StatisticWorldObjectsNodel _statisticWorldObjectsNodel;
        public StorageModel _storageModel;
        public TombsModel _tombsModel;
        public TouchpadModel _touchpadModel;
        public WeaponCoolDownModel _weaponCoolDownModel;
        public WorldCameraModel _worldCameraModel;
        public WorldObjectCreator _worldObjectCreator;
        public WorldObjectsModel _worldObjectsModel;
        public MinebleElementsModel _minebleElementsModel;
        public BlueprintObjectsModel _blueprintObjectsModel;
        public CoastLootChestModel _coastLootChestModel;
        public PlayerScenesModel _playerScenesModel;
        public SceneNamesProvider _sceneNamesProvider;
        public WaterfallProgressModel _waterfallProgressModel;
        public TreasureLootModel _treasureLootModel;
        public DeadManLootChestModel _deadManLootChestModel;
        public PlayerHealthBarViewModel _playerHealthBarViewModel;
        public PlayerDeathHandler _playerDeathHandler;
        public PurchasersStoreModel _purchasersStoreModel;
        public PlayerStatusViewModel _playerStatusViewModel;
        public FPManager _fPManager;
        public FindTombModel _findTombModel;
        public FirebaseRemoteKeyProvider _firebaseRemoteKeyProvider;
        public WeaponAssetsLoadingModel _weaponAssetsLoadingModel;
        public TreasureHuntModel _treasureHuntModel;
        public ChunksLoadersModel _chunksLoadersModel;
        public ApplyItemModel _applyItemModel;
        public DebugPrefabCreationModel _debugPrefabCreationModel;
        public InventoryDragAndDropModel _inventoryDragAndDropModel;
        public BuildingModeModel _buildingModel;
        public BuildingHotBarModel _buildingHotBarModel;
        public BuildingProcessModel _buildingProcessModel;
        public InjectionSystem _injectionSystem;
        public ObjectivesProvider _objectivesProvider;
        public ObjectivesProgressModel _objectivesProgressModel;
        public StacksProvider _stacksProvider;
        public PlayerObjectivesModel _playerObjectivesModel;
        public ObjectivesWindowModel _objectivesWindowModel;
        public TutorialModel _tutorialModel;
        public EnoughSpaceModel _enoughSpaceModel;
        public InjectionModel _injectionModel;
        public TargetsModel _targetsModel;
        public ShelterUpgradeModel _shelterUpgradeModel;
        public ShelterAttackModeModel _shelterAttackModeModel;
        public EnemiesSpawnModel _enemiesSpawnModel;
        public ConstructionDockModel _constructionDockModel;
        public QuestsModel _questsModel;
        public QuestTriggersModel _questTriggersModel;
        public QuestTriggerProvider _questTriggerProvider;
        public QuestNotificationsModel _questNotificationsModel;
        public QuestsSaveModel _questsSaveModel;
        public MainQuestsProvider _mainQuestsProvider;
        public ApplicationCallbacksModel _applicationCallbacksModel;
        public CutsceneKrakenModel _cutsceneKrakenModel;
        public FirstKrakenModel _firstKrakenModel;

        public CinematicModel _cinematicModel;
        public CoroutineModel _coroutineModel;
        public QuestsLifecycleModel _questsLifecycle;
        public ControllersModel _controllersModel;
        public ConstructionInfoModel _constructionInfoModel;
        public EnvironmentTransitionsModel _environmentTransitionsModel;
        [FormerlySerializedAs("_addItemToPlayerModel")]
        public InventoryOperationsModel _inventoryOperationsModel;
        public MedallionsModel _medallionsModel;
        public DungeonsProgressModel _dungeonsProgressModel;
        public ChurchProgressModel _churchProgressModel;
        public InputModel _inputModel;
        public ViewsStateModel _viewsStateModel;
        public EventSystemModel _eventSystemModel;
        public SideQuestsModel _sideQuestsModel;
        public MineProgressModel _mineProgressModel;
        public SideQuestsSaveModel _sideQuestsSaveModel;
        public HintsModel _hintsModel;
        public LightModel _lightModel;
        public DropContainerModel _dropContainerModel;
        public QuestStageDesciptionModel _questStageDesciptionModel;
        public TutorialHouseModel _tutorialHouseModel;
        public TutorialObjectivesDarkScreenModel _tutorialObjectivesDarkScreenModel;
        public TokensModel _tokensModel;
        public StarterPackModel _starterPackModel;
        public FullInventoryModel _fullInventoryModel;
        public ExpandInventoryModel _expandInventoryModel;
        public DropItemModel _dropItemModel;
        public TutorialBuildingModel _tutorialBuildingModel;
        public EventTokensModel _eventTokensModel;
        public FixMovePlayerFromDungeonModel _fixPlayerInWaterfallModel;
        public SideQuestsProvider _sideQuestsProvider;
        public OpenConstructionTutorialModel _openConstructionTutorialModel;
        public AudioEffectsModel _audioEffectsModel;
        public SpesialMessagesModel _spesialMessagesModel;
        public CrewModel _crewModel;
        public CrewMemberDataProvider _crewMemberDataProvider;
        public ConstructionTutorialModel _constructionTutorialModel;
        public EnvironmentTimeModel _environmentTimeModel;
        public CoreEnvironmentModel _coreEnvironmentModel;
        public TeleporHomeModel _teleporHomeModel;
        public ConstructionModel _constructionModel;
        public TreasureLootGroupProvider _treasureLootGroupProvider;
        public QuetsEventDataProvider _quetsEventDataProvider;
        public EnvironmentTransitionProvider _environmentTransitionProvider;

        public ViewsInputModel _viewsInputModel;
        public CinematicCameraModel _cinematicCameraModel;
        public ViewsSystemCameraModel _viewsSystemCameraModel;
        public PopupsTimeScaleModel _popupTimeScaleModel;
        public PerformanceStartCheckModel _performanceStartCheckModel;
        public TutorialSimpleDarkViewModel _tutorialSimpleDarkViewModel;
        public OverrideUISortingModel _overrideUISortingModel;
        public TutorialHilightModel _tutorialHilightModel;
        public InventoryButtonViewModel _inventoryButtonViewModel;
        public CraftButtonViewModel _craftButtonViewModel;
        public TutorialCraftBoostAnaliticsModel _tutorialCraftBoostAnaliticsModel;
        public AnaliticsUserIDModel _analiticsUserIDModel;
        public AttackButtonViewModel _attackButtonViewModel;
        public ResourcesExtractionModel _resourcesExtractionModel;
        public TutorialResourcesModel _tutorialResourcesModel;
        public PlayerLocationModel _playerLocationModel;
        public SECTR_Member _playerSectrMember;
        public AnalyticsPlayerLocationModel _analyticsPlayerLocationModel;
        public BonusItemsModel _bonusItemsModel;
        public FishHealthModel _fishHealthModel;
        public TooltipModel _tooltipModel;
        public CheatModel _сheatModel;
        public MoreGamesModel _moreGamesModel;
        public DiscordButtonViewModel _discordButtonViewModel;
        public FishingLinkViewModel _fishingLinkViewModel;
        public ReplaceBuildingModel _replaceBuildingModel;
        public EncountersModel _encountersModel;
        public EncountersViewModel _encountersViewModel;
        public PushNotificationsModel _pushNotificationsModel;
        public NotificationsModel _notificationsModel;
        public ActivitiesLogViewModel _activitiesLogViewModel;
        public ActivitiesLogButtonViewModel _activitiesLogButtonViewModel;
        public SideQuetsEventDataProvider _sideQuetsEventDataProvider;
        public ColorsViewModel _colorsViewModel;
        public InventoryEquipmentModel _inventoryEquipmentModel;
        public EquipmentModel _equipmentModel;
        public PlayerHealthRegenerationModel _playerHealthRegenerationModel;
        public VillageBuildingModel _villageBuildingModel;
        public PlayerWarmModel _playerWarmModel;
        public VillageAttackModel _villageAttackModel;
        public VillageCinematicsModel _villageCinematicsModel;
        public ZoneModel _zoneModel;
        public PuzzlesModel _puzzlesModel;
        public PickUpsModel _pickUpsModel;
        public HouseBuildingViewModel _houseBuildingViewModel;
        public DragItemViewModel _dragItemViewModel;
        public TaskViewModel _taskViewModel;
        public EnemiesModifierModel _enemiesModifierModel;
        public LearnInputViewModel _learnInputViewModel;
        public WipeSaveModel _wipeSaveModel;
        public DeleteItemModel _deleteItemModel;
        ///CODE_GENERATION_MODELS_SYSTEM

        private void Awake() 
        {
            variable.Value = this;
        }

        #if UNITY_EDITOR
        public void SetLinkValue(Type type, object value)
        {
            FieldInfo[] fields = typeof(ModelsSystem).GetFields(BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public);
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