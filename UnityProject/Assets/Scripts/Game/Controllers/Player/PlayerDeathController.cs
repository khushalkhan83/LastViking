using Core;
using Core.Controllers;
using Game.Models;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers
{
    public class PlayerDeathController : IPlayerDeathController, IController
    {
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public PlayerDeathModel PlayerDeathModel { get; private set; }
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }

        void IController.Enable()
        {
            PlayerHealthModel.OnDeath += OnPlayerDeath;

            if (PlayerDeathModel.IsImmunable)
            {
                PlayerDeathModel.OnEndImunitet += OnEndImunitetHandler;
                GameUpdateModel.OnUpdate += ImunitetProcess;
            }
            else
            {
                PlayerDeathModel.OnBeginImunitet += OnBeginImunitetHandler;
            }
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
            PlayerHealthModel.OnDeath -= OnPlayerDeath;
            PlayerDeathModel.OnBeginImunitet -= OnBeginImunitetHandler;
            PlayerDeathModel.OnEndImunitet -= OnEndImunitetHandler;
            GameUpdateModel.OnUpdate -= ImunitetProcess;
        }

        private void OnPlayerDeath()
        {
            PlayerEventHandler.ResetImpact();
        }

        private void OnEndImunitetHandler()
        {
            PlayerDeathModel.OnEndImunitet -= OnEndImunitetHandler;
            GameUpdateModel.OnUpdate -= ImunitetProcess;
            PlayerDeathModel.OnBeginImunitet += OnBeginImunitetHandler;
        }

        private void OnBeginImunitetHandler()
        {
            PlayerDeathModel.OnBeginImunitet -= OnBeginImunitetHandler;
            PlayerDeathModel.OnEndImunitet += OnEndImunitetHandler;
            GameUpdateModel.OnUpdate += ImunitetProcess;
        }

        private void ImunitetProcess()
        {
            PlayerDeathModel.ImmunitetProcess(Time.deltaTime);
        }
    }
}
