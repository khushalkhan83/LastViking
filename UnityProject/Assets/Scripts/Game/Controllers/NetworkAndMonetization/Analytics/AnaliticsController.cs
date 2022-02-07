using Core;
using Core.Controllers;
using Game.AI;
using Game.Models;
using Game.Purchases;
using Game.Views;
using System;
using System.Linq;
using UltimateSurvival;
using UnityEngine.Analytics;

namespace Game.Controllers
{
    public class AnaliticsController : IAnaliticsController, IController
    {
        [Inject] public StatisticWorldObjectsNodel StatisticWorldObjectsNodel { get; private set; }
        [Inject] public PlayerHungerDamagerModel PlayerHungerDamagerModel { get; private set; }
        [Inject] public PlayerThirstDamagerModel PlayerThirstDamagerModel { get; private set; }
        [Inject] public PlayerObjectivesModel PlayerObjectivesModel { get; private set; }
        [Inject] public PurchasersStoreModel PurchasersStoreModel { get; private set; }
        [Inject] public StartNightInfoModel StartNightInfoModel { get; private set; }
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public SettingsViewModel SettingsViewModel { get; private set; }
        [Inject] public BluePrintsModel BluePrintsModel { get; private set; }
        [Inject] public PlacementModel PlacementModel { get; private set; }
        [Inject] public StatisticsModel StatisticsModel { get; private set; }
        [Inject] public PurchasesModel PurchasesModel { get; private set; }
        [Inject] public PlayerRunModel PlayerRunModel { get; private set; }
        [Inject] public GameTimeModel GameTimeModel { get; private set; }
        [Inject] public InventoryModel InventoryModel { get; private set; }
        [Inject] public TouchpadModel TouchpadModel { get; private set; }
        [Inject] public AnaliticsModel AnaliticsModel { get; private set; }
        [Inject] public LoseViewModel LoseViewModel { get; private set; }
        [Inject] public SheltersModel SheltersModel { get; private set; }
        [Inject] public JoystickModel JoystickModel { get; private set; }
        [Inject] public EnemiesModel EnemiesModel { get; private set; }
        [Inject] public AnimalsModel AnimalsModel { get; private set; }
        [Inject] public StorageModel StorageModel { get; private set; }
        [Inject] public HotBarModel HotBarModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public CraftModel CraftModel { get; private set; }
        [Inject] public ItemsDB ItemsDB { get; private set; }
        [Inject] public TutorialModel TutorialModel { get; private set; }

        void IController.Enable()
        {
            if(!AnaliticsModel.EnableAnalitics) return;
            
            SheltersModel.OnDeath += OnDeathShelterHandler;
            SheltersModel.OnBuy += OnBuyShelterHandler;
            SheltersModel.OnUpgrade += OnUpgradeShelterHandler;
            PurchasesModel.OnPurchaseSuccessfully += OnPurchaseSuccessfullyHandler;
            CraftModel.OnUnlockedItem += OnUnlockedItemHandler;
            CraftModel.OnCraftedItem += OnCraftedItemHandler;
            LoseViewModel.OnPlayAgain += OnPlayAgainLosePlayerHandler;
            SettingsViewModel.OnResetGame += OnResetGameHandler;
            EnemiesModel.OnEnemyKillTarget += OnEnemyKillTargetHandler;
            AnimalsModel.OnAnimalKillTarget += OnAnimalKillTargetHandler;
            EnemiesModel.OnTargetKillEnemy += OnTargetillEnemyHandler;
            AnimalsModel.OnTargetKillAnimal += OnTargetKillAnimalHandler;
            PlayerHungerDamagerModel.OnDamage += OnHungerPlayerDamageHandler;
            PlayerThirstDamagerModel.OnDamage += OnThirstPlayerDamageHandler;
            PlacementModel.OnPlaceItem += OnPlaceItemHandler;
            InventoryModel.ItemsContainer.OnAddItems += OnAddItemsToInventoryHandler;
            HotBarModel.ItemsContainer.OnAddItems += OnAddItemsToHotBarHandler;
            GameTimeModel.OnChangeDay += OnChangeDayHandler;
            StartNightInfoModel.OnEndNight += OnEndNightHandler;
            SheltersModel.OnUpgrade += OnUpdateShelterHandler;
            TutorialModel.OnComplete += OnCompleteHandler;
            TutorialModel.OnStart += OnStartHandler;
            TutorialModel.OnNextStep += OnNextStepHandler;
            PurchasersStoreModel.OnSuccess.AddListener(ProductID.GoldTier_01, OnPurchaseIapTier01Handler);
            PurchasersStoreModel.OnSuccess.AddListener(ProductID.GoldTier_02, OnPurchaseIapTier02Handler);
            PurchasersStoreModel.OnSuccess.AddListener(ProductID.GoldTier_03, OnPurchaseIapTier03Handler);
            PurchasersStoreModel.OnSuccess.AddListener(ProductID.GoldTier_04, OnPurchaseIapTier04Handler);
            PurchasersStoreModel.OnSuccess.AddListener(ProductID.GoldTier_05, OnPurchaseIapTier05Handler);
            PurchasersStoreModel.OnSuccess.AddListener(ProductID.GoldTier_06, OnPurchaseIapTier06Handler);
            BluePrintsModel.OnAdd += OnAddBlueprints;
            CraftModel.OnUnlockedItem += OnItemUnlockHandler;
            PlayerObjectivesModel.OnPreEndTime += OnPreEndObjectiveTierTimeHandler;
            PlayerObjectivesModel.OnEndTime += OnEndObjectiveTierTimeHandler;

            if (!StorageModel.TryProcessing(AnaliticsModel._Data))
            {
                AnaliticsModel.Send(AnaliticEventID.GameSessionStart);
                AnaliticsModel.Send(AnaliticEventID.GlobalDays, 0);
                AnaliticsModel.Send(AnaliticEventID.FirstProgression, FirstProgressionProgressID.FirstStrart);

                ViewsSystem.OnEndShow.AddListener(ViewConfigID.InventoryPlayer, OnShowPlayerInventoryHandler);
                ViewsSystem.OnEndShow.AddListener(ViewConfigID.Craft, OnShowCraftHandler);
                ViewsSystem.OnEndShow.AddListener(ViewConfigID.Settings, OnShowSettingsHandler);
                ViewsSystem.OnEndShow.AddListener(ViewConfigID.Purchases, OnShowPurchasesHandler);
                HotBarModel.OnChangeEquipCell += OnChangeEquipCellHotBarHandler;
                JoystickModel.OnInteract += OnInteractJoystickHandler;
                TouchpadModel.OnInteraction += OnInteractTouchpadHandler;
                PlayerEventHandler.AttackOnce.AddListener(OnPlayerAttackOnceHandler);
                PlayerEventHandler.Jump.AddStartListener(OnPlayerJumpHandler);
                PlayerRunModel.OnChangeIsRunToggle += OnPlayerRunHandler;
            }
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
            if(!AnaliticsModel.EnableAnalitics) return;

            SheltersModel.OnDeath -= OnDeathShelterHandler;
            SheltersModel.OnBuy -= OnBuyShelterHandler;
            SheltersModel.OnUpgrade -= OnUpgradeShelterHandler;
            PurchasesModel.OnPurchaseSuccessfully -= OnPurchaseSuccessfullyHandler;
            CraftModel.OnUnlockedItem -= OnUnlockedItemHandler;
            CraftModel.OnCraftedItem -= OnCraftedItemHandler;
            LoseViewModel.OnPlayAgain -= OnPlayAgainLosePlayerHandler;
            SettingsViewModel.OnResetGame -= OnResetGameHandler;
            EnemiesModel.OnEnemyKillTarget -= OnEnemyKillTargetHandler;
            AnimalsModel.OnAnimalKillTarget -= OnAnimalKillTargetHandler;
            EnemiesModel.OnTargetKillEnemy -= OnTargetillEnemyHandler;
            AnimalsModel.OnTargetKillAnimal -= OnTargetKillAnimalHandler;
            PlayerHungerDamagerModel.OnDamage -= OnHungerPlayerDamageHandler;
            PlayerThirstDamagerModel.OnDamage -= OnThirstPlayerDamageHandler;
            PlacementModel.OnPlaceItem -= OnPlaceItemHandler;
            InventoryModel.ItemsContainer.OnAddItems -= OnAddItemsToInventoryHandler;
            HotBarModel.ItemsContainer.OnAddItems -= OnAddItemsToHotBarHandler;
            GameTimeModel.OnChangeDay -= OnChangeDayHandler;
            StartNightInfoModel.OnEndNight -= OnEndNightHandler;
            SheltersModel.OnUpgrade -= OnUpdateShelterHandler;
            TutorialModel.OnComplete -= OnCompleteHandler;
            TutorialModel.OnStart -= OnStartHandler;
            TutorialModel.OnNextStep -= OnNextStepHandler;
            PurchasersStoreModel.OnSuccess.RemoveListener(ProductID.GoldTier_01, OnPurchaseIapTier01Handler);
            PurchasersStoreModel.OnSuccess.RemoveListener(ProductID.GoldTier_02, OnPurchaseIapTier02Handler);
            PurchasersStoreModel.OnSuccess.RemoveListener(ProductID.GoldTier_03, OnPurchaseIapTier03Handler);
            PurchasersStoreModel.OnSuccess.RemoveListener(ProductID.GoldTier_04, OnPurchaseIapTier04Handler);
            PurchasersStoreModel.OnSuccess.RemoveListener(ProductID.GoldTier_05, OnPurchaseIapTier05Handler);
            PurchasersStoreModel.OnSuccess.RemoveListener(ProductID.GoldTier_06, OnPurchaseIapTier06Handler);
            BluePrintsModel.OnAdd -= OnAddBlueprints;
            CraftModel.OnUnlockedItem -= OnItemUnlockHandler;
            PlayerObjectivesModel.OnPreEndTime -= OnPreEndObjectiveTierTimeHandler;
            PlayerObjectivesModel.OnEndTime -= OnEndObjectiveTierTimeHandler;

            UnsubscribeFirstInteractionEvents();
        }

        private void OnCompleteHandler()
        {
            AnalyticsEvent.TutorialComplete();
        }

        private void OnStartHandler()
        {
            AnalyticsEvent.TutorialStart();
            AnalyticsEvent.TutorialStep(TutorialModel.Step);
        }

        private void OnNextStepHandler()
        {
            AnalyticsEvent.TutorialStep(TutorialModel.Step);
        }

        private void UnsubscribeFirstInteractionEvents()
        {
            PlayerRunModel.OnChangeIsRunToggle -= OnPlayerRunHandler;
            PlayerEventHandler.Jump.RemoveStartListener(OnPlayerJumpHandler);
            PlayerEventHandler.AttackOnce.RemoveListener(OnPlayerAttackOnceHandler);
            TouchpadModel.OnInteraction -= OnInteractTouchpadHandler;
            JoystickModel.OnInteract -= OnInteractJoystickHandler;
            HotBarModel.OnChangeEquipCell -= OnChangeEquipCellHotBarHandler;
            ViewsSystem.OnEndShow.RemoveListener(ViewConfigID.Purchases, OnShowPurchasesHandler);
            ViewsSystem.OnEndShow.RemoveListener(ViewConfigID.Settings, OnShowSettingsHandler);
            ViewsSystem.OnEndShow.RemoveListener(ViewConfigID.Craft, OnShowCraftHandler);
            ViewsSystem.OnEndShow.RemoveListener(ViewConfigID.InventoryPlayer, OnShowPlayerInventoryHandler);
        }

        private void OnPlayerRunHandler()
        {
            if (PlayerRunModel.IsRunToggle)
            {
                AnaliticsModel.SendFirst(AnaliticEventID.FirstInteractionUI, FirstInteractionUIProgressID.Run);
                UnsubscribeFirstInteractionEvents();
            }
        }

        private void OnPlayerJumpHandler()
        {
            AnaliticsModel.SendFirst(AnaliticEventID.FirstInteractionUI, FirstInteractionUIProgressID.Jump);
            UnsubscribeFirstInteractionEvents();
        }

        private void OnPlayerAttackOnceHandler()
        {
            AnaliticsModel.SendFirst(AnaliticEventID.FirstInteractionUI, FirstInteractionUIProgressID.Attack);
            UnsubscribeFirstInteractionEvents();
        }

        private void OnInteractTouchpadHandler()
        {
            AnaliticsModel.SendFirst(AnaliticEventID.FirstInteractionUI, FirstInteractionUIProgressID.Touchpad);
            UnsubscribeFirstInteractionEvents();
        }

        private void OnInteractJoystickHandler()
        {
            AnaliticsModel.SendFirst(AnaliticEventID.FirstInteractionUI, FirstInteractionUIProgressID.Joystick);
            UnsubscribeFirstInteractionEvents();
        }

        private void OnChangeEquipCellHotBarHandler()
        {
            AnaliticsModel.SendFirst(AnaliticEventID.FirstInteractionUI, FirstInteractionUIProgressID.SelectHotbar);
            UnsubscribeFirstInteractionEvents();
        }

        private void OnShowPurchasesHandler()
        {
            AnaliticsModel.SendFirst(AnaliticEventID.FirstInteractionUI, FirstInteractionUIProgressID.OpenStore);
            UnsubscribeFirstInteractionEvents();
        }

        private void OnShowSettingsHandler()
        {
            AnaliticsModel.SendFirst(AnaliticEventID.FirstInteractionUI, FirstInteractionUIProgressID.OpenSettings);
            UnsubscribeFirstInteractionEvents();
        }

        private void OnShowCraftHandler()
        {
            AnaliticsModel.SendFirst(AnaliticEventID.FirstInteractionUI, FirstInteractionUIProgressID.OpenCraft);
            UnsubscribeFirstInteractionEvents();
        }

        private void OnShowPlayerInventoryHandler()
        {
            AnaliticsModel.SendFirst(AnaliticEventID.FirstInteractionUI, FirstInteractionUIProgressID.OpenInventory);
            UnsubscribeFirstInteractionEvents();
        }

        private void OnEndNightHandler()
        {
            AnaliticsModel.Send(AnaliticEventID.FirstProgression, FirstProgressionProgressID.FirstNightAttackCompleted);
        }

        private void OnChangeDayHandler()
        {
            if (SheltersModel.ShelterActive != ShelterModelID.None)
            {
                var lifeTimeDays = GameTimeModel.GetDays(GameTimeModel.Ticks - SheltersModel.ShelterModel.StartAliveTimeTicks);
                if (lifeTimeDays <= AnaliticsModel.CountShelterGlobalDays)
                {
                    AnaliticsModel.Send(AnaliticEventID.ShelterLifeDays, lifeTimeDays);
                }
            }

            var days = GameTimeModel.Days;
            if (days <= AnaliticsModel.CountGlobalDays)
            {
                AnaliticsModel.Send(AnaliticEventID.GlobalDays, days);
            }
        }

        private void OnAddItemsToHotBarHandler(int itemId, int count)
        {
            AddItemsEvent(itemId, count);
        }

        private void OnAddItemsToInventoryHandler(int itemId, int count)
        {
            AddItemsEvent(itemId, count);
        }

        private void AddItemsEvent(int itemId, int count)
        {
            var itemName = ItemsDB.GetItem(itemId).Name;

            if (AnaliticsModel.AddedResourcesNames.Contains(itemName))
            {
                AnaliticsModel.SendWithPostfix(AnaliticEventID.GetResourcesGlobalDays, GameTimeModel.Days, itemName);
            }
        }

        private void OnPlaceItemHandler(int itemId)
        {
            AnaliticsModel.Send(AnaliticEventID.BuildItem, ItemsDB.GetItem(itemId).Name);
        }

        private void OnTargetKillAnimalHandler(Target target, AnimalID animalID)
        {
            if (target && target.ID == TargetID.Player)
            {
                AnaliticsModel.Send(AnaliticEventID.KillAnimal, animalID);
            }
        }

        private void OnTargetillEnemyHandler(Target target, EnemyID enemyID)
        {
            if (target && target.ID == TargetID.Player)
            {
                AnaliticsModel.Send(AnaliticEventID.KillEnemy, enemyID);
            }
        }

        private void OnThirstPlayerDamageHandler()
        {
            if (PlayerHealthModel.IsDead)
            {
                PlayerDeathEvent("Thirst");
            }
        }

        private void OnHungerPlayerDamageHandler()
        {
            if (PlayerHealthModel.IsDead)
            {
                PlayerDeathEvent("Hunger");
            }
        }

        private void OnAnimalKillTargetHandler(AnimalID animalID, Target target)
        {
            if (target.ID == TargetID.Player)
            {
                PlayerDeathEvent(animalID);
            }
        }

        private void OnEnemyKillTargetHandler(EnemyID enemyID, Target target)
        {
            if (target.ID == TargetID.Player)
            {
                PlayerDeathEvent(enemyID);
            }
        }

        private void PlayerDeathEvent(object lastDamage)
        {
            var lifeTimeDays = GameTimeModel.GetDays(GameTimeModel.Ticks - StatisticsModel.StartAliveTimeTicks);
            AnaliticsModel.Send(AnaliticEventID.PlayerDeath, lastDamage, lifeTimeDays);
        }

        private void OnResetGameHandler()
        {
            AnaliticsModel.Send(AnaliticEventID.GameReset);
        }

        private void OnPlayAgainLosePlayerHandler()
        {
            AnaliticsModel.Send(AnaliticEventID.GamePlayAgain);
        }

        private void OnCraftedItemHandler(int itemId)
        {
            AnaliticsModel.Send(AnaliticEventID.ItemsCrafted, ItemsDB.GetItem(itemId).Name);
            AnaliticsModel.SendWithPostfix(AnaliticEventID.ItemsCraftedGlobalDays, GameTimeModel.Days, ItemsDB.GetItem(itemId).Name);
        }

        private void OnUnlockedItemHandler(int itemId)
        {
            var item = ItemsDB.GetItem(itemId);
            AnaliticsModel.Send(AnaliticEventID.ItemUnlocked, item.Name, item.UnlockablesItems[0].Name, item.UnlockablesItems[0].Amount);
        }

        private void OnPurchaseSuccessfullyHandler(PurchaseID purchaseID)
        {
            var coinCost = PurchasesModel.GetInfo<IPurchaseCoinInfo>(purchaseID);
            if (coinCost != null)
            {
                AnaliticsModel.Send(AnaliticEventID.CoinsSpend, purchaseID, coinCost.CoinCost);
                return;
            }

            var coinReward = PurchasesModel.GetInfo<ICoinRewardInfo>(purchaseID);
            if (coinReward != null)
            {
                AnaliticsModel.Send(AnaliticEventID.CoinsSink, purchaseID);
                return;
            }

            var watch = PurchasesModel.GetInfo<IWatchPurchase>(purchaseID);
            if (watch != null)
            {
                AnaliticsModel.Send(AnaliticEventID.WatchRevardedVideo, purchaseID);
                return;
            }
        }

        private void OnUpgradeShelterHandler(ShelterModel shelterModel)
        {
            AnaliticsModel.Send(AnaliticEventID.FirstProgression, FirstProgressionProgressID.FirstShelterUpdate);
        }

        private void OnBuyShelterHandler(ShelterModel shelterModel)
        {
            var ticks = GameTimeModel.Ticks - GameTimeModel.StartTicks;
            AnaliticsModel.Send(AnaliticEventID.BuyShelter, shelterModel.ShelterID, GameTimeModel.GetDays(ticks));
            AnaliticsModel.Send(AnaliticEventID.FirstProgression, FirstProgressionProgressID.FirstShelter);

        }

        private void OnDeathShelterHandler(ShelterModel shelterModel)
        {
            var lifeTimeDays = GameTimeModel.GetDays(GameTimeModel.Ticks - shelterModel.StartAliveTimeTicks);
            AnaliticsModel.Send(AnaliticEventID.DeathShelter, shelterModel.ShelterID, shelterModel.Level, "EnemyID", lifeTimeDays);
        }

        private void OnUpdateShelterHandler(ShelterModel shelterModel)
        {
            var shelterLevel = shelterModel.Level;
            var glodalDays = GameTimeModel.GetDays(GameTimeModel.Ticks - GameTimeModel.StartTicks);
            var shelterLifeTime = GameTimeModel.GetDays(GameTimeModel.Ticks - shelterModel.StartAliveTimeTicks);
            if (shelterLevel == 1)
            {
                AnaliticsModel.Send(AnaliticEventID.UpdateShelterLevel01, shelterLevel, glodalDays, shelterLifeTime);
            }
            if (shelterLevel == 2)
            {
                AnaliticsModel.Send(AnaliticEventID.UpdateShelterLevel02, shelterLevel, glodalDays, shelterLifeTime);
            }

        }

        private void OnPurchaseIAPTier(AnaliticEventID eventID)
        {
            var globalDays = GameTimeModel.GetDays(GameTimeModel.Ticks - GameTimeModel.StartTicks);
            var worldSecs = GameTimeModel.GetSecondsTotal(Convert.ToInt64(globalDays));
            AnaliticsModel.Send(eventID, globalDays, worldSecs);
        }

        private void OnPurchaseIapTier01Handler() => OnPurchaseIAPTier(AnaliticEventID.Get_IAP_Tier01);
        private void OnPurchaseIapTier02Handler() => OnPurchaseIAPTier(AnaliticEventID.Get_IAP_Tier02);
        private void OnPurchaseIapTier03Handler() => OnPurchaseIAPTier(AnaliticEventID.Get_IAP_Tier03);
        private void OnPurchaseIapTier04Handler() => OnPurchaseIAPTier(AnaliticEventID.Get_IAP_Tier04);
        private void OnPurchaseIapTier05Handler() => OnPurchaseIAPTier(AnaliticEventID.Get_IAP_Tier05);
        private void OnPurchaseIapTier06Handler() => OnPurchaseIAPTier(AnaliticEventID.Get_IAP_Tier06);

        private void OnAddBlueprints(int count)
        {
            AnaliticsModel.Send(AnaliticEventID.GetBlueprint, count);
        }

        private void OnItemUnlockHandler(int itemId)
        {
            var globalDays = GameTimeModel.GetDays(GameTimeModel.Ticks - GameTimeModel.StartTicks);
            var worldSecs = GameTimeModel.GetSecondsTotal(Convert.ToInt64(globalDays));
            var item = ItemsDB.GetItem(itemId);
            AnaliticsModel.SendWithPostfix(AnaliticEventID.Unlock_, item.Name, globalDays, worldSecs);
        }

        private void OnEndObjectiveTierTimeHandler()
        {
            string eventName = AnaliticsModel.GetTierEventName(AnaliticEventID.Mission_Started, PlayerObjectivesModel.TierId);
            AnaliticsModel.Send(eventName, AnaliticEventID.Mission_Started);
        }

        private void OnPreEndObjectiveTierTimeHandler()
        {
            if (!PlayerObjectivesModel.IsFirst)
            {
                bool completed = PlayerObjectivesModel.Pool.All(e => !e.IsHasValue);
                if (completed)
                {
                    string eventName = AnaliticsModel.GetTierEventName(AnaliticEventID.Mission_Completed, PlayerObjectivesModel.TierId);
                    var globalDays = GameTimeModel.GetDays(GameTimeModel.Ticks - GameTimeModel.StartTicks);
                    AnaliticsModel.Send(eventName, AnaliticEventID.Mission_Completed, globalDays);
                }
                else
                {
                    string eventName = AnaliticsModel.GetTierEventName(AnaliticEventID.Mission_Failed, PlayerObjectivesModel.TierId);
                    int count = PlayerObjectivesModel.Pool.Count(e => !e.IsHasValue);
                    AnaliticsModel.Send(eventName, AnaliticEventID.Mission_Failed, count);
                }
            }
        }

    }
}
