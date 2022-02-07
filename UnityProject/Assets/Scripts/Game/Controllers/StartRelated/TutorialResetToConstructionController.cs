using Core;
using Core.Controllers;
using Game.Models;

namespace Game.Controllers
{
    public class TutorialResetToConstructionController : ITutorialResetToConstructionController, IController
    {
        [Inject] public TutorialModel TutorialModel { get; private set; }
        [Inject] public TutorialProgressModel TutorialProgressModel { get; private set; }
        [Inject] public ObjectivesViewModel ObjectivesViewModel { get; private set; }

        void IController.Enable() 
        {
            TutorialModel.Init();
            if(TutorialModel.IsComplete && !TutorialModel.RestededToConstruction)
            {
                TutorialModel.ResetToConstrution();
                TutorialProgressModel.Init();
                ObjectivesViewModel.Init();
                TutorialProgressModel.LastTutorialStep = 0;
                ObjectivesViewModel.IsHas1 = true;
            }
            TutorialModel.RestededToConstruction = true;
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
        }

    }
}
