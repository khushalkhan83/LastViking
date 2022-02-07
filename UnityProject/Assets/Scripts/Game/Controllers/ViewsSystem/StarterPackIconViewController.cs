using System;
using Core;
using Core.Controllers;
using Extensions;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class StarterPackIconViewController : ViewControllerBase<StarterPackIconView>
    {
        [Inject] public StarterPackModel StarterPackModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public RealTimeModel RealTimeModel { get; private set; }
        [Inject] public LocalizationModel LocalizationModel { get; private set; }

        protected override void Show()
        {
            GameUpdateModel.OnUpdate += OnUpdate;
            RealTimeModel.OnTimeReady += OnReadyRealtimeHandler;
            RealTimeModel.OnTimeError += OnErrorRealtimeHandler;
            View.OnClick += OnClickOnIcon;

            ActivateIcon(RealTimeModel.isReady);
        }

        protected override void Hide()
        {
            GameUpdateModel.OnUpdate -= OnUpdate;
            RealTimeModel.OnTimeReady -= OnReadyRealtimeHandler;
            RealTimeModel.OnTimeError -= OnErrorRealtimeHandler;
            View.OnClick -= OnClickOnIcon;
        }

        private void OnUpdate()
        {
            string minSec = LocalizationModel.GetString(LocalizationKeyID.ObjectivesMenu_MinSec);
            string hourMin = LocalizationModel.GetString(LocalizationKeyID.ObjectivesMenu_HourMin);
            string timeText = StarterPackModel.RemainOfferTime.TimeToString(minSec, hourMin);
            View.SetTimer(timeText);
        }

        private void OnErrorRealtimeHandler(string obj) => ActivateIcon(false);
        private void OnReadyRealtimeHandler() => ActivateIcon(true);

        private void ActivateIcon(bool on)
        {
            View.SetActiveTimer(on);
            View.SetActiveNoInetObject(!on);
            View.SetEnableButton(on);
        }

        private void OnClickOnIcon()
        {
            ViewsSystem.Show<StarterPackPopupView>(ViewConfigID.StarterPackPopupView);
            StarterPackModel.ShowStarterPackPopupFromIcon();
        }
    }
}
