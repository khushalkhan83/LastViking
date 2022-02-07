using ActionsCollections;
using Game.Models;
using System.Collections;
using System.Collections.Generic;
using UltimateSurvival;
using UnityEngine;

namespace DebugActions
{
    public class WaterfallActionGeneric : ActionBase
    {
        private WaterfallProgressModel WaterfallProgressModel => ModelsSystem.Instance._waterfallProgressModel;


        private string _operationName;

        public WaterfallActionGeneric(string name)
        {
            _operationName = name;
        }
        public override string OperationName => _operationName;

        public override void DoAction()
        {
            WaterfallProgressModel.ResetProgress();
        }
    }
}
