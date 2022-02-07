using Core;
using Core.Controllers;
using Game.Models;
using Game.Purchases;
using System;
using DevToDev;
using Analytics = DevToDev.Analytics;
using UnityEngine.Purchasing;
using System.Collections.Generic;
using Game.AI;
using Game.Views;
using System.Linq;
using UnityEngine;
using Game.QuestSystem.Data;
using Game.Providers;
using Game.Data.Locations;
using Game.Objectives;
using Encounters.Analytics;
using Game.VillageBuilding;

namespace Game.Controllers
{
    public class AnaliticsDevtodevController : IAnaliticsDevtodevController, IController
    {
        [Inject] public PlayerHungerDamagerModel PlayerHungerDamagerModel { get; private set; }
        [Inject] public PlayerThirstDamagerModel PlayerThirstDamagerModel { get; private set; }
        [Inject] public PlayerColdDamagerModel PlayerColdDamagerModel { get; private set; }
        [Inject] public PlayerObjectivesModel PlayerObjectivesModel { get; private set; }
        [Inject] public PurchasersStoreModel PurchasersStoreModel { get; private set; }
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public BluePrintsModel BluePrintsModel { get; private set; }
        [Inject] public PlacementModel PlacementModel { get; private set; }
        [Inject] public AnaliticsModel AnaliticsModel { get; private set; }
        [Inject] public PurchasesModel PurchasesModel { get; private set; }
        [Inject] public GameTimeModel GameTimeModel { get; private set; }
        [Inject] public TutorialModel TutorialModel { get; private set; }
        [Inject] public EnemiesModel EnemiesModel { get; private set; }
        [Inject] public AnimalsModel AnimalsModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public CoinsModel CoinsModel { get; private set; }
        [Inject] public CraftModel CraftModel { get; private set; }
        [Inject] public ItemsDB ItemsDB { get; private set; }
        [Inject] public StorageModel StorageModel { get; private set; }
        [Inject] public PlayerEventObjectivesModel PlayerEventObjectivesModel { get; private set; }
        [Inject] public TutorialProgressModel TutorialProgressModel { get; private set; }
        [Inject] public StarterPackModel StarterPackModel { get; private set; }
        [Inject] public CoastLootChestModel CoastLootChestModel { get; private set; }
        [Inject] public TreasureHuntModel TreasureHuntModel { get; private set; }
        [Inject] public DeadManLootChestModel DeadManLootChestModel { get; private set; }
        [Inject] public FishingModel FishingModel { get; private set; }
        [Inject] public WaterfallProgressModel WaterfallProgressModel { get; private set; }
        [Inject] public ItemsOwnershipModel ItemsOwnershipModel { get; private set; }
        [Inject] public EnoughSpaceModel EnoughSpaceModel { get; private set; }
        [Inject] public QuestsModel QuestsModel { get; private set; }
        [Inject] public SheltersModel SheltersModel { get; private set; }
        [Inject] public ConstructionModel ConstructionModel { get; private set; }
        [Inject] public SideQuestsModel SideQuestsModel { get; private set; }
        [Inject] public MainQuestsProvider MainQuestsProvider { get; private set; }
        [Inject] public SideQuestsProvider SideQuestsProvider { get; private set; }
        [Inject] public BuildingModeModel BuildingModeModel { get; private set; }
        [Inject] public TutorialCraftBoostAnaliticsModel TutorialCraftBoostAnaliticsModel { get; private set; }
        [Inject] public AnalyticsPlayerLocationModel AnalyticsPlayerLocationModel { get; private set; }
        [Inject] public CoinObjectsModel CoinObjectsModel { get; private set; }
        [Inject] public CheatModel CheatModel { get; private set; }
        [Inject] public ObjectivesWindowModel ObjectivesWindowModel { get; private set; }
        [Inject] public MedallionsModel MedallionsModel { get; private set; }
        [Inject] public LocalNotificationsModel LocalNotificationsModel { get; private set; }
        [Inject] public ObjectivesViewModel ObjectivesViewModel { get; private set; }
        [Inject] public DiscordButtonViewModel DiscordButtonViewModel { get; private set; }
        [Inject] public FishingLinkViewModel FishingLinkViewModel { get; private set; }
        [Inject] public EncountersModel EncountersModel { get; private set; }
        [Inject] public VillageBuildingModel VillageBuildingModel { get; private set; }
        [Inject] public PuzzlesModel PuzzlesModel { get; private set; }
        [Inject] public PickUpsModel PickUpsModel { get; private set; }
        [Inject] public VillageAttackModel VillageAttackModel { get; private set; }
        [Inject] public VikingTutorialModel VikingTutorialModel { get; private set; }


        void IController.Enable()
        {

            if(!AnaliticsModel.EnableAnalitics) return;
            if(!AnaliticsModel.DevToDevAnaliticsInited) return;

            // SheltersModel.OnDeath += OnDeathShelterHandler;
            // SheltersModel.OnBuy += OnBuyShelterHandler;
            // SheltersModel.OnUpgrade += OnUpgradeShelterHandler;
            // SheltersModel.OnUpgrade += OnUpdateShelterHandler;

            PurchasesModel.OnPurchaseSuccessfully += OnPurchaseSuccessfullyHandler;
            PurchasesModel.OnPurchaseUnsuccessfully += OnPurchaseUnsuccessfullyHandler;
            PurchasersStoreModel.OnSuccess.AddListener(ProductID.GoldTier_01, OnPurchaseIapTier01Handler);
            PurchasersStoreModel.OnSuccess.AddListener(ProductID.GoldTier_02, OnPurchaseIapTier02Handler);
            PurchasersStoreModel.OnSuccess.AddListener(ProductID.GoldTier_03, OnPurchaseIapTier03Handler);
            PurchasersStoreModel.OnSuccess.AddListener(ProductID.GoldTier_04, OnPurchaseIapTier04Handler);
            PurchasersStoreModel.OnSuccess.AddListener(ProductID.GoldTier_05, OnPurchaseIapTier05Handler);
            PurchasersStoreModel.OnSuccess.AddListener(ProductID.GoldTier_06, OnPurchaseIapTier06Handler);
            // PurchasersStoreModel.OnSuccess.AddListener(ProductID.survivalPack, OnPurchaseIapSurvivalPackHandler);
            // PurchasersStoreModel.OnSuccess.AddListener(ProductID.dominationPack, OnPurchaseIapDominationPackHandler);
            GameTimeModel.OnChangeDay += OnChangeDayHandler;
            // CraftModel.OnUnlockedItem += OnUnlockedItemHandler;
            EnemiesModel.OnEnemyKillTarget += OnEnemyKillTargetHandler;
            EnemiesModel.OnTargetKillEnemy += OnTargetKillEnemy;
            // AnimalsModel.OnAnimalKillTarget += OnAnimalKillTargetHandler;
            PlayerHungerDamagerModel.OnDamage += OnHungerPlayerDamageHandler;
            PlayerThirstDamagerModel.OnDamage += OnThirstPlayerDamageHandler;
            PlayerColdDamagerModel.OnHealthDecrease += OnColdPlayerDamageHandler;
            PlacementModel.OnPlaceItem += OnPlaceItemHandler;
            CraftModel.OnCraftedItem += OnCraftedItemHandler;
            // ObjectivesWindowModel.OnCompleatedObjective += OnCompletedObjective;
            BluePrintsModel.OnAdd += OnAddBlueprintsHandler;
            // PlayerEventObjectivesModel.OnStartObjective += OnStartEventObjective;
            AnaliticsModel.OnObjectSpawn += OnObjectSpawn; // TODO: посмотреть

            TutorialModel.OnIsActiveChanged += OnTutorialIsActiveChanged;
            // TutorialModel.OnStepChanged += OnTutorialStepChanged;
            VikingTutorialModel.OnAnalyticsStepChanged += OnAnalyticsStepChanged;
            TutorialModel.OnComplete += OnTutorialComplete;

            PurchasersStoreModel._TmpOnSuccess += _TmpOnSuccess; // TODO: deleate
            // ConstructionModel.OnPlacePart += OnPlaceConstrutionPartHandler;

            // StarterPackModel.OnShowStarterPackPopupFromIcon += StarterPackIconClick;
            // StarterPackModel.OnShowStarterPackPopupFirstTime += StarterPackShowedFirstTime;
            // StarterPackModel.OnShowStarterPackPopupSecondTime += StarterPackPopupShowedSecondTime;
            // StarterPackModel.OnShowStarterPackPopupFromLocalNotif += StarterPackOpenedFromLocalNotification;

            // CoastLootChestModel.ChestOpen += OnCoastChestOpen;
            // TreasureHuntModel.ChestOpen += OnTreasureHuntChestOpen;
            // DeadManLootChestModel.ChestOpen += OnDeadManChestOpen;
            // TreasureHuntModel.OnWaitActivate += OnRecieveTreasureHuntBottle;
            // TreasureHuntModel.StartDigging += OnTreasureHuntStarDigging;
            FishingModel.OnStartFishing += OnFishingStart;
            FishingModel.OnHookResult += OnFishingCatchFish;
            FishingModel.OnFisingSuccessful += OnFishingGetFish;
            // WaterfallProgressModel.OnPassedLocation += OnWaterfallExit;
            // WaterfallProgressModel.OnUnlocked += OnWaterfallUnlocked;
            // ItemsOwnershipModel.OnItemOwnedByPlayer += OnItemOwnedByPlayer;
            CraftModel.OnCraftItemStarted += OnCraftItemStarted;
            EnoughSpaceModel.OnShowNotEnoughSpaceView += OnShowNotEnoughSpaceView;
            EnoughSpaceModel.OnShowNotEnoughSpaceCriticalView += OnShowNotEnoughSpaceCriticalView;
            // QuestsModel.OnPreChangeQuest += OnPreChangeQuest;
            // QuestsModel.OnActivateQuest += OnActivateQuest;
            // QuestsModel.OnPreActivateStage += OnPreActivateStage;
            // SideQuestsModel.OnActivate += OnSideQuestActivate;
            // SideQuestsModel.OnCompleate += OnSideQuestCompleted;
            // SideQuestsModel.OnStageCompleted += OnSideQuestStageComplited;
            // BuildingModeModel.BuildingEnabled += OnBuildingEnabled;
            CoinObjectsModel.OnCollectCoin += OnCollectCoin; // TODO: check coins collect
            // MedallionsModel.OnCollect += OnMedallionCollect;
            // LocalNotificationsModel.OnGameStartedFromNotification += OnGameStartedFromNotification;
            // ObjectivesViewModel.OnRedirectedFromPushNotification += OnRedirectedFromPushNotification;
            DiscordButtonViewModel.OnClick += OnClickOnDiscordButton;
            // FishingLinkViewModel.OnClick += OnRedirectToLastFish;
            // EncountersModel.OnEncounterInit += OnEncounterInit;
            // EncountersModel.OnEncounterCompleate += OnEncounterCompleate;
            // EncountersModel.OnEncounterUnload += OnEncounterUnload;

            // Last Viking
            VillageBuildingModel.OnStartBuildingProcess += OnStartBuildingProcess;
            VillageBuildingModel.OnCompleteBuildingProcess += OnCompleteBuildingProcess;
            PuzzlesModel.OnPuzzleActivated += OnPuzzleActivated;
            PickUpsModel.OnPickUp += OnPickUp;
            VillageAttackModel.OnTownHallUpgradeFailed += OnTownHallUpgradeFailed;
            // Last Viking


            // TutorialCraftBoostAnaliticsModel.OnItemCrafted += OnTutorialFirstCraftHandler;
            // AnalyticsPlayerLocationModel.OnSendPlayerLocationEvent += OnSendPlayerLocationEvent;

            ViewsSystem.OnEndShow.AddListener(ViewConfigID.Purchases, OnShowPurchasesHandler);
            ViewsSystem.OnBeginShow.AddListener(ViewConfigID.Craft, OnShowCraftHandler);

            // this event can be triggered on game start so we check it here
            if(CheatModel.PlayerIsCheater && !CheatModel.IsCheatEventSent)
            {
                OnCheatDetected();
            }

            CheatModel.OnCheatDetected += OnCheatDetected;

            // TutorialModel.OnComplete += UpdateTutorialProperties; // TODO: redo this with new tutorial
            // UpdateTutorialProperties();
            

            StorageModel.TryProcessing(AnaliticsModel._Data);
            // if (!AnaliticsModel.HasShownTomb)
            // {
            //     AnaliticsModel.HasShownTomb = true;

            //     ViewsSystem.OnBeginShow.AddListener(ViewConfigID.TombPopup, OnShowTombPopupHandler);
            // }
        }

        void IController.Start()
        {
        }

        void IController.Disable()
        {
            
            if(!AnaliticsModel.EnableAnalitics) return;
            if(!AnaliticsModel.DevToDevAnaliticsInited) return;
            
            // SheltersModel.OnDeath -= OnDeathShelterHandler;
            // SheltersModel.OnBuy -= OnBuyShelterHandler;
            // SheltersModel.OnUpgrade -= OnUpgradeShelterHandler;
            // SheltersModel.OnUpgrade -= OnUpdateShelterHandler;

            PurchasesModel.OnPurchaseSuccessfully -= OnPurchaseSuccessfullyHandler;
            PurchasesModel.OnPurchaseUnsuccessfully -= OnPurchaseUnsuccessfullyHandler;
            PurchasersStoreModel.OnSuccess.RemoveListener(ProductID.GoldTier_01, OnPurchaseIapTier01Handler);
            PurchasersStoreModel.OnSuccess.RemoveListener(ProductID.GoldTier_02, OnPurchaseIapTier02Handler);
            PurchasersStoreModel.OnSuccess.RemoveListener(ProductID.GoldTier_03, OnPurchaseIapTier03Handler);
            PurchasersStoreModel.OnSuccess.RemoveListener(ProductID.GoldTier_04, OnPurchaseIapTier04Handler);
            PurchasersStoreModel.OnSuccess.RemoveListener(ProductID.GoldTier_05, OnPurchaseIapTier05Handler);
            PurchasersStoreModel.OnSuccess.RemoveListener(ProductID.GoldTier_06, OnPurchaseIapTier06Handler);
            // PurchasersStoreModel.OnSuccess.RemoveListener(ProductID.survivalPack, OnPurchaseIapSurvivalPackHandler);
            // PurchasersStoreModel.OnSuccess.RemoveListener(ProductID.dominationPack, OnPurchaseIapDominationPackHandler); GameTimeModel.OnChangeDay -= OnChangeDayHandler;
            GameTimeModel.OnChangeDay -= OnChangeDayHandler;
            // CraftModel.OnUnlockedItem -= OnUnlockedItemHandler;
            EnemiesModel.OnEnemyKillTarget -= OnEnemyKillTargetHandler;
            EnemiesModel.OnTargetKillEnemy -= OnTargetKillEnemy;
            // AnimalsModel.OnAnimalKillTarget -= OnAnimalKillTargetHandler;
            PlayerHungerDamagerModel.OnDamage -= OnHungerPlayerDamageHandler;
            PlayerThirstDamagerModel.OnDamage -= OnThirstPlayerDamageHandler;
            PlayerColdDamagerModel.OnHealthDecrease -= OnColdPlayerDamageHandler;
            PlacementModel.OnPlaceItem -= OnPlaceItemHandler;
            CraftModel.OnCraftedItem -= OnCraftedItemHandler;
            // ObjectivesWindowModel.OnCompleatedObjective -= OnCompletedObjective;
            BluePrintsModel.OnAdd -= OnAddBlueprintsHandler;
            // PlayerEventObjectivesModel.OnStartObjective -= OnStartEventObjective;
            AnaliticsModel.OnObjectSpawn -= OnObjectSpawn;

            TutorialModel.OnIsActiveChanged -= OnTutorialIsActiveChanged;
            // TutorialModel.OnStepChanged -= OnTutorialStepChanged;
            VikingTutorialModel.OnAnalyticsStepChanged -= OnAnalyticsStepChanged;
            TutorialModel.OnComplete -= OnTutorialComplete;

            PurchasersStoreModel._TmpOnSuccess -=_TmpOnSuccess;
            // ConstructionModel.OnPlacePart -= OnPlaceConstrutionPartHandler;
            
            // StarterPackModel.OnShowStarterPackPopupFromIcon -= StarterPackIconClick;
            // StarterPackModel.OnShowStarterPackPopupFirstTime -= StarterPackShowedFirstTime;
            // StarterPackModel.OnShowStarterPackPopupSecondTime -= StarterPackPopupShowedSecondTime;
            // StarterPackModel.OnShowStarterPackPopupFromLocalNotif -= StarterPackOpenedFromLocalNotification;

            // CoastLootChestModel.ChestOpen -= OnCoastChestOpen;
            // TreasureHuntModel.ChestOpen -= OnTreasureHuntChestOpen;
            // DeadManLootChestModel.ChestOpen -= OnDeadManChestOpen;
            // TreasureHuntModel.OnWaitActivate -= OnRecieveTreasureHuntBottle;
            // TreasureHuntModel.StartDigging -= OnTreasureHuntStarDigging;
            FishingModel.OnStartFishing -= OnFishingStart;
            FishingModel.OnHookResult -= OnFishingCatchFish;
            FishingModel.OnFisingSuccessful -= OnFishingGetFish;
            // WaterfallProgressModel.OnPassedLocation -= OnWaterfallExit;
            // WaterfallProgressModel.OnUnlocked -= OnWaterfallUnlocked;
            // ItemsOwnershipModel.OnItemOwnedByPlayer -= OnItemOwnedByPlayer;
            CraftModel.OnCraftItemStarted -= OnCraftItemStarted;
            EnoughSpaceModel.OnShowNotEnoughSpaceView -= OnShowNotEnoughSpaceView;
            EnoughSpaceModel.OnShowNotEnoughSpaceCriticalView -= OnShowNotEnoughSpaceCriticalView;
            // QuestsModel.OnPreChangeQuest -= OnPreChangeQuest;
            // QuestsModel.OnActivateQuest -= OnActivateQuest;
            // QuestsModel.OnPreActivateStage -= OnPreActivateStage;
            // SideQuestsModel.OnActivate -= OnSideQuestActivate;
            // SideQuestsModel.OnCompleate -= OnSideQuestCompleted;
            // SideQuestsModel.OnStageCompleted -= OnSideQuestStageComplited;
            // BuildingModeModel.BuildingEnabled -= OnBuildingEnabled;
            CoinObjectsModel.OnCollectCoin -= OnCollectCoin;
            // MedallionsModel.OnCollect -= OnMedallionCollect;
            // LocalNotificationsModel.OnGameStartedFromNotification -= OnGameStartedFromNotification;
            // ObjectivesViewModel.OnRedirectedFromPushNotification -= OnRedirectedFromPushNotification;
            DiscordButtonViewModel.OnClick -= OnClickOnDiscordButton;
            // FishingLinkViewModel.OnClick -= OnRedirectToLastFish;
            // EncountersModel.OnEncounterInit -= OnEncounterInit;
            // EncountersModel.OnEncounterCompleate -= OnEncounterCompleate;
            // EncountersModel.OnEncounterUnload -= OnEncounterUnload;
            // Last Viking
            VillageBuildingModel.OnStartBuildingProcess -= OnStartBuildingProcess;
            VillageBuildingModel.OnCompleteBuildingProcess -= OnCompleteBuildingProcess;
            PuzzlesModel.OnPuzzleActivated -= OnPuzzleActivated;
            PickUpsModel.OnPickUp -= OnPickUp;
            VillageAttackModel.OnTownHallUpgradeFailed -= OnTownHallUpgradeFailed;
            // Last Viking

            // TutorialCraftBoostAnaliticsModel.OnItemCrafted -= OnTutorialFirstCraftHandler;
            // AnalyticsPlayerLocationModel.OnSendPlayerLocationEvent -= OnSendPlayerLocationEvent;

            CheatModel.OnCheatDetected -= OnCheatDetected;
            // TutorialModel.OnComplete -= UpdateTutorialProperties;

            ViewsSystem.OnEndShow.RemoveListener(ViewConfigID.Purchases, OnShowPurchasesHandler);
            ViewsSystem.OnBeginShow.RemoveListener(ViewConfigID.Craft, OnShowCraftHandler);
            // ViewsSystem.OnBeginShow.RemoveListener(ViewConfigID.TombPopup, OnShowTombPopupHandler);
        }

        private void OnTutorialIsActiveChanged()
        {
            if(TutorialModel.IsActive)
            {
                Analytics.Tutorial(TutorialState.Start);
            }
            else
            {
                Analytics.Tutorial(TutorialState.Finish);
            }
        }

        private void OnAnalyticsStepChanged()
        {
            Analytics.Tutorial(VikingTutorialModel.AnalyticsStep);
        }

        private void OnTutorialComplete()
        {
            Analytics.Tutorial(TutorialState.Finish);
        }

        private bool IsStorePurchase(ICoinRewardInfo coinReward) => coinReward is IPurchaseStoreInfo;
        private bool IsStoreRewardedPurchase(ICoinRewardInfo coinReward) => IsStorePurchase(coinReward) || coinReward is IWatchPurchase;

        private void SendCustom(AnaliticEventID eventID) => Analytics.CustomEvent(AnaliticsModel.GetEventName(eventID));
        private void SendCustom(string eventName) => Analytics.CustomEvent(eventName);
        private void SendCustom(AnaliticEventID eventID, CustomEventParams args) => Analytics.CustomEvent(AnaliticsModel.GetEventName(eventID), args);
        private void SendCustom(string eventName, CustomEventParams args) => Analytics.CustomEvent(eventName, args);
        private void SendCustomWithPrefix(AnaliticEventID eventID, string prefix, CustomEventParams args) => Analytics.CustomEvent(prefix + AnaliticsModel.GetEventName(eventID), args);

        private void OnTutorialCompleteHandler()
        {
            Analytics.Tutorial(TutorialState.Finish);
        }

        private void OnTutorialStartHandler()
        {
            Analytics.Tutorial(TutorialState.Start);
        }

        private void OnTutorialStepHandler(int step)
        {
            // basic dev to dev analitics
            Analytics.Tutorial(step);

            // custom events
            string stepText = step < 10 ? "0" + step : step.ToString();
            var eventName = AnaliticEventID.CompleteTutorialStep_XX.ToString().Replace("XX",stepText);
            SendCustom(eventName);
        }

        private void OnPurchaseSuccessfullyHandler(PurchaseID purchaseID)
        {
            var coinCost = PurchasesModel.GetInfo<IPurchaseCoinInfo>(purchaseID);
            if (coinCost != null)
            {
                var args = new CustomEventParams();
                args.AddParam(AnaliticsModel.ParamPurchaseID, purchaseID.ToString());
                SendCustom(AnaliticEventID.CoinsSpend, args);
                Analytics.InAppPurchase(purchaseID.ToString(), AnaliticsModel.NoneGroup, 1, coinCost.CoinCost, AnaliticsModel.CoinCurrency); // spent currenty in-game
                return;
            }

            var coinReward = PurchasesModel.GetInfo<ICoinRewardInfo>(purchaseID);
            if (coinReward != null)
            {
                var args = new CustomEventParams();
                args.AddParam(AnaliticsModel.ParamPurchaseID, purchaseID.ToString());
                SendCustom(AnaliticEventID.CoinsGet, args);   

                if (IsStorePurchase(coinReward))
                {
                    Analytics.CurrencyAccrual(coinReward.CoinsAdd, AnaliticsModel.CoinCurrency, AccrualType.Purchased);
                }
                else
                {
                    Analytics.CurrencyAccrual(coinReward.CoinsAdd, AnaliticsModel.CoinCurrency, AccrualType.Earned);
                }
                return;
            }

            var watch = PurchasesModel.GetInfo<IWatchPurchase>(purchaseID);
            if (watch != null)
            {
                var args = new CustomEventParams();
                args.AddParam(AnaliticsModel.ParamPurchaseID, purchaseID.ToString());
                SendCustom(AnaliticEventID.WatchRevardedVideo, args);
                return;
            }
        }

        private void OnPurchaseUnsuccessfullyHandler(PurchaseID purchaseID)
        {
            var coinCost = PurchasesModel.GetInfo<IPurchaseCoinInfo>(purchaseID);
            if (coinCost != null)
            {
                var args = new CustomEventParams();
                args.AddParam(AnaliticsModel.ParamPurchaseID, purchaseID.ToString());
                SendCustom(AnaliticEventID.NotEnoughCoins, args);
                return;
            }
        }

        private void OnPurchaseIapHandler(ProductID productID)
        {
            Product product = PurchasersStoreModel.GetProduct(productID);
            Analytics.RealPayment(product.transactionID, (float)Convert.ToDouble(product.metadata.localizedPrice), product.definition.id, product.metadata.isoCurrencyCode);
        }

        private void OnPurchaseIapTier01Handler() => OnPurchaseIapHandler(ProductID.GoldTier_01);
        private void OnPurchaseIapTier02Handler() => OnPurchaseIapHandler(ProductID.GoldTier_02);
        private void OnPurchaseIapTier03Handler() => OnPurchaseIapHandler(ProductID.GoldTier_03);
        private void OnPurchaseIapTier04Handler() => OnPurchaseIapHandler(ProductID.GoldTier_04);
        private void OnPurchaseIapTier05Handler() => OnPurchaseIapHandler(ProductID.GoldTier_05);
        private void OnPurchaseIapTier06Handler() => OnPurchaseIapHandler(ProductID.GoldTier_06);
        // private void OnPurchaseIapSurvivalPackHandler() => OnPurchaseIapHandler(ProductID.survivalPack);
        // private void OnPurchaseIapDominationPackHandler() => OnPurchaseIapHandler(ProductID.dominationPack);

        private void OnChangeDayHandler()
        {
            int level = Convert.ToInt32(GameTimeModel.Days) + 1;
            Dictionary<string, int> currencies = new Dictionary<string, int>();
            currencies.Add(AnaliticsModel.CoinCurrency, CoinsModel.Coins);
            currencies.Add(AnaliticsModel.BluepringCurrency, BluePrintsModel.BluePrints);
            Analytics.LevelUp(level, currencies);
        }

        private void OnUnlockedItemHandler(int itemId)
        {
            var item = ItemsDB.GetItem(itemId);
            Analytics.InAppPurchase(item.Name, AnaliticsModel.UnlockGroup, 1, item.UnlockablesItems[0].Amount, AnaliticsModel.BluepringCurrency); // spent
        }

        private void OnThirstPlayerDamageHandler()
        {
            if (PlayerHealthModel.IsDead)
            {
                PlayerDeathEvent(AnaliticsModel.ThirstDeathReason);
            }
        }
        private void OnHungerPlayerDamageHandler()
        {
            if (PlayerHealthModel.IsDead)
            {
                PlayerDeathEvent(AnaliticsModel.HungerDeathReason);
            }
        }
        private void OnColdPlayerDamageHandler()
        {
            if (PlayerHealthModel.IsDead)
            {
                PlayerDeathEvent(AnaliticsModel.ColdDeathReason);
            }
        }
        private void OnAnimalKillTargetHandler(AnimalID animalID, Target target)
        {
            if (target.ID == TargetID.Player)
            {
                PlayerDeathEvent(animalID.ToString());
            }
        }
        private void OnEnemyKillTargetHandler(EnemyID enemyID, Target target)
        {
            if (target.ID == TargetID.Player)
            {
                PlayerDeathEvent(enemyID.ToString());
            }
        }

        private void PlayerDeathEvent(string deathReason)
        {
            var args = new CustomEventParams();
            args.AddParam(AnaliticsModel.ParamDeathReason, deathReason);
            SendCustom(AnaliticEventID.PlayerDeath, args);
        }

        private void OnShowPurchasesHandler() => SendCustom(AnaliticEventID.OpenStore);

        private void OnPlaceItemHandler(int itemId)
        {
            var args = new CustomEventParams();
            args.AddParam(AnaliticsModel.ParamItemID, ItemsDB.GetItem(itemId).Name);
            SendCustom(AnaliticEventID.BuildItem, args);
        }

        private void OnPlaceConstrutionPartHandler(string partId)
        {
            var args = new CustomEventParams();
            args.AddParam(AnaliticsModel.ParamConstructionPartID, partId);
            SendCustom(AnaliticEventID.PlaceConstructionPart, args);
        }

        private void OnCraftedItemHandler(int itemId)
        {
            var args = new CustomEventParams();
            args.AddParam(AnaliticsModel.ParamItemID, ItemsDB.GetItem(itemId).Name);
            SendCustom(AnaliticEventID.ItemsCrafted, args);
        }

        private void OnCompletedObjective(ObjectiveModel model)
        {
            bool tasksPoolCompleated;
            int predefinedMissionsTierNum;
            try
            {
                tasksPoolCompleated = IsAllObjectivesCompleate();
                TryGetPredefinedTierNum(out predefinedMissionsTierNum);
            }
            catch (System.Exception exeption)
            {
                Debug.LogError(exeption);
                return;
            }

            if(!tasksPoolCompleated) return;
            
            var globalDays = GameTimeModel.GetDays(GameTimeModel.Ticks - GameTimeModel.StartTicks);

            if(IsPrefedinedPool())
            {
                var args = new CustomEventParams();
                args.AddParam(AnaliticsModel.ParamGlobalDays, globalDays);
                args.AddParam(AnaliticsModel.ParamPredefinedMissionNum, predefinedMissionsTierNum);
                SendCustom(AnaliticEventID.PredefinedMissions_Completed,args);
                args.Log();
            }
            else
            {
                var args = new CustomEventParams();
                args.AddParam(AnaliticsModel.ParamGlobalDays, globalDays);
                SendCustom(AnaliticEventID.Missions_Completed,args);
                args.Log();
            }

            #region Help Methods
                
            bool TryGetPredefinedTierNum(out int num)
            {
                bool answer = false;
                num = -1;
                var states = ObjectivesWindowModel.RequiredTiersStates;
                
                for (int i = 0; i < states.Count; i++)
                {
                    if(states[i] == false)
                    {
                        answer = true;
                        num = i + 1;
                        return answer;
                    }
                }

                return answer;
            }

            bool IsAllObjectivesCompleate()
            {
                int notCompleatedObjectives = ObjectivesWindowModel.SelectedObjectivesData.Where(x => !x.Done).Count();
                return notCompleatedObjectives == 0;
            }

            bool IsPrefedinedPool() => !ObjectivesWindowModel.RequiredObjectivesCompleated;

            #endregion
        }

        private void OnAddBlueprintsHandler(int count)
        {
            Analytics.CurrencyAccrual(count, AnaliticsModel.BluepringCurrency, AccrualType.Earned);
        }

        private void OnShowTombPopupHandler()
        {
            ViewsSystem.OnBeginShow.RemoveListener(ViewConfigID.TombPopup, OnShowTombPopupHandler);

            var args = new CustomEventParams();
            args.AddParam(AnaliticsModel.ParamInteraction, FirstInteractionUIProgressID.Tomb.ToString());
            SendCustom(AnaliticEventID.FirstInteractionUI, args);
        }

        private void OnStartEventObjective()
        {
            var args = new CustomEventParams();
            args.AddParam(AnaliticsModel.ParamMissionID, PlayerEventObjectivesModel.CurrentObjectiveID.ToString());
            SendCustom(AnaliticEventID.GetEventsMission, args);
        }

        private void OnObjectSpawn(WorldObjectID worldObjectID)
        {
            var args = new CustomEventParams();
            args.AddParam(AnaliticsModel.ParamWorldObjectID, worldObjectID.ToString());
            SendCustom(AnaliticEventID.SpawnObjectInZero, args);
        }

        private void _TmpOnSuccess(ProductID productID)
        {
            PurchasersStoreModel._TmpOnSuccess -= _TmpOnSuccess;

            var globalDays = GameTimeModel.GetDays(GameTimeModel.Ticks - GameTimeModel.StartTicks);
            var worldSecs = GameTimeModel.GetSecondsTotal(Convert.ToInt64(globalDays));
            var args = new CustomEventParams();
            args.AddParam(AnaliticsModel.ParamPurchaseID, productID.ToString());
            args.AddParam(AnaliticsModel.ParamWorldTime, worldSecs);
            SendCustom(AnaliticEventID._PromoInapPurchase, args);
        }

        #region Starter Pack events
        private void StarterPackIconClick() => SendCustom(AnaliticEventID.StraterPackOpenView00);
        private void StarterPackShowedFirstTime() => SendCustom(AnaliticEventID.StraterPackOpenView01);
        private void StarterPackPopupShowedSecondTime() => SendCustom(AnaliticEventID.StraterPackOpenView02);
        private void StarterPackOpenedFromLocalNotification() => SendCustom(AnaliticEventID.StraterPackOpenViewNotif);
        #endregion

        private void OnCoastChestOpen() => SendCustom(AnaliticEventID.GetOceanChest);

        private void OnTreasureHuntChestOpen(int emptyDiggetCount) {
            var args = new CustomEventParams();
            args.AddParam(AnaliticsModel.ParamDiggedCount, emptyDiggetCount);
            SendCustom(AnaliticEventID.GetTreasureHuntChest, args);
        }

        private void OnDeadManChestOpen() => SendCustom(AnaliticEventID.GetWaterfallChest);

        private void OnRecieveTreasureHuntBottle() => SendCustom(AnaliticEventID.TreasureHuntGetBottle);

        private void OnTreasureHuntStarDigging() => SendCustom(AnaliticEventID.TreasureHuntStartDigging);

        private void OnFishingStart(bool isOk) {
            if (isOk)
            {
                SendCustom(AnaliticEventID.FishingStart);
            }
        }

        private void OnFishingCatchFish(bool isCatched) {
            if (isCatched)
            {
                SendCustom(AnaliticEventID.FishingCatchFish);
            }
        }

        private void OnWaterfallExit(DungeonProgressModel dungeonProgressModel) => SendCustom(AnaliticEventID.WaterfallExit);

        private void OnFishingGetFish() => SendCustom(AnaliticEventID.FishingGetFish);

        private void OnWaterfallUnlocked() => SendCustom(AnaliticEventID.WaterfallUnlocked);

        private void OnItemOwnedByPlayer(int itemId, int count) 
        {
            var item = ItemsDB.GetItem(itemId);
            if (item != null && item.Category == "Components") 
            {
                Analytics.CurrencyAccrual(count, item.Name, AccrualType.Earned);
            }
        }

        private void OnCraftItemStarted(int itemId) {
            var item = ItemsDB.GetItem(itemId);
            if (item != null)
            {
                foreach (var requiredItem in item.Recipe.RequiredItems)
                {
                    var itemData = ItemsDB.GetItem(requiredItem.Name);
                    if (itemData.Category == "Components")
                    {
                        Analytics.InAppPurchase(item.Name, AnaliticsModel.NoneGroup, 1, requiredItem.Amount, requiredItem.Name); // spent
                    }
                }
            }
        }

        private void OnShowNotEnoughSpaceView()
        {
            SendCustom(AnaliticEventID.NotEnoughStorageSpace);
        }
        private void OnShowNotEnoughSpaceCriticalView()
        {
            SendCustom(AnaliticEventID.NotEnoughStorageSpaceCritical);
        }

        private void OnPreChangeQuest()
        {
            if(MainQuestsProvider.TryGetQuestIndex(QuestsModel.ActiveQuest,out int questNum))
            {
                string eventName = AnaliticEventID.Quest_Main_QQ_Completed.ToString().Replace(AnaliticsModel.QuestNumber, $"{questNum:00}");
                SendCustom(eventName);
            }
        }

        private void OnActivateQuest()
        {
            if(MainQuestsProvider.TryGetQuestIndex(QuestsModel.ActiveQuest,out int questNum))
            {
                string eventName = AnaliticEventID.Quest_Main_QQ_Started.ToString().Replace(AnaliticsModel.QuestNumber, $"{questNum:00}");
                SendCustom(eventName);
            }
        }

        private void OnPreActivateStage()
        {
            if(MainQuestsProvider.TryGetQuestIndex(QuestsModel.ActiveQuest,out int questNum))
            {
                int stageNum = QuestsModel.ActiveStage;
                string eventName = AnaliticEventID.Quest_Main_QQ_Stage_SS.ToString()
                    .Replace(AnaliticsModel.QuestNumber, $"{questNum:00}")
                    .Replace(AnaliticsModel.QuestStageNumber, $"{stageNum:00}");

                SendCustom(eventName);
            }
        }

        private void OnSideQuestActivate(QuestData questData)
        {
            if(SideQuestsProvider.TryGetQuestIndex(questData, out int questNum))
            {
                string eventName = AnaliticEventID.Quest_Optional_QQ_Started.ToString().Replace(AnaliticsModel.QuestNumber, $"{questNum:00}");
                SendCustom(eventName);
            }
        }

        private void OnSideQuestCompleted(QuestData questData)
        {
            if(SideQuestsProvider.TryGetQuestIndex(questData, out int questNum))
            {
                string eventName = AnaliticEventID.Quest_Optional_QQ_Completed.ToString().Replace(AnaliticsModel.QuestNumber, $"{questNum:00}");
                SendCustom(eventName);
            }
        }

        private void OnSideQuestStageComplited(QuestData questData)
        {
            if(SideQuestsProvider.TryGetQuestIndex(questData,out int questNum))
            {
                int stageNum = SideQuestsModel.GetQuestStage(questData);
                string eventName = AnaliticEventID.Quest_Optional_QQ_Stage_SS.ToString()
                    .Replace(AnaliticsModel.QuestNumber, $"{questNum:00}")
                    .Replace(AnaliticsModel.QuestStageNumber, $"{stageNum:00}");

                SendCustom(eventName);
            }
        }
        
        private void OnUpdateShelterHandler(ShelterModel shelterModel)
        {
            //TODO: implement

            // var shelterLevel = shelterModel.Level;
            // var glodalDays = GameTimeModel.GetDays(GameTimeModel.Ticks - GameTimeModel.StartTicks);
            // var shelterLifeTime = GameTimeModel.GetDays(GameTimeModel.Ticks - shelterModel.StartAliveTimeTicks);
            // if (shelterLevel == 1)
            // {
            //     AnaliticsModel.Send(AnaliticEventID.UpdateShelterLevel01, shelterLevel, glodalDays, shelterLifeTime);
            // }
            // if (shelterLevel == 2)
            // {
            //     AnaliticsModel.Send(AnaliticEventID.UpdateShelterLevel02, shelterLevel, glodalDays, shelterLifeTime);
            // }

        }

        private void OnUpgradeShelterHandler(ShelterModel shelterModel)
        {  
            Analytics.ActiveUser.SetUserData(AnaliticsModel.Property_ShipLevel, SheltersModel.ShelterModel.Level);
        }

        private void OnBuyShelterHandler(ShelterModel shelterModel)
        {
            Analytics.ActiveUser.SetUserData(AnaliticsModel.Property_ShipLevel, SheltersModel.ShelterModel.Level);
        }

        private void OnDeathShelterHandler(ShelterModel shelterModel)
        {
            var glodalDays = GameTimeModel.GetDays(GameTimeModel.Ticks - GameTimeModel.StartTicks);
            var args = new CustomEventParams();
            args.AddParam(AnaliticsModel.ParamShelterLevel, shelterModel.Level.ToString());
            args.AddParam(AnaliticsModel.ParamGlobalDays, glodalDays.ToString());
            SendCustom(AnaliticEventID.DeathShelter, args);
        }

        private void OnBuildingEnabled()
        {
            if(TutorialModel.IsComplete)
            {
                SendCustom(AnaliticEventID.ConstructionEnabled);
            }
        }

        private void OnCollectCoin(string fromName)
        {
            var args = new CustomEventParams();
            args.AddParam(AnaliticsModel.ParamFrom, fromName);
            SendCustom(AnaliticEventID.CollectCoin, args);
        }

        private void OnTutorialFirstCraftHandler(bool boostUsed)
        {
            var args = new CustomEventParams();
            args.AddParam(AnaliticsModel.ParamCraftBoostUsed, boostUsed.ToString());
            SendCustom(AnaliticEventID.TutorialFirstCraftBoost, args);
        }

        private void OnShowCraftHandler()
        {
            if(TutorialModel.IsComplete)
            {
                SendCustom(AnaliticEventID.OpenCraft);
            }
        }

        private void OnSendPlayerLocationEvent(LocationId locationId)
        {
            var args = new CustomEventParams();
            args.AddParam(AnaliticsModel.ParamLocation, locationId.ToString());
            SendCustom(AnaliticEventID.PlayerLocation, args);
        }

        private void OnCheatDetected()
        {
            Analytics.ActiveUser.Cheater = true;
            Dictionary<string,object> userData = new Dictionary<string, object>();
            userData.Add(AnaliticsModel.Property_IsCheater,true);
            userData.Add(AnaliticsModel.Property_IsCheater_Altenrative,1);
            userData.Add(AnaliticsModel.Property_NotCheater,false);

            Analytics.ActiveUser.SetUserData(userData);

            CheatModel.SetEventSent();
        }

        private void UpdateTutorialProperties()
        {
            bool tutorialFinished = TutorialModel.IsComplete;
            Analytics.ActiveUser.SetUserData(AnaliticsModel.Property_NotFinishedTutorial, !tutorialFinished);
        }

        private void OnMedallionCollect()
        {
            if(MedallionsModel.Collected >= MedallionsModel.Total)
            {
                SendCustom(AnaliticEventID.AllMedallionsCollected);
            }
        }

        private void OnGameStartedFromNotification(LocalNotificationID notificationID)
        {
            var args = new CustomEventParams();
            args.AddParam(AnaliticsModel.ParamNotificationId,notificationID.ToString());

            SendCustom(AnaliticEventID.StartedGameFromNotification, args);
        }

        private void OnRedirectedFromPushNotification()
        {
            SendCustom(AnaliticEventID.RedirectedFromNotificationToObjectives);
        }

        private void OnRedirectToLastFish()
        {
            SendCustom(AnaliticEventID.RedirectToLastFishButtonClick);
        }

        private void OnClickOnDiscordButton()
        {
            SendCustom(AnaliticEventID.RedirectToDiscordButtonClick);
        }

        private void OnEncounterInit(EncounterInitEvent initEvent)
        {
            var args = new CustomEventParams();
            args.AddParam(AnaliticsModel.ParamEncounterType,initEvent.data.encounterType.ToString());
            args.AddParam(AnaliticsModel.ParamEncounterID,initEvent.data.encounterID.ToString());
            args.AddParam(AnaliticsModel.ParamEncounterDescription,initEvent.data.description);

            SendCustom(AnaliticEventID.Encounter_Init,args);
        }
        private void OnEncounterCompleate(EncounterCompleateEvent compleateEvent)
        {
            var args = new CustomEventParams();
            args.AddParam(AnaliticsModel.ParamEncounterType,compleateEvent.data.encounterType.ToString());
            args.AddParam(AnaliticsModel.ParamEncounterID,compleateEvent.data.encounterID.ToString());
            args.AddParam(AnaliticsModel.ParamEncounterDescription,compleateEvent.data.description);

            SendCustom(AnaliticEventID.Encounter_Compleate,args);

        }
        private void OnEncounterUnload(EncounterUnloadEvent unloadEvent)
        {
            var args = new CustomEventParams();
            args.AddParam(AnaliticsModel.ParamEncounterType,unloadEvent.data.encounterType.ToString());
            args.AddParam(AnaliticsModel.ParamEncounterID, unloadEvent.data.encounterID.ToString());
            args.AddParam(AnaliticsModel.ParamEncounterDescription,unloadEvent.data.description);
            args.AddParam(AnaliticsModel.ParamEncounterUnloadReason,unloadEvent.unloadReason.ToString());

            SendCustom(AnaliticEventID.Encounter_Unload,args);
        }

        private void OnStartBuildingProcess(HouseBuilding houseBuilding)
        {
            var args = new CustomEventParams();
            args.AddParam(AnaliticsModel.ParamType, houseBuilding.Type.ToString());
            args.AddParam(AnaliticsModel.ParamLevel, houseBuilding.Level);

            SendCustom(AnaliticEventID.HouseBuildingStart, args);
        }

        private void OnCompleteBuildingProcess(HouseBuilding houseBuilding)
        {
            var args = new CustomEventParams();
            args.AddParam(AnaliticsModel.ParamType, houseBuilding.Type.ToString());
            args.AddParam(AnaliticsModel.ParamLevel, houseBuilding.Level);

            SendCustom(AnaliticEventID.HouseBuildingComplete, args);
        }

        private void OnPuzzleActivated(string puzzleName)
        {
            var args = new CustomEventParams();
            args.AddParam(AnaliticsModel.ParamName, puzzleName);

            SendCustom(AnaliticEventID.PuzzleActivated, args);
        }

        private void OnPickUp(string itemName, int count)
        {
            var args = new CustomEventParams();
            args.AddParam(AnaliticsModel.ParamName, itemName);
            args.AddParam(AnaliticsModel.ParamCount, count);

            SendCustom(AnaliticEventID.PickUp, args);
        }

        private void OnTownHallUpgradeFailed()
        {
            var args = new CustomEventParams();
            args.AddParam(AnaliticsModel.ParamLevel, VillageBuildingModel.Townhall.Level);

            SendCustom(AnaliticEventID.TwonHallDeath, args);
        }

        private void OnTargetKillEnemy(Target target, EnemyID enemyID)
        {
            var args = new CustomEventParams();
            args.AddParam(AnaliticsModel.ParamEnemyID, enemyID.ToString());

            SendCustom(AnaliticEventID.KillEnemy, args);
        }
    }
}