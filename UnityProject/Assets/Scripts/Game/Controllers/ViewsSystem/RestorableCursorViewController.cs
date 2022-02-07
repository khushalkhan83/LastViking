using Core;
using Core.Controllers;
using Game.Views;
using UltimateSurvival;

namespace Game.Controllers
{
    public class RestorableCursorViewController : ViewControllerBase<RestorableCursorView>
    {
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }

        protected RestorableObject RestorableObject { get; private set; }

        protected override void Show()
        {
            RestorableObject = PlayerEventHandler.RaycastData.Value.GameObject.GetComponent<RestorableObject>();
            RestorableObject.Health.OnChangeHealth += OnChangeHealthHandler;

            UpdateView();
        }

        protected override void Hide()
        {
            RestorableObject.Health.OnChangeHealth -= OnChangeHealthHandler;
        }

        private void OnChangeHealthHandler() => UpdateView();

        private void UpdateView() => UpdateView(RestorableObject.Health.Health, RestorableObject.Health.HealthMax);

        private void UpdateView(float health, float healthMax)
        {
            View.SetFillAmount(health / healthMax);
            View.SetText($"{health:F0}/{healthMax:F0}");
        }
    }
}
