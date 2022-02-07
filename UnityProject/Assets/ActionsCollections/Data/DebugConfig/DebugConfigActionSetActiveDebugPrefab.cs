using System.Collections.Generic;
using ActionsCollections;
using Game.Models;
using UltimateSurvival;
using UltimateSurvival.Debugging;
using UnityEngine;

namespace DebugActions
{
    public class DebugConfigActionSetActiveDebugPrefab : ActionBase
    {
        private DebugPrefabCreationModel DebugPrefabCreationModel => ModelsSystem.Instance._debugPrefabCreationModel;
        private string _operationName;
        private string _prefabName;
        private bool _value;

        public DebugConfigActionSetActiveDebugPrefab(string name, string prefabName, bool value)
        {
            _operationName = name;
            _prefabName = prefabName;
            _value = value;
        }
        public override string OperationName => _operationName;


        public override void DoAction()
        {
            DebugPrefabCreationModel.SetConfig(new DebugPrefabCreationModel.DebugAssetConfig(_prefabName,_value));
        }

    }
}