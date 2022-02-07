using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class PopupsTimeScaleController : IPopupsTimeScaleController, IController
    {
        [Inject] public PopupsTimeScaleModel PopupTimeScaleModel {get; private set;}
        [Inject] public CinematicModel CinematicModel { get; private set; }
        [Inject] public GameTimeModel GameTimeModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }

        void IController.Enable() 
        {
            foreach(var configId in PopupTimeScaleModel.PopupViewIds)
            {
                ViewsSystem.OnBeginShow.AddListener(configId, OnShowPopup);
                ViewsSystem.OnEndHide.AddListener(configId, OnHidePopup);
            }
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            foreach(var configId in PopupTimeScaleModel.PopupViewIds)
            {
                ViewsSystem.OnBeginShow.RemoveListener(configId, OnShowPopup);
                ViewsSystem.OnEndHide.RemoveListener(configId, OnHidePopup);
            }
        }

        private void OnShowPopup()
        {
            if(!CinematicModel.CinematicStarted)
            {
                GameTimeModel.ScaleSave();
                GameTimeModel.ScaleStop();
            }
        }

        private void OnHidePopup()
        {
            GameTimeModel.ScaleRestore();
        }

    }
}
