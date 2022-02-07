using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class StaminaBarViewController : ViewControllerBase<StaminaBarView>
    {
        [Inject] public PlayerStaminaModel PlayerStaminaModel { get; private set; }

        protected override void Show()
        {
            PlayerStaminaModel.OnChangeStamina += OnChangeStaminaHandler;

            View.SetAmount(PlayerStaminaModel.Stamina / PlayerStaminaModel.StaminaMax);
        }

        protected override void Hide()
        {
            PlayerStaminaModel.OnChangeStamina -= OnChangeStaminaHandler;
        }

        private void OnChangeStaminaHandler() => View.SetAmount(PlayerStaminaModel.Stamina / PlayerStaminaModel.StaminaMax);
    }
}
