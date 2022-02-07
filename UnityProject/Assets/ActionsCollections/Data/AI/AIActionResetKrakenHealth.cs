using ActionsCollections;
using Game.Models;


namespace DebugActions
{
    public class AIActionResetKrakenHealth : ActionBase
    {
        private FirstKrakenModel FirstKrakenModel => ModelsSystem.Instance._firstKrakenModel;

        private string _operationName;

        public AIActionResetKrakenHealth(string name)
        {
            _operationName = name;
        }
        public override string OperationName => _operationName;

        public override void DoAction()
        {
            FirstKrakenModel.SetKrakenActive();
        }
    }
}