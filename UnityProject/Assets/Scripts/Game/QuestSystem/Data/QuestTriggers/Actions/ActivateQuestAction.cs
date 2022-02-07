using System;
using Game.Models;
using UnityEngine;

namespace Game.QuestSystem.Data.QuestTriggers.Actions
{
    [Serializable]
    public class ActivateQuestAction: QuestTriggerActionBase
    {
        private QuestsModel QuestsModel => ModelsSystem.Instance._questsModel;

        [SerializeField] private QuestData quest;
        public override void Do()
        {
            QuestsModel.ActivateQuest(quest);
        }
    }
}