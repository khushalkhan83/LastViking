using System.Collections.Generic;
using ActionsCollections;
using Game.Models;
using UltimateSurvival;
using UltimateSurvival.Debugging;
using UnityEngine;

namespace DebugActions
{
    public class DockActionSetShipLevel : ActionBase
    {
        private SheltersModel SheltersModel => ModelsSystem.Instance._sheltersModel;

        private string _operationName;
        private int _level;

        public DockActionSetShipLevel(string name, int level)
        {
            _operationName = name;
            _level = level;
        }
        public override string OperationName => _operationName;


        public override void DoAction()
        {
           SheltersModel.ShelterModel.Temp_SetLevel(_level - 1);
           var action = new DockActionUpgrade("Upgrade");
           action.DoAction();
        }
    }
}