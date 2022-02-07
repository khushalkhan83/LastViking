using Core;
using Core.Controllers;
using Game.Objectives.Conditions.Controllers;
using System;
using System.Collections.Generic;

namespace Game.Objectives
{
    public class ObjectiveConditionsController : IController, IObjectiveConditionsController
    {
        [Inject] public ConditionControllersMapper ConditionControllersMapper { get; private set; }
        [Inject] public ObjectivesModel ObjectivesModel { get; private set; }
        [Inject] public InjectionSystem InjectionSystem { get; private set; }

        public Dictionary<ConditionID, IConditionController> Controllers { get; } = new Dictionary<ConditionID, IConditionController>();

        public IConditionController Get(ConditionID conditionID)
        {
            if (!Controllers.TryGetValue(conditionID, out var controller))
            {
                controller = CreateController(conditionID);
                Controllers[conditionID] = controller;
            }

            return controller;
        }

        private IConditionController CreateController(ConditionID conditionID)
        {
            var type = ConditionControllersMapper[conditionID];
            var conditionController = (IConditionController)Activator.CreateInstance(type);
            InjectionSystem.Inject(conditionController, type);

            return conditionController;
        }

        void IController.Enable()
        {
            // if (ObjectivesModel.IsReadyData)
            // {
            //     InitializeConditions();
            // }
            // else
            // {
            //     ObjectivesModel.OnReadyData += OnReadyDataHandler;
            // }

            // ObjectivesModel.OnAddObjective += OnAddObjectiveHandler;
            // ObjectivesModel.OnRemoveObjective += OnRemoveObjectiveHandler;
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
            // ObjectivesModel.OnAddObjective -= OnAddObjectiveHandler;
            // ObjectivesModel.OnRemoveObjective -= OnRemoveObjectiveHandler;
            // ObjectivesModel.OnReadyData -= OnReadyDataHandler;

            // FinalizeConditions();
        }

        private void OnReadyDataHandler()
        {
            ObjectivesModel.OnReadyData -= OnReadyDataHandler;
            InitializeConditions();
        }

        private void InitializeConditions()
        {
            foreach (var objectiveModel in ObjectivesModel.ObjectiveModels)
            {
                OnAddObjectiveHandler(objectiveModel);
            }
        }

        private void FinalizeConditions()
        {
            foreach (var objectiveModel in ObjectivesModel.ObjectiveModels)
            {
                OnRemoveObjectiveHandler(objectiveModel);
            }
        }

        private void OnRemoveObjectiveHandler(ObjectiveModel objectiveModel)
        {
            foreach (var conditionModel in objectiveModel.Conditions)
            {
                if (!conditionModel.IsComplete)
                {
                    UnregisterCondition(conditionModel);
                }
            }
        }

        private void OnAddObjectiveHandler(ObjectiveModel objectiveModel)
        {
            foreach (var conditionModel in objectiveModel.Conditions)
            {
                if (!conditionModel.IsComplete)
                {
                    RegisterCondition(conditionModel);
                }
            }
        }

        private void RegisterCondition(ConditionModel conditionModel) => Get(conditionModel.ConditionID).Register(conditionModel);
        private void UnregisterCondition(ConditionModel conditionModel) => Get(conditionModel.ConditionID).Unregister(conditionModel);
    }
}
