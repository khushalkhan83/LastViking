using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class ActivateLearnInputViewController : ViewEnableController<LearnInputView>, IController
    {
        [Inject] public LearnInputViewModel ViewModel { get; private set; }
        public override ViewConfigID ViewConfigID => ViewConfigID.LearnInputConfig;

        public override bool IsCanShow => ViewModel.IsShow;

        public override void Enable()
        {
            UpdateViewVisible();
            ViewModel.OnShowChanged += UpdateViewVisible;
        }

        public override void Disable()
        {
            Hide();
            ViewModel.OnShowChanged -= UpdateViewVisible;
        }
    }
}
