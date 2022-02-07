using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.QuestSystem.Data.QuestTriggers.Actions
{
    [Serializable]
    public class ActionWraper
    {
        [HorizontalGroup]
        [SerializeField] private QuestTriggerActionType actionType = QuestTriggerActionType.ActivateStage;

        public void Do()
        {
            if(this.actionType == QuestTriggerActionType.ActivateQuest)
                activateQuestAction.Do();
            if(this.actionType == QuestTriggerActionType.ActivateStage)
                activateStageAction.Do();
        }

        [HorizontalGroup]
        [ShowIf("@this.actionType == QuestTriggerActionType.ActivateQuest")]
        [SerializeField] private  ActivateQuestAction activateQuestAction;

        [HorizontalGroup]
        [ShowIf("@this.actionType == QuestTriggerActionType.ActivateStage")]
        [SerializeField] private  ActivateStageAction activateStageAction;
    }
}