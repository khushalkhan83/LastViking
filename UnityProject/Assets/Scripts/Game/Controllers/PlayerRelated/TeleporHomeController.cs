using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using UnityEngine;
using System.Collections;

namespace Game.Controllers
{
    public class TeleporHomeController : ITeleporHomeController, IController
    {
        [Inject] public TeleporHomeModel TeleporHomeModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public CoroutineModel CoroutineModel { get; private set; }

        private TeleporHomeView TeleporHomeView { get; set; }

        private int corIndex = -1;

        void IController.Enable() 
        {
            TeleporHomeModel.OnShowPopup += ShowPopup;
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            TeleporHomeModel.OnShowPopup -= ShowPopup;
            CoroutineModel.BreakeCoroutine(corIndex);
            HidePopup();
        }

        private void ShowPopup()
        {
            CoroutineModel.BreakeCoroutine(corIndex);
            corIndex = CoroutineModel.InitCoroutine(ShowPopupCoroutine());
        }

        private IEnumerator ShowPopupCoroutine()
        {
            yield return new WaitForSeconds(TeleporHomeModel.ShowDelay);
            TeleporHomeView = ViewsSystem.Show<TeleporHomeView>(ViewConfigID.TeleporHomeConfig);
        }

        private void HidePopup()
        {
            if(TeleporHomeView != null && TeleporHomeView.gameObject.activeSelf)
            {
                ViewsSystem.Hide(TeleporHomeView);
                TeleporHomeView = null;
            }
        }
    }
}
