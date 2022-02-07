using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class TutorialObjectivesDarkScreenController : ViewEnableController<TutorialObjectivesDarkScreenView>, ITutorialObjectivesDarkScreenController, IController
    {
        public override ViewConfigID ViewConfigID => ViewConfigID.TutorialObjectivesDarkScreenConfig;

        public override bool IsCanShow => true;



        public override void Enable()
        {
            UpdateViewVisible();
        }

        public override void Start()
        {
            
        }

        public override void Disable()
        {
            
        }
    }
}
