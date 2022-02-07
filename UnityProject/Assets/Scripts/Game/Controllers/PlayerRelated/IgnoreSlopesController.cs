using Core;
using Core.Controllers;
using Game.Models;

namespace Game.Controllers
{
    public class IgnoreSlopesController : IIgnoreSlopesController, IController
    {
        [Inject] public PlayerMovementModel PlayerMovementModel { get; private set; }
        void IController.Enable() 
        {
            PlayerMovementModel.SetIgnoreSlopes(true);
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            PlayerMovementModel.SetIgnoreSlopes(false);
        }
    }
}
