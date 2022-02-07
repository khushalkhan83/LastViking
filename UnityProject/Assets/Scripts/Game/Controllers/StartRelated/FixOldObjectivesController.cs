using Core;
using Core.Controllers;
using Game.Models;
using Game.Objectives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Controllers
{
    public class FixOldObjectivesController : IFixOldObjectivesController, IController
    {
        [Inject] public ObjectivesModel ObjectivesModel { get; private set; }
        [Inject] public StorageModel StorageModel { get; private set; }
        [Inject] public FixOldObjectivesModel FixOldObjectivesModel { get; private set; }
        [Inject] public TutorialModel TutorialModel { get; private set; }
        [Inject] public PlayerObjectivesModel PlayerObjectivesModel { get; private set; }
        [Inject] public PlayerEventObjectivesModel PlayerEventObjectivesModel { get; private set; }
        void IController.Enable() 
        {
            StorageModel.TryProcessing(FixOldObjectivesModel._Data);
            
            TutorialModel.OnComplete += OnCompleateTutorial;

            if(TutorialModel.IsComplete) OnCompleateTutorial();
        }

        void IController.Start() {}

        void IController.Disable() 
        {
            TutorialModel.OnComplete -= OnCompleateTutorial;
        }

        private void OnCompleateTutorial()
        {
            TryDoFix();
        }

        private void TryDoFix()
        {
            if(FixOldObjectivesModel.FixUsed) return;

            RemoveOldObjectives();
            ResetEventObjective();
            FixOldObjectivesModel.SetFixUsed(true);
        }

        private void RemoveOldObjectives()
        {
            PlayerObjectivesModel.ClearPool();

            for (int i = ObjectivesModel.ObjectiveModels.Count - 1; i >= 0 ; i--)
            {
                var model = ObjectivesModel.ObjectiveModels[i];
                ObjectivesModel.RemoveObjective(model);
            }
        }

        private void ResetEventObjective()
        {
            PlayerEventObjectivesModel.ResetHasObjective();
        }
    }
}
