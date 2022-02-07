using Core;
using Core.Controllers;
using Game.Views;
using UltimateSurvival;

namespace Game.Controllers
{
    public class DotCrosshairViewController : ViewControllerBase<DotCrosshairView>
    {
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }

        protected override void Show()
        {
            PlayerEventHandler.RaycastData.OnChange += OnChangeRaycastDataHandler;

            UpdateView();
        }

        protected override void Hide()
        {
            PlayerEventHandler.RaycastData.OnChange -= OnChangeRaycastDataHandler;
        }

        private void OnChangeRaycastDataHandler() => UpdateView();

        private void UpdateView()
        {
            var isActive = PlayerEventHandler.RaycastData.Value?.GameObject?.GetComponent<HitBox>() != null;
            if (isActive)
            {
                View.SetColorCrosshair(View.ActiveColor);
            }
            else
            {
                View.SetColorCrosshair(View.PassiveColor);
            }
        }
    }
}
