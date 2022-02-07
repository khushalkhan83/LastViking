using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using UnityEngine;

namespace Game.Controllers
{
    public class TutorialDarkViewController : ViewControllerBase<TutorialDarkView>
    {
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public TutorialDarkScreenModel TutorialDarkScreenModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public LocalizationModel LocalizationModel { get; private set; }

        private readonly float clickDelay = 0.5f;

        protected override void Show()
        {
            View.transform.SetAsLastSibling();
            DisableClick();
            LocalizationModel.OnChangeLanguage += SetLocalization;

            SetLocalization();
        }

        protected override void Hide()
        {
            View.OnClick -= OnClick;
            GameUpdateModel.OnUpdate -= OnUpdate;
            LocalizationModel.OnChangeLanguage -= SetLocalization;
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

        private void OnClick()
        {
            if (TutorialDarkScreenModel.CanShowSteps)
            {
                View.SetActiveStep(TutorialDarkScreenModel.ControlStep);
            }
            TutorialDarkScreenModel.NextStep();
            DisableClick();

            if (TutorialDarkScreenModel.IsControlStepsOver)
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

        private void SetLocalization()
        {
            View.SetTextAttack(LocalizationModel.GetString(LocalizationKeyID.Ctrl_Tutorial_Attack));
            View.SetMoveAttack(LocalizationModel.GetString(LocalizationKeyID.Ctrl_Tutorial_Move));
            View.SetJumpAttack(LocalizationModel.GetString(LocalizationKeyID.Ctrl_Tutorial_Jump));
            View.SetRunAttack(LocalizationModel.GetString(LocalizationKeyID.Ctrl_Tutorial_Run));
            View.SetMissionAttack(LocalizationModel.GetString(LocalizationKeyID.Ctrl_Tutorial_Mission));
        }
    }
}
