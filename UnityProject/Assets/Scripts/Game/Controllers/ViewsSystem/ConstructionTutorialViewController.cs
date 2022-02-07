using Game.Views;
using Core.Controllers;
using Game.Models;
using Core;
using UnityEngine;

namespace Game.Controllers
{
    public class ConstructionTutorialViewController : ViewControllerBase<ConstructionTutorialView>
    {
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public ConstructionTutorialModel ConstructionTutorialModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public LocalizationModel LocalizationModel { get; private set; }
        [Inject] public BuildingModeModel BuildingModeModel { get; private set; }
        [Inject] public BuildingHotBarModel BuildingHotBarModel { get; private set; }
        [Inject] public TutorialModel TutorialModel { get; private set; }
        [Inject] public PlayerMovementModel PlayerMovementModel {get;private set;}

        private readonly float clickDelay = 0.5f;

        protected override void Show()
        {
            BuildingModeModel.BuildingActive = true;
            View.transform.SetAsLastSibling();
            DisableClick();
            LocalizationModel.OnChangeLanguage += SetLocalization;

            SetLocalization();
            ShowStartStep();
            PlayerMovementModel.SetBlockPlayerMovement(true);
        }

        protected override void Hide()
        {
            View.OnClick -= OnClick;
            View.OnHold -= OnHold;
            GameUpdateModel.OnUpdate -= OnUpdate;
            LocalizationModel.OnChangeLanguage -= SetLocalization;
            PlayerMovementModel.SetBlockPlayerMovement(false);
        }

        private float _clickTimer = 0;
        private void OnUpdate()
        {
            _clickTimer += Time.deltaTime;
            if (_clickTimer >= clickDelay)
            {
                _clickTimer = 0;
                EnableClick();
            }
        }

        private void OnClick() => NextStep();

        private void OnHold() => NextStep();

        private void ShowStartStep()
        {
            if(TutorialModel.Step == TutorialModel.BuildFoundationStep)
            {
                View.SetActiveFoundationStep();
            }
            else if(TutorialModel.Step == TutorialModel.BuildWallsStep)
            {
                View.SetActiveWallsStep();
            }
            else if(TutorialModel.Step == TutorialModel.BuildRoofStep)
            {
                View.SetActiveRoofStep();
            }
            else if(TutorialModel.Step == TutorialModel.BuildDoorStep)
            {
                ConstructionTutorialModel.ControlStep = 1;
                View.SetActiveDoorStep(ConstructionTutorialModel.ControlStep - 1);
            }
        }

        private void NextStep()
        {
            if(TutorialModel.Step == TutorialModel.BuildFoundationStep)
            {
                BuildingHotBarModel.SelectedCell = ConstructionTutorialModel.SellectedCellStepFoundation;
                RefreshBuildingHotbar();
                ViewsSystem.Hide(View);
            }
            else if(TutorialModel.Step == TutorialModel.BuildWallsStep)
            {
                BuildingHotBarModel.SelectedCell = ConstructionTutorialModel.SellectedCellStepWalls;
                RefreshBuildingHotbar();
                ViewsSystem.Hide(View);
            }
            else if(TutorialModel.Step == TutorialModel.BuildRoofStep)
            {
                BuildingHotBarModel.SelectedCell = ConstructionTutorialModel.SellectedCellStepRoof;
                RefreshBuildingHotbar();
                ViewsSystem.Hide(View);
            }
            else if(TutorialModel.Step == TutorialModel.BuildDoorStep)
            {
                 if (ConstructionTutorialModel.CanShowSteps)
                {
                    View.SetActiveDoorStep(ConstructionTutorialModel.ControlStep);
                }

                if(ConstructionTutorialModel.ControlStep == 1)
                {
                    BuildingHotBarModel.SelectedCell = ConstructionTutorialModel.SellectedCellStepDoor;
                    RefreshBuildingHotbar();
                }
                else if(ConstructionTutorialModel.ControlStep == 2)
                {
                    BuildingHotBarModel.SetSelectedOptionCell(ConstructionTutorialModel.SellectedCellStepDoor, ConstructionTutorialModel.SellectedOptionCellStepDoor);
                    RefreshBuildingHotbar();
                }

                ConstructionTutorialModel.NextStep();
                
                DisableClick();

                if (ConstructionTutorialModel.IsControlStepsOver)
                {
                    ViewsSystem.Hide(View);
                }
            }
        }

        private void RefreshBuildingHotbar()
        {
            BuildingModeModel.BuildingActive = false;
            BuildingModeModel.BuildingActive = true;
        }

        private void EnableClick()
        {
            View.OnClick += OnClick;
            View.OnHold += OnHold;
            GameUpdateModel.OnUpdate -= OnUpdate;
        }

        private void DisableClick()
        {
            View.OnClick -= OnClick;
            View.OnHold -= OnHold;
            GameUpdateModel.OnUpdate += OnUpdate;
        }

        private void SetLocalization()
        {
            View.SetTextFoundation(LocalizationModel.GetString(LocalizationKeyID.Tutorial_UI_ConstructioSelectFoundation));
            View.SetTextWalls(LocalizationModel.GetString(LocalizationKeyID.Tutorial_UI_ConstructioSelectWall));
            View.SetTextRoof(LocalizationModel.GetString(LocalizationKeyID.Tutorial_UI_ConstructioSelectRoof));
            View.SetTextDoor1(LocalizationModel.GetString(LocalizationKeyID.Tutorial_UI_ConstructionHoldCell));
            View.SetTextDoor2(LocalizationModel.GetString(LocalizationKeyID.Tutorial_UI_ConstructionSelectDoor));
        }

    }
}
