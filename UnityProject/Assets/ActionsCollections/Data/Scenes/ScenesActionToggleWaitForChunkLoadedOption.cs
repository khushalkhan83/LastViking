using System.Collections.Generic;
using ActionsCollections;
using Game.Models;
using UltimateSurvival;
using UltimateSurvival.Debugging;
using UnityEngine;

namespace DebugActions
{
    public class ScenesActionToggleWaitForChunkLoadedOption : ActionBase
    {
        private readonly string _operationName;

        private PlayerScenesModel PlayerScenesModel => ModelsSystem.Instance._playerScenesModel;

        public ScenesActionToggleWaitForChunkLoadedOption(string name)
        {
            _operationName = name;
        }
        public override string OperationName => _operationName;

        public override void DoAction()
        {
            PlayerScenesModel.WaitForChunksOnLoad = !PlayerScenesModel.WaitForChunksOnLoad;
        }
    }
}