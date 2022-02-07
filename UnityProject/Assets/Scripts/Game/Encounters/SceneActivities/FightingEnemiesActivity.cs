using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Models;
using UnityEngine;

namespace Encounters
{
    public class FightingEnemiesActivity : ISceneActivity // TODO: add usubscribe
    {
        private readonly List<EnemyConfig> configs;
        private readonly bool despawnOnToFarAway;
        protected List<Enemy> enemiesGroup = new List<Enemy>();
        private bool receivedDamage;

        private CoroutineModel CoroutineModel => ModelsSystem.Instance._coroutineModel;


        [ObsoleteAttribute("Despawn not supported", false)]
        public FightingEnemiesActivity(List<EnemyConfig> configs, bool despawnOnToFarAway)
        {
            this.configs = configs;
            this.despawnOnToFarAway = false;
        }

        #region ISceneActivity
        public void Spawn(Vector3 spawnPoint)
        {
            enemiesGroup = new List<Enemy>();
            EnemyFactory f = new EnemyFactory();
            int index = 0;
            foreach (var config in configs)
            {
                var enemy = f.GetEnemy(spawnPoint + Vector3.left * index++, null, config,despawnOnToFarAway,null);
                enemy.OnDeath += () => OnEnemyDeath(enemy); 
                enemy.OnFarAwayDespawn += OnFarAwayDespawn;
                enemy.OnReceiveDamage += OnReceiveDamage;

                enemy.Init();
                enemiesGroup.Add(enemy);
            }

            InitFight();
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

        private void InitFight()
        {
            CoroutineModel.InitCoroutine(CInitEnemies());
        }

        private void SetAttackTargetAndAttack(Enemy enemy)
        {
            var target = enemiesGroup.Find(x => x != enemy);
            if(target == null) return;

            enemy.SetHaveRightToAttackPlayer(true);
            enemy.SetOverrideTarget(target.selfTarget);
            enemy.SetAttackPlayer(true); // set agressive
        }
        
        private void OnReceiveDamage()
        {
            if(receivedDamage) return;

            receivedDamage = true;

            foreach (var enemy in enemiesGroup)
            {
                enemy.SetHaveRightToAttackPlayer(false);
                enemy.SetOverrideTarget(null);
            }

            CoroutineModel.InitCoroutine(CDoActionAfterSeconds(() =>
            {
                foreach (var enemy in enemiesGroup)
                {
                    enemy.SetHaveRightToAttackPlayer(true);
                    enemy.SetAttackPlayer(true);
                }
            }, seconds: 0.1f));
        }

        private void OnEnemyDeath(Enemy enemy)
        {
            if(enemiesGroup.Count == 1)
            {
                OnCompleated?.Invoke();
            }

            enemiesGroup.Remove(enemy);
            enemy = null;

            OnReceiveDamage();
        }

        private void OnFarAwayDespawn()
        {
            // TODO: implement despawn ?
            // DeActivate();
            // enemy = null;
            OnFarAwayDespawned?.Invoke();

            // EncountersModel.SendEncounterUnloadEvent(new EncounterUnloadEvent(enemyConfig.analiticsData,EncounterUnloadReason.farAwayDespawn)); // FIXME: wrong data structure. bad analitics
        }

        // TODO: Code duplicate here and in other classes. Move to CoroutineModel
        private IEnumerator CDoActionAfterSeconds(Action action, float seconds)
        {
            yield return new WaitForSeconds(seconds);
            action?.Invoke();
        }

        private IEnumerator CInitEnemies()
        {
            receivedDamage = false;

            foreach (var e in enemiesGroup)
            {
                e.SetHaveRightToAttackPlayer(false);
            }

            foreach (var e in enemiesGroup)
            {
                SetAttackTargetAndAttack(e);
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}