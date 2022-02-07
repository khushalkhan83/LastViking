using System;
using Core;
using Core.Controllers;
using Core.Views;
using Game.Models;
using Game.Views;
using SimpleDiskUtils;
using UnityEngine;

namespace Game.Controllers
{
    public class EnoughSpaceControllerLoadingScene : MonoBehaviour, IEnoughSpaceController
    {

        [SerializeField] private EnoughSpaceModel _enoughSpaceModel;
        [SerializeField] private NoSpaceForSaveExitPopupView _noSpaceForSaveExitPopupView;
        // [Inject] public PlayerScenesModel PlayerScenesModel{ get; set; }
        // [Inject] public ViewsSystem ViewsSystem{ get; set; }
        // [Inject] public ApplicationCallbacksModel ApplicationCallbacksModel{ get; set; }

        
        // private IView NoSpaceForSavePopupView { get; set; }
        private IView NoSpaceForSaveExitPopupView {get; set;}
        private EnoughSpaceModel EnoughSpaceModel => _enoughSpaceModel;

        
        private void OnEnable() 
        {
            // EnoughSpaceModel.OnNotEnoughSpace += OnNotEnoughSpace;
            EnoughSpaceModel.OnNotEnoughSpaceCritical += OnNotEnoughSpaceCritical;

            // PlayerScenesModel.OnEnvironmentLoaded += OnEnvironmentLoaded;

            // ApplicationCallbacksModel.ApplicationFocus += OnApplicationFocus;
            // ApplicationCallbacksModel.ApplicationPause += OnApplicationPause;
        }

        private void Start() 
        {
            
        }

        private void OnDisable() 
        {
            // EnoughSpaceModel.OnNotEnoughSpace -= OnNotEnoughSpace;
            EnoughSpaceModel.OnNotEnoughSpaceCritical -= OnNotEnoughSpaceCritical;

            // PlayerScenesModel.OnEnvironmentLoaded -= OnEnvironmentLoaded;

            // ApplicationCallbacksModel.ApplicationFocus -= OnApplicationFocus;
            // ApplicationCallbacksModel.ApplicationPause -= OnApplicationPause;

            // CloseView();
        }

//         private void OnApplicationPause(bool pause)
//         {
//             if (!pause)
//             {
//                 Refresh();
//             }
//         } 

//         private void OnApplicationFocus(bool hasFocus)
//         {
// #if !UNITY_EDITOR
//             if (focus)
//             {
//                 Refresh();
//             }
// #endif
//         }

        // private void OnEnvironmentLoaded()
        // {
        //     CheckNoSpaceForSave();
        // }

        // private void OnNotEnoughSpace(float spaceLeft)
        // {
        //     OpenNoSpaceForSavePopupView();
        // }

        private void OnNotEnoughSpaceCritical(float spaceLeft)
        {
            OpenNoSpaceForSaveExitPopupView();
        }

        // private void CheckNoSpaceForSave()
        // {
        //     bool notEnough = !EnoughSpaceModel.HasEnoughSpace(out bool critical);
            
        //     if(notEnough && !critical)
        //     {
        //         OpenNoSpaceForSavePopupView();
        //     }
        //     else if (notEnough && critical)
        //     {
        //         OpenNoSpaceForSaveExitPopupView();
        //     }
        // }

        // private void Refresh()
        // {
        //     bool notEnough = !EnoughSpaceModel.HasEnoughSpace(out bool critical);
        //     bool criticalMemoryLevel = notEnough && critical;
        //     bool criticalViewExist = NoSpaceForSaveExitPopupView != null;

        //     if (!criticalMemoryLevel && criticalViewExist)
        //     {
        //         ViewsSystem.Hide(NoSpaceForSaveExitPopupView);
        //     }
        //     else if(criticalMemoryLevel &&  !criticalViewExist)
        //     {
        //         OpenNoSpaceForSaveExitPopupView();
        //     }
        // }

        private void OpenNoSpaceForSaveExitPopupView()
        {
            if (NoSpaceForSaveExitPopupView == null)
            {
                NoSpaceForSaveExitPopupView = _noSpaceForSaveExitPopupView;
                _noSpaceForSaveExitPopupView.gameObject.SetActive(true);

                NoSpaceForSaveExitPopupView.OnHide += OnHideNoSpaceForSaveExitHandler;
                
                EnoughSpaceModel.ShowNotEnoughSpaceCriticalView();
            }
        }

        // private void OpenNoSpaceForSavePopupView()
        // {
        //     if (NoSpaceForSavePopupView == null)
        //     {
        //         NoSpaceForSavePopupView = ViewsSystem.Show<NoSpaceForSavePopupView>(ViewConfigID.NoSpaceForSavePopup);
        //         NoSpaceForSavePopupView.OnHide += OnHideNoSpaceForSaveHandler;

        //         EnoughSpaceModel.ShowNotEnoughSpaceView();
        //     }
        // }

        // private void OnHideNoSpaceForSaveHandler(IView view)
        // {
        //     view.OnHide -= OnHideNoSpaceForSaveHandler;
        //     NoSpaceForSavePopupView = null;
        // }

        private void OnHideNoSpaceForSaveExitHandler(IView view)
        {
            view.OnHide -= OnHideNoSpaceForSaveExitHandler;
            NoSpaceForSaveExitPopupView = null;
        }

        // private void CloseView()
        // {
        //     if (NoSpaceForSavePopupView != null)
        //     {
        //         ViewsSystem.Hide(NoSpaceForSavePopupView);
        //     }

        //     if (NoSpaceForSaveExitPopupView != null)
        //     {
        //         ViewsSystem.Hide(NoSpaceForSaveExitPopupView);
        //     }
        // }
    }
}
