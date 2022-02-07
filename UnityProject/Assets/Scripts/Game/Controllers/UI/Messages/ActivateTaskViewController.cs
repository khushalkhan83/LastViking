using Core;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class ActivateTaskViewController : ViewEnableController<TaskView>, IActivateTaskViewController
    {
        [Inject] public TaskViewModel ViewModel { get; private set; }
        [Inject] public PlayerDeathModel PlayerDeathModel { get; private set; }
        
        public override ViewConfigID ViewConfigID => ViewConfigID.TaskViewConfig;

        public override bool IsCanShow => ViewModel.IsShow;

        public override void Enable()
        {
            UpdateViewVisible();
            ViewModel.OnShowChanged += UpdateViewVisible;
            PlayerDeathModel.OnRevival += UpdateViewVisible;
            PlayerDeathModel.OnRevivalPrelim += UpdateViewVisible;
        }

        public override void Disable()
        {
            ViewModel.OnShowChanged -= UpdateViewVisible;
            PlayerDeathModel.OnRevival -= UpdateViewVisible;
            PlayerDeathModel.OnRevivalPrelim -= UpdateViewVisible;
        }
    }
}