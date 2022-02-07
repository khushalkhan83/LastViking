using Core;
using Core.Controllers;
using Game.Models;

namespace Game.Controllers
{
    public class TestWaterfallController : ITestWaterfallController, IController
    {
        void IController.Enable() 
        {
            "TestWaterfallController Enable".Log();
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            "TestWaterfallController Disable".Log();

        }

    }
}
