using Core;
using Core.Controllers;
using Game.Objectives.Controllers;
using System;
using System.Collections.Generic;

namespace Game.Objectives
{
    public interface IObjectiveActionsController { }

    public class ObjectiveActionsController : IController, IObjectiveActionsController
    {
        [Inject] public ActionControllersMapper ActionControllersMapper { get; private set; }
        [Inject] public ObjectivesModel ObjectivesModel { get; private set; }
        [Inject] public InjectionSystem InjectionSystem { get; private set; }

        public Dictionary<ActionID, IActionController> Controllers { get; } = new Dictionary<ActionID, IActionController>();

        private IActionController Get(ActionID actionID)
        {
            if (!Controllers.TryGetValue(actionID, out var controller))
            {
                controller = CreateController(actionID);
                Controllers[actionID] = controller;
            }

            return controller;
        }

        private IActionController CreateController(ActionID actionID)
        {
            var type = ActionControllersMapper[actionID];
            var actionController = (IActionController)Activator.CreateInstance(type);
            InjectionSystem.Inject(actionController, type);

            return actionController;
        }

        void IController.Enable()
        {
            if (ObjectivesModel.IsReadyData)
            {
                InitializeActions();
            }
            else
            {
                ObjectivesModel.OnReadyData += OnReadyDataHandler;
            }

            ObjectivesModel.OnAddObjective += OnAddObjectiveHandler;
            ObjectivesModel.OnRemoveObjective += OnRemoveObjectiveHandler;
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
            ObjectivesModel.OnAddObjective -= OnAddObjectiveHandler;
            ObjectivesModel.OnRemoveObjective -= OnRemoveObjectiveHandler;
            ObjectivesModel.OnReadyData -= OnReadyDataHandler;

            FinalizeActions();
        }

        private void OnReadyDataHandler()
        {
            ObjectivesModel.OnReadyData -= OnReadyDataHandler;
            InitializeActions();
        }

        private void InitializeActions()
        {
            foreach (var objectiveModel in ObjectivesModel.ObjectiveModels)
            {
                RegisterObjectiveModel(objectiveModel);
            }
        }

        private void FinalizeActions()
        {
            foreach (var objectiveModel in ObjectivesModel.ObjectiveModels)
            {
                UnregisterObjectiveModel(objectiveModel);
            }
        }

        private void OnAddObjectiveHandler(ObjectiveModel objectiveModel) => RegisterObjectiveModel(objectiveModel);

        private void OnRemoveObjectiveHandler(ObjectiveModel objectiveModel) => UnregisterObjectiveModel(objectiveModel);

        private void RegisterObjectiveModel(ObjectiveModel objectiveModel)
        {
            objectiveModel.OnBegin += OnBeginObjective;
            objectiveModel.OnEnd += OnEndObjective;
            objectiveModel.OnReward += OnRewardObjective;
        }

        private void UnregisterObjectiveModel(ObjectiveModel objectiveModel)
        {
            objectiveModel.OnBegin -= OnBeginObjective;
            objectiveModel.OnEnd -= OnEndObjective;
            objectiveModel.OnReward -= OnRewardObjective;
        }

        private void OnRewardObjective(ObjectiveModel objectiveModel) => ApplyActions(objectiveModel.ObjectiveData.Rewards);

        private void OnBeginObjective(ObjectiveModel objectiveModel) => ApplyActions(objectiveModel.ObjectiveData.BeginActions);

        private void OnEndObjective(ObjectiveModel objectiveModel) => ApplyActions(objectiveModel.ObjectiveData.EndActions);

        private void ApplyActions(IEnumerable<ActionBaseData> actions)
        {
            foreach (var action in actions)
            {
                Get(action.ActionID).Action(action);
            }
        }
    }
}
