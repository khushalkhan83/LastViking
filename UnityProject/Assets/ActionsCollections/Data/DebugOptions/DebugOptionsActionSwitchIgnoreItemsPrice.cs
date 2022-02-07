using System.Collections.Generic;
using ActionsCollections;
using Game.Models;
using UltimateSurvival;
using UltimateSurvival.Debugging;
using UnityEngine;

namespace DebugActions
{
    public class DebugOptionsActionSwitchIgnoreItemsPrice : ActionBase
    {
        private EditorGameSettings EditorGameSettings => EditorGameSettings.Instance;
        private string _operationName;

        public DebugOptionsActionSwitchIgnoreItemsPrice(string name)
        {
            _operationName = name;
        }
        public override string OperationName => _operationName;


        public override void DoAction()
        {
            EditorGameSettings.IgnoreItemsPrice = !EditorGameSettings.IgnoreItemsPrice;
        }
    }
}