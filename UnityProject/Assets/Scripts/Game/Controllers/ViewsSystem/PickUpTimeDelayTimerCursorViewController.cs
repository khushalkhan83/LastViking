using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using UltimateSurvival;

namespace Game.Controllers
{
    public class PickUpTimeDelayTimerCursorViewController : ViewControllerBase<PickUpTimeDelayTimerCursorView>
    {
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }

        protected ItemPickUpTimeDelayModel ItemPickUpTimeDelayModel { get; private set; }

        protected override void Show()
        {
            ItemPickUpTimeDelayModel = PlayerEventHandler.RaycastData.Value.GameObject.GetComponentInParent<ItemPickUpTimeDelayModel>();
            View.SetFillAmount(ItemPickUpTimeDelayModel.RemainingTime / ItemPickUpTimeDelayModel.SpawnTime);

            GameUpdateModel.OnUpdate += OnUpdate;
        }

        protected override void Hide()
        {
            GameUpdateModel.OnUpdate -= OnUpdate;
        }

        private void OnUpdate() => View.SetFillAmount(ItemPickUpTimeDelayModel.RemainingTime / ItemPickUpTimeDelayModel.SpawnTime);
    }
}
