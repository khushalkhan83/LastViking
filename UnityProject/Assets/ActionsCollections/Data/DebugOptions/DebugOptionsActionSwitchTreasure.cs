using System.Collections.Generic;
using ActionsCollections;
using Game.Models;
using UltimateSurvival;
using UltimateSurvival.Debugging;
using UnityEngine;

namespace DebugActions
{
    public class DebugOptionsActionSwitchTreasure : ActionBase
    {
        private TreasureHuntModel TreasureHuntModel => ModelsSystem.Instance._treasureHuntModel;
        private string _operationName;

        public DebugOptionsActionSwitchTreasure(string name)
        {
            _operationName = name;
        }
        public override string OperationName => _operationName;

        public override void DoAction()
        {
            TreasureHuntModel.IsDebug = !TreasureHuntModel.IsDebug;
        }
    }
}