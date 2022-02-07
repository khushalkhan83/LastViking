using Core;
using Core.Controllers;
using EasyBuildSystem.Runtimes.Internal.Part;
using EasyBuildSystem.Runtimes.Internal.Socket;
using Game.Models;

namespace Game.Controllers
{
    public class TutorialBuildingController : ITutorialBuildingController, IController
    {
        [Inject] public TutorialBuildingModel TutorialBuildingModel {get;set;}

        void IController.Enable() 
        {
            TutorialBuildingModel.OnIsPlacementAllowed += IsPlacementAllowed;
            TutorialBuildingModel.OnCanPartSnap += CanPartSnap;
        }

        void IController.Start() 
        {

        }

        void IController.Disable() 
        {
            TutorialBuildingModel.OnIsPlacementAllowed -= IsPlacementAllowed;
            TutorialBuildingModel.OnCanPartSnap -= CanPartSnap;
        }

        private bool IsPlacementAllowed(SocketBehaviour currentSocket)
        {
            if(TutorialBuildingModel.TutorialStage && currentSocket == null)
                return false;
            else 
                return true;
        }

        private bool CanPartSnap(PartBehaviour Part)
        {
            if(TutorialBuildingModel.TutorialStage && Part.Type != PartType.Tutorial)
                return false;
            else
                return true;
        }

    }
}
