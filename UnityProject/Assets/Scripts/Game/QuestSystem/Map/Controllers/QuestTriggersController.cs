using System;
using System.Collections.Generic;
using Game.Models;
using Game.QuestSystem.Data;
using Game.QuestSystem.Map.Quests;
using Game.QuestSystem.Map.Stages;
using Game.QuestSystem.Map.Triggers;
using UnityEngine;

namespace Game.QuestSystem.Map.Controllers
{
    // [ExecuteAlways]
    public class QuestTriggersController : MonoBehaviour
    {
        private QuestTriggersModel QuestTriggersModel => ModelsSystem.Instance._questTriggersModel;
        private QuestsModel QuestsModel => ModelsSystem.Instance._questsModel;
        private SideQuestsModel SideQuestsModel => ModelsSystem.Instance._sideQuestsModel;
        
        #region MonoBehaviour
        private void OnEnable()
        {
            QuestTriggersModel.OnActivateTriggerNew += OnActivateTriggerNew;
        }

        private void OnDisable()
        {
            QuestTriggersModel.OnActivateTriggerNew -= OnActivateTriggerNew;
        }

        #endregion
        private void OnActivateTriggerNew(QuestTrigger questTrigger)
        {
            QuestStage questStage = questTrigger.GetComponentInParent<QuestStage>();
            Quest quest = questStage.GetComponentInParent<Quest>();
            QuestData questData = quest.Data;

            if(quest.IsMainQuest)
                HandleMainQuest();
            else
                HandleSideQuest();

            void HandleMainQuest()
            {
                quest.TryGetNextStageIndex(questStage, out int? nextStageIndex);

                if(nextStageIndex != null)
                {
                    QuestsModel.ActivateStage((int)nextStageIndex);
                }
                else
                {
                    QuestsModel.ActivateQuest(questData.NextQuest);
                }
            }

            void HandleSideQuest()
            {
                quest.TryGetNextStageIndex(questStage, out int? nextStageIndex);

                // temp solution because of last stage is temp stage for message
                bool lastStage = quest.IsPreLastStage(questStage);

                if(nextStageIndex == null)
                {
                    SideQuestsModel.CompleateQuest(questData);
                }
                else if(!lastStage)
                {
                    SideQuestsModel.ActivateStage(questData,(int)nextStageIndex);
                }
                else
                {
                    SideQuestsModel.ActivateStage(questData,(int)nextStageIndex);
                    SideQuestsModel.CompleateQuest(questData);
                }
            }
        }
    }
}