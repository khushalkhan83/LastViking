using System.Collections.Generic;
using Encounters.Analytics;
using Extensions;

namespace Encounters
{
    public class ProgressableEnemyActivityFactory : ISceneActivityFactory, IEncounterAnaliticsDataProvider
    {
        private readonly List<EnemyConfigFactory.EnemyActivityConfig> configs;
        private readonly IEncounterStorage storage;
        private readonly EncounterID encounterID;
        private readonly EncounterType encounterType;
        private EnemyConfigFactory.EnemyActivityConfig lastUsedConfig;

        public ProgressableEnemyActivityFactory(List<EnemyConfigFactory.EnemyActivityConfig> configs, IEncounterStorage storage, EncounterID encounterID, EncounterType encounterType)
        {
            this.configs = configs;
            this.storage = storage;
            this.encounterID = encounterID;
            this.encounterType = encounterType;
        }

        public ISceneActivity GetActivity()
        {
            lastUsedConfig = GetConfig();
            return new EnemyEncounterActivity(EnemyConfigFactory.GetConfig(lastUsedConfig), true, null);
        }

        public EncounterAnaliticsData GetAnaliticsData()
        {
            return new EncounterAnaliticsData(encounterID,$"progressive_{lastUsedConfig.ToString()}",encounterType);
        }

        private EnemyConfigFactory.EnemyActivityConfig GetConfig()
        {
            if (configs.IndexOutOfRange(storage.CompletionCounter))
            {
                return configs.RandomElement();
            }
            else
            {
                return configs[storage.CompletionCounter];
            }
        }
    }
}