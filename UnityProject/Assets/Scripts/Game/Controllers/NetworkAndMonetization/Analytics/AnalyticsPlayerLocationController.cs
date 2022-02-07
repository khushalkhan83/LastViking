using Core;
using Core.Controllers;
using Game.Models;
using UnityEngine;

namespace Game.Controllers
{
    public class AnalyticsPlayerLocationController : IAnalyticsPlayerLocationController, IController
    {
        [Inject] public AnalyticsPlayerLocationModel AnalyticsPlayerLocationModel {get;set;}
        [Inject] public PlayerLocationModel PlayerLocationModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel {get;set;}
        [Inject] public PlayerScenesModel PlayerScenesModel {get;set;}

        private float lastSendEventTime = 0;

        void IController.Enable() 
        {
            GameUpdateModel.OnUpdate += OnUpdate;
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            GameUpdateModel.OnUpdate -= OnUpdate;
        }

        private void OnUpdate()
        {
            if(PlayerScenesModel.SceneLoading) return;
            
            if(Time.time - lastSendEventTime > AnalyticsPlayerLocationModel.SendEventIntervalTime)
            {
                var locationId = PlayerLocationModel.GetLocation().locationId;
                AnalyticsPlayerLocationModel.SendEvent(locationId);
                lastSendEventTime = Time.time;
            }
        }

    }
}
