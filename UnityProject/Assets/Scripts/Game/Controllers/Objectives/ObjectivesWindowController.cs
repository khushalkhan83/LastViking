using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Controllers;
using Game.Models;
using Game.Objectives;
using Game.Objectives.Stacks;
using UnityEngine;

namespace Game.Controllers
{
    public class ObjectivesWindowController : IObjectivesWindowController, IController
    {
        [Inject] public ObjectivesWindowModel ObjectivesWindowModel {get; private set;}
        [Inject] public PlayerObjectivesModel PlayerObjectivesModel {get; private set;}
        [Inject] public ObjectivesProgressModel ObjectivesProgressModel {get; private set;}
        [Inject] public ObjectivesModel ObjectivesModel {get; private set;}
        [Inject] public StorageModel StorageModel {get; private set;}
        [Inject] public FixOldObjectivesModel FixOldObjectivesModel {get; private set;}

        void IController.Start() {}
        void IController.Enable() 
        {
            // PlayerObjectivesModel.OnPreEndTime += OnPreEndTimeHandler;
            // PlayerObjectivesModel.OnEndTime += OnEndTimeHandler;
            // PlayerObjectivesModel.OnResetPlayerObjectivesOnStart += OnResetPlayerObjectivesOnStartHandler;
            // PlayerObjectivesModel.OnResetPlayerObjectives += OnResetPlayerObjectivesHandler;
            // StorageModel.OnPreSaveChanged += OnPreSaveChangedHandler;

            // OnResetPlayerObjectivesOnStartHandler();
        }

        void IController.Disable() 
        {
            // PlayerObjectivesModel.OnPreEndTime-= OnPreEndTimeHandler;
            // PlayerObjectivesModel.OnEndTime -= OnEndTimeHandler;
            // PlayerObjectivesModel.OnResetPlayerObjectivesOnStart -= OnResetPlayerObjectivesOnStartHandler;
            // PlayerObjectivesModel.OnResetPlayerObjectives -= OnResetPlayerObjectivesHandler;
            // StorageModel.OnPreSaveChanged -= OnPreSaveChangedHandler;
        }

        #region Handlers

        private void OnPreSaveChangedHandler()
        {
            ObjectivesProgressModel.SaveObjectives();
            PlayerObjectivesModel.ResetPlayerObjectives();
            PlayerObjectivesModel.UpdatePool();
        }

        private void OnPreEndTimeHandler()
        {
            ObjectivesWindowModel.UpdateConditions();
        }

        private void OnEndTimeHandler()
        {
            ObjectivesWindowModel.GenerateNewObjectives();
            ResetObjectivesAndPlayerObjectives();
        }

        private void OnResetPlayerObjectivesOnStartHandler()
        {
            var tempDatas = GetTempObjectivesData();
            ObjectivesProgressModel.SetObjectivewsWindow();
            ObjectivesWindowModel.StartCoroutine(DoActionAfterFrame(() => ResetPlayerObjectivesOnStart(tempDatas)));
        }

        private void OnResetPlayerObjectivesHandler()
        {
            ResetPlayerObjectives();
        }

        #endregion

        #region Methods
            
        private List<TempData> GetTempObjectivesData()
        {
            var result = ConvertObjectivesToTempData(ObjectivesModel.ObjectiveModels);
            
            result = RemoveIgnoredObjectivesFromList(result);

            return result;
        }

        private List<TempData> RemoveIgnoredObjectivesFromList(List<TempData> tempData)
        {
            List<TempData> doNotremove = new List<TempData>();

            foreach (var item in tempData)
            {
                var match = FixOldObjectivesModel.OjbectivesToIgnoreResetObjectives.Where(x => x == item.ObjectiveID);

                if(match == null || match.Count() == 0) continue;

                doNotremove.Add(item);
            }

            foreach (var item in doNotremove)
            {
                tempData.Remove(item);
            }

            return tempData;
        }

        private List<TempData> ConvertObjectivesToTempData(List<ObjectiveModel> objectiveModels)
        {
            List<TempData> tempDatas = new List<TempData>();
            foreach (var item in objectiveModels)
            {
                tempDatas.Add(new TempData(item.ObjectiveID,item.Conditions));
            }
            return tempDatas;
        }

        private void ResetPlayerObjectivesOnStart(List<TempData> tempDatas)
        {
            ResetPlayerObjectives(tempDatas);
        }

        private void ResetPlayerObjectives(List<TempData> tempDatas = null)
        {
            if(tempDatas == null)
                tempDatas = GetTempObjectivesData();

            PlayerObjectivesModel.EndAll();

            ResetObjectivesAndPlayerObjectives(true,tempDatas);
        }

        private void ResetObjectivesAndPlayerObjectives(bool exludeCompleatedObjectives = false, List<TempData> tempData = null)
        {
            byte index = 0;
            foreach (var data in ObjectivesWindowModel.SelectedObjectivesData)
            {
                if(exludeCompleatedObjectives && data.Done) continue;

                ObjectiveModel model = GetNewObjective(data);

                if(tempData != null)
                {
                    var savedModel = tempData.Find(x => x.ObjectiveID == model.ObjectiveID);
                    if(savedModel != null)
                    {
                        model.ResetConditions(savedModel.Conditions);
                    }
                }

                AddObjectiveToObjectivesModel(model);
                PlayerObjectivesModel.AddToPool_Temp(index,model);
                PlayerObjectivesModel.Create(index, ObjectivesModel.GetId(model));
                index++;
            }
        }

        private ObjectiveModel GetNewObjective(ObjectivesStack.OjbectiveStackData data)
        {
            ObjectiveID id = ObjectivesWindowModel.GetObjectiveID(data.Objective);
            return ObjectivesModel.GetObjective(id);
        }

        private void AddObjectiveToObjectivesModel(ObjectiveModel objectiveModel) => ObjectivesModel.AddObjective(objectiveModel);

        IEnumerator DoActionAfterFrame(Action action)
        {
            yield return new WaitForEndOfFrame();
            action?.Invoke();
        }

        public class TempData
        {
            public ObjectiveID ObjectiveID {get;}
            public ConditionModel[] Conditions {get;}

            public TempData(ObjectiveID objectiveID, ConditionModel[] conditions)
            {
                ObjectiveID = objectiveID;
                Conditions = conditions;
            }
        }
        #endregion
        
    }
}
