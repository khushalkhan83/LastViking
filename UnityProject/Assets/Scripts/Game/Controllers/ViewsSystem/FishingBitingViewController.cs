using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using UltimateSurvival;

namespace Game.Controllers
{
    public class FishingBitingViewController : ViewControllerBase<FishingBitingView>
    {
        [Inject] public FishingModel fishingModel { get; private set; }
        protected override void Show()
        {
            View.OnPointerDown_ += OnPointerDownHandler;
            View.SetBiting(false);

            fishingModel.OnBiting += SetBiting;
        }

        protected override void Hide()
        {
            View.OnPointerDown_ -= OnPointerDownHandler;
            fishingModel.OnBiting -= SetBiting;
        }

        void SetBiting(bool isBiting) => View.SetBiting(isBiting);

        private void OnPointerDownHandler()
        {
            fishingModel.TryHook();
        }
    }
}
