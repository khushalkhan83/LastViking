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
        [Inject] public ViewsStateModel ViewsStateModel { get; private set; }
        [Inject] public CinematicModel CinematicModel { get; private set; }
        [Inject] public GameUnscaledTimeModel GameUnscaledTimeModel { get; private set; }

        private EditorGameSettings EditorGameSettings => EditorGameSettings.Instance;

        void IController.Enable()
        {
            HandleBug_EnvironmentTimeBigerThenRegularTime();
            GameUpdateModel.OnUpdate += OnUpdate;
            ApplicationCallbacksModel.ApplicationFocus += OnApplicationFocus;
            ApplicationCallbacksModel.ApplicationPause += OnApplicationPause;
            ViewsStateModel.OnIsHUDBlockedChanged += OnIsHUDBlockedChanged;
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
            GameUpdateModel.OnUpdate -= OnUpdate;
            ApplicationCallbacksModel.ApplicationFocus -= OnApplicationFocus;
            ApplicationCallbacksModel.ApplicationPause -= OnApplicationPause;
            ViewsStateModel.OnIsHUDBlockedChanged -= OnIsHUDBlockedChanged;
        }

        // TODO: split on 2 & make sub + unsub
        private void OnUpdate()
        {
            GameTimeModel.UpdateProcess(GameUnscaledTimeModel.DeltaTime);

            if (!GameTimeModel.IsEnviroTimePaused)
                GameTimeModel.EnviroUpdateProcess(GameUnscaledTimeModel.DeltaTime);
        }

        private void OnIsHUDBlockedChanged()
        {
            bool pause = ViewsStateModel.IsHUDBlocked;

            // Don't pause game in cinematics
            if (pause && CinematicModel.CinematicStarted) return;

            GameTimeModel.SetGameOnPause(pause);
        }

        private void OnApplicationFocus(bool isHasfocus)
        {
            if (!EditorGameSettings.PauseGameOnRefocus) return;

            if (!ViewsStateModel.IsHUDBlocked)
            {
                GameTimeModel.SetGameOnPause(!isHasfocus);
            }
        }

        private void OnApplicationPause(bool isPause)
        {
            if (!ViewsStateModel.IsHUDBlocked)
            {
                GameTimeModel.SetGameOnPause(isPause);
            }
            //UpdateGamePause(!isPause);
        }

        // private void UpdateGamePause(bool isInGame)
        // {
        //     if (GameTimeModel.IsOutGame && isInGame)
        //     {
        //         GameTimeModel.InGame();
        //     }

        //     if (GameTimeModel.IsInGame && !isInGame)
        //     {
        //         GameTimeModel.OutGame();
        //     }
        // }

        private void HandleBug_EnvironmentTimeBigerThenRegularTime()
        {
            //FIXME: for example, if GameTimeModel.EnviroDays became very big number (12345325), then skeletons waves will stop to spawn
            if (GameTimeModel.EnviroTicks > GameTimeModel.Ticks)
            {
                GameTimeModel.EnviroTicks = GameTimeModel.Ticks;
            }
        }
    }
}
