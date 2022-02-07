using Core;
using Core.Controllers;
using Game.Views;

namespace Game.Controllers
{
    public class SettingsButtonViewController : ViewControllerBase<SettingsButtonView>
    {
        [Inject] public ViewsSystem ViewsSystem { get; private set; }

        protected override void Show()
        {
            View.OnDown += OnDownHandler;
        }

        protected override void Hide()
        {
            View.OnDown -= OnDownHandler;
        }

        private void OnDownHandler() => ViewsSystem.Show<SettingsView>(ViewConfigID.Settings);
    }
}
