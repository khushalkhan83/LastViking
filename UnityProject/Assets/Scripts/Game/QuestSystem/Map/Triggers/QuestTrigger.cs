using Game.Models;

namespace Game.QuestSystem.Map.Triggers
{
    public class QuestTrigger : QuestTriggerBase
    {
        private QuestTriggersModel QuestTriggersModel => ModelsSystem.Instance._questTriggersModel;

        public override void ActivateTrigger()
        {
            QuestTriggersModel.ActivateTrigger(this);
        }
    }
}