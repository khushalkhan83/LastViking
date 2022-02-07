using Game.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Interactables
{
    public class SpawnEnemiesActivatable : Activatable
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private EnemyActivatableSpawner _enemyActivatableSpawner;

#pragma warning restore 0649
        #endregion

        public EnemyActivatableSpawner EnemyActivatableSpawner => _enemyActivatableSpawner;

        public override void OnActivate() => EnemyActivatableSpawner.StartSpawn();
    }
}
