using System.Collections.Generic;
using ActionsCollections;
using Game.Models;
using UltimateSurvival;
using UltimateSurvival.Debugging;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DebugActions
{
    public class SceneActionRestartCore : ActionBase
    {
        private readonly string _operationName;
        private readonly bool _restartFromLoadingScreen;

        public SceneActionRestartCore(string name, bool restartFromLoadingScreen = false)
        {
            _operationName = name;
            _restartFromLoadingScreen = restartFromLoadingScreen;
        }
        public override string OperationName => _operationName;

        public override void DoAction()
        {
            var sceneName = _restartFromLoadingScreen ? "LoadingScene" : "CoreScene";
            SceneManager.LoadScene(sceneName);
        }
    }
}