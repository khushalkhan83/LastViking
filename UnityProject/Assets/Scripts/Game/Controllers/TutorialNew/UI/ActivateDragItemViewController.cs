using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class ActivateDragItemViewController : ViewEnableController<DragItemView>, IController
    {
        [Inject] public DragItemViewModel ViewModel { get; private set; }
        public override ViewConfigID ViewConfigID => ViewConfigID.DragItemConfig;
        public override bool IsCanShow => ViewModel.IsShow;

        public override void Enable()
        {
            ViewModel.OnShowChanged += UpdateViewVisible;
            
            UpdateViewVisible();
        }

        public override void Disable()
        {
            ViewModel.OnShowChanged -= UpdateViewVisible;
        }
    }
}