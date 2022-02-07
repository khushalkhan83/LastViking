using Core;
using Core.Controllers;
using Core.Views;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class EnableVersionInfoViewController : IEnableVersionInfoViewController, IController
    {
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public PlayerDeathModel PlayerDeathModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }

        private VersionInfoView VersionInfoView { get;set;}

        void IController.Enable()
        {
            PlayerDeathModel.OnRevival += OnRevivalHandler;
            PlayerDeathModel.OnRevivalPrelim += OnRevivalHandler;
        }

        void IController.Start()
        {
            if (!PlayerHealthModel.IsDead)
            {
                OpenView();
            }
        }

        void IController.Disable()
        {
            CloseView();
        }

        private void OnRevivalHandler() => OpenView();

        private void OpenView()
        {
            if (VersionInfoView == null)
            {
                VersionInfoView = ViewsSystem.Show<VersionInfoView>(ViewConfigID.VersionInfo);
                VersionInfoView.OnHide += OnHideHandler;
            }
        }

        private void CloseView()
        {
            if (VersionInfoView != null)
            {
                ViewsSystem.Hide(VersionInfoView);
            }
        }

        private void OnHideHandler(IView view)
        {
            view.OnHide -= OnHideHandler;
            VersionInfoView = null;
        }
    }
}
