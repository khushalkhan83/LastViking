using Core.Storage;
using Game.Controllers;
using Game.Models;
using NaughtyAttributes;
using RoboRyanTron.SearchableEnum;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Spawners.AutoRespawn
{
    public class AutoRespawner : MonoBehaviour
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
        [SearchableEnum] [ShowIf("IsDefaultWorldObjectCreator")]
        [SerializeField] private WorldObjectID _worldObjectID;

        
#if UNITY_EDITOR

        [SerializeField] private Color _colorGost = new Color(1, 1, 1, 0.5f);
        [SerializeField] private Mesh _mesh;

#endif
        [InfoBox("Alternative way to spawn",InfoBoxType.Normal)]
        [SerializeField] private WorldObjectCreatorBase _alternativeWordlObjectCreator;

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

        public WorldObjectID WorldObjectID => _worldObjectID;
        public float DurationRespawn => _durationRespawn;
        public WorldObjectCreator WorldObjectCreator => ModelsSystem.Instance._worldObjectCreator;
        private GameTimeModel GameTimeModel => ModelsSystem.Instance._gameTimeModel;
        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;
        private PlayerScenesModel PlayerScenesModel => ModelsSystem.Instance._playerScenesModel;

        public WorldObjectModel Instance { get; private set; }

        public Coroutine CSpawnProcess { get; private set; }

        private bool inited;

        #region MonoBehaviour
        private void OnEnable()
        {
            PlayerScenesModel.OnPreEnvironmentChange += OnPreEnvironmentLoad;

            if(inited) return;
            StorageModel.TryProcessing(_data);
            inited = true;

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
            PlayerScenesModel.OnPreEnvironmentChange -= OnPreEnvironmentLoad;

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

        #endregion

        private void CreateInstance()
        {
            if(_alternativeWordlObjectCreator != null && _alternativeWordlObjectCreator.TryCreateInstance(transform.position, transform.rotation, transform.localScale,DataProcessing, transform, out var answer))
            {
                Instance = answer;
            }
            else
            {
                Instance = WorldObjectCreator.CreateAsSpawnable(WorldObjectID, transform.position, transform.rotation, transform.localScale, DataProcessing, transform);
            }

            var autoDestroyer = Instance.GetComponent<DestroyItemTimeDelay>();
            if(autoDestroyer != null) autoDestroyer.RemoveComponentAndSaveIt();

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
                Gizmos.color = _colorGost;
                Gizmos.DrawMesh(_mesh, transform.position, transform.rotation, transform.localScale);
            }
        }
#endif

        private bool IsDefaultWorldObjectCreator() => _alternativeWordlObjectCreator == null;
    }
}
