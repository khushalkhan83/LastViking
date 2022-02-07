using System.Collections.Generic;
using ActionsCollections;
using Game.Models;
using UltimateSurvival;
using UltimateSurvival.Debugging;
using UnityEngine;

namespace DebugActions
{
    public class DebugOptionsActionSwitchDebugObjectivesTime : ActionBase
    {
        private PlayerObjectivesModel PlayerObjectivesModel => ModelsSystem.Instance._playerObjectivesModel;
        private string _operationName;

        public DebugOptionsActionSwitchDebugObjectivesTime(string name)
        {
            _operationName = name;
        }
        public override string OperationName => _operationName;

        public override void DoAction()
        {
           PlayerObjectivesModel.SetUseDebugTime(!PlayerObjectivesModel.UseDebugTime);
        }
    }
}