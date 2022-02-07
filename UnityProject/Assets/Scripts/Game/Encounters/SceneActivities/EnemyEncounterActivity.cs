using System;
using System.Collections.Generic;
using Game.Models;
using UnityEngine;

namespace Encounters
{
    public class EnemyEncounterActivity : ISceneActivity
    {
        private readonly EnemyConfig enemyConfig;
        private readonly bool despawnOnToFarAway;
        private readonly List<GameObject> wayPoints;
        private Enemy enemy;

        public EnemyEncounterActivity(EnemyConfig enemyConfig, bool despawnOnToFarAway, List<GameObject> wayPoints)
        {
            this.enemyConfig = enemyConfig;
            this.despawnOnToFarAway = despawnOnToFarAway;
            this.wayPoints = wayPoints;
        }

        #region ISceneActivity

        public void Spawn(Vector3 spawnPoint)
        {
            EnemyFactory f = new EnemyFactory();
            enemy = f.GetEnemy(spawnPoint, null, enemyConfig,despawnOnToFarAway,null);
            enemy.OnDeath += OnEnemyDeath; 
            enemy.OnFarAwayDespawn += OnFarAwayDespawn;

            enemy.Init();
        }

        public void Despawn()
        {
            if(enemy == null) return;

            enemy.Despawn();
            enemy = null;
        }

        public event Action OnCompleated;
        public event Action OnFarAwayDespawned;

        public Vector3 position
        {
            get
            {
                string error = "Can`t get encounter Activity position";
                
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

        private void OnEnemyDeath()
        {
            OnCompleated?.Invoke();
            enemy = null;
        }

        private void OnFarAwayDespawn()
        {
            OnFarAwayDespawned?.Invoke();
            enemy = null;
        }
    }
}