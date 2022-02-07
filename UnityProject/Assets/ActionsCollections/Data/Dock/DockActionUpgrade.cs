using System.Collections.Generic;
using ActionsCollections;
using Game.Models;
using UltimateSurvival;
using UltimateSurvival.Debugging;
using UnityEngine;

namespace DebugActions
{
    public class DockActionUpgrade : ActionBase
    {
        private ShelterUpgradeModel ShelterUpgradeModel => ModelsSystem.Instance._shelterUpgradeModel;

        private string _operationName;

        public DockActionUpgrade(string name)
        {
            _operationName = name;
        }
        public override string OperationName => _operationName;


        public override void DoAction()
        {
           ShelterUpgradeModel.CompleteUpgrad();
        }
    }
}