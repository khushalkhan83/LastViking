using Core;
using Core.Controllers;
using Game.Audio;
using Game.Models;
using UnityEngine;

namespace Game.Controllers
{
    public class PlayerHungerDamagerController : IPlayerHungerDamagerController, IController
    {
        [Inject] public PlayerHungerDamagerModel PlayerHungerDamagerModel { get; private set; }
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public PlayerAudioModel PlayerAudioModel { get; private set; }
        [Inject] public PlayerDeathModel PlayerDeathModel { get; private set; }
        [Inject] public PlayerFoodModel PlayerFoodModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public GameTimeModel GameTimeModel { get; private set; }
        [Inject] public StorageModel StorageModel { get; private set; }
        [Inject] public AudioSystem AudioSystem { get; private set; }

        private bool IsDamageProcess { get; set; }

        private bool IsCanDamageProcess => !PlayerHealthModel.IsDead
                        && !PlayerDeathModel.IsImmunable
                        && PlayerFoodModel.FoodCurrent <= 0;

        void IController.Enable()
        {
            StorageModel.TryProcessing(PlayerHungerDamagerModel._Data);

            PlayerHungerDamagerModel.OnDamage += OnDamageHandler;
            PlayerDeathModel.OnEndImunitet += OnEndImunitetPlayerHanler;
            PlayerFoodModel.OnChangeFood += OnChangeFoodHandler;
            PlayerHealthModel.OnDeath += OnDeathPlayerHandler;

            if (IsCanDamageProcess)
            {
                BeginHungerDamageProcess();
            }
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
            PlayerHungerDamagerModel.OnDamage -= OnDamageHandler;
            PlayerDeathModel.OnEndImunitet -= OnEndImunitetPlayerHanler;
            PlayerFoodModel.OnChangeFood -= OnChangeFoodHandler;
            PlayerHealthModel.OnDeath -= OnDeathPlayerHandler;

            EndHungerDamageProcess();
        }

        private void OnEndImunitetPlayerHanler() => UpdateDamageProcess();
        private void OnDeathPlayerHandler() => UpdateDamageProcess();
        private void OnChangeFoodHandler() => UpdateDamageProcess();

        private void UpdateDamageProcess()
        {
            if (IsCanDamageProcess)
            {
                BeginHungerDamageProcess();
            }
            else
            {
                EndHungerDamageProcess();
            }
        }

        private void BeginHungerDamageProcess()
        {
            if (!IsDamageProcess)
            {
                IsDamageProcess = true;
                GameUpdateModel.OnUpdate += DamageProcess;
            }
        }

        private void EndHungerDamageProcess()
        {
            if (IsDamageProcess)
            {
                IsDamageProcess = false;
                GameUpdateModel.OnUpdate -= DamageProcess;
            }
        }

        private void DamageProcess() => PlayerHungerDamagerModel.ProcessDamage(Time.deltaTime);

        private void OnDamageHandler()
        {
            AudioSystem.PlayOnce(PlayerAudioModel.AudioIDDamageRandomly);
            PlayerHealthModel.AdjustHealth(-PlayerHungerDamagerModel.HealthAdjustment);
        }
    }
}
