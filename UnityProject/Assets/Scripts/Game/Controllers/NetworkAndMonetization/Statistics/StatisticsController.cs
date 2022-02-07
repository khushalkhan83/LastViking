using Core;
using Core.Controllers;
using Game.Models;
using UnityEngine;

namespace Game.Controllers
{
    public class StatisticsController : IStatisticsController, IController
    {
        [Inject] public PlayerDeathModel PlayerDeathModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public StatisticsModel StatisticsModel { get; private set; }
        [Inject] public GameTimeModel GameTimeModel { get; private set; }

        void IController.Enable()
        {
            PlayerDeathModel.OnRevival += OnRevivalPlayerHandler;
            GameUpdateModel.OnUpdate += OnUpdate;
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
            PlayerDeathModel.OnRevival -= OnRevivalPlayerHandler;
            GameUpdateModel.OnUpdate -= OnUpdate;
        }

        private void OnRevivalPlayerHandler()
        {
            StatisticsModel.SetStartAliveTimeTicks(GameTimeModel.GameDurationTicks);
            StatisticsModel.ResetSessionData();
        }

        private void OnUpdate()
        {
            StatisticsModel.AddSesionTime(Time.deltaTime);
        }
    }
}
