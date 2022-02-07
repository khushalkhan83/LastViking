using ActionsCollections;
using Game.Models;
using UltimateSurvival;
using UnityEngine;

namespace DebugActions
{
    public class AddressablesActionSetWeaponPoolSize : ActionBase
    {
        private FPManager FPManager => ModelsSystem.Instance._fPManager;

        private string _operationName;
        private int _poolMaxSize;

        public AddressablesActionSetWeaponPoolSize(string name, int poolMaxSize)
        {
            _operationName = name;
            _poolMaxSize = poolMaxSize;
        }
        public override string OperationName => _operationName;

        public override void DoAction()
        {
            FPManager.SetPoolLimit(_poolMaxSize);
        }
    }
}