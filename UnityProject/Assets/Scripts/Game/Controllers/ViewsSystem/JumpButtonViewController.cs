using Core;
using Core.Controllers;
using Game.Views;
using UltimateSurvival;

namespace Game.Controllers
{
    public class JumpButtonViewController : ViewControllerBase<JumpButtonView>
    {
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public WeaponCoolDownModel WeaponCoolDownModel { get; private set; }

        protected override void Show()
        {
            View.OnPointerDown_ += OnPointerDownHandler;
            View.OnPointerUp_ += OnPointerUpHandler;
        }

        protected override void Hide()
        {
            View.OnPointerDown_ -= OnPointerDownHandler;
            View.OnPointerUp_ -= OnPointerUpHandler;
        }

        private void OnPointerDownHandler()
        {
            if (WeaponCoolDownModel.CoolDownTime > 0)
            {
                WeaponCoolDownModel.CoolDownTime = WeaponCoolDownModel.CoolDownTimeDefault;
            }            
            PlayerEventHandler.Jump.TryStart();
            View.SetIcon(View.IconActive);
        }

        private void OnPointerUpHandler()
        {
            View.SetIcon(View.IconDefault);
        }
    }
}
