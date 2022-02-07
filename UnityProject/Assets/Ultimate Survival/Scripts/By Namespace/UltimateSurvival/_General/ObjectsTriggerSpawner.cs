using Core.Storage;
using Game.AI;
using Game.Models;
using System;
using System.Collections;
using UnityEngine;
using Game.Progressables;

namespace Game.Controllers
{
    [Serializable]
    public class ObjectsTriggerSpawnerData : DataBase
    {
            public ProgressStatus ProgressStatus;

            public void SetProgressStatus(ProgressStatus status)
            {
                ProgressStatus = status;
                ChangeData();
            }
    }
    public abstract class ObjectsTriggerSpawner<T> : MonoBehaviour, IProgressable
    {

        #region Data
#pragma warning disable 0649

        [SerializeField] private ObjectsTriggerSpawnerData _data;

        [SerializeField] protected ColliderTriggerModel _activateCollider;
        [SerializeField] protected ColliderTriggerModel _deactivateCollider;

        [SerializeField] protected Transform _container;
        [SerializeField] protected bool _spawnOnlyOnce;
        [SerializeField] private bool _ignoreTriggers;

// #if UNITY_EDITOR // 
        [Header("Editor")]
        [SerializeField] private Color _colorGost = new Color(1, 1, 1, 0.5f);
        [SerializeField] private Mesh _mesh;
// #endif

#pragma warning restore 0649
        #endregion

        public ColliderTriggerModel ActivateCollider => _activateCollider;
        public ColliderTriggerModel DeactivateCollider => _deactivateCollider;
        public Transform Container => _container;
        public bool SpawnOnlyOnce => _spawnOnlyOnce;

        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;


        #region IProgressable
        public ProgressStatus ProgressStatus
        {
            get => _data.ProgressStatus;
            set => _data.SetProgressStatus(value);
        }
        public abstract void ClearProgress();
            
        #endregion

        public T ObjectInstance { protected set; get; }


        #region MonoBehaviour
        private void OnEnable() => StorageModel.TryProcessing(_data);

        private void OnDisable()
        {
            StorageModel.Untracking(_data);
            ActivateCollider.OnEnteredTrigger -= OnActivateTrigger;
            if (DeactivateCollider)
                DeactivateCollider.OnEnteredTrigger -= OnDeactivateTrigger;
        }

        private IEnumerator Start()
        {
            yield return null;
            if (ProgressStatus != ProgressStatus.WaitForResetProgress)
            {
                if(_ignoreTriggers && ProgressStatus != ProgressStatus.WaitForResetProgress)
                {
                    ProgressStatus = ProgressStatus.InProgress;
                    SpawnObject();
                    yield break;
                }

                ActivateCollider.OnEnteredTrigger += OnActivateTrigger;
                if (DeactivateCollider)
                    DeactivateCollider.OnEnteredTrigger += OnDeactivateTrigger;

                SpawnObject();
            }
        }

        #endregion
    
        protected void SpawnObject(float delay = 0) => StartCoroutine(WaitForSpawnObject(delay));
        protected abstract T CreateObject();
        protected abstract void OnCreate();
        protected abstract void ActivateObject();
        protected abstract void DeactivateObject();
        protected virtual void OnDelete()
        {
            ProgressStatus = ProgressStatus.WaitForResetProgress;
            ActivateCollider.OnEnteredTrigger -= OnActivateTrigger;
            if (DeactivateCollider != null)
                DeactivateCollider.OnEnteredTrigger -= OnDeactivateTrigger;
        }
        protected virtual void OnActivateTrigger(Collider collision)
        {
            var target = collision.GetComponent<Target>();
            if (target?.ID == TargetID.Player)
            {
                ActivateObject();
                ProgressStatus = ProgressStatus.InProgress;
                if (SpawnOnlyOnce)
                    ActivateCollider.OnEnteredTrigger -= OnActivateTrigger;
            }
        }
        protected virtual void OnDeactivateTrigger(Collider collision)
        {
            var target = collision.GetComponent<Target>();
            if (target?.ID == TargetID.Player)
            {
                DeactivateObject();
                ProgressStatus = ProgressStatus.NotInProgress;
                if (SpawnOnlyOnce)
                    DeactivateCollider.OnEnteredTrigger -= OnDeactivateTrigger;
            }
        }
        private IEnumerator WaitForSpawnObject(float delay = 0)
        {
            yield return new WaitForSeconds(delay);

            ObjectInstance = CreateObject();
            OnCreate();
        }   

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_mesh)
            {
                Gizmos.color = _colorGost;
                Gizmos.DrawMesh(_mesh, Container.transform.position, Container.transform.rotation, Container.transform.localScale);
            }
        }
#endif
    }
}
