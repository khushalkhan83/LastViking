using Core.Storage;
using Game.AI.Behaviours.Zombie;
using Game.Models;
using Game.StateMachine.Parametrs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Controllers
{
    public class WorldEnemyObjectSpawner : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase
        {
            public long TimeSpawnTicks;

            public void SetTimeSpawnTicks(long timeSpawnTicks)
            {
                TimeSpawnTicks = timeSpawnTicks;
                ChangeData();
            }
        }     

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] private string _id;
        [SerializeField] private float _durationRespawn;
        [SerializeField] private WorldObjectID _worldObjectID;
        [SerializeField] private Transform _targetBase;

#if UNITY_EDITOR

        [SerializeField] private Color _colorGost = new Color(1, 1, 1, 0.5f);
        [SerializeField] private Mesh _mesh;

#endif

#pragma warning restore 0649
        #endregion

        public long TimeSpawnTicks
        {
            get
            {
                return _data.TimeSpawnTicks;
            }
            protected set
            {
                _data.SetTimeSpawnTicks(value);
            }
        }

        public string Id => _id;

        private WorldObjectID WorldObjectID => _worldObjectID;
        private float DurationRespawn => _durationRespawn;
        private WorldObjectCreator WorldObjectCreator => ModelsSystem.Instance._worldObjectCreator;
        private GameTimeModel GameTimeModel => ModelsSystem.Instance._gameTimeModel;
        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;
        private Transform TargetBase => _targetBase;

        public WorldObjectModel Instance { get; private set; }

        public Coroutine CSpawnProcess { get; private set; }

        private void OnEnable()
        {
            StorageModel.TryProcessing(_data);

            if (GameTimeModel.RealTimeNowTick >= TimeSpawnTicks)
            {
                CreateInstance();
            }
            else
            {
                SpawnProcess();
            }
        }

        private void OnDisable()
        {
            if (CSpawnProcess != null)
            {
                StopCoroutine(CSpawnProcess);
            }
        }

        private IEnumerator Spawn(float remainingSeconds)
        {
            yield return new WaitForSecondsRealtime(remainingSeconds);
            CreateInstance();
        }

        private void CreateInstance()
        {
            Instance = WorldObjectCreator.CreateAsSpawnable(WorldObjectID, transform.position, transform.rotation, transform.localScale, DataProcessing, transform);
            Instance.GetComponent<InitializationLinks>().TargetBase.SetTarget(TargetBase);
            Instance.gameObject.SetActive(true);
            Instance.OnDelete += OnDeleteInstanceHandler;
        }

        private void OnDeleteInstanceHandler()
        {
            Instance.OnDelete -= OnDeleteInstanceHandler;
            TimeSpawnTicks = GameTimeModel.TicksRealNow + GameTimeModel.GetTicks(DurationRespawn);
            SpawnProcess();
        }

        private void SpawnProcess()
        {
            CSpawnProcess = StartCoroutine(Spawn(GameTimeModel.GetSecondsTotal(TimeSpawnTicks - GameTimeModel.RealTimeNowTick)));
        }

        private void DataProcessing(IEnumerable<IUnique> uniques)
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
