using Game.AI.Behaviours.Zombie;
using Game.Models;
using System;
using System.Collections;
using UnityEngine;

namespace Game.Controllers
{
    public class AnimalPointSpawner : MonoBehaviour, ISpawner
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private float _durationRespawn;
        [SerializeField] private AnimalID _animalID;

        [SerializeField] private Transform _container;
        [SerializeField] private AnimalsProvider _animalsProvider;
        [SerializeField] private Transform _targetBase;
        [SerializeField] private bool _initializeTargetBase = true;

#if UNITY_EDITOR

        [SerializeField] private Color _colorGost = new Color(1, 1, 1, 0.5f);
        [SerializeField] private Vector3 _ghostRotation;
        [SerializeField] private Mesh _mesh;

#endif

#pragma warning restore 0649
        #endregion

        private Initable _instance;

        public event Action OnSpawned;

        public AnimalID AnimalID => _animalID;
        public AnimalsProvider AnimalsProvider => _animalsProvider;
        public Transform Container => _container;
        public float DurationRespawn => _durationRespawn;
        private GameTimeModel GameTimeModel => ModelsSystem.Instance._gameTimeModel;
        public Transform TargetBase => _targetBase;

        public long TimeSpawnTicks { get; private set; }
        public Initable Instance
        {
            get => _instance;
            set => _instance = value;
        }

        public Coroutine CSpawnProcess { get; private set; }

        private void OnEnable()
        {
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
            Instance = Instantiate(AnimalsProvider[AnimalID], transform.position, transform.rotation, Container);
            var health = Instance.GetComponentInChildren<EnemyHealthModel>();
            health.OnDeath += OnInstanceDead;
            if(_initializeTargetBase) //  (false for kraken)
                Instance.GetComponent<InitializationLinks>().TargetBase.SetTarget(TargetBase);
            Instance.Init();

            OnSpawned?.Invoke();
        }

        private void OnInstanceDead()
        {
            var health = Instance.GetComponentInChildren<EnemyHealthModel>();
            health.OnDeath -= OnInstanceDead;

            TimeSpawnTicks = GameTimeModel.TicksRealNow + GameTimeModel.GetTicks(DurationRespawn);
            SpawnProcess();
        }

        private void SpawnProcess()
        {
            CSpawnProcess = StartCoroutine(Spawn(GameTimeModel.GetSecondsTotal(TimeSpawnTicks - GameTimeModel.RealTimeNowTick)));
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_mesh)
            {
                Gizmos.color = _colorGost;
                Gizmos.DrawMesh(_mesh, transform.position, Quaternion.Euler(_ghostRotation), transform.localScale);
            }
        }
#endif

    }
}
