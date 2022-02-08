using Core;
using Core.Controllers;
using Game.Models;

namespace Game.Controllers
{
    public class StartUpController : IController, IStartUpController
    {
        [Inject] public ControllersModel ControllersModel { get; private set; }
        [Inject] public TutorialModel TutorialModel { get; private set; }
        // [Inject] public TutorialModel TutorialModel { get; private set; }
        [Inject] public CoinsModel CoinsModel { get; private set; }
        private EditorGameSettings EditorGameSettings => EditorGameSettings.Instance;

        // init data
        [Inject] public PlayerObjectivesModel PlayerObjectivesModel { get; private set; }

        void IController.Enable()
        {
            var isTutorial = (TutorialModel.IsStart && !TutorialModel.IsComplete) || (!TutorialModel.IsComplete && (CoinsModel.IsFirstStart || TutorialModel.IsHasObjectiveId));

            if (EditorGameSettings.IsPerformanceTest)
                isTutorial = false;

#if UNITY_EDITOR
            if (EditorGameSettings.ignoreTutorial)
                isTutorial = false;
#endif

            ControllersModel.ApplyState(ControllersStateID.Default);
            if (!isTutorial && !TutorialModel.IsComplete)
            {
                TutorialModel.SkipTutorial();
                TutorialModel.Complete();
            }
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {

        }
    }
}
