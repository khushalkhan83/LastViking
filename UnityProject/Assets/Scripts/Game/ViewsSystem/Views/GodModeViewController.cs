using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class GodModeViewController : IGodModeViewController, IController
    {
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public DebugTimeModel DebugTimeModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }

        public GodModeView GodModeView { get; private set; }

        void IController.Enable()
        {
            DebugTimeModel.OnChangeIsGodMode += OnChangeModeHandler;
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
            DebugTimeModel.OnChangeIsGodMode -= OnChangeModeHandler;
        }

        private void OnChangeModeHandler()
        {
            if (DebugTimeModel.IsGodMode)
            {
                if (GodModeView == null)
                {
                    GodModeView = ViewsSystem.Show<GodModeView>(ViewConfigID.GodMode);
                }
            }
            else
            {
                if (GodModeView != null)
                {
                    ViewsSystem.Hide(GodModeView);
                    GodModeView = null;
                }
            }
        }
    }
}
