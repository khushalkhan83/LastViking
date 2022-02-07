using Core;
using Core.Controllers;
using Game.Models;
using Game.Purchases;
using Game.Views;
using UnityEngine;

namespace Game.Controllers
{
    public class BackgroundPauseController : IBackgroundPauseController, IController
    {
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public PurchasesModel PurchasesModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public ApplicationCallbacksModel ApplicationCallbacksModel { get; private set; }
        [Inject] public CinematicModel CinematicModel { get; private set; }

        private bool IsShowing { get; set; }

        void IController.Enable()
        {
            ViewsSystem.OnBeginShow.AddListener(ViewConfigID.Settings, OnBeginShowSettingsHandler);
            ViewsSystem.OnEndHide.AddListener(ViewConfigID.Settings, OnEndHideSettingsHandler);
            ApplicationCallbacksModel.ApplicationPause += OnApplicationPause;
            ApplicationCallbacksModel.ApplicationFocus += OnApplicationFocus;
        }

        void IController.Start()
        {
        }

        void IController.Disable()
        {
            ViewsSystem.OnBeginShow.RemoveListener(ViewConfigID.Settings, OnBeginShowSettingsHandler);
            ViewsSystem.OnEndHide.RemoveListener(ViewConfigID.Settings, OnEndHideSettingsHandler);
            ApplicationCallbacksModel.ApplicationPause -= OnApplicationPause;
            ApplicationCallbacksModel.ApplicationFocus -= OnApplicationFocus;
        }

        private void OnBeginShowSettingsHandler() => IsShowing = true;

        private void OnEndHideSettingsHandler() => IsShowing = false;

        //[Unityengine]
        private void OnApplicationPause(bool pause) => Processing(!pause);

        //[Unityengine]
        private void OnApplicationFocus(bool isFocus) => Processing(isFocus);

        private void Processing(bool isFocus)
        {
            var isCanShow =
                !isFocus
                && (!IsShowing)
                && !PurchasesModel.IsPurchasing
                && !PlayerHealthModel.IsDead
                && !ViewsSystem.IsShow(ViewConfigID.GameSparksAccount)
                && !ViewsSystem.IsShow(ViewConfigID.GameSparksStart)
                && !ViewsSystem.IsShow(ViewConfigID.NoSpaceForSaveExitConfig)
                && !CinematicModel.CinematicStarted;

#if UNITY_EDITOR
            isCanShow = false;
#endif

            if (isCanShow)
            {
                ViewsSystem.Show<SettingsView>(ViewConfigID.Settings);
            }
        }
    }
}
