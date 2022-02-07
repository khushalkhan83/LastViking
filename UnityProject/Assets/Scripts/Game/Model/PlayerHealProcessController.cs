using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using UnityEngine;

namespace Game.Controllers
{
    public class PlayerHealProcessController : IPlayerHealProcessController, IController
    {
        [Inject] public PlayerHealProcessModel PlayerHealProcessModel { get; private set; }
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }

        private bool IsHealProceesing { get; set; }

        void IController.Enable()
        {
            if (PlayerHealProcessModel.IsInitializeData)
            {
                StartHealProcessData();
            }
            else
            {
                PlayerHealProcessModel.OnLoadData += OnLoadDataHandler;
            }

            PlayerHealProcessModel.OnAddHeal += OnAddHealHandler;
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
            StopHealProcess();
            PlayerHealProcessModel.OnLoadData -= OnLoadDataHandler;
            PlayerHealProcessModel.OnAddHeal -= OnAddHealHandler;
            GameUpdateModel.OnUpdate -= HealProcessing;
        }

        private void OnLoadDataHandler() => StartHealProcessData();

        private void StartHealProcessData()
        {
            if (PlayerHealProcessModel.IsHasHeal)
            {
                StartHealProcess();
            }
        }

        private void OnAddHealHandler() => StartHealProcess();

        private void StartHealProcess()
        {
            if (!IsHealProceesing)
            {
                GameUpdateModel.OnUpdate += HealProcessing;
                IsHealProceesing = true;
            }
        }

        private void StopHealProcess()
        {
            PlayerHealProcessModel.EndHeal();
            GameUpdateModel.OnUpdate -= HealProcessing;
            IsHealProceesing = false;
        }

        private void HealProcessing()
        {
            ApplyHeal();
            PlayerHealProcessModel.UpdateHealValues(Time.deltaTime); 

            if (!PlayerHealProcessModel.IsHasHeal)
            {
                StopHealProcess();
            }
        }

        private void ApplyHeal()
        {
            foreach (var h in PlayerHealProcessModel.HealDatas)
            {
                var heal = PlayerHealProcessModel.GetHealValue(h, Time.deltaTime);
                var isOverflow = PlayerHealProcessModel.GetIsOverflow(h);
                PlayerHealthModel.AdjustHealth(heal, isOverflow);
            }
        }
    }
}
