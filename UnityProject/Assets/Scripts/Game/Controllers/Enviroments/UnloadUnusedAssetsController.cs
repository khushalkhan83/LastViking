using System;
using System.Linq;
using Core;
using Core.Controllers;
using Game.Models;
using Helpers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Controllers
{
    public class UnloadUnusedAssetsController : IUnloadUnusedAssetsController, IController
    {
        [Inject] public UnloadUnusedAssetsModel UnloadUnusedAssetsModel {get;private set;}
        [Inject] public PlayerScenesModel PlayerScenesModel {get;private set;}

        private int _scenesCountToUnload;

        void IController.Enable() 
        {
            PlayerScenesModel.OnEnvironmentChange += OnEnvironmentLoadHandler;
        }

        void IController.Start() 
        {
            OnEnvironmentLoadHandler();
        }

        void IController.Disable() 
        {
            PlayerScenesModel.OnEnvironmentChange -= OnEnvironmentLoadHandler;
        }

        private void OnEnvironmentLoadHandler()
        {
            var scenes = ScenesHelper.GetLoadedScenes();
            _scenesCountToUnload = scenes.Count() - 1;
            bool scenesToUnload = _scenesCountToUnload > 0;

            if(_scenesCountToUnload == 0)
            {
                UnloadUnusedAssetsModel.SetUnusedSceneAssetsUnloaded();
            }
            else
            {
                SceneManager.sceneUnloaded += OnSceneUnloadedHandler;
            }
        }

        private void OnSceneUnloadedHandler(Scene scene)
        {
            _scenesCountToUnload--;

            if (_scenesCountToUnload == 0)
            {
                Resources.UnloadUnusedAssets();
                SceneManager.sceneUnloaded -= OnSceneUnloadedHandler;
                UnloadUnusedAssetsModel.SetUnusedSceneAssetsUnloaded();
            }
        }
    }
}
