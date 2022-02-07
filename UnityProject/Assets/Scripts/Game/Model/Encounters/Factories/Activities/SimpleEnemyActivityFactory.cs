
using Encounters.Analytics;

namespace Encounters
{
    public class SimpleEnemyActivityFactory : ISceneActivityFactory
    {
        private readonly bool canDespawnOnFarAway;

        public SimpleEnemyActivityFactory(EnemyConfigFactory.EnemyActivityConfig enemyConfigID, bool canDespawnOnFarAway)
        {
            EnemyConfigID = enemyConfigID;
            this.canDespawnOnFarAway = canDespawnOnFarAway;
        }

        public EnemyConfigFactory.EnemyActivityConfig EnemyConfigID { get; }

        public ISceneActivity GetActivity()
        {
            var config = EnemyConfigFactory.GetConfig(EnemyConfigID);

            return new EnemyEncounterActivity(config, canDespawnOnFarAway, null);
        }
    }
}