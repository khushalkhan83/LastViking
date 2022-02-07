using System;
using System.Collections.Generic;
using Core;
using Core.Controllers;
using Game.Models;
using Game.Providers;
using Game.QuestSystem.Data;
using static Game.Models.QuestsLifecycleModel;
using UnityEngine;

namespace Game.Controllers
{
    public class QuestsLifecycleController : IQuestsLifecycleController, IController
    {
        [Inject] public QuestsLifecycleModel QuestsLifecycleModel { get; private set; }
        [Inject] public MainQuestsProvider MainQuestsProvider { get; private set; }
        [Inject] public QuestsModel QuestsModel { get; private set; }
        [Inject] public QuetsEventDataProvider QuetsEventDataProvider { get; private set; }
        [Inject] public SideQuetsEventDataProvider SideQuetsEventDataProvider { get; private set; }
        [Inject] public SideQuestsModel SideQuestsModel { get; private set; }

        void IController.Enable() 
        {
            QuestsModel.OnActivateQuest += OnActivateQuest;
            QuestsModel.OnActivateStage += OnActivateStage;
            SideQuestsModel.OnStageChanged += OnSideStageChanged;
        }

        void IController.Start() 
        {

        }

        void IController.Disable() 
        {
            QuestsModel.OnActivateQuest -= OnActivateQuest;
            QuestsModel.OnActivateStage -= OnActivateStage;
            SideQuestsModel.OnStageChanged -= OnSideStageChanged;
        }

        private void OnActivateQuest()
        {
            
        }

        private void OnActivateStage()
        {
            foreach (QuestEvent questEvent in Enum.GetValues(typeof(QuestEvent)))
            {
                try
                {
                    var eventData = QuetsEventDataProvider[questEvent];
                    {
                        if(QuestsModel.GetActiveQuestIndex() == eventData.QuestIndex && QuestsModel.ActiveStage == eventData.QuestStage)
                        {
                            QuestsLifecycleModel.TriggerQuestEvent(questEvent);
                        }
                    }
                }
                catch(Exception ex) 
                {
                    Debug.LogError("Cant handle quest event: " + ex.Message);
                }
            }
        }

        private void OnSideStageChanged(QuestData quest)
        {
            foreach (SideQuestEvent sideQuestEvent in Enum.GetValues(typeof(SideQuestEvent)))
            {
                try
                {
                    var eventData = SideQuetsEventDataProvider[sideQuestEvent];
                    {
                        if(quest == eventData.QuestData)
                        {
                            int questStage = SideQuestsModel.GetQuestStage(quest);
                            if(questStage == eventData.QuestStage)
                            {
                                QuestsLifecycleModel.TriggerSideQuestEvent(sideQuestEvent);
                            }
                        }
                    }
                }
                catch(Exception ex) 
                {
                    Debug.LogError("Cant handle side quest event: " + ex.Message);
                }
            }
        }
    }
}
