using Game.Views;
using Core.Controllers;
using Game.Models;
using Core;

namespace Game.Controllers
{
    public class ActivitiesLogButtonViewController : ViewControllerBase<ActivitiesLogButtonView>
    {
        [Inject] public ActivitiesLogButtonViewModel ViewModel { get; private set; }

        protected override void Show() 
        {
            View.OnClick += ClickHandler;
            ViewModel.OnCounterChanged += UpdateView;
            ViewModel.OnIsPulseAnimationChanged += UpdateView;

            UpdateView();
        }

        protected override void Hide() 
        {
            View.OnClick -= ClickHandler;
            ViewModel.OnCounterChanged -= UpdateView;
            ViewModel.OnIsPulseAnimationChanged -= UpdateView;
        }

        private void UpdateView()
        {
            View.Counter = ViewModel.Counter.ToString();
            View.ShowCounter(ViewModel.ShowCounter);
            View.SetPlayPulseAnimation(ViewModel.IsPulseAnimation);
        }

        private void ClickHandler() => ViewModel.Click();
    }
}
