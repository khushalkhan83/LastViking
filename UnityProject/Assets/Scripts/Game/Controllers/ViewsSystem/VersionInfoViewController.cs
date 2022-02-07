using System.Collections.Generic;
using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using UnityEngine;

namespace Game.Controllers
{
    public class VersionInfoViewController : ViewControllerBase<VersionInfoView>
    {
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public GameTimeModel GameTimeModel { get; private set; }
        [Inject] public StatisticsModel StatisticsModel { get; private set; }
        [Inject] public QuestsModel QuestsModel { get; private set; }
        [Inject] public SheltersModel SheltersModel { get; private set; }

        private string QuestInfo => " Q: " + QuestsModel.ActiveQuest.name.Replace("SO_Quest_Main_","") + " ST: " + QuestsModel.ActiveStage + ";";
        private string ShipInfo => " SL: " + (SheltersModel.ShelterModel != null ? SheltersModel.ShelterModel.Level : 0);
        private string TimeInfo => " TS: " + GetGameTimeStatus();

        private float deltaSample;
        private float timeDelay = 0.6f;

        private string PlatformLable;

        protected override void Show()
        {
            GameUpdateModel.OnUpdate += OnUpdate;
            SetPlatformLable();
        }

        protected override void Hide()
        {
            GameUpdateModel.OnUpdate -= OnUpdate;
        }

        private void SetPlatformLable()
        {
            if(Application.platform == RuntimePlatform.IPhonePlayer)
                PlatformLable = "iOS EA";
            else if (Application.platform == RuntimePlatform.Android)
                PlatformLable = "Android EA";
            else
                PlatformLable = "EA";
        }

        private void OnUpdate()
        {
            if (timeDelay >= 0)
            {
                timeDelay -= Time.unscaledDeltaTime;
                return;
            }
            else
            {
                deltaSample += (Time.unscaledDeltaTime - deltaSample) * 0.05f;
                View.SetDescriptionText($"{PlatformLable}: v{Application.version} {1 / deltaSample:00}fps TD: {GameTimeModel.Days + 1}; CD: {GameTimeModel.GetDays(GameTimeModel.Ticks - StatisticsModel.StartAliveTimeTicks) + 1};" + QuestInfo + ShipInfo + TimeInfo);
            }
        }

        private string GetGameTimeStatus()
        {
            if(GameTimeModel.Scale == 1) return "OK";
            else if(GameTimeModel.Scale == 0) return "STOP";
            else return "NotOK";
        }
    }
}