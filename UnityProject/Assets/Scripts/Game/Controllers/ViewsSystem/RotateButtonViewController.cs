using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using UltimateSurvival;

namespace Game.Controllers
{
    public class RotateButtonViewController : ViewControllerBase<RotateButtonView, RotateButtonData>
    {
        [Inject] public RotateBuildablePreviewModel RotateBuildablePreviewModel { get; private set; }
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }

        protected override void Show()
        {
            View.OnPointerDown_ += OnPointerDownHandler;
            View.OnPointerUp_ += OnPointerUpHandler;

            View.SetIcon(Data.DefaultIcon);
        }

        protected override void Hide()
        {
            View.OnPointerDown_ -= OnPointerDownHandler;
            View.OnPointerUp_ -= OnPointerUpHandler;
        }

        private void OnPointerUpHandler()
        {
            View.SetIcon(Data.DefaultIcon);
        }

        private void OnPointerDownHandler()
        {
            RotateBuildablePreviewModel.Rotate();
            PlayerEventHandler.RotateObject.Try(RotateBuildablePreviewModel.RotationAngleCurrent);

            View.SetIcon(Data.ActiveIcon);
        }
    }
}
