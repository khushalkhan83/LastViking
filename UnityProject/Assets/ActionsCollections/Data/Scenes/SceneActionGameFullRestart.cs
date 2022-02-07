using System.Collections.Generic;
using ActionsCollections;
using Game.Models;
using UltimateSurvival;
using UltimateSurvival.Debugging;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DebugActions
{
    public class SceneActionGameFullRestart : ActionBase
    {
        private readonly string _operationName;

        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;

        public SceneActionGameFullRestart(string name)
        {
            _operationName = name;
        }
        public override string OperationName => _operationName;

        public override void DoAction()
        {
            StorageModel.ClearAll();
            StorageModel.ClearAllWithImmortal();
            SceneManager.LoadScene("LoadingScene");
        }
    }
}