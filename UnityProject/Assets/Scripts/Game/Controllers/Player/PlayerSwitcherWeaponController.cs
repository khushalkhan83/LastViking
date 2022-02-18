using Core;
using Core.Controllers;
using Game.Audio;
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

        public AudioSystem AudioSystem => AudioSystem.Instance;

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
                if (PlayerEventHandler.ChangeEquippedItem.Try(null, false))
                {
                    AudioSystem.PlayOnce(AudioID.Draw02);
                }
            }
            else
            {
                if (PlayerEventHandler.ChangeEquippedItem.Try(HotBarModel.EquipCell.Item, false))
                {
                    AudioSystem.PlayOnce(AudioID.Draw02);
                }
            }
        }
    }
}
