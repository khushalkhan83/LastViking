using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Controllers;
using EnemiesAttack;
using Extensions;
using Game.AI.BehaviorDesigner;
using Game.Models;
using MarchingBytes;
using SOArchitecture;
using UnityEngine;


namespace Game.Controllers
{
    // FIXME: issue when go to next location, spawn will be canceled but shelter still upgraded
    public class EnemiesSpawnController : IEnemiesSpawnController, IController
    {
        [Inject] public EnemiesSpawnModel EnemiesSpawnModel {get; private set;}
        [Inject] public CoroutineModel CoroutineModel {get; private set;}
        [Inject] public PlayerScenesModel PlayerScenesModel {get; private set;}
        [Inject] public VillageAttackModel VillageAttackModel {get; private set;}

        private ScenePoints _skeletonSpawnPoints;
        private ScenePoints SkeletSpawnPoints => _skeletonSpawnPoints ?? (_skeletonSpawnPoints = GameObject.FindObjectOfType<ScenePoints>());

        private IEnumerable<Wave> Waves;
        private EnemiesAttackConfig config;

        private int mainProcessCoroutine = -1;
        private bool finalWave;
        private int enemiesLeftInActiveWave;
        private bool mainCoroutineActive;
        private List<DeathController> currentWaveEnemies = new List<DeathController>();
        private List<GameObject> spawnedEnemies = new List<GameObject>();
        private Dictionary<EnemyID,int> aliveCountByEnemyID = new Dictionary<EnemyID,int>();


        void IController.Enable() 
        {
            EnemiesSpawnModel.OnStartSpawnSession += OnStartSpawnSession;
            EnemiesSpawnModel.OnStopSpawnSession +=  StopSpawnAndRemoveEnemies;
            PlayerScenesModel.OnEnvironmentChange += OnEnvironmentChange;
            PlayerScenesModel.OnEnvironmentLoaded += OnEnvironmentLoaded;
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            EnemiesSpawnModel.OnStartSpawnSession -= OnStartSpawnSession;
            EnemiesSpawnModel.OnStopSpawnSession -=  StopSpawnAndRemoveEnemies;
            PlayerScenesModel.OnEnvironmentChange -= OnEnvironmentChange;
            PlayerScenesModel.OnEnvironmentLoaded -= OnEnvironmentLoaded;
        }

        private void OnStartSpawnSession(EnemiesAttackConfig enemiesAttackConfig)
        {
            Waves = enemiesAttackConfig.Waves;
            config = enemiesAttackConfig;
            finalWave = false;

            // TODO: not wokring as expected when checking this coroutine. Maybe because of this coroutine waiting for wawe process and its waiting for spawn process
            mainCoroutineActive = true;
            mainProcessCoroutine = CoroutineModel.InitCoroutine(MainProcess());
        }

        private IEnumerator MainProcess()
        {
            while(IsWavesLeft())
            {
                if(IsFinalWave()) finalWave = true;
                yield return WaveProcess();
                if(!mainCoroutineActive)
                    yield break;
                VillageAttackModel.SetWaveCompleated();
                
                if(!finalWave) 
                    yield return new WaitForSeconds(config.PauseBetweenWaves);
            }

            Debug.Log("Global process done");

            EnemiesSpawnModel.AllEnemiesDestroyed();
            mainCoroutineActive = false;

            #region Help Methods
                
            int FinalWaveIndex() => Waves.Count() - 1;
            bool IsFinalWave() => VillageAttackModel.WaveIndex == FinalWaveIndex();
            bool IsWavesLeft() => VillageAttackModel.WaveIndex <= FinalWaveIndex();
            #endregion
        }

        private IEnumerator WaveProcess()
        {
            EnemiesSpawnModel.WaveStart();

            Wave wave = Waves.ElementAt(VillageAttackModel.WaveIndex);
            currentWaveEnemies.Clear();

            enemiesLeftInActiveWave = wave.Data.Sum(x => x.Count);

            aliveCountByEnemyID.Clear();
            foreach (SpawnData data in wave.Data)
            {
                CoroutineModel.InitCoroutine(SpawnProcess(data));
            }

            do 
            { 
                if(!mainCoroutineActive)
                    yield break;
                else
                    yield return null; 
            }
            while(enemiesLeftInActiveWave != 0);

            Debug.Log("Wave process done");
            
        }

        private IEnumerator SpawnProcess(SpawnData spawnData)
        {
            EnemyID id = spawnData.EnemyID;
            var spawned = 0;

            while (spawned < spawnData.Count)
            {
                if(!mainCoroutineActive)
                    yield break;

                if(CanSpawn())
                {
                    SpawnEnemy(id);
                    spawned++;
                    RegisterAlive();
                }
                else
                {
                    yield return new WaitForSeconds(1);
                }
            }

            Debug.Log("Spawn process done");

            void RegisterAlive()
            {
                int alive = GetAliveCount();
                aliveCountByEnemyID[id] = alive + 1;
            }

            int GetAliveCount()
            {
                bool exist = aliveCountByEnemyID.TryGetValue(id, out int value);
                if(exist) return value;
                else
                {
                    aliveCountByEnemyID.Add(id,0);
                    return 0;
                }
            }

            bool CanSpawn()
            {
                return GetAliveCount() < spawnData.MaxCountOnScene;
            }
        }


        private void SpawnEnemy(EnemyID id)
        {
            var instance = SpawnAtRandomPoint(id);

            IHealth health = instance.GetComponentInChildren<IHealth>();

            
            DeathController deathController = instance.gameObject.GetComponent<DeathController>();
            if(deathController == null)
            {
                deathController = instance.gameObject.AddComponent<DeathController>();
            }

            deathController.Init(health,id);

            deathController.OnDeathAction += OnEnemyDeath;

            EnemiesGroup enemiesGroup = instance.gameObject.GetComponent<EnemiesGroup>();
            if(enemiesGroup == null)
            {
                enemiesGroup = instance.gameObject.AddComponent<EnemiesGroup>();
            }

            enemiesGroup.SetRuntimeSet(EnemiesSpawnModel.EnemiesRuntimeSet);

            currentWaveEnemies.Add(deathController);
            spawnedEnemies.Add(instance.gameObject);
            EnemiesSpawnModel.EnemySpawned(deathController);
        }

        private void OnEnemyDeath(DeathController enemy)
        {
            if(!mainCoroutineActive) return;

            enemy.OnDeathAction -= OnEnemyDeath;

            if(currentWaveEnemies.Contains(enemy))
            {
                currentWaveEnemies.Remove(enemy);
            }

            UnRegisterEnemy(enemy);
            enemiesLeftInActiveWave--;
            
            EnemiesSpawnModel.EnemyDestroyed(enemy);
        }

        private void UnRegisterEnemy(DeathController controller)
        {
            var id = controller.EnemyID;

            bool error =  !aliveCountByEnemyID.TryGetValue(id, out int value);
            if(error)
            {
                Debug.LogError("Error here");
            }

            aliveCountByEnemyID[id]--;
        }

        private Initable SpawnAtRandomPoint(EnemyID id)
        {
            Transform randomPoint = SkeletSpawnPoints.Points.RandomElement();

            GameObject enemy = EasyObjectPool.instance.GetObjectFromPool(id.ToString(), randomPoint.position, Quaternion.identity, null);
            EnemyContext context = enemy.GetComponentInChildren<EnemyContext>(true);
            Initable enemyInstance = enemy.GetComponent<Initable>();

            context.SetAttackShelter(true);
            enemyInstance.Init();

            return enemyInstance;
        }

        private void OnAttackModeFailed()
        {
            StopSpawnAndRemoveEnemies();
        }

        private void OnAttackModeFinish()
        {
            StopSpawnAndRemoveEnemies();
        }

        private void OnAttackModeCancel()
        {
            StopSpawnAndRemoveEnemies();
        }

        private void StopSpawnAndRemoveEnemies()
        {
            StopMainCoroutine();
            KillAllEnemies();
        }

        
        private void OnEnvironmentChange()
        {
            StopMainCoroutine();
        }

        private void OnEnvironmentLoaded()
        {
            ResetSpawnPoints();
        }

        private void ResetSpawnPoints()
        {
            _skeletonSpawnPoints = null;
        }

        private void StopMainCoroutine()
        {
            CoroutineModel.BreakeCoroutine(mainProcessCoroutine);
            mainCoroutineActive = false;
        }

        private void KillAllEnemies()
        {
            foreach(var enemy in spawnedEnemies){
                if(enemy != null)
                {
                    DeathController destroyController = enemy.GetComponent<DeathController>();
                    if(destroyController != null)
                    {
                        destroyController.OnDeathAction -= OnEnemyDeath;
                    }
                    enemy.GetComponentInChildren<EnemyHealthModel>()?.SetHealth(0);
                }
            }
            spawnedEnemies.Clear();
        }
    }
}
