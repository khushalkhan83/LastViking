using Game.Views;
using Core.Controllers;
using Core;
using Game.Models;
using UnityEngine;

namespace Game.Controllers
{
    public class TutorialObjectivesDarkScreenViewController : ViewControllerBase<TutorialObjectivesDarkScreenView>
    {
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public TutorialObjectivesDarkScreenModel TutorialObjectivesDarkScreenModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public LocalizationModel LocalizationModel { get; private set; }
        [Inject] public ObjectivesViewModel ObjectivesViewModel { get; private set; }
        [Inject] public PlayerMovementModel PlayerMovementModel {get;private set;}


        private readonly float clickDelay = 0.5f;

        protected override void Show() 
        {
            TutorialObjectivesDarkScreenModel.OnNextStep += OnNextControlStep;
            LocalizationModel.OnChangeLanguage += SetLocalization;
            
            SetOnTop();
            DisableClick();
            SetLocalization();
            PlayerMovementModel.SetBlockPlayerMovement(true);
        }

        protected override void Hide() 
        {
            TutorialObjectivesDarkScreenModel.OnNextStep -= OnNextControlStep;
            LocalizationModel.OnChangeLanguage -= SetLocalization;
            PlayerMovementModel.SetBlockPlayerMovement(false);
        }

        private bool needToShow = true;
        // TODO: move from view controller to controller
        private void OnNextControlStep()
        {
            if(needToShow)
            {
                needToShow = false;
                ViewsSystem.Show(ViewConfigID.Objectives);
                SetOnTop();

                ObjectivesViewModel.ShowTutorialButtonsOnTop();
                ObjectivesViewModel.OnPickUpAnyTutorialStep += OnPickUpAnyTutorialStep;
            }
            // TODO: implement for analitics
            // TutorialStep(TutorialDarkScreenModel.ControlStep - 1);
            if (TutorialObjectivesDarkScreenModel.IsControlStepsOver)
            {
                TutorialObjectivesDarkScreenModel.OnNextStep -= OnNextControlStep;
            }
        }

        private void OnPickUpAnyTutorialStep()
        {
            if(ObjectivesViewModel.IsHasAny) return;

            ObjectivesViewModel.OnPickUpAnyTutorialStep -= OnPickUpAnyTutorialStep;

            OnClick();
        }

        private void OnClick()
        {
            if (TutorialObjectivesDarkScreenModel.CanShowSteps)
            {
                View.SetActiveStep(TutorialObjectivesDarkScreenModel.ControlStep);
            }
            TutorialObjectivesDarkScreenModel.NextStep();
            DisableClick();

            if (TutorialObjectivesDarkScreenModel.IsControlStepsOver)
            {
                ViewsSystem.Hide(View);
            }
        }

        private void EnableClick()
        {
            View.OnClick += OnClick;
            GameUpdateModel.OnUpdate -= OnUpdate;
        }

        private void DisableClick()
        {
            View.OnClick -= OnClick;
            GameUpdateModel.OnUpdate += OnUpdate;
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

        private void SetOnTop()
        {
            View.transform.SetAsLastSibling();
        }

        private void SetLocalization()
        {
            View.SetObjectivesButtonText(LocalizationModel.GetString(LocalizationKeyID.ObjectivesMenu_Title));
            View.SetTextTakeReward(LocalizationModel.GetString(LocalizationKeyID.Tutorial_UI_Take_Rewards));
        }
    }
}
