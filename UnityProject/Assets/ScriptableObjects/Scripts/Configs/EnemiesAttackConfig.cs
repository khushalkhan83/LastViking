using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;
using Sirenix.OdinInspector;
using Extensions;

namespace EnemiesAttack
{
    [CreateAssetMenu(fileName = "SO_config_enemiesAttack_new", menuName = "Configs/EnemiesAttack/EnemiesAttackConfig", order = 0)]
    public class EnemiesAttackConfig : ScriptableObject
    { 
        // [TableList]
        [SerializeField] private List<Wave> waves = new List<Wave>();
        [SerializeField] private float pauseBetweenWaves = 5;
        public IEnumerable<Wave> Waves => waves;
        public float PauseBetweenWaves => pauseBetweenWaves;
        public int EnemiesTotal => waves.Sum(x => x.Data.Sum(y => y.Count));

        public int GetEnemiesLeft(int waveIndex)
        {
            if(waves.IndexOutOfRange(waveIndex)) return 0;

            int count = 0;
            for (int i = waveIndex; i < waves.Count; i++)
            {
                count += waves[i].Data.Sum(x => x.Count);
            }         
            return count;
        }
    }

    [System.Serializable]
    public class Wave
    {
        [TableList]
        [SerializeField] private List<SpawnData> data = new List<SpawnData>();

        public IEnumerable<SpawnData> Data => data;
    }


    [System.Serializable]
    public class SpawnData
    {
        [SerializeField] private EnemyID enemyID = EnemyID.skeleton_super_lite;
        [SerializeField] private int count = 1;
        [SerializeField] private int maxCountOnScene = 1;

        public EnemyID EnemyID { get => enemyID;}
        public int Count { get => count;}
        public int MaxCountOnScene { get => maxCountOnScene;}
    }

}
