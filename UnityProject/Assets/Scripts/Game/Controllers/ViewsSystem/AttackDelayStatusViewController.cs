using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class AttackDelayStatusViewController : ViewControllerBase<AttackDelayStatusView>
    {
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public GameTimeModel GameTimeModel { get; private set; }
        [Inject] public AttackDelayStatusViewModel AttackDelayStatusViewModel { get; private set; }
        [Inject] public SkeletonSpawnManager SkeletonSpawnManager { get; private set; }

        private SkeletonSpawnManager.SessionSettings NextSession => SkeletonSpawnManager.Sessions[SkeletonSpawnManager.SessionId + 1];

        protected override void Show()
        {
            GameUpdateModel.OnUpdate += OnUpdate;
        }

        private string TimeToString(float time) => System.TimeSpan.FromSeconds(time).ToString(@"mm\:ss");

        private void OnUpdate()
        {
            var leftTime = AttackDelayStatusViewModel.TargetTime - GameTimeModel.EnviroTimeOfDay;
            View.SetLeftTimeText(TimeToString(60 * leftTime));
        }

        protected override void Hide()
        {
            GameUpdateModel.OnUpdate -= OnUpdate;
        }
    }
}
