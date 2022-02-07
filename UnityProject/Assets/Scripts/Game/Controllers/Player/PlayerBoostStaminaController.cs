using Core;
using Core.Controllers;
using Game.Models;
using UnityEngine;

namespace Game.Controllers
{
    public class PlayerBoostStaminaController : IPlayerBoostStaminaController, IController
    {
        [Inject] public PlayerStaminaModel PlayerStaminaModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }

        void IController.Enable()
        {
            PlayerStaminaModel.OnBeginBoost += OnBeginBoostHandler;
            PlayerStaminaModel.OnEndBoost += OnEndBoostHandler;

            if (PlayerStaminaModel.IsBoost)
            {
                BeginBoostProcess();
            }
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
            PlayerStaminaModel.OnBeginBoost -= OnBeginBoostHandler;
            PlayerStaminaModel.OnEndBoost -= OnEndBoostHandler;

            if (PlayerStaminaModel.IsBoost)
            {
                EndBoostProcess();
            }
        }

        private void OnBeginBoostHandler() => BeginBoostProcess();

        private void OnEndBoostHandler() => EndBoostProcess();

        private void BeginBoostProcess() => GameUpdateModel.OnUpdate += BoostProcess;
        private void EndBoostProcess() => GameUpdateModel.OnUpdate -= BoostProcess;

        private void BoostProcess() => PlayerStaminaModel.BoostProcessing(Time.deltaTime);
    }
}
