using Core;
using Core.Controllers;
using Game.Models;

namespace Game.Controllers
{
    public class DisableCoreWaterController : IDisableCoreWaterController, IController
    {
        [Inject] public CoreEnvironmentModel CoreEnvironmentModel { get; private set; }
        void IController.Enable() 
        {
            CoreEnvironmentModel.SetWaterActive(false);
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            CoreEnvironmentModel.SetWaterActive(true);
        }

    }
}
