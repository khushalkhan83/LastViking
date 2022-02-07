using Core;
using Core.Controllers;
using Game.Models;
using UnityEngine;

namespace Game.Controllers
{
    public class GameTimeController : IGameTimeController, IController
    {
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public GameTimeModel GameTimeModel { get; private set; }
        [Inject] public ApplicationCallbacksModel ApplicationCallbacksModel { get; private set; }

        private EditorGameSettings EditorGameSettings => EditorGameSettings.Instance;

        void IController.Enable()
        {
            HandleBug_EnvironmentTimeBigerThenRegularTime();
            GameUpdateModel.OnUpdate += OnUpdate;
            ApplicationCallbacksModel.ApplicationFocus += OnApplicationFocus;
            ApplicationCallbacksModel.ApplicationPause += OnApplicationPause;
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
            GameUpdateModel.OnUpdate -= OnUpdate;
            ApplicationCallbacksModel.ApplicationFocus -= OnApplicationFocus;
            ApplicationCallbacksModel.ApplicationPause -= OnApplicationPause;
        }

        // TODO: split on 2 & make sub + unsub
        private void OnUpdate()
        {
            GameTimeModel.UpdateProcess(Time.unscaledDeltaTime);

            if (!GameTimeModel.IsEnviroTimePaused)
                GameTimeModel.EnviroUpdateProcess(Time.unscaledDeltaTime);
        }

        private void OnApplicationFocus(bool isHasfocus)
        {
            if(!EditorGameSettings.PauseGameOnRefocus) return;
            
            UpdateGamePause(isHasfocus);
        }

        private void OnApplicationPause(bool isPause) => UpdateGamePause(!isPause);

        private void UpdateGamePause(bool isInGame)
        {
            if (GameTimeModel.IsOutGame && isInGame)
            {
                GameTimeModel.InGame();
            }

            if (GameTimeModel.IsInGame && !isInGame)
            {
                GameTimeModel.OutGame();
            }
        }

        private void HandleBug_EnvironmentTimeBigerThenRegularTime()
        {
            //FIXME: for example, if GameTimeModel.EnviroDays became very big number (12345325), then skeletons waves will stop to spawn
            if(GameTimeModel.EnviroTicks > GameTimeModel.Ticks)
            {
                GameTimeModel.EnviroTicks = GameTimeModel.Ticks;
            }
        }
    }
}
