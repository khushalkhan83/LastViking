using Core;
using Core.Controllers;
using Game.Audio;
using Game.Models;
using UnityEngine;

namespace Game.Controllers
{
    public class PlayerThirstDamagerController : IPlayerThirstDamagerController, IController
    {
        [Inject] public PlayerThirstDamagerModel PlayerThirstDamagerModel { get; private set; }
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public PlayerAudioModel PlayerAudioModel { get; private set; }
        [Inject] public PlayerDeathModel PlayerDeathModel { get; private set; }
        [Inject] public PlayerWaterModel PlayerWaterModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public StorageModel StorageModel { get; private set; }
        [Inject] public AudioSystem AudioSystem { get; private set; }

        private bool IsBeginDamageProcess => !PlayerHealthModel.IsDead
                       && PlayerDeathModel.ImmunitetRemaining <= 0
                       && PlayerWaterModel.WaterCurrent <= 0;

        public bool IsDamageProcess { get; private set; }

        void IController.Enable()
        {
            StorageModel.TryProcessing(PlayerThirstDamagerModel._Data);

            PlayerThirstDamagerModel.OnDamage += OnDamageHandler;
            PlayerWaterModel.OnChangeWater += OnChangeWaterHandler;
            PlayerDeathModel.OnEndImunitet += OnEndImunitetPlayerHanler;
            PlayerHealthModel.OnDeath += OnDeathPlayerHandler;

            if (IsBeginDamageProcess)
            {
                BeginThirstDamageProcess();
            }
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
            PlayerThirstDamagerModel.OnDamage -= OnDamageHandler;

            PlayerWaterModel.OnChangeWater -= OnChangeWaterHandler;
            PlayerDeathModel.OnEndImunitet -= OnEndImunitetPlayerHanler;
            PlayerHealthModel.OnDeath -= OnDeathPlayerHandler;

            EndThirstDamageProcess();
        }

        private void OnEndImunitetPlayerHanler() => UpdateThirstProcess();
        private void OnDeathPlayerHandler() => UpdateThirstProcess();
        private void OnChangeWaterHandler() => UpdateThirstProcess();

        private void UpdateThirstProcess()
        {
            if (IsBeginDamageProcess)
            {
                BeginThirstDamageProcess();
            }
            else
            {
                EndThirstDamageProcess();
            }
        }

        private void BeginThirstDamageProcess()
        {
            if (!IsDamageProcess)
            {
                IsDamageProcess = true;
                GameUpdateModel.OnUpdate += DamageProcess;
            }
        }

        private void EndThirstDamageProcess()
        {
            if (IsDamageProcess)
            {
                IsDamageProcess = false;
                GameUpdateModel.OnUpdate -= DamageProcess;
            }
        }

        private void DamageProcess() => PlayerThirstDamagerModel.ProcessDamage(Time.deltaTime);

        private void OnDamageHandler()
        {
            AudioSystem.PlayOnce(PlayerAudioModel.AudioIDDamageRandomly);
            PlayerHealthModel.AdjustHealth(-PlayerThirstDamagerModel.HealthAdjustment);
        }
    }
}
