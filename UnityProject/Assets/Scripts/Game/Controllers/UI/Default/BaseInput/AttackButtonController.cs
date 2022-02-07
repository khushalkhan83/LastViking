using Core;
using Core.Controllers;
using Core.Views;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class AttackButtonController : IAttackButtonController, IController
    {
        [Inject] public PlayerMovementModel PlayerMovementModel { get; private set; }
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public PlayerDeathModel PlayerDeathModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }

        IView View { get; set; }

        void IController.Enable()
        {
            // PlayerMovementModel.OnChangeMovementID += OnChangeMovementIDHandler;
            // PlayerDeathModel.OnRevival += OnRevivalHandler;
            // PlayerDeathModel.OnRevivalPrelim += OnRevivalPrelimHandler;
            // ViewsSystem.OnHideAll += OnHideAllHandler;
            // UpdateViewVisible();
        }

        void IController.Start()
        {
            // UpdateViewVisible();
        }

        void IController.Disable()
        {
            // PlayerMovementModel.OnChangeMovementID -= OnChangeMovementIDHandler;
            // PlayerDeathModel.OnRevival -= OnRevivalHandler;
            // PlayerDeathModel.OnRevivalPrelim -= OnRevivalPrelimHandler;
            // ViewsSystem.OnHideAll -= OnHideAllHandler;

            // HideView();
        }

        private void OnHideAllHandler() => View = null;

        private void OnRevivalPrelimHandler() => UpdateViewVisible();
        private void OnRevivalHandler() => UpdateViewVisible();
        private void OnChangeMovementIDHandler() => UpdateViewVisible();

        private bool IsCanShow => PlayerMovementModel.MovementID == PlayerMovementID.Ground
            && !PlayerHealthModel.IsDead;

        private void UpdateViewVisible()
        {
            if (IsCanShow)
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
                View = ViewsSystem.Show<AttackButtonView>(ViewConfigID.AttackButton);
            }
        }

        private void HideView()
        {
            if (View != null)
            {
                ViewsSystem.Hide(View);
                View = null;
            }
        }
    }
}
