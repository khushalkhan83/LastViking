using System;
using EnemiesAttack;
using Game.Controllers;
using SOArchitecture;
using UnityEngine;

namespace Game.Models
{
    public class EnemiesSpawnModel : MonoBehaviour
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private EnemiesContextRuntimeSet _enemiesContextRuntimeSet;
        [SerializeField] private int _maxEnemiesAttackingPlayer = 5;
        [SerializeField] private float _helpAgroDistance = 3f;
        
        #pragma warning restore 0649
        #endregion

        public EnemiesContextRuntimeSet EnemiesRuntimeSet => _enemiesContextRuntimeSet;
        public int MaxEnemiesAttackingPlayer => _maxEnemiesAttackingPlayer;
        public float HelpAgroDistance => _helpAgroDistance;

        public event Action<EnemiesAttackConfig> OnStartSpawnSession;
        public event Action OnStopSpawnSession;

        public event Action<DeathController> OnEnemySpawned;

        public event Action<DeathController> OnEnemyDestroyed;

        public event Action OnWaveStart;
        public event Action OnAllEnemiesDestroyed;

        public void StartSpawnSession(EnemiesAttackConfig enemiesAttackConfig)
        {
            OnStartSpawnSession?.Invoke(enemiesAttackConfig);
        }

        public void StopSpawnSession()
        {
            OnStopSpawnSession?.Invoke();
        }

        public void EnemySpawned(DeathController enemy) => OnEnemySpawned?.Invoke(enemy);
        public void EnemyDestroyed(DeathController enemy) => OnEnemyDestroyed?.Invoke(enemy);

        public void WaveStart()
        {
            OnWaveStart?.Invoke();
        }

        public void AllEnemiesDestroyed() => OnAllEnemiesDestroyed?.Invoke();
    }
}
