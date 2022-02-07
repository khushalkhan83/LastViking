using Core;
using Core.Controllers;
using Game.Models;
using Game.Objectives;
using Game.Views;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Game.Controllers
{
    public class PlayerEventObjectivesController : IPlayerObjectivesController, IController
    {
        [Inject] public NotificationContainerViewModel NotificationContainerViewModel { get; private set; }
        [Inject] public ObjectivesModel ObjectivesModel { get; private set; }
        [Inject] public PlayerEventObjectivesModel PlayerEventObjectivesModel { get; private set; }
        [Inject] public StorageModel StorageModel { get; private set; }
        [Inject] public CoroutineModel CoroutineModel { get; private set; }

        ObjectiveModel CurrentObjectiveModel { get; set; }

        private int _waitForShowCoroutineId = -1;

        void IController.Enable()
        {
            // StorageModel.OnPreSaveChanged += OnPreSaveHandler;
            // PlayerEventObjectivesModel.OnStartObjective += OnStartObjectiveHandler;
            // PlayerEventObjectivesModel.OnEndObjective += OnEndObjectiveHandler;
            // ObjectivesModel.OnRemoveObjective += OnRemoveObjective;
            // ObjectivesModel.OnAddObjective += OnAddObjective;
        }

        void IController.Start()
        {
            // if (PlayerEventObjectivesModel.IsHasObjectiveId)
            // {
            //     PlayerEventObjectivesModel.CurrentObjectiveID.Log();
            //     StartInitializeObjective();
            //     PlayerEventObjectivesModel.CurrentObjectiveID.Log();
            //     if (CurrentObjectiveModel != null)
            //     {
            //         PlayerEventObjectivesModel.SetCurrentObjectiveID(CurrentObjectiveModel.ObjectiveID);
            //     } 
            // }
        }

        void IController.Disable()
        {
            // StorageModel.OnPreSaveChanged -= OnPreSaveHandler;
            // PlayerEventObjectivesModel.OnStartObjective -= OnStartObjectiveHandler;
            // PlayerEventObjectivesModel.OnEndObjective -= OnEndObjectiveHandler;
            // ObjectivesModel.OnRemoveObjective -= OnRemoveObjective;
            // ObjectivesModel.OnAddObjective -= OnAddObjective;
            // CoroutineModel.BreakeCoroutine(_waitForShowCoroutineId);
        }

        private void OnPreSaveHandler()
        {
            if (CurrentObjectiveModel != null)
            {
                PlayerEventObjectivesModel.Save(ObjectivesModel.GetId(CurrentObjectiveModel));
            }
        }

        private void OnRemoveObjective(ObjectiveModel removedObjective) => UpdateCurrentObjectiveID(removedObjective);
        private void OnAddObjective(ObjectiveModel addedModel) => UpdateCurrentObjectiveID(addedModel);

        private void UpdateCurrentObjectiveID(ObjectiveModel model)
        {
            if (PlayerEventObjectivesModel.IsHasObjectiveId && CurrentObjectiveModel != model)
            {
                UpdateCurrentObjectiveID();
            }
        }

        private void UpdateCurrentObjectiveID()
        {
            PlayerEventObjectivesModel.SetObjectiveId(ObjectivesModel.GetId(CurrentObjectiveModel));
        }
        

        private void StartInitializeObjective()
        {
            if (ObjectivesModel.IsReadyData)
            {
                SetObjective(GetObjective());
            }
            else
            {
                ObjectivesModel.OnReadyData += OnReadyDataHandler;
            }
        }

        private void OnEndObjectiveHandler()
        {
            if (PlayerEventObjectivesModel.IsHasObjectiveId)
            {
                var objectiveModel = ObjectivesModel.Get(PlayerEventObjectivesModel.ObjectiveId);
                objectiveModel.ObjectiveData.Done = true; // FIXME: refactor
                objectiveModel.End();
                objectiveModel.OnComplete -= OnComplete;
                ObjectivesModel.RemoveObjective(objectiveModel);
                CurrentObjectiveModel = null;
            }
        }

        private void OnStartObjectiveHandler() => SetObjective(CreateObjective());

        private void OnReadyDataHandler() => SetObjective(GetObjective());

        private ObjectiveModel GetObjective()
        {
            if (PlayerEventObjectivesModel.IsHasObjectiveId)
            {
                return GetExistObjective();
            }
            return CreateObjective();
        }

        private ObjectiveModel GetExistObjective()
        {
            CurrentObjectiveModel = ObjectivesModel.Get(PlayerEventObjectivesModel.ObjectiveId);
            return CurrentObjectiveModel;
        }

        private ObjectiveModel CreateObjective()
        {
            CurrentObjectiveModel = ObjectivesModel.CreateAndAddObjective(PlayerEventObjectivesModel.CurrentObjectiveID);
            return CurrentObjectiveModel;
        }

        private void SetObjective(ObjectiveModel objectiveModel)
        {
            SetupObjective(objectiveModel);
            _waitForShowCoroutineId = CoroutineModel.InitCoroutine(WaitForShowNotification(objectiveModel));
        }

        private void SetupObjective(ObjectiveModel objectiveModel)
        {
            PlayerEventObjectivesModel.Save(ObjectivesModel.GetId(objectiveModel));
            objectiveModel.Begin();
            objectiveModel.OnComplete += OnComplete;
        }

        private void OnComplete(ObjectiveModel objectiveModel)
        {
            PlayerEventObjectivesModel.EndObjective();
        }

        private void ShowNotification(ObjectiveModel objectiveModel, float? showTime = null) => NotificationContainerViewModel.Show(PriorityID.EventObjectives, ViewConfigID.EventObjectiveView, new EventObjectiveProcessViewControllerData(objectiveModel));
        private IEnumerator WaitForShowNotification(ObjectiveModel objectiveModel)
        {
            yield return new WaitForSeconds(3f);
            ShowNotification(objectiveModel);
        }

    }
}
