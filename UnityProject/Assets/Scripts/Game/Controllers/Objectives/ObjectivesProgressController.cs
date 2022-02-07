using System;
using Core;
using Core.Controllers;
using Game.Models;
using Game.Objectives;
using System.Linq;
using System.Collections.Generic;
using static Game.Models.ObjectivesProgressModel;
using UnityEngine;
using Game.Objectives.Stacks;

namespace Game.Controllers
{
    public class ObjectivesProgressController : IObjectivesProgressController, IController
    {
        [Inject] public ObjectivesProgressModel ObjectivesProgressModel {get; private set;}
        [Inject] public StorageModel StorageModel {get; private set;}
        [Inject] public ObjectivesProvider ObjectivesProvider {get; private set;}
        [Inject] public ObjectivesWindowModel ObjectivesWindowModel {get; private set;}

        private int RequiredTiersCount => ObjectivesWindowModel.RequiredTiersCount;

        void IController.Start() 
        {

        }

        void IController.Enable() 
        {
            StorageModel.TryProcessing(ObjectivesProgressModel._Data);

            ObjectivesProgressModel.OnSave += OnSaveHandler;
            ObjectivesProgressModel.OnSetObjectivewsWindow += OnSetObjectivewsWindowHandler;
            ObjectivesProgressModel.OnSaveAsEmpty += OnSaveAsEmptyHandler;
            ObjectivesProgressModel.OnLoadFromSave += LoadFromSaveHandler;
        }

        void IController.Disable() 
        {
            ObjectivesProgressModel.OnSave -= OnSaveHandler;
            ObjectivesProgressModel.OnSetObjectivewsWindow -= OnSetObjectivewsWindowHandler;
            ObjectivesProgressModel.OnSaveAsEmpty -= OnSaveAsEmptyHandler;
            ObjectivesProgressModel.OnLoadFromSave -= LoadFromSaveHandler;
        }

        private void LoadFromSaveHandler()
        {
            StorageModel.TryProcessing(ObjectivesProgressModel._Data);
        }

        private void OnSaveAsEmptyHandler()
        {
            ObjectivesProgressModel.ClearSave();
        }

        private void OnSetObjectivewsWindowHandler()
        {
            ObjectivesWindowModel.Reset();
        }


        private void OnSaveHandler()
        {
            var objectiveIds = Helpers.EnumsHelper.GetValues<ObjectiveID>().ToList();
            objectiveIds.Remove(ObjectiveID.None);

            foreach (var id in objectiveIds)
            {
                ObjectiveData objective = ObjectivesProvider[id];
                int firstConditionID = objective.Conditions.FirstOrDefault().Id;

                ObjectivesProgressModel.SetObjectiveState(firstConditionID,objective.Done);
            }

            for (int i = 0; i < RequiredTiersCount; i++)
            {
                var states = GetRequiredTiersStates();
                ObjectivesProgressModel.SetTierState(i, states[i]);
            }

            var selectedObjectivesData = ObjectivesWindowModel.SelectedObjectivesData;

            var stackDatas = new List<StackProgress>();
            foreach (var d in selectedObjectivesData)
            {
                StackProgress stackData = d.GetStackData();
                if(stackData == null)
                {
                    Debug.LogError("Stack is null");
                    continue;
                }

                stackDatas.Add(stackData);
            }

            ObjectivesProgressModel.SetStackDatas(stackDatas);
        }

        private List<bool> GetRequiredTiersStates() => ObjectivesWindowModel.RequiredTiersStates;
    }
}
