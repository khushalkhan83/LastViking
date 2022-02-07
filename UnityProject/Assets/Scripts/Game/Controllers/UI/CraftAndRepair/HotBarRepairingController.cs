using Core;
using Core.Controllers;
using Core.Views;
using Game.Models;
using Game.Views;
using UltimateSurvival;

namespace Game.Controllers
{
    public class HotBarRepairingController : IHotBarRepairingController, IController
    {
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public PlayerDeathModel PlayerDeathModel { get; private set; }

        protected HotBarRepairingContainerView HotBarRepairingContainerView { private set; get; }

        void IController.Enable()
        {
            PlayerDeathModel.OnRevival += OnRevivalHandler;
            PlayerDeathModel.OnRevivalPrelim += OnRevivalPrelimHandler;
        }

        void IController.Start()
        {
            ShowView();
        }

        void IController.Disable()
        {
            PlayerDeathModel.OnRevival -= OnRevivalHandler;
            PlayerDeathModel.OnRevivalPrelim -= OnRevivalPrelimHandler;

            HideView();
        }

        private void OnRevivalPrelimHandler() => ShowView();
        private void OnRevivalHandler() => ShowView();

        private void ShowView()
        {
            if (HotBarRepairingContainerView == null)
            {
                HotBarRepairingContainerView = ViewsSystem.Show<HotBarRepairingContainerView>(ViewConfigID.HotBarRepairingContainerView);
                HotBarRepairingContainerView.OnHide += OnHideHandler;
            }
        }

        private void HideView()
        {
            if (HotBarRepairingContainerView != null)
            {
                ViewsSystem.Hide(HotBarRepairingContainerView);
            }
        }

        private void OnHideHandler(IView view)
        {
            view.OnHide -= OnHideHandler;
            HotBarRepairingContainerView = null;
        }
    }
}
