using ActionsCollections;
using Game.Models;

namespace DebugActions
{
    public class EncountersActionReset : ActionBase
    {
        private EncountersModel EncountersModel => ModelsSystem.Instance._encountersModel;

        private string _operationName;
        private float _time;

        public EncountersActionReset(string name)
        {
            _operationName = name;
        }

        public override string OperationName => _operationName;


        public override void DoAction()
        {
           EncountersModel.Reset();
        }

    }
}