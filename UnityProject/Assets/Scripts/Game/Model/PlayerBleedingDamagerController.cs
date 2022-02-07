using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using UnityEngine;

namespace Game.Controllers
{
    public class PlayerBleedingDamagerController : IPlayerBleedingDamagerController, IController
    {
        [Inject] public PlayerBleedingDamagerModel PlayerBleedingDamagerModel { get; private set; }
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }

        void IController.Enable()
        {
            if (PlayerBleedingDamagerModel.IsInitializeData)
            {
                StartBleedingProcess();
            }
            else
            {
                PlayerBleedingDamagerModel.OnLoadData += OnLoadDataHandler;
            }

            PlayerBleedingDamagerModel.OnChangeHitFrom += OnChangeHitFromHandler;
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
            PlayerBleedingDamagerModel.OnLoadData -= OnLoadDataHandler;
            PlayerBleedingDamagerModel.OnChangeHitFrom -= OnChangeHitFromHandler;
            GameUpdateModel.OnUpdate -= OnUpdateHandler;
        }

        private void OnLoadDataHandler()
        {
            if (PlayerBleedingDamagerModel.IsBleeding)
            {
                StartBleedingProcess();
            }
        }

        private void StartBleedingProcess()
        {
            if (PlayerBleedingDamagerModel.IsBleeding && PlayerHealthModel.Health >= PlayerBleedingDamagerModel.LowerHealthThreshold)
            {
                GameUpdateModel.OnUpdate += OnUpdateHandler;
            }
        }

        private void StopBleedingProcess()
        {
            GameUpdateModel.OnUpdate -= OnUpdateHandler;
        }

        private void OnUpdateHandler()
        {
            PlayerBleedingDamagerModel.AdjustTimeDelay(-Time.deltaTime);
            if (PlayerBleedingDamagerModel.DamageTimeDelay <= 0)
            {
                PlayerHealthModel.AdjustHealth(-PlayerBleedingDamagerModel.Damage);
                PlayerBleedingDamagerModel.SetTimeDelayDefault();
            }
            if (PlayerHealthModel.Health <= PlayerBleedingDamagerModel.LowerHealthThreshold)
            {
                PlayerBleedingDamagerModel.SetBleeding(false);
            }
        }

        private void OnChangeHitFromHandler()
        {
            if (PlayerBleedingDamagerModel.IsBleeding && PlayerHealthModel.Health >= PlayerBleedingDamagerModel.LowerHealthThreshold)
            {
                StartBleedingProcess();
            }
            else
            {
                StopBleedingProcess();
            }
        }
    }
}
