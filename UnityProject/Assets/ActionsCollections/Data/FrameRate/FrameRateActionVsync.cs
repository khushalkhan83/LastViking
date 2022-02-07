using ActionsCollections;
using Game.Models;
using UltimateSurvival;
using UnityEngine;

namespace DebugActions
{
    public class FrameRateActionVsync : ActionBase
    {
        private string _operationName;
        private int _vCount;

        public FrameRateActionVsync(string name, int vCount)
        {
            _operationName = name;
            _vCount = vCount;
        }
        public override string OperationName => _operationName;

        public override void DoAction()
        {
            QualitySettings.vSyncCount = _vCount;
        }
    }
}