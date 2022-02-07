using System;
using Core;
using Core.Controllers;
using Game.Models;
using Game.Providers;
using Helpers;

namespace Game.Controllers
{
    public class QuestsSaveController : IQuestsSaveController, IController
    {
        [Inject] public QuestsSaveModel QuestsSaveModel { get; private set; }
        [Inject] public QuestsModel QuestsModel { get; private set; }
        [Inject] public StorageModel StorageModel { get; private set; }
        [Inject] public MainQuestsProvider MainQuestsProvider { get; private set; }
        void IController.Enable() 
        {
            QuestsModel.OnActivateQuest += OnActivateQuest;
            QuestsModel.OnActivateStage += OnActivateStage;

            bool dataExist = StorageModel.TryProcessing(QuestsSaveModel._Data);

            if(QuestsSaveModel.DebugMode && ApplicationHelper.IsEditorApplication()) return;

            if(!dataExist)
            {
                QuestsSaveModel.SetMainQuestIndex(QuestsModel.k_defaultQuestIndex);
                QuestsSaveModel.SetMainQuestStageIndex(QuestsModel.k_defaultStageIndex);
            }

            QuestsModel.ActivateQuest_Internal(QuestsSaveModel.MainQuestIndex);
            QuestsModel.ActivateStage_Internal(QuestsSaveModel.MainQuestStageIndex);

        }


        void IController.Start() 
        {
            
        }

        void IController.Disable() 
        {
            QuestsModel.OnActivateQuest -= OnActivateQuest;
            QuestsModel.OnActivateStage -= OnActivateStage;
        }

        private void OnActivateQuest()
        {
            bool error = !MainQuestsProvider.TryGetQuestIndex(QuestsModel.ActiveQuest, out int mainQuestIndex);
            if(error) return;

            QuestsSaveModel.SetMainQuestIndex(mainQuestIndex);
        }

        private void OnActivateStage()
        {
            QuestsSaveModel.SetMainQuestStageIndex(QuestsModel.ActiveStage);
        }
    }
}
