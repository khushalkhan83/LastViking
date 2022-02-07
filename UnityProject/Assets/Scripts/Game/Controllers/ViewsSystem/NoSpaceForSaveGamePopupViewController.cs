using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using UnityEngine;


namespace Game.Controllers
{
    public class NoSpaceForSaveGamePopupViewController : ViewControllerBase<NoSpaceForSavePopupView>
    {
        [Inject] public LocalizationModel LocalizationModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public GameTimeModel GameTimeModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel{ get; private set; }

        private float _waiteTime = 10f;
        private bool _updateTimer;
        private float _timePassed;

        protected override void Show()
        {
            View.OnOk += OnOk;
            GameUpdateModel.OnUpdate += OnUpdate;
            LocalizationModel.OnChangeLanguage += SetLocalization;

            View.SetOkButtonInteractable(false);
            _updateTimer = true;
            _timePassed = 0;
            View.SetTextTimerText(_waiteTime.ToString());

            SetLocalization();
        }

        protected override void Hide()
        {
            View.OnOk -= OnOk;
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
                    View.SetOkButtonInteractable(true);
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

        private void OnOk()
        {
            ViewsSystem.Hide(View);
        }

        private void SetLocalization()
        {
            View.SetTextMessageText(LocalizationModel.GetString(LocalizationKeyID.NotEnoughStorageSpacePopUp_Text));
            View.SetTextOkButtonText(LocalizationModel.GetString(LocalizationKeyID.ResetWarning_OkBtn));
        }
    }
}
