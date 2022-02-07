using Core;
using Core.Controllers;
using Game.Audio;
using Game.Interactables;
using Game.Models;
using Game.Models.RemoteSettings;
using Game.Purchases;
using Game.Views;
using System.Linq;
using UltimateSurvival;
using Game.Progressables;

namespace Game.Controllers
{
    public class ProgressCursorViewController : ViewControllerBase<ProgressCursorView>
    {
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public DungeonsProgressModel DungeonsProgressModel { get; private set; }
        [Inject] public LocalizationModel LocalizationModel { get; private set; }

        private DungeonEnter DungeonEnter { get; set; }
        private DungeonProgressModel DungeonProgressModel { get; set; }

        protected override void Show()
        {
            View.OnInteract += OnDownHandler;

            DungeonEnter = PlayerEventHandler.RaycastData.Value.GameObject.GetComponentInParent<DungeonEnter>();
            if(DungeonEnter != null)
            {
                DungeonProgressModel = DungeonsProgressModel.GetDungeonProgressModel(DungeonEnter.SceneID);
            }

            DungeonProgressModel.OnProgressResetted += UpdateViewState;

            UpdateViewState();
        }

        protected override void Hide()
        {
            View.OnInteract -= OnDownHandler;
            GameUpdateModel.OnUpdate -= OnUpdate;
            if(DungeonProgressModel != null)
                DungeonProgressModel.OnProgressResetted -= UpdateViewState;

            DungeonEnter = null;
            DungeonProgressModel = null;
        }

        private void ShowTimer()
        {
            View.SetTimerVisible(true);
            View.SetCursorVisible(false);
            GameUpdateModel.OnUpdate += OnUpdate;
        }

        private void HideTimer()
        {
            View.SetTimerVisible(false);
            View.SetCursorVisible(true);
            GameUpdateModel.OnUpdate -= OnUpdate;
        }

        private void UpdateViewState()
        {
            if (DungeonProgressModel != null && DungeonProgressModel.ProgressStatus == ProgressStatus.WaitForResetProgress)
            {
                if (DungeonEnter != null) ShowTimer();
            }
            else
            {
                HideTimer();
            }
        }

        private void OnUpdate()
        { 
            if(DungeonProgressModel != null)
            {
                var remainingTime = DungeonProgressModel.GetProgressRemainingTime();
                View.SetFillAmount(remainingTime / DungeonProgressModel.ProgressResetTime);
                View.SetTimerText(TimeToString(remainingTime));
            }
        }

        private string TimeToString(float time)
        {
            System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(time);
            if (timeSpan.Hours == 0)
            {
                return string.Format(LocalizationModel.GetString(LocalizationKeyID.ObjectivesMenu_MinSec), timeSpan.Minutes, timeSpan.Seconds);
            }
            return string.Format(LocalizationModel.GetString(LocalizationKeyID.ObjectivesMenu_HourMin), timeSpan.Hours, timeSpan.Minutes);
        }

        private void OnDownHandler() => TryInteract();

        private void TryInteract()
        {
            if(DungeonProgressModel.ProgressStatus == ProgressStatus.WaitForResetProgress) return;

            DungeonEnter?.ManualOpen();
        }
    }
}