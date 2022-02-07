using System;
using Core;
using Core.Controllers;
using Game.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Controllers
{
    public class DebugPrefabCreationController : IDebugPrefabCreationController, IController
    {
        [Inject] public DebugPrefabCreationModel DebugPrefabCreationModel {get; private set;}
        [Inject] public StorageModel StorageModel {get; private set;}
        private EditorGameSettings EditorGameSettings => EditorGameSettings.Instance;

        private string ActiveSceneName {get => SceneManager.GetActiveScene().name;}
        private const string CoreSceneName = "CoreScene";
        private bool CoreIsActive {get => ActiveSceneName == CoreSceneName;}

        void IController.Start() { }
        void IController.Enable() 
        {
            StorageModel.TryProcessing(DebugPrefabCreationModel._Data);

            if(CoreIsActive)
            {
                DefaultSpawnStrategy();
            }
            else
            {
                DelayedSpawnStrategy();
            }
        }

        void IController.Disable() 
        {
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }

        #region Strategies
        private void DefaultSpawnStrategy()
        {
            ResetDebugActionsCollection();

            foreach (var config in DebugPrefabCreationModel.Configs)
            {
                if(!config.spawnOnStart) continue;
                
                var debugPrefab = EditorGameSettings.settings.DebugAssets.GetPrefab(config.debugAssetsName);
                GameObject.Instantiate(debugPrefab,Vector3.zero,Quaternion.identity);
            }
        }

        private void DelayedSpawnStrategy()
        {
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }
        #endregion

        private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            if(!CoreIsActive) return;

            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
            DefaultSpawnStrategy();
        }

        private void ResetDebugActionsCollection()
        {
            DebugActions.DebugActionCollection.Instance.Reset();
        }
    }
}
