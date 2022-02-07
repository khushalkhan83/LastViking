using Core;
using Core.Controllers;
using Core.Views;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class EnableObjectivesButtonController : IEnableObjectivesButtonController, IController
    {
        [Inject] public EnableObjectivesButtonModel EnableObjectivesButtonModel { get; private set; }
        [Inject] public TutorialModel TutorialModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public PlayerDeathModel PlayerDeathModel { get; private set; }
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }

        public IView View { get; private set; }

        public bool IsCanShow => TutorialModel.IsComplete;

        void IController.Enable()
        {
            // if (IsCanShow)
            // {
            //     if (!PlayerHealthModel.IsDead)
            //     {
            //         OpenView();
            //     }
            // }
            // else
            // {
            //     TutorialModel.OnComplete += OnOnCompleteTutorialHandler;
            // }

            // PlayerDeathModel.OnRevival += OnRevivalHandler;
            // PlayerDeathModel.OnRevivalPrelim += OnRevivalHandler;
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
            // TutorialModel.OnComplete -= OnOnCompleteTutorialHandler;
            // PlayerDeathModel.OnRevival -= OnRevivalHandler;
            // PlayerDeathModel.OnRevivalPrelim -= OnRevivalHandler;

            // CloseView();
        }

        private void OnRevivalHandler()
        {
            if (IsCanShow)
            {
                OpenView();
            }
        }

        private void OnOnCompleteTutorialHandler()
        {
            EnableObjectivesButtonModel.SetFirst();
            UpdateLog();
        }

        private void UpdateLog()
        {
            TutorialModel.OnComplete -= OnOnCompleteTutorialHandler;
            OpenView();
        }

        private void OpenView()
        {
            if (View == null)
            {
                View = ViewsSystem.Show<ObjectivesButtonView>(ViewConfigID.ObjectiveButton);
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
