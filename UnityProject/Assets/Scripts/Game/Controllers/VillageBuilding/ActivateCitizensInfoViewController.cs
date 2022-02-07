using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class ActivateCitizensInfoViewController : ViewEnableController<CitizensInfoView>, IController
    {
        public override ViewConfigID ViewConfigID => ViewConfigID.CitizensInfoViewConfig;

        public override bool IsCanShow => false;

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
