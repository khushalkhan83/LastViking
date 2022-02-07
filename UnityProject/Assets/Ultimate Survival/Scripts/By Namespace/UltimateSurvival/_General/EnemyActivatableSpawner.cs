using Core.Storage;
using Game.AI.BehaviorDesigner;
using Game.Models;
using Game.Progressables;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Controllers
{
    public class EnemyActivatableSpawner : MonoBehaviour, IProgressable
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

        [SerializeField] private EnemiesProvider _enemiesProvider;
        [SerializeField] private Transform _container;

        [SerializeField] private Wave[] _waves;
        [SerializeField] private UnityEvent _onKilledAll;

#if UNITY_EDITOR
        [Header("Editor")]
        [SerializeField] private Color _colorGost = new Color(1, 1, 1, 0.5f);
        [SerializeField] private float _radius = 1;

#endif

#pragma warning restore 0649
        #endregion

        [Serializable]
        public class Wave
        {
            [SerializeField] private EnemyID _enemyID;
            [SerializeField] private int _count;
            [SerializeField] private float _spawnDelay;

            public EnemyID EnemyID => _enemyID;
            public int Count =>_count;
            public float SpawnDelay => _spawnDelay;
        }

        // [CHANGE]

        private EnemiesProvider EnemiesProvider => ModelsSystem.Instance.enemiesProvider;

        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;

        private Wave[] Waves => _waves;
        private Transform Container => _container;

        private int KilledEnemies { get; set; }
        private int TotalEnemies => Waves.Sum(w => w.Count);
        private bool KilledAll() => KilledEnemies >= TotalEnemies;


        #region MonoBehaviour
        private void OnEnable() => StorageModel.TryProcessing(_data);
        private void OnDisable() => StorageModel.Untracking(_data);

        #endregion

        #region IProgressable
        public void ClearProgress() { }

        public ProgressStatus ProgressStatus
        {
            get => _data.ProgressStatus;
            set => _data.SetProgressStatus(value);
        }
        #endregion

        public void StartSpawn()
        {
            if (ProgressStatus == ProgressStatus.NotInProgress)
                StartCoroutine(DoSpawn());
        }
        private IEnumerator DoSpawn()
        {
            foreach (var wave in Waves)
            {
                yield return new WaitForSeconds(wave.SpawnDelay);

                Initable enemy = CreateEnemy(wave.EnemyID);
                var destroyController = enemy.gameObject.AddComponent<DestroyController>();
                destroyController.OnDestroyAction += OnDestoyEnemy;

                enemy.transform.parent = Container;
                var offs = GetRandomOffs();
                enemy.transform.position += new Vector3(offs.x, 0, offs.y);
                enemy.Init();

                var context = enemy.gameObject.GetComponentInChildren<EnemyContext>();
                if(context != null)
                {
                    context.SetAttackPlayer(true);
                    context.SetHaveRightToAttackPlayer(true);
                }
            }
        }

        private void OnDestoyEnemy(DestroyController destroyController)
        {
            destroyController.OnDestroyAction -= OnDestoyEnemy;
            KilledEnemies++;
            if (KilledAll())
            {
                ProgressStatus = ProgressStatus.WaitForResetProgress;
                _onKilledAll?.Invoke();
            } 
        }

        private Vector2 GetRandomOffs() => UnityEngine.Random.insideUnitCircle;

        private Initable CreateEnemy(EnemyID enemyId) => Instantiate(EnemiesProvider[enemyId], transform.position, transform.rotation);

        

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = _colorGost;
            Gizmos.DrawSphere(transform.position, _radius);
        }
#endif
    }
}
