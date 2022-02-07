using Core;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class TutorialSimpleDarkController : ViewEnableController<TutorialSimpleDarkView>
    {
        [Inject] public TutorialSimpleDarkViewModel ViewModel { get; private set; }
        [Inject] public PlayerMovementModel PlayerMovementModel {get;private set;}

        public override ViewConfigID ViewConfigID => ViewConfigID.TutorialSimpleDarkConfig;
        public override bool IsCanShow => ViewModel.Show;

        public override void Start() { }
        public override void Enable()
        {
            ViewModel.OnShowChanged += UpdateViewVisible;
            UpdateViewVisible();
        }
        public override void Disable()
        {
            ViewModel.OnShowChanged -= UpdateViewVisible;
            Hide();
        }

        protected override void OnShowView() => PlayerMovementModel.SetBlockPlayerMovement(true);
        protected override void OnHideView() => PlayerMovementModel.SetBlockPlayerMovement(false);
    }
}