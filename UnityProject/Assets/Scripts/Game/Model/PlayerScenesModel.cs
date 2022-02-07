using UnityEngine;
using System;
using Core.Storage;
using Game.Providers;

namespace Game.Models
{
    public class PlayerScenesModel : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase
        {
            public EnvironmentSceneID ActiveEnvironmentSceneID;
            public int DestinationIndex;
            public bool HasDestinationIndex;
            public bool SceneTransition;

            public override SaveTime TimeSave => SaveTime.Instantly;

            public void SetSceneID(EnvironmentSceneID sceneID)
            {
                ActiveEnvironmentSceneID = sceneID;
                ChangeData();
            }

            public void SetDestinationIndex(int index)
            {
                DestinationIndex = index;
                ChangeData();
            }

            public void SetHasDestinationIndex(bool value)
            {
                HasDestinationIndex = value;
                ChangeData();
            }

            public void SetSceneTransition(bool active)
            {
                SceneTransition = active;
                ChangeData();
            }
        }
        #region Data
        #pragma warning disable 0649
        [SerializeField] private Data _data;
        [SerializeField] private float _pauseTimeAfterSceneLoaded;
        [SerializeField] private  EnvironmentSceneID _defaultSceneID = EnvironmentSceneID.MainIsland;
        
        private bool _dataInited = false;
        
        #pragma warning restore 0649
        #endregion

        #region Dependencies
        private EnvironmentTransitionProvider EnvironmentTransitionProvider => ModelsSystem.Instance._environmentTransitionProvider;
            
        #endregion

        public float PauseTimeAfterSceneLoaded {get => _pauseTimeAfterSceneLoaded; set => _pauseTimeAfterSceneLoaded = value;}
        public EnvironmentSceneID DefaultSceneID => _defaultSceneID;

        public EnvironmentSceneID ActiveEnvironmentSceneID
        {
            get => _data.ActiveEnvironmentSceneID;
            set => _data.SetSceneID(value);
        }

        public int ActiveTransitionIndex
        {
            get => _data.DestinationIndex;
            private set => _data.SetDestinationIndex(value);
        }

        public bool PlayerIsOnMainLocation => ActiveEnvironmentSceneID == _defaultSceneID;
        public bool WaitForChunksOnLoad {get;set;} = true;
        public bool PauseGameOnLocationLoadStarted {get;set;} = true;
        public bool SceneTransition
        {
            get => _data.SceneTransition;
            private set => _data.SetSceneTransition(value);
        }
        public bool HasDestinationIndex
        {
            get => _data.HasDestinationIndex;
            private set => _data.SetHasDestinationIndex(value);
        }
        public EnvironmentTransition DestinationPlace {get;private set;} = null;
        public bool MoveToDefaultTransitionPoint {get; private set;}
        public bool SceneLoading {get; private set;} = true;
        
        public event Action OnPreEnvironmentChange;
        public event Action OnEnvironmentChange;
        public event Action OnPreEnvironmentLoaded;
        public event Action OnEnvironmentLoaded;

        // called at PreInitState
        internal void Init()
        {
            if(_dataInited)
            {
                return;
            }
            _dataInited = true;

            ModelsSystem.Instance._storageModel.TryProcessing(_data);
        }

        public void TransitionToEnvironment(EnvironmentSceneID sceneID, EnvironmentTransition destinationPlace)
        {
            DestinationPlace = destinationPlace;
            SceneTransition = true;

            bool ok = EnvironmentTransitionProvider.TryGetTransitionIndex(destinationPlace, out int index);
            if(ok)
                ActiveTransitionIndex = index;

            HasDestinationIndex = ok;
                
            ChangeEnvironmentScene(sceneID);
        }

        public void TransitionToEnvironment(EnvironmentSceneID sceneID, bool moveToDefaultEnvironmentPoint)
        {
            MoveToDefaultTransitionPoint = moveToDefaultEnvironmentPoint;
            TransitionToEnvironment(sceneID, null);
        }

        private void ChangeEnvironmentScene(EnvironmentSceneID sceneID)
        {
            OnPreEnvironmentChange?.Invoke();
            ActiveEnvironmentSceneID = sceneID;
            SceneLoading = true;
            OnEnvironmentChange?.Invoke();
        }


        // called after chunks loaded as well if they are present in scene
        public void EnvironmentLoaded()
        {
            SceneTransition = false;
            SceneLoading = false;
            HasDestinationIndex = false;
            MoveToDefaultTransitionPoint = false;
            OnPreEnvironmentLoaded?.Invoke();
            OnEnvironmentLoaded?.Invoke();
        }

        public bool PlayerOnLocation(EnvironmentSceneID location) => ActiveEnvironmentSceneID == location;
        
        public void ClearDestination() => DestinationPlace = null;
    }
}