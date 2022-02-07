using Core;
using Core.Controllers;
using Core.Views;
using Game.Models;
using Game.Views;
using System.Collections;
using UnityEngine;

namespace Game.Controllers
{
    public class TreasureHuntRewardController : ITreasureHuntRewardController, IController
    {
        [Inject] public TreasureHuntModel TreasureHuntModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }

        public IView View { get; private set; }

        void IController.Enable()
        {
            TreasureHuntModel.OnGetTreasure += OnGetTreasure;
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
            TreasureHuntModel.OnGetTreasure -= OnGetTreasure;
            CloseView();
        }

        private void OnGetTreasure()
        {
            TreasureHuntModel.StartCoroutine(WaitForShowView());
        }

        private IEnumerator WaitForShowView()
        {
            yield return new WaitForSeconds(0.3f);
            OpenView();
        }

        private void OpenView()
        {
            if (View == null)
            {
                View = ViewsSystem.Show<TreasureLootPopupView>(ViewConfigID.TreasureHuntLootPopup);
                View.OnHide += OnHideHandler;
            }
        }

        private void CloseView()
        {
            if (View != null)
            {
                ViewsSystem.Hide(View);
            }
        }

        private void OnHideHandler(IView view)
        {
            view.OnHide -= OnHideHandler;
            View = null;
        }
    }
}
