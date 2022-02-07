using Core;
using Game.Models;
using Game.Purchases;
using Game.QuestSystem.Map.Extra;
using Game.VillageBuilding;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers.TutorialSteps
{
    public abstract class TutorialStep_Building : TutorialStepBase
    {
        [Inject] public HouseBuildingViewModel HouseBuildingViewModel  { get; private set; }
        [Inject] public TutorialSimpleDarkViewModel TutorialSimpleDarkViewModel  { get; private set; }
        [Inject] public CoinsModel CoinsModel  { get; private set; }
        [Inject] public PurchasesModel PurchasesModel  { get; private set; }
        [Inject] public VillageBuildingModel VillageBuildingModel { get; private set; }

        protected HouseBuilding houseBuilding;
        protected Sprite icon;
        protected string message;
        protected int skipGoldPrice;
        private ITutorialStep stepTapHint;

        private HouseBuildingType GetHouseType() => houseBuilding.Type;

        private bool TargetBuildingIsSelected => HouseBuildingViewModel.Target == houseBuilding;


        public TutorialStep_Building(TutorialEvent StepStartedEvent) : base(StepStartedEvent) { }

        public override void OnStart()
        {
            skipGoldPrice = PurchasesModel.GetInfo<IPurchaseCoinInfo>(PurchaseID.SkipBuildingProcessGold).CoinCost;
            houseBuilding.OnCompleteBuildingProcess += OnCompleteBuildingProcess;
            houseBuilding.GetComponent<TokenTarget>().enabled = true;
            VillageBuildingModel.BuildingsAvailableInTutorial.Add(houseBuilding.Type);
            HouseBuildingViewModel.OnShowChanged += ProcessState;
            HouseBuildingViewModel.OnUpgradeClicked += OnUpgradeStarted;

            InitState();
            ProcessState();
        }

        public override void OnEnd()
        {
            houseBuilding.OnCompleteBuildingProcess -= OnCompleteBuildingProcess;
            HouseBuildingViewModel.OnUpgradeClicked -= OnUpgradeStarted;
            VillageBuildingModel.BuildingsAvailableInTutorial.Remove(houseBuilding.Type);
            houseBuilding.GetComponent<TokenTarget>().enabled = false;
            HouseBuildingViewModel.OnShowChanged -= ProcessState;

            UpdateHilight(forceDisableHilight:true);
            stepTapHint.Exit();
        }

        private void InitState()
        {
            EnsureEnoughItemsForBuildingUpgrade();
            ShowTaskMessage(true, message, icon);
            stepTapHint = new TutorialStep_TapHint(null,ShowTapCondition, null);
            InjectionSystem.Inject(stepTapHint);
            stepTapHint.Enter();
        }

        private void ProcessState()
        {
            UpdateHilight();
            CheckConditions();
        }


        private void UpdateHilight(bool forceDisableHilight = false)
        {
            bool hilight = forceDisableHilight? false : TargetBuildingIsSelected && HouseBuildingViewModel.IsShow;
            TutorialSimpleDarkViewModel.SetShow(hilight);

            if(hilight)
            {
                TutorialSimpleDarkViewModel.PlayAnimation();

                bool buildProcessStarted = houseBuilding.IsBuildingProcess;

                HouseBuildingViewModel.SetSkipButtonHilight(buildProcessStarted);
                HouseBuildingViewModel.SetBuildButtonHilight(!buildProcessStarted);
                stepTapHint.Exit();
            }
            else
            {
                HouseBuildingViewModel.RemoveAllHilight();
            }
        }

        private void OnCompleteBuildingProcess(HouseBuilding obj) => ProcessState();
        

        private void OnUpgradeStarted()
        {
            if(TargetBuildingIsSelected)
            {
                HouseBuildingViewModel.OnUpgradeClicked -= OnUpgradeStarted;
                AddNotEnoughCoinsForSkip();
            }

            ProcessState();
        }


        private void AddNotEnoughCoinsForSkip()
        {
            if(CoinsModel.Coins > skipGoldPrice) return;

            var dif = skipGoldPrice - CoinsModel.Coins;

            CoinsModel.Adjust(dif);
        }

        private void CheckConditions()
        {
            bool nextStep = houseBuilding.Level > 0;

            if(nextStep) TutorialNextStep();
        }

        private void EnsureEnoughItemsForBuildingUpgrade()
        {
            var houseData = VillageBuildingModel.GetHouseBuildingInfo(GetHouseType());
            int nextLevel = houseBuilding.Level + 1;
            var levelData = houseData.GetLevelInfo(nextLevel);

            if(levelData != null)
            {
                foreach (var item in levelData.requiredItems)
                {
                    EnsureEnoughItem(item.Name,item.Amount);
                }
            }
        }

        private bool ShowTapCondition(GameObject raycastGameObject)
        {
            return raycastGameObject != null && raycastGameObject.GetComponentInParent<HouseBuilding>() == houseBuilding;
        }
    }
}