using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using UnityEngine;

namespace Game.Controllers
{
    public class PlayerPoisonDamagerController : IPlayerPoisonDamagerController, IController
    {
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public PlayerPoisonDamagerModel PlayerPoisonDamagerModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }

        private bool IsPoisonProceesing { get; set; }

        void IController.Enable()
        {
            if (PlayerPoisonDamagerModel.IsInitializeData)
            {
                StartPoisonProcessData();
            }
            else
            {
                PlayerPoisonDamagerModel.OnLoadData += OnLoadDataHandler;
            }

            PlayerPoisonDamagerModel.OnAddPosion += OnAddPoisonHandler;
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
            StopPoisonProcess();
            PlayerPoisonDamagerModel.OnLoadData -= OnLoadDataHandler;
            PlayerPoisonDamagerModel.OnAddPosion -= OnAddPoisonHandler;
            GameUpdateModel.OnUpdate -= PoisonProcessing;
        }

        private void OnLoadDataHandler() => StartPoisonProcessData();

        private void StartPoisonProcessData()
        {
            if (PlayerPoisonDamagerModel.IsHasPoison)
            {
                StartPoisonProcess();
            }
        }

        private void OnAddPoisonHandler() => StartPoisonProcess();

        private void StartPoisonProcess()
        {
            if (!IsPoisonProceesing)
            {
                GameUpdateModel.OnUpdate += PoisonProcessing;
                IsPoisonProceesing = true;
            }
        }

        private void StopPoisonProcess()
        {
            PlayerPoisonDamagerModel.EndPoison();
            GameUpdateModel.OnUpdate -= PoisonProcessing;
            IsPoisonProceesing = false;
        }

        private void PoisonProcessing()
        {
            PlayerPoisonDamagerModel.AdjustDamageTimeDelay(-Time.deltaTime);
            if (PlayerPoisonDamagerModel.DamageTimeDelay <= 0)
            {
                GetDamage();
            }
        }

        private void GetDamage()
        {
            var poisonDamage = PlayerPoisonDamagerModel.GetPoisonDamage(PlayerPoisonDamagerModel.DamageCoolDown);
            PlayerHealthModel.AdjustHealth(-poisonDamage);

            PlayerPoisonDamagerModel.SetDamageTimeDelay(PlayerPoisonDamagerModel.DamageCoolDown);
            if (!PlayerPoisonDamagerModel.IsHasPoison)
            {
                StopPoisonProcess();
            }
        }
    }
}
