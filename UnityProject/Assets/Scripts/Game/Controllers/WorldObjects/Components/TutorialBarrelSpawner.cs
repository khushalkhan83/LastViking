using Core.Storage;
using Game.Models;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Controllers
{
    public class TutorialBarrelSpawner : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase
        {
            public bool IsHasObject;

            public void SetIsHasObject(bool isHasObject)
            {
                IsHasObject = isHasObject;
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] private string _id;
        [SerializeField] private WorldObjectID _worldObjectID;

#if UNITY_EDITOR

        [SerializeField] private Color _colorGhost;
        [SerializeField] private Mesh _mesh;

#endif

#pragma warning restore 0649
        #endregion

        public bool IsHasObject
        {
            get
            {
                return _data.IsHasObject;
            }
            protected set
            {
                _data.SetIsHasObject(value);
            }
        }

        public string Id => _id;

        private WorldObjectID WorldObjectID => _worldObjectID;
        private WorldObjectCreator WorldObjectCreator => ModelsSystem.Instance._worldObjectCreator;
        private GameTimeModel GameTimeModel => ModelsSystem.Instance._gameTimeModel;
        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;
        private PlayerScenesModel PlayerScenesModel => ModelsSystem.Instance._playerScenesModel;
        private TutorialModel TutorialModel => ModelsSystem.Instance._tutorialModel;

        public WorldObjectModel Instance { get; private set; }

        private bool inited;

        private void OnEnable()
        {
            PlayerScenesModel.OnPreEnvironmentChange += OnPreEnvironmentLoad;
            if(inited) return;
            StorageModel.TryProcessing(_data);
            inited = true;

            if (IsHasObject && TutorialModel.Step <= 1)
            {
                CreateInstance();
            }
        }

        private void OnDisable()
        {
            PlayerScenesModel.OnPreEnvironmentChange -= OnPreEnvironmentLoad;
        }

        private void CreateInstance()
        {
            Instance = WorldObjectCreator.CreateAsSpawnable(WorldObjectID, transform.position, transform.rotation, transform.localScale, DataProcessing, transform);
            Instance.OnDelete += OnDeleteInstanceHandler;
        }

        private void OnDeleteInstanceHandler()
        {
            Instance.OnDelete -= OnDeleteInstanceHandler;
            IsHasObject = false;
        }

        private void DataProcessing(IEnumerable<IUnique> uniques)
        {
            foreach (var item in uniques)
            {
                item.UUID = item.UUID + '.' + Id;
            }
        }

        private void OnPreEnvironmentLoad()
        {
            if (Instance != null)
            {
                Instance.OnDelete -= OnDeleteInstanceHandler;
                Instance.Delete();
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_mesh)
            {
                Gizmos.color = _colorGhost;
                Gizmos.DrawMesh(_mesh, transform.position, transform.rotation, transform.localScale);
            }
        }
#endif

    }
}
