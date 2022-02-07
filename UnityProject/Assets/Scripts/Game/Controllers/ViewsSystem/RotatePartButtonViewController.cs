using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using UnityEngine;

namespace Game.Controllers
{
    public class RotatePartButtonViewController : ViewControllerBase<RotateButtonView, RotateButtonData>
    {
        [Inject] public BuildingProcessModel BuildingProcessModel { get; private set; }
        [Inject] public RotateBuildablePreviewModel RotateBuildablePreviewModel { get; private set; }

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
            View.SetIcon(Data.ActiveIcon);
            BuildingProcessModel.Rotate(RotateBuildablePreviewModel.RotateAngle);
        }
    }
}
