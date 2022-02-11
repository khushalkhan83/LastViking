using System;
using System.Collections;
using System.Linq;
using Core;
using Extensions;
using Game.Components;
using Game.Models;
using Game.QuestSystem.Map.Extra;
using UnityEngine;

namespace Game.Controllers.TutorialSteps
{
    public class TutorialStep_CookMeat : TutorialStepBase
    {
        #region Dependencies
        [Inject] public CampFireViewModel CampFireViewModel { get; private set; }
        [Inject] public TutorialSimpleDarkViewModel TutorialSimpleDarkViewModel { get; private set; }
        [Inject] public ApplyItemModel ApplyItemModel { get; private set; }
        [Inject] public CoinsModel CoinsModel { get; private set; }
        [Inject] public InventoryModel InventoryModel { get; private set; }
        [Inject] public HotBarModel HotBarModel { get; private set; }

        private const string k_coockedMeatName = "food_meat_chicken_coocked"; // TODO: move to constructor ?
        private const string k_rawMeatName = "food_meat_chicken_raw"; // TODO: move to constructor ?
        private const int k_requiaredRawMeatCount = 1;
        private const int priceForBoost = 10; // TODO: move to constructor ?

        private CampFireModel TutorialCampFire;
        private TokenTarget firePlaceToken;
        private ITutorialStep stepTapHint;
        #endregion

        public TutorialStep_CookMeat(TutorialEvent StepStartedEvent) : base(StepStartedEvent) { }

        #region Base Methods

        public override void OnStart()
        {
            Init();
            Subscribe();
            ProcessState();
        }
        public override void OnEnd()
        {
            UnSubscribe();
            UnDoTutorialModifications();
        }

        #endregion 

        #region Subscriptions

        private void Subscribe()
        {
            CampFireViewModel.OnShowChanged += ProcessState;
            CampFireViewModel.OnIsHasDragChanged += ProcessDragChanged;
            TutorialCampFire.OnCook += ProcessCoockedItemMessage;
            TutorialCampFire.OnChangeFireState += ProcessState; // TODO: handle null camp fire model subscription
            TutorialCampFire.OnStartBoost += ProcessState; // TODO: handle null camp fire model subscription
            ApplyItemModel.OnPreApplyItem += ProcessApllyItem;
        }

        private void UnSubscribe()
        {
            firePlaceToken.enabled = false;
            CampFireViewModel.OnShowChanged -= ProcessState;
            CampFireViewModel.OnIsHasDragChanged -= ProcessDragChanged;
            TutorialCampFire.OnCook -= ProcessCoockedItemMessage;
            TutorialCampFire.OnChangeFireState -= ProcessState;
            TutorialCampFire.OnStartBoost -= ProcessState;
            ApplyItemModel.OnPreApplyItem -= ProcessApllyItem;
        }

        #endregion

        #region Event Processing

        private void ProcessDragChanged() { DoActionAfterFrame(() => { if(!CampFireViewModel.IsHasDrag) { ProcessState();}});}
        private void ProcessCoockedItemMessage(string itemName, int count) {if(itemName == k_coockedMeatName) ProcessState();}
        private void ProcessApllyItem(ItemsContainer container, CellModel cellModel) { if( cellModel.IsHasItem && cellModel.Item.Name == k_coockedMeatName) CheckConditions();}

        #endregion

        #region Initialization and Deinitialization
        private void Init()
        {
            var sceneContext = GameObject.FindObjectOfType<SimpleTutorialController>();
            firePlaceToken = sceneContext.firePlace.GetComponent<TokenTarget>();
            TutorialCampFire = sceneContext.firePlace.GetComponentInChildren<CampFireModel>();

            firePlaceToken.enabled = true;

            stepTapHint = new TutorialStep_TapHint(null,ShowTapCondition, null);
            InjectionSystem.Inject(stepTapHint);
        }


        private void UnDoTutorialModifications()
        {
            TutorialCampFire.BlockCoockingOnFire = false;

            ShowDarkScreen(false);
            RemoveTutorialHilights();
            ShowTaskMessage(false);

            stepTapHint.Exit();
        }


        private void RemoveTutorialHilights()
        {
            CampFireViewModel.GetApplyButton().SafeDeactivateComponent<TutorialHilightAndAnimation>();
            CampFireViewModel.GetSwitchFireModeButton().SafeDeactivateComponent<TutorialHilightAndAnimation>();
            CampFireViewModel.GetBoostsButton().SafeDeactivateComponent<TutorialHilightAndAnimation>();
            CampFireViewModel.RemoveAllTutorialCellsHilight();
            CampFireViewModel.StopTutorialDragAnimation();
        }

        #endregion

        #region State Processing

        private void ProcessState()
        {
            ShowDarkScreen(false);
            RemoveTutorialHilights();
            TutorialCampFire.BlockCoockingOnFire = true;

            bool coockWindowIsOpened = CampFireViewModel.IsShow;
            if(!coockWindowIsOpened)
            {
                ShowTaskMessage(true,"Use campfire");
                OpenCoockingWindowState();
                return;
            }

            bool meatIsReadyToEat = HasConsumableMeatInCoockingSection();
            if(meatIsReadyToEat)
            {
                ShowTaskMessage(true,"Eat meat");
                ShowDarkScreen(true);
                ConsumeCoockedMeatState();
                return;
            }

            bool meatIsInCoockingSection = HasRawMeatInCoockingSection();
            if(!meatIsInCoockingSection)
            {
                ShowTaskMessage(true,"Put meat in campfire");
                ShowDarkScreen(true);
                EnsureEnoughItem(k_rawMeatName, k_requiaredRawMeatCount);
                DragFoodToCoockingSectionState();
                return;
            }

            bool fireIsOn = TutorialCampFire.IsFire;
            if(!fireIsOn)
            {
                ShowTaskMessage(true,"Turn on the fire");
                ShowDarkScreen(true);
                TurnOnFireState();
                return;
            }

            bool boostIsActive = TutorialCampFire.IsBoost;
            if(!boostIsActive)
            {
                ShowTaskMessage(true,"Speed up the process");
                ShowDarkScreen(true);
                TurnCoockingBoostState();
                return;
            }
            else
            {
                ShowTaskMessage(true,"Wait");
                ShowDarkScreen(true);
                TutorialCampFire.BlockCoockingOnFire = false;
            }
        }

        #endregion

        #region State Conditions
        private bool HasConsumableMeatInCoockingSection() => HasItemInCoockingSection(k_coockedMeatName);
        private bool HasRawMeatInCoockingSection() => HasItemInCoockingSection(k_rawMeatName);
        private bool HasItemInCoockingSection(string itemName)
        {
            var target = TutorialCampFire.ItemsContainer.Cells.ToList().Find(x => x.IsHasItem && x.Item.Name == itemName);
            return target != null;
        }
        #endregion

        #region States

        private void OpenCoockingWindowState()
        {
            stepTapHint.Enter();
        }

        private void ConsumeCoockedMeatState()
        {
            var cookedMeatCell = TutorialCampFire.ItemsContainer.Cells.ToList().Find(x => x.IsHasItem && x.Item.Name == k_coockedMeatName);
            CampFireViewModel.SelectCell(ContainerID.CampFire, cookedMeatCell.Id);
            CampFireViewModel.HighlightCell(ContainerID.CampFire, cookedMeatCell.Id);
            CampFireViewModel.GetApplyButton().SafeActivateComponent<TutorialHilightAndAnimation>();
        }

        private void DragFoodToCoockingSectionState()
        {
            stepTapHint.Exit();
            TutorialSimpleDarkViewModel.PlayAnimation();

            var rawMeatCellData = GetRawMeatCell();

            ContainerID containerID_2 = ContainerID.CampFire;
            int cellID_2 = GetEmptyCampCellID();
            
            CampFireViewModel.TutorialHilightCell(rawMeatCellData.containerID,rawMeatCellData.model.Id);
            CampFireViewModel.TutorialHilightCell(containerID_2,cellID_2);

            CampFireViewModel.SelectCell(rawMeatCellData.containerID, rawMeatCellData.model.Id);
            CampFireViewModel.HighlightCell(rawMeatCellData.containerID, rawMeatCellData.model.Id);

            CampFireViewModel.DoTutorialDragAnimation(rawMeatCellData.containerID,rawMeatCellData.model.Id, containerID_2,cellID_2);
        }

        private void TurnOnFireState()
        {
            CampFireViewModel.GetSwitchFireModeButton().SafeActivateComponent<TutorialHilightAndAnimation>();
        }

        private void TurnCoockingBoostState()
        {
            AddNotEnoughCoinsForBoost();
            CampFireViewModel.GetBoostsButton().SafeActivateComponent<TutorialHilightAndAnimation>();
        }

        #endregion

        #region Final conditions

        private void CheckConditions()
        {
            bool nextStep = true;

            if (nextStep) TutorialNextStep();
        }

        #endregion

        #region Help Functions

        private void ShowDarkScreen(bool show) => TutorialSimpleDarkViewModel.SetShow(show);
        private void AddNotEnoughCoinsForBoost()
        {
            if(CoinsModel.Coins > priceForBoost) return;

            var dif = priceForBoost - CoinsModel.Coins;

            CoinsModel.Adjust(dif);
        }

        private (CellModel model,ContainerID containerID) GetRawMeatCell()
        {
            ContainerID containerID = ContainerID.HotBar;
            var meat = GetTargetCellFromContainer(HotBarModel.ItemsContainer,k_rawMeatName);
            if(meat == null)
            {
                containerID = ContainerID.Inventory;
                meat = GetTargetCellFromContainer(InventoryModel.ItemsContainer,k_rawMeatName);
            }

            if(meat == null)
            {
                //TODO: ensure that item is added
            }

            return (meat,containerID);
            
            CellModel GetTargetCellFromContainer(ItemsContainer container, string itemName)
            {
                return container.Cells.Where(x => x.IsHasItem && x.Item.Name == itemName).FirstOrDefault();
            }
        }

        private int GetEmptyCampCellID()
        {
            var cell = TutorialCampFire.ItemsContainer.Cells.Where(x => x.IsEmpty).FirstOrDefault();

            if(cell == null)
            {
                //TODO: add empty cell
                return 2;
            }

            return cell.Id;
        }

        private void DoActionAfterFrame(Action action) => CampFireViewModel.StartCoroutine(CDoActionAfterFrame(action));

        // TODO: Code duplicate here and in other classes. Move to CoroutineModel
        private IEnumerator CDoActionAfterFrame( Action action)
        {
            yield return null;
            action?.Invoke();
        }

        private bool ShowTapCondition(GameObject raycastGameObject)
        {
            return raycastGameObject != null && IsCampFire();

            bool IsCampFire()
            {
                return raycastGameObject.TryGetComponent<CampFireModel>(out var campFire);
            }
        }

        #endregion
    }
}