using Core;
using Core.Controllers;
using Game.Models;
using UltimateSurvival;

namespace Game.Controllers
{
    public class PlayerRunController : IPlayerRunController, IController
    {
        [Inject] public GameLateUpdateModel GameLateUpdateModel { get; private set; }
        [Inject] public PlayerStaminaModel PlayerStaminaModel { get; private set; }
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public PlayerRunModel PlayerRunModel { get; private set; }

        void IController.Enable()
        {
            PlayerRunModel.OnChangeIsRun += OnChangeIsRunHandler;
            GameLateUpdateModel.OnLaterUpdate += LateUpdate;
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
            PlayerRunModel.OnChangeIsRun -= OnChangeIsRunHandler;
            GameLateUpdateModel.OnLaterUpdate -= LateUpdate;
        }

        //Hack
        private void LateUpdate()
        {
            if (PlayerRunModel.IsRunToggle && !PlayerRunModel.IsRun)
            {
                PlayerRunModel.RunStart();
            }
        }

        private void OnChangeIsRunHandler()
        {
            if (!PlayerRunModel.IsRun)
            {
                if (!PlayerStaminaModel.IsHasStamina)
                {
                    PlayerRunModel.RunTogglePassive();
                }
            }
        }
    }
}
