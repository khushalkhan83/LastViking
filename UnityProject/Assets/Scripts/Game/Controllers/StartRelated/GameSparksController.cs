using Core;
using Core.Controllers;
using Core.Views;
using Game.Models;
using Game.Views;
using UnityEngine;

namespace Game.Controllers
{
    public class GameSparksController : MonoBehaviour, IController, IGameSparksController
    {
        [Inject] public GameSparksModel GameSparksModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public PlayerScenesModel PlayerScenesModel { get; private set; }

        IView View { get; set; }

        void IController.Enable()
        {
            PlayerScenesModel.OnEnvironmentLoaded += StartLogin;
        }

        void IController.Start()
        {
            
        }

        void IController.Disable()
        {
            PlayerScenesModel.OnEnvironmentLoaded -= StartLogin;
            HideLoginView();
        }

        private void StartLogin()
        {
            if (!GameSparksModel.IsHasUserLogined && !GameSparksModel.IsHasUserDeviceLogined && !GameSparksModel.IsHasUserRegistered)
            {
                ShowLoginView();
            }
            PlayerScenesModel.OnEnvironmentLoaded -= StartLogin;
        }

        private void ShowLoginView()
        {
            View = ViewsSystem.Show<GameSparksStartView>(ViewConfigID.GameSparksStart);
            View.OnHide += OnHideViewHandler;
        }

        private void OnHideViewHandler(IView view)
        {
            view.OnHide -= OnHideViewHandler;
            View = null;
        }

        void HideLoginView()
        {
            if (View != null)
            {
                ViewsSystem.Hide(View);
            }
        }

        //private void AutoLogin()
        //{
        //    if (!GS.Available) return;

        //    if (GameSparksModel.IsHasUserLogined)
        //    {
        //        new AuthenticationRequest()
        //            .SetUserName(GameSparksModel.UserName)
        //            .SetPassword(GameSparksModel.UserPass)
        //            .Send(x => { });
        //    }
        //    else if (GameSparksModel.IsHasUserDeviceLogined)
        //    {
        //        new DeviceAuthenticationRequest()
        //            .SetDisplayName(GameSparksModel.UserName)
        //            .Send(x => { });
        //    }
        //}
    }
}
