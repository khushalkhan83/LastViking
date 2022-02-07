using Core;
using Core.Controllers;
using Game.Views;
using UltimateSurvival;
using Extensions;
using Game.Models;

namespace Game.Controllers
{
    public class OpenLootCursorHealthViewController : ViewControllerBase<OpenLootHealthCursorView>
    {
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }

        protected RestorableObject RestorableObject { get; private set; }

        protected override void Show()
        {
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

        private void OnUpdate()
        {
            if(PlayerInput.Instance.AttackTap)
            {
                OnDownHandler();
            }
        }

        private void OnDownHandler()
        {
            var lootObject = PlayerEventHandler.RaycastData.Value?.GameObject.CheckNull()?.GetComponent<LootObject>();
            if(lootObject != null)
                lootObject.Open();
        }

        private void OnChangeHealthHandler() => UpdateView();

        private void UpdateView() => UpdateView(RestorableObject.Health.Health, RestorableObject.Health.HealthMax);

        private void UpdateView(float health, float healthMax)
        {
            View.SetHealthFillAmount(health / healthMax);
            View.SetHealthText($"{health:F0}/{healthMax:F0}");
        }
    }
}
