using Core;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class ZoneMessageController : ViewEnableController<ZoneMessageView>, IZoneMessageController
    {
        [Inject] public ZoneModel ZoneModel {get;set;}

        public override ViewConfigID ViewConfigID => ViewConfigID.ZoneMessageConfig;

        public override bool IsCanShow => true;

        public override void Enable()
        {
            ZoneModel.OnPlayerEnterZone += OnPlayerEnterZone;
            if(ZoneModel.PlayerZoneIsSet)
                UpdateViewVisible();
        }

        public override void Disable()
        {
            ZoneModel.OnPlayerEnterZone -= OnPlayerEnterZone;
        }

        private void OnPlayerEnterZone()
        {
            Hide();
            UpdateViewVisible();
        }
    }
}
