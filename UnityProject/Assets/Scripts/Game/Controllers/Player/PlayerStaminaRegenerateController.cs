using Core;
using Core.Controllers;
using Game.Audio;
using Game.Models;
using UnityEngine;

namespace Game.Controllers
{
    public class PlayerStaminaRegenerateController : IPlayerStaminaRegenerateController, IController
    {
        [Inject] public PlayerStaminaRegenerateModel PlayerStaminaRegenerateModel { get; private set; }
        [Inject] public PlayerStaminaModel PlayerStaminaModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public PlayerRunModel PlayerRunModel { get; private set; }
        [Inject] public AudioSystem AudioSystem { get; private set; }

        private bool IsRegenerateProcess { get; set; }

        private bool IsCanRegenerateProcess => !(PlayerRunModel.IsRun || (!PlayerRunModel.IsRun && PlayerStaminaModel.IsFullStamina));

        void IController.Enable()
        {
            PlayerRunModel.OnChangeIsRun += OnChangeIsRunHandler;
            PlayerStaminaModel.OnChangeStamina += OnChangeStaminaHandler;

            if (IsCanRegenerateProcess)
            {
                BeginRegenerateProcess();
            }
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
            PlayerRunModel.OnChangeIsRun -= OnChangeIsRunHandler;
            PlayerStaminaModel.OnChangeStamina -= OnChangeStaminaHandler;

            EndRegenerateProcess();
        }

        private void OnChangeStaminaHandler() => UpdateRegenerateProcess();

        private void UpdateRegenerateProcess()
        {
            if (IsCanRegenerateProcess)
            {
                BeginRegenerateProcess();
            }
            else
            {
                EndRegenerateProcess();
            }
        }

        private void BeginRegenerateProcess()
        {
            if (!IsRegenerateProcess)
            {
                IsRegenerateProcess = true;
                GameUpdateModel.OnUpdate += RegenerateStaminaProcess;
            }
        }

        private void EndRegenerateProcess()
        {
            if (IsRegenerateProcess)
            {
                IsRegenerateProcess = false;
                GameUpdateModel.OnUpdate -= RegenerateStaminaProcess;
            }
        }

        private void OnChangeIsRunHandler()
        {
            UpdateRegenerateProcess();

            {//??[]REFACTOR
                if (!PlayerRunModel.IsRun && PlayerStaminaModel.Stamina == 0)
                {
                    AudioSystem.PlayOnce(AudioID.PlayerBreathing);
                }
            }
        }

        private void RegenerateStaminaProcess() => PlayerStaminaModel.AdjustStamina(PlayerStaminaRegenerateModel.GetRegeneration(Time.deltaTime));
    }
}
