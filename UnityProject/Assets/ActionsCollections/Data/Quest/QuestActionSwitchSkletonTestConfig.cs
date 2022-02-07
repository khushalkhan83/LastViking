using ActionsCollections;
using Game.Models;

namespace DebugActions
{
    public class QuestActionSwitchSkeletonTestConfig : ActionBase
    {
        private readonly string _operationName;
        private readonly int _stageNum;

        private QuestsModel QuestsModel => ModelsSystem.Instance._questsModel;

        public QuestActionSwitchSkeletonTestConfig(string name)
        {
            _operationName = name;
        }
        public override string OperationName => _operationName;

        public override void DoAction()
        {
            QuestsModel.SetTestAttackConfig(!QuestsModel.ActiveTestConfig);
        }
    }
}