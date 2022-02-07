using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using UltimateSurvival;

namespace Game.Controllers
{
    public class MiniGameExitViewController : ViewControllerBase<MiniGameExitView>
    {
        [Inject] public MiniGameStateModel minigameModel { get; private set; }
        protected override void Show()
        {
            View.OnPointerDown_ += OnPointerDownHandler;

            View.SetIcon(View.IconActive);
        }

        protected override void Hide()
        {
            View.OnPointerDown_ -= OnPointerDownHandler;
        }

        private void OnPointerDownHandler()
        {
            //AimModel.SetActive(!AimModel.IsActive);
            minigameModel.IsMinigame = false;
        }
    }
}