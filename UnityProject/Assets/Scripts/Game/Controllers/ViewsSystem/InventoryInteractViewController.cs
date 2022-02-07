using Core;
using Core.Controllers;
using Core.Views;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class InventoryInteractViewController : ViewControllerBase<InventoryInteractView>
    {
        [Inject] public InventoryButtonViewModel ViewModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public TutorialModel TutorialModel { get; private set; }

        protected override void Show()
        {
            View.OnClick += OnClickHandler;
            ViewModel.OnPulseAnimationChanged += HandlePulseAnimation;

            HandlePulseAnimation();
        }

        protected override void Hide()
        {
            View.OnClick -= OnClickHandler;
            ViewModel.OnPulseAnimationChanged -= HandlePulseAnimation;
        }

        private void HandlePulseAnimation()
        {
            if (ViewModel.PulseAnimation)
            {
                View.PlayPulse();
            }
            else
            {
                View.PlayDefault();
            }
        }

        private void OnClickHandler()
        {
            View.SetIsInteractable(false);
            ViewModel.SetPulseAnimation(false);

            var view = ViewsSystem.Show(ViewConfigID.InventoryPlayer);
            view.OnHide += OnHideInventoryPlayerView;
        }

        private void OnHideInventoryPlayerView(IView view)
        {
            view.OnHide -= OnHideInventoryPlayerView;
            View.SetIsInteractable(true);
        }
    }
}
