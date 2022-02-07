using System;
using System.Collections.Generic;
using System.Linq;
using Game.Controllers;
using Game.Models;
using UnityEngine;
using Extensions;
using System.Collections;
using Game.Data;

namespace Game.OpenWorld.EnemiesCamp
{
    [RequireComponent(typeof(EnemiesCampModel))]
    public class EnemiesCampController : MonoBehaviour
    {
        private const float k_registerEnemiesDelay = 0.5f;
        private EnemiesCampModel campModel;

        #region Data
#pragma warning disable 0649
        [SerializeField] private string zoneName;
        [SerializeField] private List<GameObject> enemiesSpawnGroups;
        [SerializeField] private List<GameObject> lootLevels;
        [SerializeField] private Sprite victoryIcon;

#pragma warning restore 0649
        #endregion

        #region Dependencies
        private ZoneModel ZoneModel => ModelsSystem.Instance._zoneModel;
        private SpesialMessagesModel SpesialMessagesModel => ModelsSystem.Instance._spesialMessagesModel;
        #endregion

        private List<EnemyHealthModel> enemies;
        private int aliveEnemiesCount;
        private bool campActive;
        private GameObject spawnersGroup;
        private Coroutine CSpawnProcess { get; set; }

        #region MonoBehaviour
        private void Awake()
        {
            enemies = new List<EnemyHealthModel>();
            campModel = GetComponent<EnemiesCampModel>();

            campModel.Init();
        }

        private void OnDisable()
        {
            foreach (var enemy in enemies)
            {
                if (enemy == null) return;

                enemy.OnDeath -= OnEnemyDeath;
            }

            if (CSpawnProcess != null)
            {
                StopCoroutine(CSpawnProcess);
            }

            this.StopAllCoroutines();
        }

        private void OnValidate()
        {
            // validate spawners
            foreach (var item in enemiesSpawnGroups)
            {
                if (item == null || item.activeSelf == true)
                {
                    Debug.LogError("Error. Disable all spawn levels at start");
                    return;
                }
            }

            // validate loot
            foreach (var item in lootLevels)
            {
                if (item == null || item.activeSelf == true)
                {
                    Debug.LogError("Error. Disable all loot spawners at start");
                    return;
                }
            }
        }
        #endregion

        public void PlayerEnterArea()
        {
            if (campActive) return;

            campActive = true;
            ShowNotification();
            InitEnemies();
            InitLoot();
        }

        private void RegisterEnemies()
        {
            enemies = spawnersGroup.GetComponentsInChildren<EnemyHealthModel>().ToList();
            foreach (var enemy in enemies)
            {
                enemy.OnDeath += OnEnemyDeath;
            }

            aliveEnemiesCount = enemies.Count;
        }

        private void OnEnemyDeath()
        {
            aliveEnemiesCount--;
            if (aliveEnemiesCount == 0)
            {
                CampeClearedHandler();
            }
        }

        private void ShowNotification()
        {
            ZoneModel.PlayerEnterZone(zoneName);
        }

        private void CampeClearedHandler()
        {
            campModel.SetCleared();
            enemiesSpawnGroups.ForEach(x => x.SetActive(false));

            var loot = GetGameObjectByLevel(lootLevels,campModel.Counter - 1);
            loot.SetActive(true);

            SpesialMessagesModel.RecivedItem(new RecivedItemMessageData(victoryIcon,"Camp cleared",string.Empty));
        }

        private void InitEnemies()
        {
            if (campModel.EnemiesActive)
            {
                SpawnEnemies();
            }
            else
            {
                SpawnEnemiesDelayed();
            }
        }

        private void InitLoot()
        {
            if(!campModel.EnemiesActive) return;
        }

        private void SpawnEnemies()
        {
            if (enemiesSpawnGroups.Count == 0) return;

            spawnersGroup = GetGameObjectByLevel(enemiesSpawnGroups,campModel.Counter);

            spawnersGroup.SetActive(true);
            
            StartCoroutine(DoActionAfterSecondsRealtime(k_registerEnemiesDelay, RegisterEnemies));
        }

        private GameObject GetGameObjectByLevel(List<GameObject> container, int level)
        {
            GameObject spawnersGroup;
            if (!container.IndexOutOfRange(level))
            {
                spawnersGroup = container[level];
            }
            else
            {
                spawnersGroup = container.Last();
            }

            return spawnersGroup;
        }

        private void SpawnEnemiesDelayed()
        {
            if(campModel.TryGetEnemiesCooldownTime(out var seconds))
            {
                CSpawnProcess = StartCoroutine(DoActionAfterSecondsRealtime(seconds, SpawnEnemies));
            }
        }

        private IEnumerator DoActionAfterSecondsRealtime(float remainingSeconds, Action action)
        {
            yield return new WaitForSecondsRealtime(remainingSeconds);
            action?.Invoke();
        }
    }
}