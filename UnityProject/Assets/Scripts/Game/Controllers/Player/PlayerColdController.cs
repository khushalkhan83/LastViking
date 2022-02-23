using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using UnityEngine;

namespace Game.Controllers
{
    public class PlayerColdController : IPlayerColdController, IController
    {
        [Inject] public PlayerMovementModel PlayerMovementModel { get; private set; }
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public PlayerDeathModel PlayerDeathModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public PlayerWarmModel PlayerWarmModel { get; private set; }
        [Inject] public StorageModel StorageModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public TutorialModel TutorialModel { get; private set; }

        public ColdStatusView ColdView { set; get; }

        private bool IsCanColdingProcess => !PlayerDeathModel.IsImmunable && !PlayerHealthModel.IsDead && !TutorialModel.IsTutorialNow;

        private bool isRunColdingProcess = false;

        void IController.Enable()
        {
            StorageModel.TryProcessing(PlayerWarmModel._Data);
            PlayerWarmModel.OnChangeWarm += OnChangeWarm;
            PlayerDeathModel.OnEndImunitet += UpdateColdingProcess;
            PlayerHealthModel.OnDeath += UpdateColdingProcess;
            UpdateColdingProcess();
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
            PlayerWarmModel.OnChangeWarm -= OnChangeWarm;
            PlayerDeathModel.OnEndImunitet -= UpdateColdingProcess;
            PlayerHealthModel.OnDeath -= UpdateColdingProcess;
        }

        private void OnChangeWarm()
        {
            if(PlayerWarmModel.Warm < PlayerWarmModel.WarmToFreeze)
            {
                PlayerWarmModel.StartColding();
            }
            else
            {
                PlayerWarmModel.StopColding();
            }
        }

        private void UpdateColdingProcess()
        {
            if(IsCanColdingProcess)
            {
                StatColdingProcess();
            }
            else
            {
                StopColdingProcess();
            }
        }

        private void StatColdingProcess()
        {
            if(!isRunColdingProcess)
            {
                GameUpdateModel.OnUpdate += ColdingProcess;
                isRunColdingProcess = true;
            }
        }

        private void StopColdingProcess()
        {
            if(isRunColdingProcess)
            {
                GameUpdateModel.OnUpdate -= ColdingProcess;
                isRunColdingProcess = false;
            }
        }

        private void ColdingProcess()
        {
            bool isColding = PlayerWarmModel.IsInColdZone && !PlayerWarmModel.IsInWarmZone;

            if(isColding && PlayerWarmModel.Warm > 0)
                PlayerWarmModel.AdjustWarm(-PlayerWarmModel.ColdPerSecond * Time.deltaTime);
            else if(!isColding && PlayerWarmModel.Warm < PlayerWarmModel.MaxWarm)
                PlayerWarmModel.AdjustWarm(PlayerWarmModel.WarmPerSecond * Time.deltaTime);
        }
    }
}
