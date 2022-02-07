using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using UltimateSurvival;

namespace Game.Controllers
{
    public class FishingInitViewController : ViewControllerBase<AimButtonView>
    {
        [Inject] public FishingModel fishingModel { get; private set; }
        protected override void Show()
        {
            fishingModel.OnThrowStart += ThrowStart;
            fishingModel.OnThrowAnimationEnd += ThrowsEnd;

            ThrowsEnd();
        }

        void ThrowStart()
        {
            View.SetIcon(View.IconDefault);
            View.OnPointerDown_ -= OnPointerDownHandler;
        }

        void ThrowsEnd()
        {
            View.SetIcon(View.IconActive);
            View.OnPointerDown_ += OnPointerDownHandler;
        }

        protected override void Hide()
        {
            ThrowStart();
            fishingModel.OnThrowStart -= ThrowStart;
            fishingModel.OnThrowAnimationEnd -= ThrowsEnd;
        }

        private void OnPointerDownHandler()
        {
            //AimModel.SetActive(!AimModel.IsActive);
            fishingModel.Throw();
        }
    }
}