using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using Extensions;
using Game.Components;

namespace Game.Controllers
{
    public class PlacePartButtonViewController : ViewControllerBase<PlaceButtonView, PlaceButtonData>
    {
        [Inject] public BuildingProcessModel BuildingProcessModel { get; private set; }
        [Inject] public BuildingModeModel BuildingModeModel { get; set; }

        protected override void Show()
        {
            View.SetIcon(Data.DefaultIcon);

            View.OnPointerDown_ += OnPointerDownHandler;
            View.OnPointerUp_ += OnPointerUpHandler;
            BuildingProcessModel.OnPartPlaced += OnPlacedPart;
            BuildingModeModel.OnHighlightPlaceButton += HighlightPlaceButton;
        }

        protected override void Hide()
        {
            View.OnPointerDown_ -= OnPointerDownHandler;
            View.OnPointerUp_ -= OnPointerUpHandler;
            BuildingProcessModel.OnPartPlaced -= OnPlacedPart;
            BuildingModeModel.OnHighlightPlaceButton -= HighlightPlaceButton;
        }

        private void OnPlacedPart()
        {
            HighlightPlaceButton(false);
        }

        private void OnPointerUpHandler()
        {
            BuildingModeModel.PlacePartButtonPressed();
            View.SetIcon(Data.DefaultIcon);
        }

        private void OnPointerDownHandler()
        {
            View.SetIcon(Data.ActiveIcon);
        }

        private void HighlightPlaceButton(bool highlight)
        {
            if(highlight)
                View.PlayPulse();
            else
                View.PlayDefault();
        }
    }
}
