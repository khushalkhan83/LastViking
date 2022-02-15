using System;
using Core;
using Core.Controllers;
using Game.Features.DailyRewards.Models;
using Game.Models;

namespace Game.Controllers
{
    public class TimeInGameController : ITimeInGameController, IController
    {
        [Inject] public TimeInGameModel TimeInGameModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public GameUnscaledTimeModel GameUnscaledTimeModel { get; private set; }
        [Inject] public ApplicationCallbacksModel ApplicationCallbacksModel { get; private set; }
        [Inject] public RealTimeModel RealTimeModel { get; private set; }
        [Inject] public CoroutineModel CoroutineModel { get; private set; }

        public void Enable()
        {
            GameUpdateModel.OnUpdate += OnUpdateHandler;

            ApplicationCallbacksModel.ApplicationFocus += ApplicationFocusHandler;
            ApplicationCallbacksModel.ApplicationPause += ApplicationPauseHandler;
            ApplicationCallbacksModel.ApplicationQuit += ApplicationQuitHandler;
            

            TimeInGameModel.StartTrackTime();
        }

        public void Start() { }

        public void Disable()
        {
            GameUpdateModel.OnUpdate -= OnUpdateHandler;

            ApplicationCallbacksModel.ApplicationFocus -= ApplicationFocusHandler;
            ApplicationCallbacksModel.ApplicationPause -= ApplicationPauseHandler;
            ApplicationCallbacksModel.ApplicationQuit -= ApplicationQuitHandler;

            TimeInGameModel.StopTrackTime();
        }


        private void OnUpdateHandler()
        {
            if (TimeInGameModel.IsTrackTime)
            {
                TimeInGameModel.AdjustTimeInGame(GameUnscaledTimeModel.DeltaTime);
            }
        }

        private void ApplicationFocusHandler(bool hasFocus)
        {
            if (hasFocus)
            {
                TimeInGameModel.StartTrackTime();
            }
            else
            {
                TimeInGameModel.StopTrackTime();
            }
        }

        private void ApplicationPauseHandler(bool isPause)
        {
            if (isPause)
            {
                TimeInGameModel.StopTrackTime();
            }
            else
            {
                TimeInGameModel.StartTrackTime();
            }
        }

        private void ApplicationQuitHandler()
        {
            TimeInGameModel.StopTrackTime();
        }
    }
}
