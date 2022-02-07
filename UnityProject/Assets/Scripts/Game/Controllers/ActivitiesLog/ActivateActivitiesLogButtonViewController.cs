using Core;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class ActivateActivitiesLogButtonViewController : ViewEnableController<ActivitiesLogButtonView>, IActivateActivitiesLogButtonViewController
    {
        [Inject] public ActivitiesLogButtonViewModel ViewModel { get; private set; }
        [Inject] public ActivitiesLogViewModel LogViewModel { get; private set; }
        [Inject] public PlayerDeathModel PlayerDeathModel  { get; private set; }
        public override ViewConfigID ViewConfigID => ViewConfigID.ActivitiesLogButtonConfig;

        public override bool IsCanShow => ViewModel.IsShow;

        public override void Enable()
        {
            // ViewModel.OnShowChanged += UpdateViewVisible;
            // ViewModel.OnClick += ClickHandler;
            // PlayerDeathModel.OnPreRevival += UpdateViewVisible;
            // PlayerDeathModel.OnRevival += UpdateViewVisible;
            // UpdateViewVisible();
        }

        public override void Start() { }

        public override void Disable()
        {
            // ViewModel.OnShowChanged -= UpdateViewVisible;
            // ViewModel.OnClick -= ClickHandler;
            // PlayerDeathModel.OnPreRevival -= UpdateViewVisible;
            // PlayerDeathModel.OnRevival -= UpdateViewVisible;
            // Hide();
        }

        private void ClickHandler() => LogViewModel.SwitchShow();
    }
}
