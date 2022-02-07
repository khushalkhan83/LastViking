using Core;
using Core.Controllers;
using Game.Models;
using UltimateSurvival;

namespace Game.Controllers
{
    public class PlayerSwitcherWeaponController : IPlayerSwitcherWeaponController, IController
    {
        [Inject] public PlayerMovementModel PlayerMovementModel { get; private set; }
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public HotBarModel HotBarModel { get; private set; }
        [Inject] public ItemsDB ItemsDB { get; private set; }

        void IController.Enable()
        {
            PlayerMovementModel.OnChangeMovementID += OnChangeMovement;
            HotBarModel.OnChangeEquipItem += OnChangeMovement;
            HotBarModel.OnChangeEquipCell += OnChangeMovement;
        }

        void IController.Start()
        {
            OnChangeMovement();
        }

        void IController.Disable()
        {
            PlayerMovementModel.OnChangeMovementID -= OnChangeMovement;
            HotBarModel.OnChangeEquipItem -= OnChangeMovement;
            HotBarModel.OnChangeEquipCell -= OnChangeMovement;
        }

        private void OnChangeMovement()
        {
            if (PlayerMovementModel.MovementID == PlayerMovementID.Water)
            {
                PlayerEventHandler.ChangeEquippedItem.Try(null, false);
            }
            else
            {
                PlayerEventHandler.ChangeEquippedItem.Try(HotBarModel.EquipCell.Item, false);
            }
        }
    }
}
