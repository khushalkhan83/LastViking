using Core;
using Core.Controllers;
using Game.Models;
using UltimateSurvival;

namespace Game.Controllers
{
    public class PlayerAimController : IPlayerAimController, IController
    {
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public HotBarModel HotBarModel { get; private set; }

        void IController.Enable()
        {
            HotBarModel.OnChangeEquipItem += OnChangeEquipItemHandler;
            HotBarModel.OnChangeEquipCell += OnChangeEquipCellHandler;
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
            HotBarModel.OnChangeEquipItem -= OnChangeEquipItemHandler;
            HotBarModel.OnChangeEquipCell -= OnChangeEquipCellHandler;
        }

        private void OnChangeEquipItemHandler()
        {
            if (!HotBarModel.EquipCell.IsHasItem)
            {
                if (PlayerEventHandler.Aim.Active)
                {
                    PlayerEventHandler.Aim.ForceStop();
                }
            }
        }

        private void OnChangeEquipCellHandler()
        {
            if (PlayerEventHandler.Aim.Active)
            {
                PlayerEventHandler.Aim.ForceStop();
            }
        }
    }
}
