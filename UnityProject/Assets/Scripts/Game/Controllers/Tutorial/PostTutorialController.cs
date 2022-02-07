using System.Collections;
using Core;
using Core.Controllers;
using Game.Controllers.Controllers.States.Modificators;
using Game.Models;
using UnityEngine;

namespace Game.Controllers
{
    public class PostTutorialController : IPostTutorialController, IController
    {
        [Inject] public TutorialModel TutorialModel { get; private set; }
        [Inject] public ControllersModel ControllersModel { get; private set; }
        [Inject] public TutorialObjectivesDarkScreenModel TutorialObjectivesDarkScreenModel { get; private set; }
        [Inject] public ObjectivesViewModel ObjectivesViewModel { get; private set; }
        [Inject] public CoroutineModel CoroutineModel { get; private set; }
        [Inject] public ViewsStateModel ViewsStateModel { get; private set; }
        [Inject] public PlayerObjectivesModel PlayerObjectivesModel { get; private set; }

        private TutorialModificator HideObjectivesMenuButtonModificator => TutorialModel.StartPostTutorialModificator;
        private TutorialModificator TakeObjectiveRewardsFlowModificator => TutorialModel.PostTutorialModificator;
        private TutorialModificator NoModificators => TutorialModel.NoModificator;

        private int coroutineId = -1;
        void IController.Enable()
        {
            // if (TutorialModel.IsComplete)
            // {
            //     MainLogic();
            // }
            // else if (!TutorialModel.IsComplete)
            // {
            //     TutorialModel.OnComplete += MainLogic;
            // }
        }

        void IController.Start()
        {
        }

        void IController.Disable()
        {
            // TutorialModel.OnComplete -= MainLogic;
            // TutorialObjectivesDarkScreenModel.OnStepsOver -= OnObjectivesTutorialStepsOver;
            // CoroutineModel.BreakeCoroutine(coroutineId);
            // ControllersModel.ApplyModificator(NoModificators);
        }

        private void MainLogic()
        {
            if (TutorialModel.IsPostTutorialCompleated) return;

            coroutineId = CoroutineModel.InitCoroutine(PostTutorialProcess());
        }

        private IEnumerator PostTutorialProcess()
        {
            ControllersModel.ApplyModificator(HideObjectivesMenuButtonModificator);
            if (!PlayerObjectivesModel.ObjectivesInitedFirstTime)
            {
                yield return null;
                yield return null;
                PlayerObjectivesModel.NextTier();
                PlayerObjectivesModel.ObjectivesInitedFirstTime = true;
            }

            if (ObjectivesViewModel.IsHasAny)
            {
                yield return new WaitForSeconds(TutorialObjectivesDarkScreenModel.ShowDelay);

                while (true)
                {
                    if (!ViewsStateModel.WindowOrPopupOpened())
                    {
                        ControllersModel.ApplyModificator(TakeObjectiveRewardsFlowModificator);
                        TutorialObjectivesDarkScreenModel.OnStepsOver += OnObjectivesTutorialStepsOver;
                        break;
                    }
                    yield return new WaitForSeconds(1f);
                }
            }
            else
            {
                TutorialModel.IsPostTutorialCompleated = true;
                ControllersModel.ApplyModificator(NoModificators);
            }

        }
        private void OnObjectivesTutorialStepsOver()
        {
            TutorialModel.IsPostTutorialCompleated = true;
            ControllersModel.ApplyModificator(NoModificators);
            CoroutineModel.BreakeCoroutine(coroutineId);
        }
    }
}