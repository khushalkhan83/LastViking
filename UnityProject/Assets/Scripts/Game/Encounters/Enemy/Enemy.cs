using Game.AI;
using Game.AI.BehaviorDesigner;
using Game.Controllers;
using Game.Models;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Encounters
{
    public class Enemy
    {
        private readonly IHealth health;
        private readonly EnemyContext context;
        private readonly Initable initable;
        private readonly DeathController deathController;
        private readonly EnemyConfig enemyConfig;
        private readonly EnemyDamageReciver damageReciver;
        private readonly IDistanceFromPlayerChecker distanceFromPlayerCheker;
        public readonly Target selfTarget;

        public Transform Transform => context.transform;

        public event Action OnDeath;
        public event Action OnReceiveDamage;
        public event Action OnFarAwayDespawn;

        public Enemy(IHealth health,
                     EnemyContext context,
                     Initable initable,
                     DeathController deathController,
                     EnemyConfig enemyConfig,
                     EnemyDamageReciver damageReciver,
                     IDistanceFromPlayerChecker distanceFromPlayerCheker,
                     List<GameObject> wayPoints,
                     Target selfTarget)
        {
            this.health = health;
            this.context = context;
            this.initable = initable;
            this.deathController = deathController;
            this.enemyConfig = enemyConfig;
            this.damageReciver = damageReciver;
            this.distanceFromPlayerCheker = distanceFromPlayerCheker;
            this.selfTarget = selfTarget;
            deathController.Init(health, enemyConfig.id);

            deathController.OnDeathAction += OnEnemyDeath;
            damageReciver.OnReceiveDamage += OnEnemyReceiveDamage;

            if(distanceFromPlayerCheker != null)
                distanceFromPlayerCheker.OnFarAwayFromPlayer += DespawnAsFarAway;

            SetAttackPlayer(enemyConfig.attackPlayer);
            SetHaveRightToAttackPlayer(enemyConfig.haveRightToAttackPlayer);
            SetAfraidOfPlayer(enemyConfig.afraidOfPlayer);

            context.SetPatrolPoins(wayPoints);
        }

        public void Init()
        {
            initable.Init();
        }

        private void DespawnAsFarAway()
        {
            Despawn();

            OnFarAwayDespawn?.Invoke();
        }

        private void OnEnemyDeath(DeathController enemy)
        {
            if(distanceFromPlayerCheker != null)
                distanceFromPlayerCheker.OnFarAwayFromPlayer -= DespawnAsFarAway;

            deathController.OnDeathAction -= OnEnemyDeath;
            damageReciver.OnReceiveDamage -= OnEnemyReceiveDamage;

            OnDeath?.Invoke();
        }

        private void OnEnemyReceiveDamage(EnemyDamageReciver damageReciver)
        {
            OnReceiveDamage?.Invoke();
        }

        public void Despawn()
        {
            if(distanceFromPlayerCheker != null)
                distanceFromPlayerCheker.OnFarAwayFromPlayer -= DespawnAsFarAway;

            deathController.OnDeathAction -= OnEnemyDeath;
            damageReciver.OnReceiveDamage -= OnEnemyReceiveDamage;

            health.SetHealth(0);
        }

        public void SetAttackPlayer(bool attack) => context.SetAttackPlayer(attack);
        public void SetHaveRightToAttackPlayer(bool haveRight) => context.SetHaveRightToAttackPlayer(haveRight);
        public void SetAfraidOfPlayer(bool afraidOfPlayer) => context.SetAfraidPlayer(afraidOfPlayer);
        public void SetOverrideTarget(Target target) => context.SetOverrideTarget(target);
    }
}