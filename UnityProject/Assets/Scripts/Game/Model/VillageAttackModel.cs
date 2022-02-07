using System;
using Core.Storage;
using EnemiesAttack;
using UnityEngine;

namespace Game.Models
{
    public class VillageAttackModel : InitableModel<VillageAttackModel.Data>
    {
        [Serializable]
        public class Data: DataBase
        {
            [SerializeField] private bool _attackModeActive;
            [SerializeField] private int _waveIndex;

            public bool AttackModeActive
            {
                get => _attackModeActive;
                set { _attackModeActive = value; ChangeData();}
            }
            public int WaveIndex
            {
                get => _waveIndex;
                set { _waveIndex = value; ChangeData();}
            }
        }
        
        [SerializeField] private IntEnemiesAttackConfigDictionary enemiesAttackConfigs;
        [SerializeField] private EnemiesAttackConfig debugEnemyConfig;
        [SerializeField] private Data data;
        [SerializeField] private Sprite attackFailedIcon;

        #region Dependencies

        private bool UseDebugEnemiesForBaseProtection => EditorGameSettings.Instance.UseDebugEnemiesForBaseProtection;
            
        #endregion

        public bool AttackModeActive
        {
            get { return data.AttackModeActive;}
            set { data.AttackModeActive = value;}
        }
        public int WaveIndex
        {
            get{ return data.WaveIndex; }
            set{ data.WaveIndex = value; }
        }

        protected override Data DataBase => data;
        public Sprite AttackFailedIcon => attackFailedIcon;

        public int EnemiesLeft {get; set;}
        public int EnemiesTotal {get; set;}


        public event Action OnStartTownHallUpgrade;
        public event Action OnWaveComplete;
        public event Action OnTownHallUpgradeCompleated;
        public event Action OnTownHallUpgradeFailed;


        public EnemiesAttackConfig GetEnemiesAttackConfig(int townHallLevel)
        {
            if(UseDebugEnemiesForBaseProtection)
            {
                return debugEnemyConfig;
            }
            enemiesAttackConfigs.TryGetValue(townHallLevel, out var config);
            return config;
        }

        public bool HasEnemiesAttackConfigForLevel(int townHallLevel)
        {
            return enemiesAttackConfigs.ContainsKey(townHallLevel);
        }

        public void StartTownHallUpgrade() => OnStartTownHallUpgrade?.Invoke();

        public void SetWaveCompleated()
        {
            WaveIndex++;
            OnWaveComplete?.Invoke();
        }

        public void TownHallUpgradeCompleated()
        {
            OnTownHallUpgradeCompleated?.Invoke();
        }

        public void TownHallUpgradeFailed()
        {
            OnTownHallUpgradeFailed?.Invoke();
        }
    }
}
