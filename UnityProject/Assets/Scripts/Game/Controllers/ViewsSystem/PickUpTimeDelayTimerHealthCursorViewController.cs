using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using UltimateSurvival;

namespace Game.Controllers
{
    public class PickUpTimeDelayTimerHealthCursorViewController : ViewControllerBase<PickUpTimeDelayTimerHealthCursorView>
    {
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }

        protected ItemPickUpTimeDelayModel ItemPickUpTimeDelayModel { get; private set; }
        protected RestorableObject RestorableObject { get; private set; }

        protected override void Show()
        {
            ItemPickUpTimeDelayModel = PlayerEventHandler.RaycastData.Value.GameObject.GetComponentInParent<ItemPickUpTimeDelayModel>();
            View.SetFillAmount(ItemPickUpTimeDelayModel.RemainingTime / ItemPickUpTimeDelayModel.SpawnTime);

            GameUpdateModel.OnUpdate += OnUpdate;

            RestorableObject = PlayerEventHandler.RaycastData.Value.GameObject.GetComponent<RestorableObject>();
            RestorableObject.Health.OnChangeHealth += OnChangeHealthHandler;
            UpdateView();
        }

        protected override void Hide()
        {
            GameUpdateModel.OnUpdate -= OnUpdate;
            RestorableObject.Health.OnChangeHealth -= OnChangeHealthHandler;
        }

        private void OnUpdate() => View.SetFillAmount(ItemPickUpTimeDelayModel.RemainingTime / ItemPickUpTimeDelayModel.SpawnTime);

        private void OnChangeHealthHandler() => UpdateView();

        private void UpdateView() => UpdateView(RestorableObject.Health.Health, RestorableObject.Health.HealthMax);

        private void UpdateView(float health, float healthMax)
        {
            View.SetFillAmountHealth(health / healthMax);
            View.SetText($"{health:F0}/{healthMax:F0}");
        }
    }
}
