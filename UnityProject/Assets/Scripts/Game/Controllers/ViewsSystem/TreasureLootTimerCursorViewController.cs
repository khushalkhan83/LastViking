using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using UltimateSurvival;

namespace Game.Controllers
{
    public class TreasureLootTimerCursorViewController : ViewControllerBase<TreasureLootTimerCursorView>
    {
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public GameTimeModel GameTimeModel { get; private set; }

        protected TreasureLootObject TreasureLootObject { get; private set; }

        protected override void Show()
        {
            TreasureLootObject = PlayerEventHandler.RaycastData.Value?.GameObject?.GetComponent<TreasureLootObject>();
            if (TreasureLootObject != null)
            {
                View.SetFillAmount(GetRemainingTimeSec() / TreasureLootObject.RespawnSec);
                GameUpdateModel.OnUpdate += OnUpdate;
            }
            else 
            {
                UnityEngine.Debug.LogError("There is no TreasureLootObject component!");
            }
        }

        protected override void Hide()
        {
            GameUpdateModel.OnUpdate -= OnUpdate;
        }

        private void OnUpdate()
        {
            View.SetFillAmount(GetRemainingTimeSec() / TreasureLootObject.RespawnSec);
            View.SetTimerText(TimeToString(GetRemainingTimeSec()));
        }

        private float GetRemainingTimeSec() {
            long remainingTicks = TreasureLootObject.TimeSpawnTicks - GameTimeModel.RealTimeNowTick;
            return GameTimeModel.GetSecondsTotal(remainingTicks);
        }

        private string TimeToString(float time) => System.TimeSpan.FromSeconds(time).ToString(@"mm\:ss");

    }
}
