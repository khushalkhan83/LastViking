using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class DebugTimeInteractViewController : ViewControllerBase<DebugTimeInteractView>
    {
        [Inject] public DebugTimeInteractViewModel DebugTimeInteractViewModel { get; private set; }

        protected override void Show()
        {
            View.OnSwitchVisible += OnSwitchVisibleHandler;
        }

        protected override void Hide()
        {
            View.OnSwitchVisible -= OnSwitchVisibleHandler;
        }

        private void OnSwitchVisibleHandler()
        {
            if (DebugTimeInteractViewModel.IsCanVisibleDebug)
            {
                DebugTimeInteractViewModel.HideDebug();
            }
            else
            {
                DebugTimeInteractViewModel.ShowDebug();
            }
        }
    }
}
