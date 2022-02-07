using System;
using Core.Storage;
using Game.Models;
using UnityEngine;
using Extensions;

namespace Game.Controllers
{
    [DefaultExecutionOrder(-1)]
    public class LimitedLifesSpawner : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649
        [Serializable]
        public class Data : DataBase
        {
            [SerializeField] private int _deathCount;

            public int DeathCount
            {
                get { return _deathCount; }
                set { _deathCount = value; ChangeData(); }
            }
        }

        [SerializeField] private Data _data;
        [SerializeField] private int maxDeathCount = 1;

        [SerializeField] private GameObject _spawner;

#pragma warning restore 0649
        #endregion

        private ISpawner _iSpawner;
        private bool CanSpawn => _data.DeathCount < maxDeathCount;
        private Action _unRegisterOnDeathEvent;


        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;

        #region MonoBehaviour

        private void OnEnable()
        {
            UnSubscribe();
            Subscribe();
            
            bool ok = StorageModel.TryProcessing(_data);

            if (!CanSpawn)
                _spawner?.SetActive(false);
        }

        private void OnDisable()
        {
            UnSubscribe();
        }

        private void OnValidate()
        {
            _iSpawner = _spawner.CheckNull()?.GetComponent<ISpawner>();
            if(_iSpawner == null) _spawner = null;
        }

        #endregion

        private void Subscribe()
        {
            _iSpawner = _spawner.CheckNull()?.GetComponent<ISpawner>();
            if(_iSpawner == null) return;

            _iSpawner.OnSpawned += RegisterSpawnedEntity;
        }

        private void UnSubscribe()
        {
            if(_iSpawner == null) return;

            _iSpawner.OnSpawned -= RegisterSpawnedEntity;
        }

        public void RegisterSpawnedEntity()
        {
            var health = _spawner.GetComponentInChildren<IHealth>();

            health.OnDeath += OnDeath;

            _unRegisterOnDeathEvent = () =>
            {
                if (health != null) health.OnDeath -= OnDeath;
            };
        }

        private void OnDeath()
        {
            _data.DeathCount++;

            _unRegisterOnDeathEvent.Invoke();
        }
    }
}