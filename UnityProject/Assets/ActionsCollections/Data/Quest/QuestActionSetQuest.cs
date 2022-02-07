using ActionsCollections;
using Game.Models;

namespace DebugActions
{
    public class QuestActionSetQuest : ActionBase
    {
        private readonly string _operationName;
        private readonly int _questNum;

        private QuestsModel QuestsModel => ModelsSystem.Instance._questsModel;

        public QuestActionSetQuest(string name, int questNum)
        {
            _operationName = name;
            _questNum = questNum;
        }
        public override string OperationName => _operationName;

        public override void DoAction()
        {
            var q = QuestsModel.GetQuest(_questNum);
            QuestsModel.ActivateQuest(q);
        }
    }
}