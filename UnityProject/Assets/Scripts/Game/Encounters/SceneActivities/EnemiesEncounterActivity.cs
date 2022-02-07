using System;
using System.Collections.Generic;
using System.Linq;
using Game.Models;
using UnityEngine;

namespace Encounters
{
    public class EnemiesEncounterActivity : ISceneActivity // TODO: add usubscribe
    {
        private readonly List<EnemyConfig> configs;
        private readonly List<GameObject> wayPoints;
        private readonly bool despawnOnToFarAway;
        protected List<Enemy> enemiesGroup = new List<Enemy>();

        [ObsoleteAttribute("Despawn not supported", false)]
        public EnemiesEncounterActivity(List<EnemyConfig> configs, bool despawnOnToFarAway, List<GameObject> wayPoints)
        {
            this.configs = configs;
            this.wayPoints = wayPoints;
            this.despawnOnToFarAway = false;
        }

        #region ISceneActivity
        public void Spawn(Vector3 spawnPoint)
        {
            enemiesGroup = new List<Enemy>();
            EnemyFactory f = new EnemyFactory();
            foreach (var config in configs)
            {
                var enemy = f.GetEnemy(spawnPoint, null, config,despawnOnToFarAway,wayPoints);
                enemy.OnDeath += () => OnEnemyDeath(enemy); 
                enemy.OnFarAwayDespawn += OnFarAwayDespawn;
                enemy.OnReceiveDamage += OnReceiveDamage;

                enemy.Init();
                enemiesGroup.Add(enemy);
            }
        }

        public void Despawn()
        {
            for (int i = enemiesGroup.Count - 1; i >= 0 ; i--)
            {
                var enemy = enemiesGroup[i];
                enemy.Despawn();
                enemy = null;
            }

            enemiesGroup.Clear();
        }

        public event Action OnCompleated;
        public event Action OnFarAwayDespawned;

        public Vector3 position
        {
            get
            {
                string error = "Can`t get encounter Activity position";
                if(enemiesGroup.Count == 0)
                {
                    Debug.LogError(error);
                    return Vector3.zero;
                }

                var enemy = enemiesGroup.FirstOrDefault();
                if(enemy == null)
                {
                    Debug.LogError(error);
                    return Vector3.zero;
                }

                if(enemy.Transform == null)
                {
                    Debug.LogError(error);
                    return Vector3.zero;
                }

                return enemy.Transform.position;
            }
        }

        #endregion

        private void OnReceiveDamage()
        {
            foreach (var enemy in enemiesGroup)
            {
                enemy.SetAttackPlayer(true);
                enemy.SetHaveRightToAttackPlayer(true);
            }
        }

        private void OnEnemyDeath(Enemy enemy)
        {
            if(enemiesGroup.Count == 1)
            {
                OnCompleated?.Invoke();
            }

            enemiesGroup.Remove(enemy);
            enemy = null;
        }

        private void OnFarAwayDespawn()
        {
            // OnFarAwayDespawned?.Invoke();
            // TODO: implement despawn ?
            // DeActivate();
            // enemy = null;

            // EncountersModel.SendEncounterUnloadEvent(new EncounterUnloadEvent(enemyConfig.analiticsData,EncounterUnloadReason.farAwayDespawn)); // FIXME: wrong data structure. bad analitics
        }
    }
}