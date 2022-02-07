using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using UltimateSurvival;

namespace Game.Controllers
{
    public class AimButtonViewController : ViewControllerBase<AimButtonView>
    {
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public AimModel AimModel { get; private set; }

        protected override void Show()
        {
            View.OnPointerDown_ += OnPointerDownHandler;
            PlayerEventHandler.Aim.OnStart += OnStartAimHandler;
            PlayerEventHandler.Aim.OnStop += OnStopAimHandler;

            SetIsActiveIcon(PlayerEventHandler.Aim.Active);
        }

        protected override void Hide()
        {
            View.OnPointerDown_ -= OnPointerDownHandler;
            PlayerEventHandler.Aim.OnStart -= OnStartAimHandler;
            PlayerEventHandler.Aim.OnStop -= OnStopAimHandler;
        }

        private void OnStopAimHandler()
        {
            View.SetIcon(View.IconDefault);
        }

        private void OnStartAimHandler()
        {
            View.SetIcon(View.IconActive);
        }

        private void OnPointerDownHandler()
        {
            AimModel.SetActive(!AimModel.IsActive);

            if (!PlayerEventHandler.Aim.Active)
            {
                PlayerEventHandler.Aim.TryStart();
            }
            else
            {
                PlayerEventHandler.Aim.ForceStop();
            }
        }

        private void SetIsActiveIcon(bool value)
        {
            if (value)
            {
                View.SetIcon(View.IconActive);
            }
            else
            {
                View.SetIcon(View.IconDefault);
            }
        }
    }
}
