using Core.Storage;
using Game.Models;
using System;
using UnityEngine;
using Game.Progressables;

namespace Game.Interactables
{
    public class DungeonEnter : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private EnvironmentSceneID _sceneID;
        [SerializeField] private GameObject _doorObject;
        [SerializeField] private GameObject _solidDoor;
        [SerializeField] private GameObject _brokenDoor;

#pragma warning restore 0649
        #endregion

        private GameObject DoorObject => _doorObject;
        private DungeonProgressModel DungeonProgressModel => ModelsSystem.Instance._dungeonsProgressModel.GetDungeonProgressModel(_sceneID);
        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;
        private bool IsLocked => DungeonProgressModel.ProgressStatus == ProgressStatus.WaitForResetProgress;

        public EnvironmentSceneID SceneID => _sceneID;
        
        #region MonoBehaviour
        private void OnEnable()
        {
            DungeonProgressModel.OnProgressResetted += UpdateState;
        }

        private void Start()
        {
            UpdateState();
        }

        private void OnDisable()
        {
            DungeonProgressModel.OnProgressResetted -= UpdateState;
        }
        #endregion

        public void ManualOpen()
        {
            SetDoorLocked(false);
        }

        private void UpdateState()
        {
            SetDoorActive(IsLocked);
            SetDoorLocked(IsLocked);
        }

        private void SetDoorActive(bool on)
        {
            DoorObject.SetActive(on);
        }

        private void SetDoorLocked(bool on)
        {
            _solidDoor.SetActive(on);
            _brokenDoor.SetActive(!on);
        }
    }
}
