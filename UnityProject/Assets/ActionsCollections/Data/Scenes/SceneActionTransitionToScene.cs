using System.Collections.Generic;
using ActionsCollections;
using Game.Models;
using UltimateSurvival;
using UltimateSurvival.Debugging;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DebugActions
{
    public class SceneActionTransitionToScene : ActionBase
    {
        private readonly string _operationName;
        private readonly EnvironmentSceneID _sceneID;

        private PlayerScenesModel PlayerScenesModel => ModelsSystem.Instance._playerScenesModel;

        public SceneActionTransitionToScene(string name, EnvironmentSceneID sceneID)
        {
            _operationName = name;
            _sceneID = sceneID;
        }
        public override string OperationName => _operationName;

        public override void DoAction()
        {
            PlayerScenesModel.TransitionToEnvironment(_sceneID,true);
        }
    }
}