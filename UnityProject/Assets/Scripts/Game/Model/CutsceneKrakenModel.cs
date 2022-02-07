using CodeStage.AntiCheat.ObscuredTypes;
using Core.Storage;
using SOArchitecture;
using System;
using UnityEngine;

namespace Game.Models
{
    public class CutsceneKrakenModel : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649
        [SerializeField] private LayerMask _activatorLayer;
        [SerializeField] private string _sceneName;
        [SerializeField] private GameEvent _gameEventSceneShown;

        [SerializeField] private StorageModel _storageModel;

#pragma warning restore 0649
        #endregion

        public LayerMask ActivatorLayer => _activatorLayer;
        public string SceneName => _sceneName;
        public StorageModel StorageModel => _storageModel;

        public event Action OnCutsceneEnded;


        public void SetShown(bool value = true)
        {
            if(value) 
                _gameEventSceneShown?.Raise();
        }
    }
}
