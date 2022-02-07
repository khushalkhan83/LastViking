using System;
using Game.AI;
using UnityEngine;

namespace Game.Models
{
    public delegate void EnemyKillTarget(EnemyID enemyID, Target target);
    public delegate void TargetKillEnemy(Target target, EnemyID enemyID);

    public class EnemiesModel : MonoBehaviour
    {
        public event EnemyKillTarget OnEnemyKillTarget;
        public event TargetKillEnemy OnTargetKillEnemy;

        public void EnemyKillTarget(EnemyID enemyID, Target target) => OnEnemyKillTarget?.Invoke(enemyID, target);

        public void TargetKillAnimal(Target target, EnemyID enemyID) => OnTargetKillEnemy?.Invoke(target, enemyID);
    }
}
