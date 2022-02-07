using Game.Views;
using Core.Controllers;
using Core;
using Game.Models;
using UnityEngine;

namespace Game.Controllers
{
    public class NoSpaceForSaveGameExitPopupViewController : ViewControllerBase<NoSpaceForSaveExitPopupView>
    {
        [Inject] public LocalizationModel LocalizationModel { get; private set; }
        [Inject] public GameTimeModel GameTimeModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel{ get; private set; }

        private float _waiteTime = 10f;
        private bool _updateTimer;
        private float _timePassed;

        protected override void Show()
        {
            View.OnExit += OnExit;
            GameUpdateModel.OnUpdate += OnUpdate;
            LocalizationModel.OnChangeLanguage += SetLocalization;

            View.SetExitButtonInteractable(false);
            _updateTimer = true;
            _timePassed = 0;
            View.SetTextTimerText(_waiteTime.ToString());

            SetLocalization();
        }

        protected override void Hide()
        {
            View.OnExit -= OnExit;
            GameUpdateModel.OnUpdate -= OnUpdate;
            LocalizationModel.OnChangeLanguage -= SetLocalization;
        }

        private void OnUpdate()
        {
            if(_updateTimer)
            {
                _timePassed += Time.unscaledDeltaTime;
                if(_timePassed > _waiteTime)
                {
                    View.SetExitButtonInteractable(true);
                    View.SetTextTimerText("");
                    _updateTimer = false;
                }
                else
                {
                    int _timer = Mathf.CeilToInt(_waiteTime - _timePassed);
                    View.SetTextTimerText(_timer.ToString());
                }
            }
        }

        private void OnExit()
        {
            Application.Quit();
        }

        private void SetLocalization()
        {
            View.SetTextMessageText(LocalizationModel.GetString(LocalizationKeyID.NotEnoughStorageSpaceCriticalPopUp_Text));
            View.SetTextExitButtonText(LocalizationModel.GetString(LocalizationKeyID.Close_Game_Button));
        }

    }
}
