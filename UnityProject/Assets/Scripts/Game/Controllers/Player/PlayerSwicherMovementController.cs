using Core;
using Core.Controllers;
using Game.Models;
using UltimateSurvival;

namespace Game.Controllers
{
    public class PlayerSwicherMovementController : IPlayerSwicherMovementController, IController
    {
        [Inject] public PlayerMovementModel PlayerMovementModel { get; private set; }
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public CoreEnvironmentModel CoreEnvironmentModel { get; private set; }

        void IController.Enable()
        {
            GameUpdateModel.OnUpdate += Update;
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
            GameUpdateModel.OnUpdate -= Update;
        }

        private void Update()
        {
            bool playerInWater = CoreEnvironmentModel.WaterActive && PlayerEventHandler.transform.position.y <= PlayerMovementModel.WaterLevel;

            if (playerInWater)
            {
                if (PlayerMovementModel.MovementID != PlayerMovementID.Water)
                {
                    PlayerMovementModel.SetMevementID(PlayerMovementID.Water);
                }
            }
            else
            {
                if (PlayerMovementModel.MovementID != PlayerMovementID.Ground)
                {
                    PlayerMovementModel.SetMevementID(PlayerMovementID.Ground);
                }
            }
        }
    }
}
