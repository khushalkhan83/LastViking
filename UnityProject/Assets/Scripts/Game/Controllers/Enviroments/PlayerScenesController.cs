using Core;
using Core.Controllers;
using Game.Models;
using Game.Providers;
using System.Collections;
using UnityEngine.SceneManagement;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using Game.Views;
using UnityEngine;
using TeleportActionGeneric = DebugActions.TeleportActionGeneric;
using UltimateSurvival;

namespace Game.Controllers
{
    public class PlayerScenesController : IController, IPlayerScenesController
    {

        [Inject] public PlayerScenesModel PlayerScenesModel { get; private set; }
        [Inject] public SceneNamesProvider SceneNamesProvider { get; private set; }
        [Inject] public GameTimeModel GameTimeModel { get; private set; }
        [Inject] public ViewsSystem  ViewsSystem {get;private set;}
        [Inject] public UnloadUnusedAssetsModel  UnloadUnusedAssetsModel {get;private set;}
        [Inject] public InjectionModel  InjectionModel {get;private set;}
        [Inject] public EnvironmentTransitionProvider  EnvironmentTransitionProvider {get;private set;}
        [Inject] public ViewsInputModel ViewsInputModel {get;private set;}

        // TODO: [Inject(true)] not working because SceneTransitionsModel not present in core. You can create or use controlelr that enabled on scene loaded (SO_modificator_location)
        private SceneTransitionsModel  SceneTransitionsModel => GameObject.FindObjectOfType<SceneTransitionsModel>();
        
        private EditorGameSettings EditorGameSettings => EditorGameSettings.Instance;

        private EnvironmentLoadingView loadingView;
        private Coroutine animationCoroutine;

        private string SceneNameToLoad => SceneNamesProvider[PlayerScenesModel.ActiveEnvironmentSceneID];

        ///TODO: refactor OnEnvironmentLoaded and OnSceneDependenciesInjected. One callback for star
        void IController.Enable()
        {
            PlayerScenesModel.Init();

            if(EditorGameSettings.startInSelectedEnvironment)
            {
                PlayerScenesModel.ActiveEnvironmentSceneID = EditorGameSettings.debugStartEnvironment;
            }

            PlayerScenesModel.OnEnvironmentChange += OnEnvironmentChange;
            InjectionModel.OnSceneDependenciesInjected += OnSceneDependenciesInjected;
            UnloadUnusedAssetsModel.OnUnusedSceneAssetsUnloaded += OnUnusedSceneAssetsUnloadedHandler;
        }


        void IController.Start()
        {
            UnloadAllEnvironmentScenes(); // just for safety
            OnEnvironmentChange();
        }

        void IController.Disable() 
        {
            PlayerScenesModel.OnEnvironmentChange -= OnEnvironmentChange;
            InjectionModel.OnSceneDependenciesInjected -= OnSceneDependenciesInjected;
            UnloadUnusedAssetsModel.OnUnusedSceneAssetsUnloaded -= OnUnusedSceneAssetsUnloadedHandler;
            
            SceneManager.sceneLoaded -= OnAnyChunkLoadedHandler;
            SceneManager.sceneLoaded -= OnEnvironmentSceneLoaded;

        }


        private void OnSceneDependenciesInjected()
        {
            EnvironmentTransition destination = GetDestination();

            if(EditorGameSettings.startInSelectedEnvironment)
            {
                MovePlayerToDestination(null);
            }
            else if(PlayerScenesModel.MoveToDefaultTransitionPoint)
            {
                MovePlayerToDestination(null);
            }
            else if(destination != null)
            {
                MovePlayerToDestination(destination);
            }

            PlayerScenesModel.ClearDestination();
        }

        private EnvironmentTransition GetDestination()
        {
            EnvironmentTransition destination = PlayerScenesModel.DestinationPlace;
            if (destination == null && PlayerScenesModel.SceneTransition && PlayerScenesModel.HasDestinationIndex)
                destination = EnvironmentTransitionProvider[PlayerScenesModel.ActiveTransitionIndex];
            return destination;
        }

        private void MovePlayerToDestination(EnvironmentTransition destination)
        {
            Transform transitionPoint = SceneTransitionsModel.TransitionPoint(destination);
            Vector3 position = transitionPoint.position;
            Vector3 rotation = new Vector3(0, transitionPoint.eulerAngles.y, 0);
            TeleportActionGeneric action = new TeleportActionGeneric("Teleport", position, rotation);
            action.DoAction();
        }

        private void OnEnvironmentChange()
        {
            ViewsInputModel.BlockInput(true);
            loadingView =  ViewsSystem.Show<EnvironmentLoadingView>(ViewConfigID.ScenesTransition);
            PauseTime();
            UnloadAllEnvironmentScenes();
        }

        private void OnUnusedSceneAssetsUnloadedHandler()
        {
            LoadEnvironmentScene();
        }

        private void LoadEnvironmentScene()
        {
            SceneManager.sceneLoaded += OnEnvironmentSceneLoaded;
            var asyncOperation = SceneManager.LoadSceneAsync(SceneNameToLoad, LoadSceneMode.Additive);
            animationCoroutine = PlayerScenesModel.StartCoroutine(ProgressBarAnimation(asyncOperation));
        }

        private void UnloadAllEnvironmentScenes()
        {
            var scenesToUnload = GetEnvironmentScenesNames();

            foreach (var sceneName in scenesToUnload)
            {
                bool needToUnloadScene = ScenesHelper.IsSceneOpened(sceneName);
                if (needToUnloadScene)
                    UnloadScene(sceneName);
            }

            List<string> GetEnvironmentScenesNames()
            {
                var allEnvironmentScenesIDs = EnumsHelper.GetValues<EnvironmentSceneID>().ToList();
                allEnvironmentScenesIDs.Remove(EnvironmentSceneID.None);
                List<string> answer = new List<string>();

                foreach (EnvironmentSceneID sceneID in allEnvironmentScenesIDs)
                {
                    var sceneName = SceneNamesProvider[sceneID];
                    answer.Add(sceneName);
                }
                
                return answer;
            }
        }

        
        #region Handle loading and chunks if needed
            
    
        private event Action OnAllChunksLoaded;

        private int ChunksCountInScene => SECTR_Sector.All.Count;
        private int ChunksLoadingCount => SECTR_Sector.All.Where(x => x.Members.Count != 0).ToList().Count;
        private void OnEnvironmentSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnEnvironmentSceneLoaded;

            PlayerScenesModel.StartCoroutine(Initialization());
        }

        private IEnumerator Initialization()
        {
            // one frame for chunks start loading
            yield return null;

            var scenes = ScenesHelper.GetLoadedScenes();

            _chunksRegistered = false;
            if(PlayerScenesModel.WaitForChunksOnLoad && TryRegisterChunks())
            {
                OnAllChunksLoaded += OnAllChanksAreLoadedHandler;
            }
            else
                OnAllChanksAreLoadedHandler();
        }

        private int _chunksLeftToLoad;
        private bool _chunksRegistered;
        private bool TryRegisterChunks()
        {
            RegisterChunks();

            var chunksLoading = _chunksLeftToLoad > 0;
            if(chunksLoading)
            {
                SceneManager.sceneLoaded += OnAnyChunkLoadedHandler;
            }

            return chunksLoading;
        }

        private void RegisterChunks()
        {
            if(_chunksRegistered) return;

            var scenes = ScenesHelper.GetLoadedScenes().ToList();

            _chunksRegistered = true;
            _chunksLeftToLoad = scenes.Where(x => x.isLoaded == false).ToList().Count;
        }

        private void OnAnyChunkLoadedHandler(Scene scene, LoadSceneMode mode)
        {
            if(_chunksLeftToLoad > 0)
                _chunksLeftToLoad--;
                
            if(_chunksLeftToLoad != 0) return;

            SceneManager.sceneLoaded -= OnAnyChunkLoadedHandler;
            OnAllChunksLoaded?.Invoke();
        }

        #endregion

        private void OnAllChanksAreLoadedHandler()
        {
            OnAllChunksLoaded -= OnAllChanksAreLoadedHandler;
            PlayerScenesModel.StartCoroutine(EnvironmentLoadedProcess(PlayerScenesModel.PauseTimeAfterSceneLoaded));
        }

        private IEnumerator ProgressBarAnimation(AsyncOperation asyncOperation)
        {
            do
            {
                // loadingView.SetSliderValue(GetProgressNormalized());
                // loadingView.SetSliderText((asyncOperation.progress).ToString("P"));
                if(loadingView != null)
                    loadingView.Update();
                   
                yield return null;
            }
            while (!asyncOperation.isDone);

            yield return null;

            // float GetProgressNormalized() => Mathf.Clamp01(asyncOperation.progress / 0.9f);
        }


        private void UnloadScene(string name)
        {
            SceneManager.UnloadSceneAsync(name);
        }

        private void PauseTime()
        {
            if(PlayerScenesModel.PauseGameOnLocationLoadStarted == false) return;

            GameTimeModel.ScaleSave();
            GameTimeModel.ScaleStop();
        }

        private IEnumerator EnvironmentLoadedProcess(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            GameTimeModel.ScaleRestore();
            ViewsInputModel.BlockInput(false);
            PlayerScenesModel.EnvironmentLoaded();

            RemoveLoadingView();
        }

        private void RemoveLoadingView()
        {
            PlayerScenesModel.StopCoroutine(animationCoroutine);
            if(loadingView != null)
                ViewsSystem.Hide(loadingView);
        }
    }
}
