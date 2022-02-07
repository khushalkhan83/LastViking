using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using UltimateSurvival;

namespace Game.Controllers
{
    public class PlaceReplacedButtonViewController : ViewControllerBase<PlaceButtonView, PlaceButtonData>
    {
        [Inject] public ReplaceBuildingModel ReplaceBuildingModel { get; private set; }

        protected override void Show()
        {
            View.SetIcon(Data.DefaultIcon);

            View.OnPointerDown_ += OnPointerDownHandler;
            View.OnPointerUp_ += OnPointerUpHandler;
        }

        protected override void Hide()
        {
            View.OnPointerDown_ -= OnPointerDownHandler;
            View.OnPointerUp_ -= OnPointerUpHandler;
        }

        private void OnPointerUpHandler()
        {
            ReplaceBuildingModel.PlaceReplacedBuilding();
            View.SetIcon(Data.DefaultIcon);
        }

        private void OnPointerDownHandler()
        {
            View.SetIcon(Data.ActiveIcon);
        }
    }
}
