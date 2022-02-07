using System.Collections.Generic;

namespace Encounters
{
    public class FightingEnemiesActivityFactory : ISceneActivityFactory
    {
        private readonly List<EnemyConfigFactory.EnemyActivityConfig> enemyConfigIDs;

        public FightingEnemiesActivityFactory(List<EnemyConfigFactory.EnemyActivityConfig> enemyConfigIDs)
        {
            this.enemyConfigIDs = enemyConfigIDs;
        }

        public ISceneActivity GetActivity()
        {
            List<EnemyConfig> configs = new List<EnemyConfig>();
            foreach (var enemyConfigID in enemyConfigIDs)
            {
                var config = EnemyConfigFactory.GetConfig(enemyConfigID);
                configs.Add(config);
            }

            return new FightingEnemiesActivity(configs, false);
        }
    }
}