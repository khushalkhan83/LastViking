using System;
using System.Collections.Generic;
using Core;
using Core.Controllers;
using Game.AI.BehaviorDesigner;
using Game.Models;
using System.Linq;
using UnityEngine;

namespace Game.Controllers
{
    public class ShelterCombatController : IShelterCombatController, IController
    {
        [Inject] public EnemiesSpawnModel EnemiesSpawnModel  { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel  { get; private set; }

        private List<EnemyContext> Enemies => EnemiesSpawnModel.EnemiesRuntimeSet.Items;
        private int MaxEnemiesAttackingCanAttackPlayer => EnemiesSpawnModel.MaxEnemiesAttackingPlayer;
        
        void IController.Enable() 
        {
            GameUpdateModel.OnUpdate += OnUpdate;
            EnemiesSpawnModel.OnEnemySpawned += OnEnemySpawned;
            EnemiesSpawnModel.OnEnemyDestroyed += OnEnemyDestroyed;
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            GameUpdateModel.OnUpdate -= OnUpdate;
            EnemiesSpawnModel.OnEnemySpawned -= OnEnemySpawned;
            EnemiesSpawnModel.OnEnemyDestroyed -= OnEnemyDestroyed;
        }

        private void OnUpdate()
        {
            var attackingEnemies = Enemies.Where(x => x.HaveRightToAttackPlayer && x.AttackPlayer == true);
            var notAttackingEnemies = Enemies.Except(attackingEnemies);
            var attackingCount = attackingEnemies.Count();

            if(attackingCount > MaxEnemiesAttackingCanAttackPlayer)
            {
                EnemyContext enemyContext = attackingEnemies.FirstOrDefault();
                enemyContext?.SetHaveRightToAttackPlayer(false);
            }
            else if(attackingCount < MaxEnemiesAttackingCanAttackPlayer)
            {
                EnemyContext enemyContext = notAttackingEnemies.FirstOrDefault(x => x.AttackPlayer);
                enemyContext?.SetHaveRightToAttackPlayer(true);
            }
        }

        private void OnEnemySpawned(DeathController nemeyDeathController)
        {
            var damageReciver = nemeyDeathController.GetComponentInChildren<EnemyDamageReciver>();
            if(damageReciver != null) damageReciver.OnReceiveDamage += OnEnemyReceiveDamage;
        }

        private void OnEnemyDestroyed(DeathController nemeyDeathController)
        {
            var damageReciver = nemeyDeathController.GetComponentInChildren<EnemyDamageReciver>();
            if(damageReciver != null) damageReciver.OnReceiveDamage -= OnEnemyReceiveDamage;
        }

        private void OnEnemyReceiveDamage(EnemyDamageReciver enemyDamageReciver)
        {
            Vector3 enemyPosition = enemyDamageReciver.transform.position;
            foreach(var enemy in Enemies)
            {
                if((enemyPosition - enemy.transform.position).magnitude <= EnemiesSpawnModel.HelpAgroDistance)
                {
                    enemy.SetAttackPlayer(true);
                }
            }
        }
    }
}
