using Core;
using Core.Controllers;
using Game.Models;
using Game.Models.RemoteSettings;
using UnityEngine;

namespace Game.Controllers
{
    public class PlayerThirstController : IPlayerThirstController, IController
    {
        [Inject] public RemoteSettingsModel RemoteSettingsModel { get; private set; }
        [Inject] public PlayerDeathModel PlayerDeathModel { get; private set; }
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public PlayerThirstModel PlayerThirstModel { get; private set; }
        [Inject] public PlayerWaterModel PlayerWaterModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public TutorialModel TutorialModel { get; private set; }

        public bool IsRunThirstProcess { get; private set; }

        private bool IsCanThirstProcess => PlayerWaterModel.WaterCurrent > 0
            && !PlayerDeathModel.IsImmunable
            && !PlayerHealthModel.IsDead;

        void IController.Enable()
        {
            PlayerWaterModel.OnChangeWater += OnChangeWaterHandler;
            PlayerThirstModel.OnChangeThirstProcess += OnChangeThirstProcessHandler;
            PlayerDeathModel.OnEndImunitet += OnEndImunitetHandler;
            PlayerHealthModel.OnDeath += OnDeathHandler;

            UpdateThirstProcess();
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
            PlayerWaterModel.OnChangeWater -= OnChangeWaterHandler;
            PlayerThirstModel.OnChangeThirstProcess -= OnChangeThirstProcessHandler;
            PlayerDeathModel.OnEndImunitet -= OnEndImunitetHandler;
            PlayerHealthModel.OnDeath -= OnDeathHandler;

            EndThirstProcess();
        }

        private void OnDeathHandler() => UpdateThirstProcess();
        private void OnEndImunitetHandler() => UpdateThirstProcess();

        private void OnChangeThirstProcessHandler()
        {
            if (PlayerThirstModel.IsThirstProcess)
            {
                BeginThirstProcess();
            }
            else
            {
                EndThirstProcess();
            }
        }

        private void OnChangeWaterHandler()
        {
            UpdateThirstProcess();

            { //??[REFACTOR]
                if (PlayerWaterModel.WaterCurrent > 0)
                {
                    PlayerWaterModel.StopThirst();
                }
                if (PlayerWaterModel.WaterCurrent <= PlayerWaterModel.WaterToThirst)
                {
                    PlayerWaterModel.StartThirst();
                }
            }
        }

        private void UpdateThirstProcess()
        {
            if (IsCanThirstProcess)
            {
                PlayerThirstModel.BeginThirst();
            }
            else
            {
                PlayerThirstModel.EndThirst();
            }
        }

        private void BeginThirstProcess()
        {
            if (!IsRunThirstProcess)
            {
                IsRunThirstProcess = true;
                GameUpdateModel.OnUpdate += ThirstProcess;
            }
        }

        private void EndThirstProcess()
        {
            if (IsRunThirstProcess)
            {
                IsRunThirstProcess = false;
                GameUpdateModel.OnUpdate -= ThirstProcess;
            }
        }

        private void ThirstProcess()
        {
            float adjustWater = RemoteSettingsModel.Get<PlayerThirstRemoteSettings>(RemoteSettingID.PlayerThirst).ThirstPerSecond * Time.deltaTime;

            if(!TutorialModel.IsComplete && PlayerWaterModel.Water - adjustWater < PlayerWaterModel.WaterToThirst)
                return;

            PlayerWaterModel.AdjustWater(-adjustWater);
        }
    }
}
