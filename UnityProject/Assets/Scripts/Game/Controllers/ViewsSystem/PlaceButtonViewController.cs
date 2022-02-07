using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using UltimateSurvival;

namespace Game.Controllers
{
    public class PlaceButtonViewController : ViewControllerBase<PlaceButtonView, PlaceButtonData>
    {
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public HotBarModel HotBarModel { get; private set; }
        [Inject] public PlacementModel PlacementModel { get; private set; }

        protected override void Show()
        {
            View.SetIcon(Data.DefaultIcon);

            View.OnPointerDown_ += OnPointerDownHandler;
            View.OnPointerUp_ += OnPointerUpHandler;
            PlayerEventHandler.PlaceObject.AddListener(OnPlaceHandler);
        }

        protected override void Hide()
        {
            View.OnPointerDown_ -= OnPointerDownHandler;
            View.OnPointerUp_ -= OnPointerUpHandler;
            PlayerEventHandler.PlaceObject.RemoveListener(OnPlaceHandler);
        }

        private void OnPointerUpHandler()
        {
            PlayerEventHandler.PlaceObject.Try();
        }

        private void OnPlaceHandler()
        {
            PlacementModel.PlaceItem(HotBarModel.EquipCell.Item.ItemData.Id);
            HotBarModel.ItemsContainer.RemoveItemsFromCell(HotBarModel.EquipCellId, 1);
            View.SetIcon(Data.DefaultIcon);
        }

        private void OnPointerDownHandler()
        {
            View.SetIcon(Data.ActiveIcon);
        }
    }
}
