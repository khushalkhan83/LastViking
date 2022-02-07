using Game.Models;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Game
{
    public class RangeSpawner : MonoBehaviour, ISpawner
    {
        #region Data
#pragma warning disable 0649

#if UNITY_EDITOR
        [SerializeField] private Color ColorGizmos;
#endif

        [SerializeField] private AnimalsProvider _animalsProvider;
        [SerializeField] private Transform _container;
        [Range(0.0f, 24.0f)]
        [SerializeField] private float _timeSpawnBegin;
        [Range(0.0f, 24.0f)]
        [SerializeField] private float _timeSpawnEnd;
        [SerializeField] private float _radius;
        [SerializeField] private float _intervalSpawnSeconds;
        [SerializeField] private int _countMax;
        [SerializeField] private bool _spawnOnce = false;
        [SerializeField] private AnimalID _animalID;

#pragma warning restore 0649
        #endregion

        public int CountMax => _countMax;
        public bool SpawnOnce => _spawnOnce;
        public float TimeSpawnBegin => _timeSpawnBegin;
        public float TimeSpawnEnd => _timeSpawnEnd;
        public float IntervalSpawnSeconds => _intervalSpawnSeconds;
        public float Radius => _radius;
        public float TimeOfDay => GameTimeModel.TimeOfDay;
        public Transform Container => _container;
        public AnimalsProvider AnimalsProvider => _animalsProvider;
        public AnimalID AnimalID => _animalID;
        public event Action OnSpawned;

        public int CountSpawned { get; private set; }
        public bool SpawnedOnce { get; private set; }
        public bool IsStartSpawnStarted { get; private set; }
        public bool IsSpawnTime { get; private set; }

        public bool IsCanSpawn => IsSpawnTime && CountSpawned < CountMax && gameObject.activeInHierarchy && !(SpawnOnce && SpawnedOnce);

        private GameTimeModel GameTimeModel => ModelsSystem.Instance._gameTimeModel;

        private GameUpdateModel GameUpdateModel => ModelsSystem.Instance._gameUpdateModel;

        public int NeedToSpawn { get; private set; }

        public void OnEnable()
        {
            NeedToSpawn = 1; //spwan first without delay
            GameUpdateModel.OnUpdate += OnUpdate;
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            GameUpdateModel.OnUpdate -= OnUpdate;
        }

        private void OnUpdate()
        {
            var isSpawnTime = GetIsCanSpawn(TimeOfDay);

            if (isSpawnTime != IsSpawnTime) //if can spawn changed
            {
                IsSpawnTime = isSpawnTime;
                if (IsCanSpawn) //if can spawn first now
                {
                    for (int i = 0; i < NeedToSpawn && i < CountMax - 1; i++)
                    {
                        if (IsCanSpawn)
                        {
                            SpawnAfterTime(0);
                        }
                    }
                    NeedToSpawn = 0;
                }
            }

            SpawnProcess();
        }

        private bool GetIsCanSpawn(float time)
        {
            if (TimeSpawnEnd < TimeSpawnBegin) //if interval is splited
            {
                return time >= 0 && time <= TimeSpawnEnd || time >= TimeSpawnBegin && time <= 24;
            }
            else
            {
                return time >= TimeSpawnBegin && time <= TimeSpawnEnd;
            }
        }

        private void SpawnProcess()
        {
            if (!IsStartSpawnStarted)
            {
                StartCoroutine(_SpawnProcess());
            }
        }

        private void SpawnAfterTime(float time)
        {
            StartCoroutine(_SpawnAfterTime(time));
        }

        private IEnumerator _SpawnProcess()
        {
            IsStartSpawnStarted = true;

            while (IsCanSpawn)
            {
                yield return _SpawnAfterTime(IntervalSpawnSeconds);
            }

            IsStartSpawnStarted = false;
        }

        private IEnumerator _SpawnAfterTime(float time)
        {
            yield return new WaitForSeconds(time);

            if (IsCanSpawn)
            {
                Spawn();
            }

            yield break;
        }

        private void Spawn()
        {
            ++CountSpawned;
            SpawnedOnce = true;

            Vector3 position = GetPositionRandom();
            Quaternion rotation = GetRotationRandom();

            Initable instance = Instantiate(AnimalsProvider[AnimalID], position, rotation, Container);
            RangeSpawnObject rangeSpawnObject = instance.gameObject.AddComponent<RangeSpawnObject>();
            rangeSpawnObject.SpawnTime = Time.time;
            rangeSpawnObject.OnDeath += OnDeathHandler;

            instance.Init();

            OnSpawned?.Invoke();
        }

        private void OnDeathHandler(RangeSpawnObject rangeSpawnObject)
        {
            rangeSpawnObject.OnDeath -= OnDeathHandler;

            --CountSpawned;

            if (IsCanSpawn)
            {
                SpawnAfterTime(GetTimeLeft(rangeSpawnObject.SpawnTime));
            }
            else
            {
                ++NeedToSpawn;
            }
        }

        private Quaternion GetRotationRandom()
        {
            return Quaternion.Euler(Vector3.up * Random.Range(0.0f, 360.0f));
        }

        private Vector3 GetPositionRandom()
        {
            var position = transform.position + Random.insideUnitSphere.normalized * Random.Range(0.0f, Radius);

            if (NavMesh.SamplePosition(position, out var hit, Radius * 1.5f, NavMesh.AllAreas))
            {
                return hit.position;
            }

            return position;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = ColorGizmos;
            Gizmos.DrawWireSphere(transform.position, Radius);
            Gizmos.DrawSphere(transform.position, Radius);
        }
#endif

        private float GetTimeLeft(float timeSpawn)
        {
            return IntervalSpawnSeconds - Mathf.Clamp(Time.time - timeSpawn, 0, IntervalSpawnSeconds);
        }
    }
}
