using Core.Storage;
using Game.Models;
using Game.Progressables;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Progressables
{
    public abstract class ProgressSpawner : MonoBehaviour, IProgressable
    {
        [Serializable]
        public class Data : DataBase
        {
            public ProgressStatus ProgressStatus;

            public void SetProgressStatus(ProgressStatus status)
            {
                ProgressStatus = status;
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] private string _id;
        [SerializeField] private WorldObjectID _worldObjectID;

#if UNITY_EDITOR

        [SerializeField] private Color _colorGost = new Color(1, 1, 1, 0.5f);
        [SerializeField] private Mesh _mesh;

#endif

#pragma warning restore 0649
        #endregion

        public string Id => _id;
        
        #region IProgressable
        public ProgressStatus ProgressStatus
        {
            get => _data.ProgressStatus;
            set => _data.SetProgressStatus(value);
        }

        public void ClearProgress() => Instance?.Delete();
            
        #endregion

        public WorldObjectID WorldObjectID => _worldObjectID;
        public WorldObjectCreator WorldObjectCreator => ModelsSystem.Instance._worldObjectCreator;
        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;

        public WorldObjectModel Instance { get; protected set; }

        #region MonoBehaviour
        private void OnEnable() => StorageModel.TryProcessing(_data);
        private void OnDisable() => StorageModel.Untracking(_data);

        private IEnumerator Start()
        {
            yield return null;
            if (CanSpawn())
            {
                CreateInstance();
            }
        }
        #endregion


        protected abstract bool CanSpawn();
        protected abstract void CreateInstance();

        protected void DataProcessing(IEnumerable<IUnique> uniques)
        {
            foreach (var item in uniques)
            {
                item.UUID = item.UUID + '.' + Id;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_mesh)
            {
                Gizmos.color = _colorGost;
                Gizmos.DrawMesh(_mesh, transform.position, transform.rotation, transform.localScale);
            }
        }
#endif
    }
}
