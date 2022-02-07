using System;
using UnityEngine;
using Game.Objectives.Stacks;
using System.Collections.Generic;
using Game.Objectives;

namespace Game.Models
{
    public class ObjectivesWindowModel : MonoBehaviour
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private ObjectivesWindow objectivesWindow;
        
        #pragma warning restore 0649
        #endregion

        private ObjectivesProvider ObjectivesProvider => ModelsSystem.Instance._objectivesProvider;
        private ObjectiveIDProvider _objectiveIDProvider;
        private ObjectiveIDProvider ObjectiveIDProvider
        {
            get
            {
                if(_objectiveIDProvider == null) _objectiveIDProvider = new ObjectiveIDProvider(ObjectivesProvider);
                return _objectiveIDProvider;
            }
        }

        public event Action<ObjectiveModel> OnCompleatedObjective;

        public ObjectiveID GetObjectiveID(ObjectiveData objectiveData) => ObjectiveIDProvider.GetID(objectiveData);

        public List<ObjectivesStack.OjbectiveStackData> SelectedObjectivesData => objectivesWindow.SelectedObjectives;
        public int RequiredTiersCount => objectivesWindow.RequiredTiersCount;
        public List<bool> RequiredTiersStates => objectivesWindow.RequiredTiersStates;
        public bool RequiredObjectivesCompleated => objectivesWindow.RequiredObjectivesCompleated;
        public void Complete(ObjectiveModel objectiveModel)
        {
            var match = SelectedObjectivesData.Find(x => x.Objective == objectiveModel.ObjectiveData);
            if(match == null)
            {
                Debug.LogError("Error here. Cant find match");
                return;
            }

            match.Complete();
            OnCompleatedObjective?.Invoke(objectiveModel);
        }

        public void Reset() => objectivesWindow.Reset();

        public bool CanRerollObjective(ObjectiveID objectiveID)
        {
            var objectiveData = ObjectivesProvider[objectiveID];
            bool canReroll = objectivesWindow.CanRerollObjective(objectiveData);
            return canReroll;
        }

        public void UpdateConditions()
        {
            objectivesWindow.UpdateConditions();
        }

        public void GenerateNewObjectives()
        {
            objectivesWindow.NextTurn();
        }

        public void RerollObjectiveInWindow(ObjectiveModel objectiveModel, int index) => objectivesWindow.RerollObjective(objectiveModel.ObjectiveData, index);
    }
}
