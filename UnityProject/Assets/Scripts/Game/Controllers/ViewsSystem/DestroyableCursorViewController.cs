using Core;
using Core.Controllers;
using Game.Views;
using UltimateSurvival;

namespace Game.Controllers
{
    public class DestroyableCursorViewController : ViewControllerBase<DestroyableCursorView>
    {
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }

        protected DestroyableObject CreatureObject { get; private set; }

        protected override void Show()
        {
            CreatureObject = PlayerEventHandler.RaycastData.Value.GameObject.GetComponent<DestroyableObject>();
            CreatureObject.Health.OnChangeHealth += OnChangeHealthHandler;

            UpdateView();
        }

        protected override void Hide()
        {
            CreatureObject.Health.OnChangeHealth -= OnChangeHealthHandler;
        }

        private void OnChangeHealthHandler() => UpdateView();

        private void UpdateView() => UpdateView(CreatureObject.Health.Health, CreatureObject.Health.HealthMax);

        private void UpdateView(float health, float healthMax) => View.SetFillAmount(health / healthMax);
    }
}
