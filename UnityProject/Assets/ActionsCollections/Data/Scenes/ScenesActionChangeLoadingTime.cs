using System.Collections.Generic;
using ActionsCollections;
using Game.Models;
using UltimateSurvival;
using UltimateSurvival.Debugging;
using UnityEngine;

namespace DebugActions
{
    public class ScenesActionChangeLoadingTime : ActionBase
    {
        private readonly string _operationName;
        private readonly float _time;

        private PlayerScenesModel PlayerScenesModel => ModelsSystem.Instance._playerScenesModel;

        public ScenesActionChangeLoadingTime(string name, float time)
        {
            _operationName = name;
            _time = time;
        }
        public override string OperationName => _operationName;

        public override void DoAction()
        {
            PlayerScenesModel.PauseTimeAfterSceneLoaded = _time;
        }
    }
}