using System;
using Game.Models;
using UnityEngine;

namespace Game.QuestSystem.Data.QuestTriggers.Actions
{
    [Serializable]
    public class ActivateStageAction: QuestTriggerActionBase
    {
        private QuestsModel QuestsModel => ModelsSystem.Instance._questsModel;
        [SerializeField] private int stage;
        public override void Do()
        {
            QuestsModel.ActivateStage(stage);
        }
    }
}