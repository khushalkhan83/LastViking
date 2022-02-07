using System;
using System.Collections.Generic;
using Core;
using Extensions;
using Game.Audio;
using Game.Models;
using Game.Purchases;
using Game.Purchases.Purchasers;
using Game.Views;
using Game.VillageBuilding;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers
{
    public class HouseBuildingViewControllerData : IDataViewController
    {
        public delegate void FetchViewDataCommand();
        public struct BuildingUpgradeTimerRequest
        {
            public bool success;
            public string result;
        }
        public delegate BuildingUpgradeTimerRequest GetBuildingUpgradeTimerRequest();
        
        public readonly FetchViewDataCommand fetchViewDataCommand;
        public readonly GetBuildingUpgradeTimerRequest buildingUpgradeTimerRequets;

        public HouseViewData ViewData {get; private set;}

        public readonly UserInput userInput;

        public HouseBuildingViewControllerData(UserInput userInput, FetchViewDataCommand fetchViewDataCommand, GetBuildingUpgradeTimerRequest buildingUpgradeTimerRequets)
        {
            this.userInput = userInput;
            this.fetchViewDataCommand = fetchViewDataCommand;
            this.buildingUpgradeTimerRequets = buildingUpgradeTimerRequets;
        }

        public void SetDada(HouseViewData viewData)
        {
            ViewData = viewData;
            OnViewDataChanged?.Invoke();
        }

        public event Action OnViewDataChanged;
    }

    public class HouseViewData
    {
        public Sprite icon;
        public string headerText;
        public string descriptionText;
        public int level;
        public bool showSkipButton;
        public bool showTimer;
        public bool showBuildButton;
        public bool showHealthBar;
        public string citizensCountText;
        public bool showRestoreButton;
        public bool showCitizensResources;
        public bool showRequiredBuildings;
        public float health;
        public float healthMax;
        public string coinsCostPriceText;

        public RequiaredItemsViewData[] requiaredItemsViewDatas;
        public CitizensViewData citizensViewData;
        public RequiredBuildingsViewData[] requiredBuildingsViewDatas;

        public HouseViewData(Sprite icon, string headerText, string descriptionText, int level, bool showSkipButton, bool showTimer, bool showBuildButton, bool showHealthBar, string citizensCountText, RequiaredItemsViewData[] requiaredItemsViewDatas, bool showRestoreButton, CitizensViewData citizensViewData, bool showCitizensResources, bool showRequiredBuildings, RequiredBuildingsViewData[] requiredBuildingsViewDatas, float health, float healthMax, string coinsCostPriceText)
        {
            this.icon = icon;
            this.headerText = headerText;
            this.descriptionText = descriptionText;
            this.level = level;
            this.showSkipButton = showSkipButton;
            this.showTimer = showTimer;
            this.showBuildButton = showBuildButton;
            this.showHealthBar = showHealthBar;
            this.citizensCountText = citizensCountText;
            this.requiaredItemsViewDatas = requiaredItemsViewDatas;
            this.showRestoreButton = showRestoreButton;
            this.citizensViewData = citizensViewData;
            this.showCitizensResources = showCitizensResources;
            this.showRequiredBuildings = showRequiredBuildings;
            this.requiredBuildingsViewDatas = requiredBuildingsViewDatas;
            this.health = health;
            this.healthMax = healthMax;
            this.coinsCostPriceText = coinsCostPriceText;
        }
    }

    public class RequiaredItemsViewData
    {
        public int count;
        public ResourceCellData cellData;
    }

    public class CitizensViewData
    {
        public ResourceCellData resourceCellData;
    }

    public class RequiredBuildingsViewData
    {
        public HouseBuildingCellData cellData;
    }

    public class UserInput
    {
        public delegate bool CloseViewRequest();
        public delegate bool BuildRequest();
        public delegate bool SkipBuildRequest();
        public delegate bool RestoreRequest();

        public readonly CloseViewRequest closeViewRequest;
        public readonly BuildRequest buildRequest;
        public readonly SkipBuildRequest skipBuildRequest;
        public readonly RestoreRequest restoreRequest;

        public UserInput(CloseViewRequest closeViewRequest, BuildRequest buildRequest, SkipBuildRequest skipBuildRequest, RestoreRequest restoreRequest)
        {
            this.closeViewRequest = closeViewRequest;
            this.buildRequest = buildRequest;
            this.skipBuildRequest = skipBuildRequest;
            this.restoreRequest = restoreRequest;
        }
    }


    // TODO: add localization
    public class ActivateHouseBuildingViewController : ViewEnableController<HouseBuildingView>, IActivateHouseBuildingViewController
    {
        [Inject] public HouseBuildingViewModel ViewModel  { get; private set; }
        [Inject] public VillageBuildingModel VillageBuildingModel { get; private set; }
        [Inject] public VillageAttackModel VillageAttackModel { get; private set; }
        [Inject] public InventoryModel InventoryModel { get; private set; }
        [Inject] public HotBarModel HotBarModel { get; private set; }
        [Inject] public ItemsDB ItemsDB { get; private set; }
        [Inject] public AudioSystem AudioSystem { get; private set; }
        [Inject] public InventoryOperationsModel InventoryOperationsModel { get; private set; }
        [Inject] public PurchasesModel PurchasesModel { get; private set; }

        public override ViewConfigID ViewConfigID => ViewConfigID.HouseBuildingConfig;

        public override bool IsCanShow => ViewModel.IsShow && GetHouse() != null;

        private HouseBuildingViewControllerData _data;
        public override IDataViewController Data => _data;

        private HouseBuilding GetHouse() => ViewModel.Target;

        public override void Enable()
        {
            InitDadta();

            ViewModel.OnShowChanged += UpdateViewVisible;
            ViewModel.OnTargetChanged += OnTargetChanged;
            InventoryModel.ItemsContainer.OnChangeCell += OnChangeCellHandler;
            HotBarModel.ItemsContainer.OnChangeCell += OnChangeCellHandler;
            VillageBuildingModel.OnHouseStateChanged += OnHouseStateChanged;
            SubscribeOnTargetEvents();

            UpdateViewVisible();
        }

        public override void Disable()
        {
            ViewModel.OnShowChanged -= UpdateViewVisible;

            ViewModel.OnTargetChanged -= OnTargetChanged;
            InventoryModel.ItemsContainer.OnChangeCell -= OnChangeCellHandler;
            HotBarModel.ItemsContainer.OnChangeCell -= OnChangeCellHandler;
            VillageBuildingModel.OnHouseStateChanged -= OnHouseStateChanged;

            TryUnsubscribeFromOldTarget();
        }

        private HouseBuilding oldTarget;
        private void OnTargetChanged()
        {
            TryUnsubscribeFromOldTarget();
            SubscribeOnTargetEvents();

            SetViewData();

            UpdateViewVisible();
        }

        private void TryUnsubscribeFromOldTarget()
        {
            if(oldTarget != null)
            {
                oldTarget.OnStateChanged -= OnHouseStateChanged;
                oldTarget.OnCompleteBuildingProcess -= OnCompleteBuildingProcess;
            }
        }

        private void SubscribeOnTargetEvents()
        {
            var house = GetHouse();
            if(house != null) 
            {
                house.OnStateChanged += OnHouseStateChanged;
                house.OnCompleteBuildingProcess += OnCompleteBuildingProcess;
            }

            oldTarget = house;
        }

        private void OnChangeCellHandler(CellModel cell)
        { 
            if(ViewModel.IsShow)
            {
                SetViewData();
            }
        }

        private void OnHouseStateChanged()
        {
            if(ViewModel.IsShow)
            {
                SetViewData();
            }
        }

        private void OnCompleteBuildingProcess(HouseBuilding building)
        {
            Hide();
        }

        private void InitDadta()
        {
            var userInput = new UserInput(CloseViewRequest,BuildRequest,SkipBuildRequest,RestoreRequest);
            _data = new HouseBuildingViewControllerData(userInput,FetchViewDataCommand, BuildingUpgradeTimerRequets);
        }

        public void SetViewData()
        {
            _data.SetDada(GetViewData());
        }

        private HouseViewData GetViewData()
        {
            var house = GetHouse();
            var houseBuildingInfo = VillageBuildingModel.GetHouseBuildingInfo(house.Type);

            var destroyableBuilding = house.GetCurrentLevelDestroyableBuilding();

            bool isDestroyable = destroyableBuilding != null;

            float health = 0;
            float healthMax = 0;
            bool needRepair = false;

            if(isDestroyable)
            {
                health = (float)Mathf.Clamp(destroyableBuilding.BuildingHealthModel.Health, 0, destroyableBuilding.BuildingHealthModel.HealthMax);
                healthMax = (float)destroyableBuilding.BuildingHealthModel.HealthMax;
                needRepair = destroyableBuilding.BuildingHealthModel.Health != destroyableBuilding.BuildingHealthModel.HealthMax;
            }

            var icon = houseBuildingInfo.icon;
            var headerText = house.Type.ToString();
            var description = houseBuildingInfo.descriptionText;
            var level = house.Level;
            var citizensCount = $"{VillageBuildingModel.FreeCitizensCount}/{VillageBuildingModel.AllCitizensCount}";
            bool canUpgrade = VillageBuildingModel.CanUpgradeBuilding(house);
            bool isTownHall = house.Type == HouseBuildingType.TownHall;

            bool showRestoreButton = needRepair;
            bool showCitizensResources = !needRepair && TryGetNextLevelRequiaredCitizens(out int requiredCitizens) && requiredCitizens > 0;
            bool showRequiredBuildings = !needRepair;

            bool isBuildProcess = house.IsBuildingProcess;
            bool townHallCanBeAttacked = VillageAttackModel.HasEnemiesAttackConfigForLevel(house.Level);

            string coinsCostPriceText = PurchasesModel.GetInfo<SkipBuildingProcessGold>(PurchaseID.SkipBuildingProcessGold).CoinCost.ToString(); // TODO: handle errors here

            HouseViewData viewData = new HouseViewData(icon,
                headerText,
                description,
                level,
                CanSkipBuildingProcess(isTownHall,isBuildProcess,townHallCanBeAttacked),
                ShowSkipTimer(isTownHall,townHallCanBeAttacked),
                canUpgrade,
                needRepair,
                citizensCount,
                GetNeededItemsViews(needRepair),
                showRestoreButton,
                GetCitizensViewData(),
                showCitizensResources,
                showRequiredBuildings,
                GetRequiredBuildingsViewData(needRepair),
                health,
                healthMax,
                coinsCostPriceText);

            return viewData;
        }

        private RequiredBuildingsViewData[] GetRequiredBuildingsViewData(bool needRepair)
        {
            var answer = new List<RequiredBuildingsViewData>();
            var house = GetHouse();
            var houseBuildingInfo = VillageBuildingModel.GetHouseBuildingInfo(house.Type);
            var targetLevel = needRepair ? house.Level : house.Level + 1;

            var levelInfo = houseBuildingInfo.GetLevelInfo(targetLevel);

            if (levelInfo != null)
            {
                for (int i = 0; i < levelInfo.requiredBuildings.Length; i++)
                {
                    RequiredBuildingsViewData viewData = new RequiredBuildingsViewData();

                    RequiredBuilding building = levelInfo.requiredBuildings[i];
                    int currentBuildingsCount = VillageBuildingModel.GetBuildingsCount(building.type, building.level);

                    HouseBuildingCellData cellData = GetHouseBuildingCellData(building, currentBuildingsCount);
                    viewData.cellData = cellData;

                    answer.Add(viewData);
                }
            }

            return answer.ToArray();

            HouseBuildingCellData GetHouseBuildingCellData(RequiredBuilding requiredBuilding, int countCurrent)
            {
                var requiredBuildingInfo = VillageBuildingModel.GetHouseBuildingInfo(requiredBuilding.type);
                return new HouseBuildingCellData
                {
                    Icon = requiredBuildingInfo.icon,
                    levelText = "lvl " + requiredBuilding.level,
                    countText = countCurrent + "/" + requiredBuilding.count,
                    IsActive = countCurrent >= requiredBuilding.count
                };
            }
            
        }

        private bool TryGetNextLevelRequiaredCitizens(out int requiredCitizens)
        {
            requiredCitizens = 0;
            var house = GetHouse();
            var houseBuildingInfo = VillageBuildingModel.GetHouseBuildingInfo(house.Type);
            if(houseBuildingInfo == null) return false;

            var houseNextLevelInfo = houseBuildingInfo.GetLevelInfo(house.Level + 1);
            if(houseNextLevelInfo == null) return false;
            
            requiredCitizens = houseNextLevelInfo.requiredCitizens;

            return true;
        }

        private CitizensViewData GetCitizensViewData()
        {
            var answer = new CitizensViewData();
            TryGetNextLevelRequiaredCitizens(out int requiredCitizens);
            
            var cellData = GetCitizensCellData(VillageBuildingModel.FreeCitizensCount, requiredCitizens);
            answer.resourceCellData = cellData;

            return answer;

            ResourceCellData GetCitizensCellData(int countCurrent, int countNeeded)
            {
                return new ResourceCellData
                {
                    Icon = ViewModel.CitizensIconSprite,
                    Message = countCurrent + "/" + countNeeded,
                    IsActive = countCurrent >= countNeeded,
                };
            }
        }

        private bool ShowSkipTimer(bool isTownhall, bool townHallCanBeAttacked)
        {
            var house = GetHouse();
            bool cantSkip = isTownhall && house.IsBuildingProcess && townHallCanBeAttacked;

            return house.IsBuildingProcess && !cantSkip;
        }

        private RequiaredItemsViewData[] GetNeededItemsViews(bool needRepair)
        {
            List<RequiaredItemsViewData> answer = new List<RequiaredItemsViewData>();

            var house = GetHouse();
            var houseBuildingInfo = VillageBuildingModel.GetHouseBuildingInfo(house.Type);
            var houseLevel = needRepair ? house.Level : house.Level + 1;
            var houseInfo = houseBuildingInfo.GetLevelInfo(houseLevel);

            RequiredItem[] requiredItems = needRepair ? house.GetCurrentLevelDestroyableBuilding().GetRestoreItems().ToArray() : houseInfo?.requiredItems;
            
            if(requiredItems == null) return null;

            foreach (var requiredItem in requiredItems)
            {
                var itemData = ItemsDB.GetItem(requiredItem.Name);


                RequiaredItemsViewData viewData = new RequiaredItemsViewData();
                var count = InventoryModel.ItemsContainer.GetItemsCount(itemData.Id) + HotBarModel.ItemsContainer.GetItemsCount(itemData.Id);


                viewData.count = count;
                viewData.cellData = new ResourceCellData // FIXME: handle errors here in case something heppen
                {
                    Icon = itemData.Icon,
                    Message = count + "/" + requiredItem.Amount,
                    IsActive = count >= requiredItem.Amount,
                    ItemRarity = itemData.ItemRarity,
                    IsComponent = itemData.Category == "Components",
                };

                answer.Add(viewData);
            }

            return answer.ToArray();
        }

        private bool CanSkipBuildingProcess(bool isTownHall, bool isBuildProcess, bool townHallCanBeAttacked)
        {
            var house = GetHouse();
            bool isAttackModel = isTownHall && townHallCanBeAttacked;

            return isBuildProcess && !isAttackModel;
        }

        #region Requests/Commands

        private void FetchViewDataCommand()
        {
            throw new NotImplementedException();
        }

        private HouseBuildingViewControllerData.BuildingUpgradeTimerRequest BuildingUpgradeTimerRequets()
        {
            var answer = new HouseBuildingViewControllerData.BuildingUpgradeTimerRequest();
            var house = GetHouse();
            answer.success = house != null && house.IsBuildingProcess;

            if(answer.success) answer.result = house.BuildingTimeRemaining.ToString(@"hh\:mm\:ss");

            return answer;
        }

        private bool RestoreRequest()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            var destroyableBuilding = GetHouse().GetCurrentLevelDestroyableBuilding();

            RequiredItem[] repairItems = destroyableBuilding.GetRestoreItems().ToArray();
            if(InventoryOperationsModel.PlayerHasItems(repairItems))
            {
                InventoryOperationsModel.RemoveItemsFromPlayer(repairItems);
                destroyableBuilding.RestoreBuilding();
                Hide();
                return true;
            }

            return false;
        }
            
        private bool SkipBuildRequest()
        {
            var house = GetHouse();
            AudioSystem.PlayOnce(AudioID.Button);
            if(house.IsBuildingProcess)
            {
                PurchasesModel.Purchase(PurchaseID.SkipBuildingProcessGold, (result) => 
                    {
                        if (result == PurchaseResult.Successfully)
                        {
                            house.CompleteBuildingProcess();
                        }
                        else
                        {
                            ViewsSystem.Show<PurchasesView>(ViewConfigID.Purchases); // TODO: do like this ?
                        }
                    });

                return true;
            }
            return false;
        }

        private bool BuildRequest()
        {
            var house = GetHouse();
            AudioSystem.PlayOnce(AudioID.Button);
            if(VillageBuildingModel.CanUpgradeBuilding(house))
            {
                var houseBuildingInfo = VillageBuildingModel.GetHouseBuildingInfo(house.Type);
                var houseNextLevelInfo = houseBuildingInfo.GetLevelInfo(house.Level + 1);
                InventoryOperationsModel.RemoveItemsFromPlayer(houseNextLevelInfo.requiredItems);
                if(house.Type == HouseBuildingType.TownHall)
                {
                    VillageAttackModel.StartTownHallUpgrade();
                    bool upgradeWillTriggerAttack = VillageAttackModel.HasEnemiesAttackConfigForLevel(house.Level);

                    if(upgradeWillTriggerAttack)
                        CloseViewRequest();
                    else
                        SetViewData();
                }
                else
                {
                    house.StartBuildingProcess();
                    SetViewData();
                }

                return true;
            }
            
            return false;
        }

        private bool CloseViewRequest()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            Hide();

            return true;
        }

        #endregion
    }
}
