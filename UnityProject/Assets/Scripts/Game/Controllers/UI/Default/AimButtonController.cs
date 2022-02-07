using Core;
using Core.Controllers;
using Core.Views;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class AimButtonController : IAimButtonController, IController
    {
        [Inject] public PlayerMovementModel PlayerMovementModel { get; private set; }
        [Inject] public HotBarModel HotBarModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }

        public IView View { get; private set; }

        void IController.Enable()
        {
            HotBarModel.OnChangeEquipCell += OnChangeEquipCellHandler;
            HotBarModel.OnChangeEquipItem += OnChangeEquipItemHandler;
            PlayerMovementModel.OnChangeMovementID += OnChangePlayerMovement;
            OnChangePlayerMovement();
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
            HotBarModel.OnChangeEquipCell += OnChangeEquipCellHandler;
            HotBarModel.OnChangeEquipItem += OnChangeEquipItemHandler;
            PlayerMovementModel.OnChangeMovementID += OnChangePlayerMovement;
            CloseView();
        }

        private void OnChangeEquipItemHandler() => UpdateAim();
        private void OnChangeEquipCellHandler() => UpdateAim();
        private void OnChangePlayerMovement() => UpdateAim();
        private void ChangeEquipItem() => UpdateAim();

        private void UpdateAim()
        {
            if
            (
                HotBarModel.EquipCell.IsHasItem
                && HotBarModel.EquipCell.Item. HasProperty("IsAiming")
                && PlayerMovementModel.MovementID == PlayerMovementID.Ground
            )
            {
                OpenView();
            }
            else
            {
                CloseView();
            }
        }

        private void OpenView()
        {
            if (View == null)
            {
                View = ViewsSystem.Show<AimButtonView>(ViewConfigID.AimButton);
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
