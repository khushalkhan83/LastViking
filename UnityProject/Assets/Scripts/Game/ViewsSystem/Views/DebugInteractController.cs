using Core;
using Core.Controllers;
using Core.Views;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class DebugInteractController : IDebugInteractController, IController
    {
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public DebugTimeInteractViewModel DebugTimeInteractViewModel { get; private set; }

        public IView View { get; private set; }

        void IController.Enable()
        {
            DebugTimeInteractViewModel.OnChangeIsCanVisibleDebug += OnChangeIsCanVisibleDebugHandler;
            UpdateDebugView();
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
            DebugTimeInteractViewModel.OnChangeIsCanVisibleDebug -= OnChangeIsCanVisibleDebugHandler;

            HideView();
        }

        private void OnChangeIsCanVisibleDebugHandler() => UpdateDebugView();

        private void UpdateDebugView()
        {
            if (DebugTimeInteractViewModel.IsCanVisibleDebug)
            {
                ShowView();
            }
            else
            {
                HideView();
            }
        }

        private void ShowView()
        {
            if (View == null)
            {
                View = ViewsSystem.Show<DebugTimeView>(ViewConfigID.DebugTime);
                View.OnHide += OnHideHandler;
            }
        }

        private void HideView()
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
