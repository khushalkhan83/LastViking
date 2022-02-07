using System.Collections.Generic;
using UnityEngine;

namespace Encounters
{
    public class EnemiesGroupActivityFactory : ISceneActivityFactory
    {
        private readonly List<EnemyConfigFactory.EnemyActivityConfig> enemyConfigIDs;
        private readonly List<GameObject> wayPoints;

        public EnemiesGroupActivityFactory(List<EnemyConfigFactory.EnemyActivityConfig> enemyConfigIDs, List<GameObject> wayPoints)
        {
            this.enemyConfigIDs = enemyConfigIDs;
            this.wayPoints = wayPoints;
        }

        public ISceneActivity GetActivity()
        {
            var configs = new List<EnemyConfig>();
            foreach (var configID in enemyConfigIDs)
            {
                var config = EnemyConfigFactory.GetConfig(configID);
                configs.Add(config);
            }

            return new EnemiesEncounterActivity(configs, false, wayPoints);
        }
    }
}