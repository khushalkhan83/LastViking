using ActionsCollections;
using Game.Models;

namespace DebugActions
{
    public class QuestActionSetStage : ActionBase
    {
        private readonly string _operationName;
        private readonly int _stageNum;

        private QuestsModel QuestsModel => ModelsSystem.Instance._questsModel;

        public QuestActionSetStage(string name, int stageNum)
        {
            _operationName = name;
            _stageNum = stageNum;
        }
        public override string OperationName => _operationName;

        public override void DoAction()
        {
            QuestsModel.ActivateStage(_stageNum);
        }
    }
}