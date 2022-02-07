using Core;
using Core.Controllers;
using Game.Models;
using Game.Objectives;
using Game.Views;
using System.Collections;
using UnityEngine;

namespace Game.Controllers
{
    public class TutorialController : MonoBehaviour, ITutorialController, IController
    {
        [Inject] public NotificationContainerViewModel NotificationContainerViewModel { get; private set; }
        [Inject] public LocalizationModel LocalizationModel { get; private set; }
        [Inject] public ObjectivesModel ObjectivesModel { get; private set; }
        [Inject] public TutorialModel TutorialModel { get; private set; }
        [Inject] public StorageModel StorageModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public ControllersModel ControllersModel { get; private set; }

        ObjectiveModel ObjectiveModel { get; set; }

        void IController.Enable()
        {
            
        }

        void IController.Start()
        {
            // if (!TutorialModel.IsComplete)
            // {
            //     StorageModel.OnPreSaveChanged += OnPreSaveHandler;
            //     TutorialModel.OnComplete += OnCompleteTutorialHandler;
            //     TutorialModel.OnNextStep += OnNextStepHandler;
            //     TutorialModel.OnEndStep += OnEndStepHandler;
            // }

            // if (TutorialModel.IsStart)
            // {
            //     if (!TutorialModel.IsComplete)
            //     {
            //         StartInitializeObjective();
            //     }
            // }
            // else
            // {
            //     TutorialModel.OnStart += OnStartTutorialHandler;
            // }
        }

        void IController.Disable()
        {
            // if (ObjectivesModel)
            // {
            //     ObjectivesModel.OnReadyData -= OnReadyDataHandler;
            // }
            // StorageModel.OnPreSaveChanged -= OnPreSaveHandler;
            // TutorialModel.OnComplete -= OnCompleteTutorialHandler;
            // TutorialModel.OnNextStep -= OnNextStepHandler;
            // TutorialModel.OnStart -= OnStartTutorialHandler;
            // TutorialModel.OnEndStep -= OnEndStepHandler;
        }

        private void OnEndStepHandler()
        {
            if (TutorialModel.IsHasObjectiveId)
            {
                var objectiveModel = ObjectivesModel.Get(TutorialModel.ObjectiveId);
                objectiveModel.End();
                objectiveModel.OnComplete -= OnComplete;
                ObjectivesModel.RemoveObjective(objectiveModel);
            }
        }

        private void OnStartTutorialHandler()
        {
            ControllersModel.ApplyModificator(TutorialModel.StartModificator);
            StartInitializeObjective();
        }

        private void StartInitializeObjective()
        {
            if (ObjectivesModel.IsReadyData)
            {
                InitializeObjective();
            }
            else
            {
                ObjectivesModel.OnReadyData += OnReadyDataHandler;
            }
        }

        private void OnNextStepHandler() => SetObjective(CreateObjective());

        private void OnCompleteTutorialHandler() => ShowFinalNotification();

        private void OnPreSaveHandler()
        {
            if (ObjectiveModel != null)
            {
                TutorialModel.Save(ObjectivesModel.GetId(ObjectiveModel));
            }
        }

        private void OnReadyDataHandler() => InitializeObjective();

        private void InitializeObjective() => SetObjectiveFirst(GetObjective());
        
        private ObjectiveModel GetObjective()
        {
            if (TutorialModel.IsHasObjectiveId)
            {   
                ObjectiveModel answer;
                bool exist = ObjectivesModel.TryGet(TutorialModel.ObjectiveId, out answer);

                if(TutorialModel.ObjectiveIDCurrent == ObjectiveID.TutorialBuildFoundation)
                {
                    var objective = ObjectivesModel.CreateAndAddObjective(TutorialModel.ObjectiveIDCurrent);
                }
                else
                {

                }
                
                if(exist) return answer;
                else 
                {
                    return CreateObjective();
                }
            }
            else return CreateObjective();
        }

        private ObjectiveModel CreateObjective() => ObjectivesModel.CreateAndAddObjective(TutorialModel.ObjectiveIDCurrent);

        private void SetObjectiveFirst(ObjectiveModel objectiveModel)
        {
            SetupObjective(objectiveModel);
            ShowNotification(objectiveModel);
        }

        private void SetObjective(ObjectiveModel objectiveModel)
        {
            SetupObjective(objectiveModel);
            StartCoroutine(StartStep(objectiveModel));
        }

        private void SetupObjective(ObjectiveModel objectiveModel)
        {
            TutorialModel.Save(ObjectivesModel.GetId(objectiveModel));
            objectiveModel.Begin();
            objectiveModel.OnComplete += OnComplete;
        }

        private void OnComplete(ObjectiveModel objectiveModel) => TutorialModel.EndStep();

        private IEnumerator StartStep(ObjectiveModel objectiveModel)
        {
            yield return new WaitForSecondsRealtime(4.5f); //3.5 sec time complete notify show + 1 sec hide
            ShowNotification(objectiveModel);
        }

        private IEnumerator StartFinal()
        {
            yield return new WaitForSecondsRealtime(4.5f); //3.5 sec time complete notify show + 1 sec hide
            NotificationContainerViewModel.Show(PriorityID.Tutorial, ViewConfigID.TutorialCompleted, new TutorialCompletedControllerData(LocalizationKeyID.Tutorial_Completed, 5));
        }

        private void ShowNotification(ObjectiveModel objectiveModel) => NotificationContainerViewModel.Show(PriorityID.Tutorial, ViewConfigID.TutorialObjective, new ObjectiveProcessViewControllerData(objectiveModel));

        private void ShowFinalNotification() => StartCoroutine(StartFinal());
    }
}
