using ActionsCollections;
using Game.Models;
using UltimateSurvival;
using UnityEngine;

namespace DebugActions
{
    public class FrameRateActionGeneric : ActionBase
    {
        private string _operationName;
        private int _targetFrameRate;

        public FrameRateActionGeneric(string name, int targetFrameRate)
        {
            _operationName = name;
            _targetFrameRate = targetFrameRate;
        }
        public override string OperationName => _operationName;

        public override void DoAction()
        {
            Application.targetFrameRate = _targetFrameRate;
        }
    }
}