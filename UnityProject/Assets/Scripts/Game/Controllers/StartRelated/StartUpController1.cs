using Core;
using Core.Controllers;
using Game.Models;

namespace Game.Controllers
{
    public class StartUpController : IController, IStartUpController
    {
        [Inject] public ControllersModel ControllersModel { get; private set; }
        // [Inject] public TutorialModel TutorialModel { get; private set; }
        // [Inject] public CoinsModel CoinsModel { get; private set; }

        // init data
        [Inject] public PlayerObjectivesModel PlayerObjectivesModel { get; private set; }

        void IController.Enable()
        {   
            // var isTutorial = false;
            // // var isTutorial = (TutorialModel.IsStart && !TutorialModel.IsComplete) || (!TutorialModel.IsComplete && (CoinsModel.IsFirstStart || TutorialModel.IsHasObjectiveId));

            // #if UNITY_EDITOR
            // if(EditorGameSettings.Instance.ignoreTutorial) isTutorial = false;
            // #endif
            
            ControllersModel.ApplyState(ControllersStateID.Default);
            // if (!isTutorial && !TutorialModel.IsComplete)
            // {
            //     TutorialModel.Complete();
            // }
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {

        }
    }
}
