using System;
using ActivityLog.Data;
using Encounters.Analytics;
using Game.Models;
using Game.Models.Encounters;
using UnityEngine;

namespace Encounters
{
    public static class Factory
    {
        public static IEncounter GetSimpleEnemyEncounter(bool init, EncounterConfig config, IEncounterStorage storage, ISceneActivityFactory activityFactory, IEncounterAnaliticsDataProvider analiticsDataProvider)
        {
            if(!init) return null;
            var encounter = GetSimpleEncounter(config, storage);

            EnemyEncounter e = new EnemyEncounter(encounter, activityFactory, analiticsDataProvider);

            return e;
        }

        public static ISpecialEncounter GetSpecialEnemyEncounter(bool init, 
                                                                 EncounterConfig config,
                                                                 SpecialEncounterModel storage,
                                                                 ISceneActivityFactory activityFactory,
                                                                 IEncounterAnaliticsDataProvider analiticsDataProvider)
        {
            if(!init) return null;

            var encounter = GetSimpleEncounter(config, storage);
            SpecialEncounter e = new SpecialEncounter(encounter, activityFactory, storage, null, null, activitiLogData:null, null, analiticsDataProvider, null);

            return e;
        }
        public static ISpecialEncounter GetSpecialEnemyEncounter(bool init,
                                                                 EncounterConfig config,
                                                                 SpecialEncounterModel storage,
                                                                 ISceneActivityFactory activityFactory,
                                                                 IEncounterAnaliticsDataProvider analiticsDataProvider,
                                                                 Func<string> initNotification,
                                                                 Func<string> winNotification,
                                                                 LocalNotificationID pushNotificationID,
                                                                 Sprite icon,
                                                                 LootGroupID lootGroupID)
        {
            if(!init) return null;
            
            var encounter = GetSimpleEncounter(config, storage);

            ActivityLogEnterenceData notificationData = new ActivityLogEnterenceData(initNotification,icon);

            SpecialEncounter e = new SpecialEncounter(encounter, activityFactory, storage,
                                                              new ScreenTopNotification(initNotification, icon),
                                                              new ScreenTopNotification(winNotification, icon),
                                                              notificationData,
                                                              new RewardDrop(lootGroupID),
                                                              analiticsDataProvider, pushNotificationID);

            return e;
        }

        private static Encounter GetSimpleEncounter(EncounterConfig config,IEncounterStorage model)
        {
            return new Encounter(config, model);
        }
    }
}